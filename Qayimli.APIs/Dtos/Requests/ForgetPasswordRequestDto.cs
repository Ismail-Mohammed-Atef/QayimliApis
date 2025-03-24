using Qayimli.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Qayimli.APIs.Dtos.Requests
{
    public class ForgetPasswordRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
