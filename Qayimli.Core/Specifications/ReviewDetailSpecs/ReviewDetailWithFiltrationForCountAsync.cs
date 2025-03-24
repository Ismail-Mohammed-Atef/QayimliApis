using Qayimli.Core.Entities;
using Qayimli.Core.Specifications.ReviewSpecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qayimli.Core.Specifications
{
    public class ReviewDetailWithFiltrationForCountAsync : BaseSpecifications<ReviewDetail>
    {


        public ReviewDetailWithFiltrationForCountAsync(ReviewDetailSpecParams specParams)
            : base(P =>
                    (string.IsNullOrEmpty(specParams.Search) || P.Review.Title.ToLower().Contains(specParams.Search)))
        {

        }
    }
}
