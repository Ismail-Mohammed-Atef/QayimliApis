using Qayimli.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Qayimli.APIs.Dtos.Requests
{
    public class ReviewDetailRequestDto
    {
        public string ReviewContent { get; set; }
        public string? Description { get; set; }
   //     public IFormFile? ImageFile { get; set; }
    }
}
