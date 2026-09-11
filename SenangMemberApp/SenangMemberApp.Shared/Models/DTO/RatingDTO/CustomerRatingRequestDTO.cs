using System;

namespace SenangMemberApp.Shared.Models.DTO.RatingDTO
{
    public class CustomerRatingRequestDTO
    {
        public string ratingID { get; set; } = Guid.NewGuid().ToString();
        public string customerID { get; set; } = string.Empty;
        public string documentID { get; set; } = string.Empty;
        public int rating { get; set; }
        public string comment { get; set; } = string.Empty;
        public DateTime submissionDate { get; set; } = DateTime.UtcNow;
        public bool isGoogleReviewRedirected { get; set; } = false;
        public string branchID { get; set; } = string.Empty;
        public string groupID { get; set; } = string.Empty;
        public string saveAction { get; set; } = "Changed";
        public bool isDirty { get; set; } = true;
    }
}
