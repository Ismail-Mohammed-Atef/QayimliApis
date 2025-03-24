using Qayimli.Core.Entities;
using Qayimli.Core.Specifications.ReviewSpecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qayimli.Core.Specifications
{
    public class ReviewCategoryWithFiltrationForCountAsync : BaseSpecifications<ReviewCategory>
    {


        public ReviewCategoryWithFiltrationForCountAsync(ReviewCategorySpecParams specParams)
            : base(P =>
                    (string.IsNullOrEmpty(specParams.Search) || (P.NameEN.ToLower() + P.NameAR.ToLower()).Contains(specParams.Search)))
        {

        }
    }
}
