using Qayimli.Core.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace Qayimli.APIs.Dtos.Requests
{
    public class UserAddressRequestDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
    }
}
