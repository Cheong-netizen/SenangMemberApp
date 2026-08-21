# AppointmentApp.Web: prefer FCM, fall back to WhatsApp

Implement the smallest necessary changes in the current `AppointmentApp.Web` solution so appointment notifications are sent through FCM when the customer has at least one usable registered app device, and through WhatsApp when the customer has no usable app device.

## Goal

For each transactional appointment notification:

```text
Resolve canonical customer
        |
        v
Has enabled FCM device token(s)?
   | yes                    | no
   v                        v
Send FCM               Send WhatsApp
   |
   +-- at least one FCM send accepted -> stop; do not send WhatsApp
   |
   +-- every token permanently invalid -> disable invalid tokens and send WhatsApp
   |
   +-- only retryable failures -> use existing Hangfire retry policy;
                                  after final retry is exhausted, send WhatsApp once
```

This routing applies to outbound appointment alerts such as booking confirmation, reschedule, cancellation, reminder, and a real completion event. It must not replace or suppress interactive WhatsApp chatbot replies needed to complete the user's conversation.

## Important limitation

The server cannot reliably detect whether an Android app is physically installed. Treat an enabled, valid row in `dbo.tbl_PushDevice` as the app-availability signal. An uninstall is normally discovered when FCM returns an invalid/unregistered-token response. FCM send success means Firebase accepted the message; it does not prove that the user saw it. Do not add a client delivery-receipt API as part of this minimal change.

## Existing components to preserve and reuse

- `/api/push-devices`
- `dbo.tbl_PushDevice`
- `PushNotificationService`
- `PushNotificationJob`
- `NotificationJobProcessor` or the current WhatsApp notification sender
- `AsyncNotificationController`
- Hangfire retries
- `dbo.tbl_PushDelivery` and existing deduplication, if present
- Canonical `PushCustomerID`/SenangMember customer mapping
- Existing English, Malay, and Chinese notification localization
- Existing Firebase credential and IIS configuration

Inspect the current working tree before editing. Do not replace current files with an old backup or redesign the notification pipeline.

## Required behavior

### 1. Add one server-side channel-routing/orchestration decision

- Route by the canonical push customer identity that matches `tbl_PushDevice.CustomerID`.
- Query all enabled tokens for that customer.
- If there are no enabled tokens, enqueue/send the existing WhatsApp appointment notification exactly once.
- If at least one enabled token exists, execute the existing FCM send path first.
- If at least one token is accepted successfully by FCM, treat the event as routed through the app and do not send the WhatsApp fallback.
- Continue supporting multiple devices. One successful device is sufficient to avoid WhatsApp fallback.
- Do not use app version, device ID, phone suffix, company-scoped appointment customer ID, or the mere existence of an old disabled row as proof that FCM is available.

### 2. Handle FCM failures correctly

Classify the existing per-token FCM results without weakening current behavior:

- Permanent invalid/unregistered token: disable that token using the existing mechanism.
- If every eligible token is permanently invalid and no token succeeded, send the WhatsApp fallback once in the same logical notification workflow.
- Retryable/transient failure: preserve the existing Hangfire retry behavior. Do not immediately send WhatsApp on the first transient failure, because that can create duplicate FCM and WhatsApp alerts.
- If all configured FCM retries are exhausted without any accepted send, enqueue/send WhatsApp once.
- Unexpected programming/configuration errors must remain visible as failures and must not be silently converted into successful WhatsApp routing. Use fallback only according to an explicit, tested policy.

### 3. Prevent duplicate notifications

- Use the existing appointment event/delivery deduplication key. Extend it minimally if it does not distinguish routing attempts.
- Hangfire retries, webhook retries, repeated async-notify calls, and concurrent workers must not produce duplicate WhatsApp fallbacks.
- Record enough safe delivery state to distinguish outcomes such as:
  - `FcmAccepted`
  - `WhatsAppNoEnabledDevice`
  - `WhatsAppAllTokensInvalid`
  - `WhatsAppAfterFcmRetriesExhausted`
- Never store or log full FCM tokens, Bearer tokens, Firebase credentials, or secret WhatsApp credentials in routing diagnostics.

### 4. Preserve event-source behavior

- Apply the same routing policy whether the appointment action originated from the mobile app, web UI, server process, or WhatsApp.
- A successful WhatsApp chatbot mutation may still require its normal conversational confirmation reply. Do not mistake that reply for the separate transactional notification or suppress it through this router.
- `IsFromChatbot` may continue preventing a duplicate transactional WhatsApp alert where appropriate, but it must not suppress the normal chatbot reply and must not prevent FCM routing.
- Never send notification before the appointment create/update/delete operation has succeeded.
- Do not invent appointment completion from elapsed time; connect completion only to a real server-side completed transition.

### 5. Preserve language behavior

- FCM must continue using each registered device's `LanguageCode`.
- WhatsApp fallback must use the existing trusted WhatsApp/customer language preference. If none exists, use the customer's last known supported preference when safely available, otherwise fall back to `en-US`.
- Supported languages remain `en-US`, `ms-MY`, and `zh-CN`.

## Suggested minimal design

Prefer a small orchestration result rather than duplicating notification logic, for example:

```csharp
public sealed record PushAttemptResult(
    int TargetDeviceCount,
    int SuccessfulDeliveryCount,
    int PermanentFailureCount,
    int RetryableFailureCount);
```

The existing FCM service/job should return or persist an equivalent result. A coordinator can then make the fallback decision. Adapt this to existing types rather than introducing a parallel notification system.

Do not call a controller endpoint from another server class when the existing service/job can be invoked directly.

## Tests required

Add focused tests proving:

1. Customer with no enabled token receives one WhatsApp appointment alert and no FCM send.
2. Customer with one valid token receives FCM and no transactional WhatsApp fallback.
3. Customer with multiple tokens where at least one succeeds receives no WhatsApp fallback.
4. Customer whose every token is permanently invalid has those tokens disabled and receives one WhatsApp fallback.
5. A transient FCM failure is retried and does not immediately send WhatsApp.
6. Exhausted transient retries produce one WhatsApp fallback.
7. Repeated jobs/webhooks and concurrent workers do not duplicate either channel.
8. Canonical `PushCustomerID` is used rather than a company-scoped appointment customer ID.
9. WhatsApp chatbot conversational replies still work and are not duplicated.
10. Existing language selection, appointment notifications, invalid-token cleanup, and delivery history remain working.

Run the focused tests, existing notification tests, and build the solution. Fix only failures caused by this work.

## Manual verification

After deployment and IIS app-pool recycle:

1. With an enabled valid device row, trigger a booking event and verify FCM is accepted and no transactional WhatsApp fallback is sent.
2. Disable all of that customer's device rows, trigger a different booking event, and verify one WhatsApp notification is sent and no FCM target is attempted.
3. Re-register the app token, verify the row is enabled again, and confirm routing returns to FCM.
4. Test one invalid token and confirm it is disabled; if it was the customer's only token, verify exactly one WhatsApp fallback.
5. Verify Hangfire and delivery records without displaying full tokens or credentials.

When finished, report the exact files changed, database changes (if any), tests/build results, deployment steps, and manual outcomes. Do not claim the fallback is reliable until the no-device, valid-device, invalid-token, and retry-exhaustion cases have been tested.
