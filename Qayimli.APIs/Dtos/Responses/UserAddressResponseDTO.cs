using Qayimli.Core.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace Qayimli.APIs.Dtos.Responses
{
    public class UserAddressResponseDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
