using System.ComponentModel.DataAnnotations;

namespace Qayimli.APIs.Dtos.Requests
{
    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression("^(?=.*[a-zA-Z])(?=.*\\d).{8,}$", ErrorMessage = "the password contains both letters and numbers and has a minimum length of 8 characters. You can modify the minimum length or other requirements to suit your specific needs.")]
        public string Password { get; set; }
    }
    public class GoogleLoginRequest
    {
        public string Token { get; set; }
    }
}
