# AppointmentApp.Web: WhatsApp-triggered and multilingual FCM

Implement the smallest necessary changes in the current `AppointmentApp.Web` solution so that appointment actions performed through WhatsApp also trigger FCM, and every registered device receives the notification in the language selected in SenangMemberApp.

## Scope and constraints

- Work only in `AppointmentApp.Web` and its existing shared/test projects. We do not have access to the EBI API server and must not require changes there.
- Inspect the current working tree first and preserve all existing FCM, canonical-customer mapping, chatbot, WhatsApp, Hangfire, and credential changes. Do not replace current files with an older backup.
- Make minimal changes. Reuse the existing `/api/push-devices`, `PushNotificationService`, `PushNotificationJob`, `AsyncNotificationController`, Hangfire, and Firebase Admin pipeline.
- Do not change or weaken authentication, expose tokens, log FCM tokens/Bearer tokens, embed Firebase credentials, or commit service-account JSON.
- `IsFromChatbot` may continue preventing a duplicate WhatsApp message, but it must never prevent an FCM job.
- Do not invent appointment completion from elapsed time. Leave completion unwired until a real completed transition exists.

## Client contract already implemented

SenangMemberApp now sends the selected app language whenever it registers or refreshes a device:

```http
POST /api/push-devices
Authorization: Bearer <EBI member access token>
Content-Type: application/json
```

```json
{
  "token": "<FCM token>",
  "platform": "android",
  "deviceId": "<installation id>",
  "appVersion": "1.1",
  "languageCode": "en-US"
}
```

Supported values are exactly:

- `en-US` — English
- `ms-MY` — Malay
- `zh-CN` — Simplified Chinese/Mandarin

The client sends the same token again immediately when the user changes the existing app language preference. Old clients will omit `languageCode`, so the server must remain backward compatible and use `en-US` as the default.

## Required server changes

### 1. Persist language per device

- Add an idempotent database migration for `dbo.tbl_PushDevice.LanguageCode nvarchar(10) NOT NULL` with default `en-US`.
- Extend the registration request, `PushDeviceRegistration`, repository reads, and upsert to store the normalized language for the FCM token.
- Normalize `en`/`en-*` to `en-US`, `ms`/`ms-*` to `ms-MY`, and `zh`/`zh-*` to `zh-CN`. Missing or unsupported values must safely fall back to `en-US`.
- Updating the language for an existing token must update the same row, not create another device registration.
- Include `LanguageCode` in the successful registration response for diagnostics, but never return the FCM token.

### 2. Localize FCM on the server

- Firebase must receive the final localized title/body from `AppointmentApp.Web`; do not depend on the app being open to translate it.
- Preserve or add an Android notification payload suitable for background delivery: `AndroidConfig.Priority = FirebaseAdmin.Messaging.Priority.High`, a non-empty `Notification` title/body, and an `AndroidNotification` with the configured `ChannelId`, `Priority = FirebaseAdmin.Messaging.NotificationPriority.HIGH`, `DefaultSound = true`, and `DefaultVibrateTimings = true`. Do not convert appointment pushes to data-only messages.
- Explicitly set `AndroidNotification.EventTimestamp` to the real notification event time (use `DateTime.UtcNow` when there is no better appointment-event timestamp). Never leave it as `DateTime.MinValue`: that serializes as year 0001 and Android displays the notification age as approximately `2032y`.
- Ensure the server uses Firebase Admin .NET 3.0.1 or newer because 3.0.1 includes an `EventTimestamp` serialization fix. If the installed package is older, update only that package to a compatible stable version and rerun the build/tests.
- The app already creates the `appointments` channel with high importance. Keep the server channel ID aligned with it.
- Add deterministic English, Malay, and Simplified Chinese templates for:
  - test notification
  - booking confirmed/created
  - booking rescheduled/updated
  - booking cancelled/deleted
  - booking reminder
  - booking completed (template only; do not invent a completion trigger)
- Format booking date/time using the appropriate culture while retaining the existing `Asia/Kuala_Lumpur` business-time behavior.
- Resolve language per enabled device at send time. If one customer has devices in different languages, send each device the matching localized message exactly once.
- Preserve invalid-token disabling, retry classification, delivery deduplication, and existing FCM data keys/navigation payloads.
- Scheduled reminders must use the device's current language when the job executes, so a later language change is respected.

