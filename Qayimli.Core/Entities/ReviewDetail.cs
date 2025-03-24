using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qayimli.Core.Entities
{
    public class ReviewDetail : BaseEntity
    {
        [DataType(DataType.Text)]
        public string ReviewContent { get; set; }
        public string? Description { get; set; }
        public int ReviewId { get; set; }
        public Review Review { get; set; }
        public ICollection<Vote> Votes { get; set; } = new HashSet<Vote>();
    }
}
