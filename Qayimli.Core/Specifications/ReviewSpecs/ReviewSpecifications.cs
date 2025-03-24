using Microsoft.EntityFrameworkCore;
using Qayimli.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qayimli.Core.Specifications.ReviewSpecs
{
    public class ReviewSpecifications : BaseSpecifications<Review>
    {
        // This Constructor will be Used for Creating an Object, That will be Used to Get All Reviews 
        public ReviewSpecifications(ReviewSpecParams specParams)
        : base(P => (string.IsNullOrEmpty(specParams.Search) || P.Title.ToLower().Contains(specParams.Search.ToLower()))
        && (!specParams.ReviewCategoryId.HasValue || P.ReviewCategoryId == specParams.ReviewCategoryId))
        {
            AddIncludes();
            AddSort(specParams.Sort);
            AddPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
        }
        public ReviewSpecifications(int id)
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
                    case "PostDateAsc":
                        AddOrderBy(P => P.PostDate);
                        break;
                    case "PostDateDesc":
                        AddOrderByDescending(P => P.PostDate);
                        break;
                    case "TitleAsc":
                        AddOrderBy(P => P.Title);
                        break;
                    case "TitleDesc":
                        AddOrderByDescending(P => P.Title);
                        break;
                    default:
                        AddOrderBy(P => P.PostDate);
                        break;
                }
            }
            else
                AddOrderBy(P => P.Title);
        }
        private void AddIncludes()
        {
            AddInclude(q => q.Include(r => r.ReviewDetails).ThenInclude(rd => rd.Votes));
            AddInclude(q => q.Include(r => r.ReviewCategory));
        }
    }
}
