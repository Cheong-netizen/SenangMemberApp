# Senang Member App — User Manual

Welcome to **Senang Member App**, your all-in-one mobile and web membership application for seamless service bookings, package management, loyalty rewards tracking, and personalized salon/merchant customer experience.

---

## Table of Contents

1. [Overview & System Requirements](#1-overview--system-requirements)
2. [Getting Started](#2-getting-started)
   - [Registration (New User)](#registration-new-user)
   - [Login](#login)
   - [Forgot / Reset Password](#forgot--reset-password)
3. [Merchant & Outlet Selection](#3-merchant--outlet-selection)
4. [Home Dashboard](#4-home-dashboard)
5. [Service & Product Catalog](#5-service--product-catalog)
6. [Step-by-Step Appointment Booking](#6-step-by-step-appointment-booking)
   - [Step 1: Select Outlet / Branch](#step-1-select-outlet--branch)
   - [Step 2: Select Services](#step-2-select-services)
   - [Step 3: Select Staff / Specialist](#step-3-select-staff--specialist)
   - [Step 4: Select Date & Time Slot](#step-4-select-date--time-slot)
   - [Step 5: Review & Confirm Booking](#step-5-review--confirm-booking)
7. [Managing Appointments](#7-managing-appointments)
   - [Viewing Appointments](#viewing-appointments)
   - [Appointment Details & Directions](#appointment-details--directions)
   - [Cancelling / Modifying Appointments](#cancelling--modifying-appointments)
8. [AI Chatbot Assistant](#8-ai-chatbot-assistant)
9. [Member Balances & Wallet](#9-member-balances--wallet)
   - [Prepaid Credits](#prepaid-credits)
   - [Service Packages](#service-packages)
   - [Loyalty Points & Rebates](#loyalty-points--rebates)
10. [Purchase History & Service Records](#10-purchase-history--service-records)
11. [Account Settings & Profile Management](#11-account-settings--profile-management)
    - [Edit Profile](#edit-profile)
    - [Change Password](#change-password)
    - [Switch Language](#switch-language)
    - [Delete Account](#delete-account)
    - [Logout](#logout)
12. [Frequently Asked Questions (FAQ) & Troubleshooting](#12-frequently-asked-questions-faq--troubleshooting)

---

## 1. Overview & System Requirements

Senang Member App is built with .NET Blazor and MAUI, supporting both native mobile devices (Android & iOS) and web browsers.

### Key Capabilities:
- **Multi-Merchant Support**: Access all your registered salon and retail merchants within a single account.
- **Smart Appointment Booking**: Schedule appointments by branch, service category, preferred staff specialist, and available time slots.
- **AI Smart Assistant**: Chat or speak with an AI assistant to check availability, book slots, and answer questions.
- **Member Balances & Packages**: Real-time tracking of prepaid credits, package sessions, vouchers, and loyalty points.
- **Service Records & Receipts**: Review past transaction histories, invoices, and leave service reviews.
- **Multi-Language Support**: English, Bahasa Malaysia, and Simplified Chinese (中文).

---

## 2. Getting Started

### Registration (New User)
1. Open the app and tap **Register** / **Sign Up** on the welcome screen.
2. Enter your details:
   - **Full Name**
   - **Mobile Phone Number**
   - **Email Address**
   - **Password** (must meet minimum security requirements)
   - **Confirm Password**
3. Tap **Submit** to create your account.

### Login
1. Open the app to reach the **Login** screen.
2. Enter your registered **Phone Number**.
3. Enter your **Password**.
4. Tap **Log In**.
5. Once logged in, the app securely stores your authentication session.

### Forgot / Reset Password
1. On the login screen, tap **Forgot Password?**.
2. Enter your registered phone number.
3. An OTP (One-Time Password) will be dispatched to your phone via SMS.
4. Enter the verification code and set your **New Password**.
5. Tap **Reset Password** to confirm.

---

## 3. Merchant & Outlet Selection

If you are a member of multiple affiliated shops or merchants:
1. Tap the **Merchant / Shop Name** at the top of the Home page or via the **Profile** menu.
2. Select your desired merchant from the list.
3. The app will automatically switch context, refreshing your credits, packages, announcements, and appointment booking options for that specific merchant.

---

## 4. Home Dashboard

The **Home** screen gives you a quick snapshot of your membership status:

- **Merchant Selector**: Located at the top header to easily switch shops.
- **Member Balance Summary Card**:
  - **Credits**: Prepaid cash credits available for redemption.
  - **Packages**: Active service packages and remaining sessions.
  - **Points**: Accumulated loyalty reward points.
  - **Vouchers**: Available discount or service vouchers.
- **Quick Action Bar**:
  - **Book Appointment**: Instant shortcut to schedule a new appointment.
  - **AI Assistant**: Launch the interactive chatbot.
- **Latest Announcements & Promotions**: Swipeable banners displaying merchant news, promotions, and announcements from the past 6 months.
- **Next Upcoming Appointment Card**: Displays your nearest upcoming appointment details (date, time, branch, service, and specialist) with direct action to view or modify.

---

## 5. Service & Product Catalog

Access the **Catalog** tab from the bottom navigation bar to browse products and services offered by the merchant:

1. **Browse by Category**: View category tiles (e.g., Hair Care, Facial Treatments, Nail Art, Spa & Massage, Packages).
2. **Category Details**: Tap any category to view all items available within that group.
3. **Item Details**: Tap on an item card to inspect high-resolution images, sales descriptions, brand information, and details.

---

## 6. Step-by-Step Appointment Booking

Senang Member App features a guided, 5-step appointment reservation process:

```
[1. Outlet] ──► [2. Services] ──► [3. Staff] ──► [4. Date & Time] ──► [5. Confirmation]
```

### Step 1: Select Outlet / Branch
1. Navigate to the **Booking** tab from the bottom menu or tap **Book Appointment** on Home.
2. If multiple outlets exist, choose your preferred branch location.
3. Tap **Next** to proceed.

### Step 2: Select Services
1. **Category Pills Bar**: Tap category pills (`All`, `Hair`, `Nails`, `Facial`, etc.) to filter services dynamically.
2. **Search Box**: Type keywords in the search box to find specific services instantly.
3. **Select Service(s)**: Tap on one or more service cards to select them. Selected items will be highlighted.
4. **Service Details Modal**: Long-press on any service card to open the detail popup with full descriptions and images.
5. Tap **Next** at the bottom to continue.

### Step 3: Select Staff / Specialist
1. Choose your preferred stylist or service specialist from the list.
2. Or choose **Any Staff** / **Online** if you do not have a preference.
3. Tap **Next**.

### Step 4: Select Date & Time Slot
1. **Calendar View**: Navigate through weeks and select your desired booking date.
2. **Time Slots**: Select an available time slot (e.g., `10:30`, `11:30`, `14:00`, etc.). Time slots in the past or unavailable slots are automatically disabled.
3. Tap **Next**.

### Step 5: Review & Confirm Booking
1. Review your booking summary:
   - **Store & Branch Location**
   - **Selected Services**
   - **Selected Specialist / Staff**
   - **Scheduled Date, Time & Estimated Duration**
2. (Optional) Add a special request note or memo.
3. Tap **Confirm Booking**.
4. A confirmation dialog will confirm your booking reservation.

---

## 7. Managing Appointments

### Viewing Appointments
1. Tap the **Booking** (`Appointments`) tab on the bottom navigation bar.
2. Switch between:
   - **1-Month View**: Shows appointments within the next 30 days.
   - **1-Year View**: Shows all upcoming scheduled appointments for the year.

### Appointment Details & Directions
1. Tap on any appointment card to view full details.
2. View appointment reference ID, branch address, contact details, scheduled time, and specialist.
3. Tap the map or location icon to launch external navigation (Google Maps / Waze).

### Cancelling / Modifying Appointments
1. On the appointment details page or action menu, tap **Cancel Appointment**.
2. Confirm the cancellation prompt.
3. The appointment will be cancelled, and your time slot released.

---

## 8. AI Chatbot Assistant

The built-in **AI Chatbot Assistant** enables natural language interaction with your merchant:

1. Tap the **Chat / AI Assistant** icon on the Home page.
2. **Text Chat**: Type questions like:
   - *"What time are you open tomorrow?"*
   - *"I'd like to book a haircut this Friday at 3 PM with Alan."*
   - *"How many package credits do I have left?"*
3. **Voice Input**: Tap and hold the **Microphone** button to speak your query. The AI will process your voice input and respond.
4. **Automated Booking Actions**: The chatbot can check real-time availability and assist you in completing a booking directly in the chat stream.

---

## 9. Member Balances & Wallet

Access detailed breakdowns of your membership funds from the Home screen:

### Prepaid Credits
- Tap the **Credits** widget on Home.
- View total balance and transaction history (top-ups, debits, and remaining balances).

### Service Packages
- Tap the **Packages** widget on Home.
- View active packages, package names, remaining service counts/units, and expiration dates.

### Loyalty Points & Rebates
- Tap the **Points** widget on Home.
- View accumulated reward points, rebate equivalents, and point redemption criteria.

---

## 10. Purchase History & Service Records

Tap the **History** tab on the bottom navigation bar to review past activities:

1. **Monthly Filter**: Select the year and month to view past records.
2. **History Categories**:
   - **Services**: Records of completed appointments, treatments, styling, and services rendered.
   - **Purchases / Invoices**: Retail products, packages, or credit top-ups purchased.
3. **Receipt Details**: Tap any history record to view invoice lines, unit quantities, stylist/staff name, and branch details.
4. **Ratings & Reviews**: Tap the review link on completed services to submit feedback and ratings for your experience.

---

## 11. Account Settings & Profile Management

Tap the **Profile** tab on the bottom navigation bar to manage your account:

### Edit Profile
- Update your **Name**, **Email**, **Gender**, and **Date of Birth**.
- Tap **Save** to apply updates.

### Change Password
- Enter your **Current Password**, followed by your **New Password** and **Confirm Password**.
- Tap **Update Password**.

### Switch Language
- Tap **Language** in Profile settings.
- Select your preferred language:
  - **English**
  - **Bahasa Malaysia**
  - **简体中文 (Simplified Chinese)**
- The entire app interface will immediately translate to your chosen language.

### Delete Account
- If you wish to permanently delete your account, tap **Delete Account**.
- Re-enter your password to confirm identity.
- Your personal information will be purged in accordance with privacy compliance policies.

### Logout
- Tap **Logout** at the bottom of the Profile page to safely sign out of your account.

---

## 12. Frequently Asked Questions (FAQ) & Troubleshooting

### Q1: Why are there no services displayed on the booking page?
- **Answer**: Ensure you have an active Internet connection. If a specific category is empty, tap **All** to view all available services. Only services configured by the merchant with `inventoryTypeID: 3` (Services) will be displayed.

### Q2: How do I change the merchant shop?
- **Answer**: Tap the merchant name banner at the top of the Home page, or go to **Profile > Switch Merchant** to select another shop.

### Q3: Can I book multiple services in a single appointment?
- **Answer**: Yes. On the **Select Services** screen, simply tap each service you wish to include. The selected count will update on the **Next** button.

### Q4: What should I do if I forgot my login password?
- **Answer**: On the login page, tap **Forgot Password?** and verify via SMS OTP to set a new password.

### Q5: How do I contact customer support?
- **Answer**: Visit your merchant's branch details in the app or contact them via the phone number and address listed on your Appointment / Store Info card.

---

*Thank you for using Senang Member App!*
