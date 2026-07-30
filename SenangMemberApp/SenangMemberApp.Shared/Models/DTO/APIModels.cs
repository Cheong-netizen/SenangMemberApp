using System;
using System.Collections.Generic;

namespace SenangMemberApp.Shared.Models
{
    public class LoginRequest
    {
        public string Phone { get; set; } = string.Empty;
        public string? Name { get; set; } // ADDED: Accept Name during Login
    }

    public class LoginResponse
    {
        public string CustomerId { get; set; } = string.Empty;
        public ChatResponse WelcomeMessage { get; set; } = new();
    }

    public class ChatMessageRequest
    {
        public string CustomerId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    // NEW: Used by the Web UI to stream recorded audio back to the C# Backend
    public class ChatVoiceRequest
    {
        public string CustomerId { get; set; } = string.Empty;
        public string Base64Audio { get; set; } = string.Empty;
        public string MimeType { get; set; } = "audio/webm";
    }

    public class AppointmentNotificationRequest
    {
        public string Action { get; set; } = "Create"; // Create, Update, Delete
        public AppointmentData Appointment { get; set; } = new();

        public DateTime? ReminderDate { get; set; }

        public string? CustomerRace { get; set; }
        public string? CompanyContactNumber { get; set; }
        public string? ShopName { get; set; }
        public string? GreenApiInstanceId { get; set; }
        public string? GreenApiToken { get; set; }

        public bool IsFromChatbot { get; set; } = false;
    }

    // ========================================================================
    // --- DTO CLASSES --------------------------------------------------------
    // ========================================================================
    public class AppointmentData
    {
        public string Id { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsAllDay { get; set; }
        public string RecurrenceRule { get; set; } = string.Empty;
        public string RecurrenceException { get; set; } = string.Empty;
        public Nullable<int> RecurrenceID { get; set; }
        public string StaffId { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;

        // Appointment Details
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int NumberOfPax { get; set; }
        public string Services { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public StaffAttendance? AssignedStaff { get; set; } // Single staff member
        public bool IsBlockTimeSlot { get; set; }
        public string Reminder { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // e.g., "Completed", "NoShow", etc.
    }

    public class StaffAttendance
    {
        public String Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime? FirstClockIn { get; set; }
        public DateTime? LastClockOut { get; set; }
        public double HoursWorked { get; set; }
        public double MinsWorked { get; set; }
        public double OvertimeHours { get; set; }
        public double OvertimeMins { get; set; }
        public double DaysCount { get; set; }
        public double LateCount { get; set; }
        public bool IsLate { get; set; }
        public double HoursLate { get; set; }
        public string PhoneNo { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty; // "Male" or "Female"
        public string Status { get; set; } = string.Empty; // "Working", "Late", "Off Day", "On Leave", "Overtime"
        public bool HasOvertime { get; set; } = false;
    }
}
