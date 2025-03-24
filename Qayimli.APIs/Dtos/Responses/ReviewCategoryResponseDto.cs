using System.ComponentModel.DataAnnotations;

namespace Qayimli.APIs.Dtos.Responses
{
    public class ReviewCategoryResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NameEN { get; set; }
        public string NameAR { get; set; }
    }
}
