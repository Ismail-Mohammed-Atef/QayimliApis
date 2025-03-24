using Qayimli.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Qayimli.APIs.Dtos.Responses
{
    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ReviewType { get; set; }
        public DateTime PostDate { get; set; }
        public List<ReviewDetailResponseDto> ReviewDetails { get; set; }
        public UserResponseDto ReviewOwner { get; set; }
        public ReviewCategoryResponseDto ReviewCategory { get; set; }
    }
}