### 3. Fix WhatsApp appointment events not triggering FCM

Trace the real accepted WhatsApp flow:

```text
GreenApiWebhookController
→ ChatOrchestrator.ProcessInput(...)
→ successful create/update/delete in EBI
→ POST /api/appointments-async/notify
→ AsyncNotificationController
→ Hangfire PushNotificationJob
```

The mobile app currently receives FCM for app/web appointment actions, but a successful WhatsApp action does not trigger FCM.

- Find the exact point where the WhatsApp create, reschedule, or cancel operation has definitely succeeded.
- Ensure that path calls the existing appointment notification orchestration and enqueues the same FCM job used by the working app/web flow.
- Do not send FCM before the EBI appointment mutation succeeds.
- Keep WhatsApp reply suppression separate from FCM. `IsFromChatbot == true` may suppress `NotificationJobProcessor` if that prevents a duplicate WhatsApp response, but it must not suppress `PushNotificationJob`.
- Use the canonical SenangMember/EBI login customer identity that matches `tbl_PushDevice.CustomerID`. Do not use a company-scoped EBI appointment customer ID or an unrelated local row ID as the push recipient.
- The current solution may already contain a `PushCustomerID` field/migration added for this distinction. If present, use and preserve it. Ensure the WhatsApp path populates it before calling `/api/appointments-async/notify`, and ensure create/update/delete/reminder jobs read that canonical value.
- Resolve an accepted WhatsApp sender through the existing trusted customer/phone mapping created by authenticated app login. Do not guess a recipient, broadcast by phone suffix, or send to another user when mapping is missing or ambiguous. Log a safe reason and skip FCM if no canonical identity can be established.
- Keep `Appointment.CustomerId` available for the company-scoped EBI appointment logic if it is needed; use a distinct canonical push recipient field rather than overloading identities.

## Tests required

Add or update focused tests proving:

1. Registration without `languageCode` stores `en-US`.
2. Registration normalizes and persists all three supported languages.
3. Registering the same token with a new language updates the row without duplication.
4. One customer with English, Malay, and Chinese devices receives one correctly localized payload per device.
5. Invalid-token disabling, retries, and deduplication still work.
6. A successful WhatsApp create enqueues an FCM booking-confirmed job for the canonical push customer.
7. A successful WhatsApp reschedule enqueues an FCM rescheduled job.
8. A successful WhatsApp cancellation enqueues an FCM cancelled job even when `IsFromChatbot` is true.
9. A missing or ambiguous WhatsApp-to-canonical-customer mapping does not send to any device and records a safe diagnostic.
10. Existing app/web appointment notification behavior and WhatsApp replies remain unchanged.
11. A background notification has a valid non-MinValue event timestamp, high display priority, default sound, and default vibration in the generated Android FCM payload.

Run the existing test suite and build the solution. Fix only failures caused by this work.

## Manual verification and expected result

After deployment, recycle the IIS app pool and keep the existing Firebase environment/credential configuration. Then:

1. Open SenangMemberApp, choose each language once, and confirm the existing device row's `LanguageCode` changes.
2. Call authenticated `POST /api/push-devices/test`; the emulator must receive the test notification in the selected app language.
3. From the same member's WhatsApp number, create, reschedule, and cancel an appointment.
4. Confirm each successful action creates a `PushNotificationJob`, targets the same canonical customer as `tbl_PushDevice.CustomerID`, and delivers the localized FCM notification.
5. Verify Hangfire and `tbl_PushDelivery` without displaying any full token.

Also test with the app in the foreground, background, and removed from the recent-apps screen. Android intentionally does not deliver notifications to an app that the user has force-stopped in system settings; do not add unsafe workarounds for force-stopped apps.

Useful safe query:

```sql
SELECT Id, CustomerID, Platform, DeviceID, AppVersion, LanguageCode,
       IsEnabled, UpdatedUtc, LastSeenUtc
FROM dbo.tbl_PushDevice
ORDER BY UpdatedUtc DESC;
```

When finished, report the exact files changed, database migration/deployment steps, tests/build results, and any remaining blocker. Do not claim WhatsApp-to-FCM works unless the job is enqueued with the canonical recipient and a delivery test confirms it.
