using Qayimli.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Qayimli.APIs.Dtos.Requests
{
    public class ResetPasswordRequestDto
    {
        [Required]
        public string Token { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
