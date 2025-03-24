using Microsoft.EntityFrameworkCore;
using Qayimli.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qayimli.Core.Specifications.ReviewDetailSpecs
{
    public class ReviewDetailSpecifications : BaseSpecifications<ReviewDetail>
    {
        // This Constructor will be Used for Creating an Object, That will be Used to Get All Reviews 
        public ReviewDetailSpecifications(ReviewDetailSpecParams specParams)
        : base(P => (string.IsNullOrEmpty(specParams.Search) || P.Review.Title.ToLower().Contains(specParams.Search.ToLower())
        && (!specParams.ReviewCategoryId.HasValue || P.Review.ReviewCategoryId == specParams.ReviewCategoryId)))
        {
            AddIncludes();
            AddSort(specParams.Sort);
            AddPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
        }
        public ReviewDetailSpecifications(int id)
        : base(P => P.Id == id)
        {
            AddIncludes();
        }
        public void AddSort(string? Sort)
        {
            if (!string.IsNullOrEmpty(Sort))
            {
                switch (Sort)
                {
                    default:
                        AddOrderBy(P => P.ReviewId);
                        break;
                }
            }
        }
        private void AddIncludes()
        {
            AddInclude(q => q.Include(rd => rd.Votes));
        }
    }
}
