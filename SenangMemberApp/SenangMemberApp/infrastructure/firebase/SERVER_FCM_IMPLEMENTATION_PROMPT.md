# Server-side FCM implementation prompt

I need you to finish and verify Firebase Cloud Messaging in the `AppointmentApp` server with the smallest necessary changes. Preserve the existing chatbot, WhatsApp/Hangfire notifications, appointment logic, authentication, and database behavior. Do not redesign unrelated code.

## Existing implementation

The server source already contains:

- `AppointmentApp.Web/Controllers/PushDevicesController.cs`
- `AppointmentApp.Web/Notifications/FirebaseMessagingClient.cs`
- `PushDeviceRepository.cs`
- `PushNotificationService.cs`
- `PushNotificationJob.cs`
- `PushNotificationTemplates.cs`
- push notification tests

Review and reuse those files. Do not create a second competing FCM implementation.

The Android client uses Firebase project `senangmember`, package `com.ebi.senangmemberapp`, and notification channel `appointments`.

## Required client/server contract

All device endpoints are hosted by `AppointmentApp.Web`, not by `https://ebisoftware.com.my:9088`.

Authenticated requests send the signed-in member access token as:

`Authorization: Bearer <EBI member access token>`

Endpoints:

1. `POST /api/push-devices`

```json
{
  "token": "<FCM registration token>",
  "platform": "android",
  "deviceId": "<stable app-installation GUID>",
  "appVersion": "<version>"
}
```

2. `DELETE /api/push-devices`

```json
{
  "token": "<FCM registration token>",
  "deviceId": "<stable app-installation GUID>"
}
```

3. `POST /api/push-devices/test`

This must send a test message only to devices belonging to the authenticated member and return delivery counts.

FCM data payloads must use string values and include:

- `type`: one of `booking_confirmed`, `booking_reminder`, `booking_completed`, `booking_cancelled`, `booking_rescheduled`, or `announcement`
- `bookingId` for booking messages

Use Android channel ID `appointments`.

## Fix the current blockers

### 1. Firebase configuration and server startup

`appsettings.Development.json` currently sets `Firebase:Enabled` to `false`, so localhost intentionally never sends FCM. Production enables Firebase but has no credential configured, which can make the entire ASP.NET Core application fail during startup with IIS `HTTP 500.30`.

- Keep committed defaults safe (`Firebase:Enabled=false` is acceptable).
- Enable FCM in the actual development/deployment environment with `Firebase__Enabled=true` only when credentials are present.
- Use Firebase Admin credentials through `GOOGLE_APPLICATION_CREDENTIALS` or a protected absolute `Firebase__CredentialsPath`.
- Never commit the service-account JSON, private key, access token, or credential contents.
- The Android `google-services.json` is not an Admin credential and must not be used by the server.
- When enabled, validate at startup with a clear error identifying the missing path/permission. When disabled, the chatbot and the rest of the API must continue starting normally.
- For IIS, ensure the application-pool identity has read access to the credential file and that environment variables are visible to the worker process after restart.

### 2. Use one canonical member identity for push targeting

There is an identity mismatch in the current source:

- `PushDevicesController` stores devices under the canonical member `CustomerId` returned by `AuthenticateEbiAccessTokenAsync`.
- Appointment notification creation currently passes `ebiCustomerId`, which comes from a company token.
- The source comments correctly state that company-specific customer IDs are not expected to equal the member ID.

Do not target by phone number and do not trust a customer ID supplied by an unauthenticated client.

Make the smallest schema/model change that preserves both identities:

- Keep the existing appointment `CustomerID` for the company-specific EBI customer identity.
- Add a separate nullable `PushCustomerID` (or clearly named equivalent) to `tbl_AppointmentHF` and the appointment DTO/model.
- Populate `PushCustomerID` from the authenticated canonical member identity (`_context.CustomerId`) when the chatbot creates or updates an appointment.
- Continue passing `ebiCustomerId` to EBI appointment APIs and existing scheduling fields.
- Enqueue FCM jobs using `PushCustomerID`, never the company-specific `CustomerID`.
- Load/preserve `PushCustomerID` during update/delete hydration.
- Make `PushDeviceRepository.GetBookingAsync` return the push/canonical identity for reminder and completion targeting.
- Add an idempotent migration using `COL_LENGTH` so existing databases gain the new nullable column safely.
- If an appointment has no canonical push identity, skip FCM with a structured log; never guess the recipient.

### 3. Wire every required appointment event

- Create/confirm: enqueue an immediate `booking_confirmed` push after the appointment mirror is successfully saved.
- Reschedule/update: enqueue `booking_rescheduled` and replace previous reminder jobs.
- Cancel/delete: enqueue `booking_cancelled` for the preserved push owner before/while deleting local mirror data.
- Reminder: schedule according to `Firebase:ReminderOffsetsHours`; re-read the booking before sending and skip past, cancelled, completed, deleted, or rescheduled stale jobs.
- Completion: `SendBookingCompletedAsync` currently exists but is not called anywhere. Connect it to the real server-side transition that marks an appointment completed. Do not infer completion only from the current time.
- Use deterministic deduplication/event keys so retrying the same business event cannot notify the same device twice.

### 4. Preserve existing behavior

- FCM failures must be logged and retried through Hangfire but must not roll back a successfully created EBI appointment.
- Do not remove or change the existing WhatsApp notification flow.
- Do not expose full FCM tokens or bearer tokens in logs.
- Disable invalid/unregistered FCM tokens based on Firebase responses.
- Keep multiple devices per member supported and reassign a rotated token safely.
- Logout/unregister must only disable the authenticated member's matching token/device.

## Verification required

Add or update automated tests for:

1. Registration derives the user only from the bearer token.
2. Two members' devices cannot receive each other's notifications.
3. A canonical member ID different from the company EBI customer ID still receives their booking notification.
4. Create, reschedule, cancel, reminder, and completion produce the expected `type` and `bookingId` data.
5. Duplicate business events send once per device.
6. Invalid tokens are disabled; transient Firebase errors are retried.
7. Firebase disabled or missing optional configuration does not break chatbot startup.

Then perform this end-to-end check:

1. Start the server with valid Firebase Admin credentials and `Firebase__Enabled=true`.
2. Log into the Android client and confirm `POST /api/push-devices` returns 200.
3. Verify one enabled `tbl_PushDevice` row under the authenticated canonical member ID.
4. Call `POST /api/push-devices/test` with the same bearer token and confirm `TargetDeviceCount >= 1` and `SuccessfulDeliveryCount >= 1`.
5. Create an appointment for that same authenticated member and confirm the Hangfire FCM job succeeds.
6. Confirm the phone receives the notification in foreground and background.
7. Report the exact files changed, migration applied, build/test results, and any remaining deployment configuration steps.
