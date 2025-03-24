using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qayimli.Core.Entities
{
    public class Vote : BaseEntity
    {
        public string? VoteOwnerEmail {  get; set; }   
        public double Rate {  get; set; }
        public string? Comment { get; set; }
        public DateTime VoteDate { get; set; } = DateTime.Now;
        public int ReviewDetailId { get; set; }
        public ReviewDetail ReviewDetail { get; set; }
    }
}
