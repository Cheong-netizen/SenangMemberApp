using System;
using System.Collections.Generic;

namespace SenangMemberApp.Shared.Models
{
    public enum ChatStep
    {
        Greeting = 0,
        SelectBranch = 1,
        SelectDate = 2,
        SelectTime = 3,
        SelectService = 4,
        SelectStaffAndSave = 5,
        DeleteAppointment = 10,
        ModifyAppointmentInit = 20,
        ModifyAppointmentOptions = 21,
        RegisterName = 99
    }

    public class ChatMessage
    {
        public string Sender { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsTyping { get; set; } = false;
        public List<string> QuickReplies { get; set; } = new();
        /// <summary>
        /// When true, QuickReplies contain appointment IDs and should be rendered
        /// as ID pill buttons (append-to-input for delete, send immediately for modify).
        /// </summary>
        public bool IsIdSelection { get; set; } = false;
        public bool AllowsMultipleAppointmentSelection { get; set; } = false;
    }

    public class ChatResponse
    {
        public string Message { get; set; } = string.Empty;
        public List<string> QuickReplies { get; set; } = new();
        public string OptionsTitle { get; set; } = "Options:";
        /// <summary>
        /// Optional preceding messages rendered as separate chat bubbles (no quick replies).
        /// Used e.g. when cancelling multiple appointments so each confirmation is its own bubble.
        /// </summary>
        public List<string> AdditionalMessages { get; set; } = new();
        /// <summary>
        /// When true, QuickReplies contain appointment IDs to render as ID pill buttons.
        /// </summary>
        public bool IsIdSelection { get; set; } = false;
        /// <summary>
        /// When true, appointment IDs can be collected for a multi-appointment action such as cancellation.
        /// </summary>
        public bool AllowsMultipleAppointmentSelection { get; set; } = false;
    }

    public class AppointmentContext
    {
        public string? ModifyingAppointmentId { get; set; }
        public string? BranchId { get; set; }
        public string? BranchName { get; set; }
        public DateTime? Date { get; set; }
        public string? TimeSlot { get; set; }
        public string? ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public string? StaffId { get; set; }
        public string? StaffName { get; set; }

        public string CustomerId { get; set; } = string.Empty;
        public string? CustomerName { get; set; }

        public string Language { get; set; } = "English";
        public ChatStep Step { get; set; } = ChatStep.Greeting;

        // Dynamic GreenAPI Credentials
        public string? GreenApiInstanceId { get; set; }
        public string? GreenApiToken { get; set; }
    }

    public class CustomerAppointment
    {
        public string Id { get; set; } = string.Empty;
        public string BranchId { get; set; } = string.Empty;
        public string ServiceId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string StaffName { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
    }

    //public class DaySchedule
    //{
    //    public string StaffId { get; set; } = string.Empty;
    //    public TimeSpan StartTime { get; set; }
    //    public TimeSpan EndTime { get; set; }
    //}

    public class Customer
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Phone { get; set; } = "";
        public string RaceName { get; set; } = "";
    }

    public class Branch
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
    }

    public class Service
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public int DurationMinutes { get; set; }
    }

    public class Staff
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
    }

}
