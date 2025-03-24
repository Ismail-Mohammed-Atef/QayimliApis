using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qayimli.Core.Specifications
{
    public class ReviewDetailSpecParams
    {
        public string? Sort { get; set; }

        public int? ReviewCategoryId;

        private string? search;

        public string? Search
        {
            get { return search; }
            set { search = value?.ToLower(); }
        }


        private int pageSize = 20;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = pageSize > 20 ? 20 : value; }
        }

        public int PageIndex { get; set; } = 1;
    }
}
