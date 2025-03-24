using Qayimli.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Qayimli.APIs.Dtos.Requests
{
    public class ReviewRequestDto
    {
        [Required]
        [MaxLength(40)]
        public string Title { get; set; }
        [Required]
        public string ReviewType { get; set; }
        public List<ReviewDetailRequestDto> ReviewDetails { get; set; }
        public int ReviewCategoryId { get; set; }
    }
}
