using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qayimli.Core.Entities
{
    public class Review : BaseEntity
    {
        public string Title { get; set; }
        public string ReviewOwnerEmail { get; set; }
        public DateTime PostDate { get; set; } = DateTime.Now;
        public ReviewType ReviewType { get; set; }
        public ICollection<ReviewDetail> ReviewDetails { get; set; } = new HashSet<ReviewDetail>();
        public int ReviewCategoryId { get; set; }
        public ReviewCategory ReviewCategory { get; set; }
    }
}
