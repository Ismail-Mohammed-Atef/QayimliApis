using Qayimli.Core.Entities;
using Qayimli.Core.Specifications.ReviewSpecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qayimli.Core.Specifications
{
    public class ReviewWithFiltrationForCountAsync : BaseSpecifications<Review>
    {


        public ReviewWithFiltrationForCountAsync(ReviewSpecParams specParams)
        : base(P => (string.IsNullOrEmpty(specParams.Search) || P.Title.ToLower().Contains(specParams.Search.ToLower()))
        && (!specParams.ReviewCategoryId.HasValue || P.ReviewCategoryId == specParams.ReviewCategoryId))
        {

        }
    }
}
