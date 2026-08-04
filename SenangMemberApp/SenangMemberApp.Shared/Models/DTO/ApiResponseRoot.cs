using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models.DTO
{
    public class ApiResponseRoot<T>
    {
        public int statusCode { get; set; }
        public string message { get; set; } = string.Empty;
        public T? result { get; set; }
    }
}
