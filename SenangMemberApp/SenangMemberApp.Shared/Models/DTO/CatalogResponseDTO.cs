using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SenangMemberApp.Shared.Models.DTO
{
    public class CatalogResponseDTO
    {
        public string id { get; set; }
        public string description { get; set; }
        public string imagePath { get; set; }
    }
}
