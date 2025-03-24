namespace Qayimli.APIs.Dtos.Responses
{
    public class UserResponseDto
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PictureUrl { get; set; }
        public string Token { get; set; }
    }
}
