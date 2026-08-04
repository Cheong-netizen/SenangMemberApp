using System;
using System.Collections.Generic;
using System.Text;

namespace SenangMemberApp.Shared.Models.DTO.LoginDTO
{
    public class RegisterApiResponseDTO
    {
        public string? Type { get; set; }
        public string? Title { get; set; }
        public int Status { get; set; }
        public string? Instance { get; set; }
        public bool IsError { get; set; }
        public object? Errors { get; set; }
        public object? ValidationErrors { get; set; }
        public string? Details { get; set; }

        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public RegisterResponseDTO? Result { get; set; }
    }
}
