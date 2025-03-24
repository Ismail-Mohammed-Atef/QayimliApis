using System.ComponentModel.DataAnnotations;

namespace Qayimli.APIs.Dtos.Requests
{
    public class ReviewCategoryRequestDto
    {

        [Required]
        [MaxLength(40)]
        public string NameEN { get; set; }

        [Required]
        public string NameAR { get; set; }
    }
}
