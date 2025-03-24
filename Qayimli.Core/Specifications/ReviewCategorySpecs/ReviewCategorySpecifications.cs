using Qayimli.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qayimli.Core.Specifications.ReviewSpecs
{
    public class ReviewCategorySpecifications : BaseSpecifications<ReviewCategory>
    {
        // This Constructor will be Used for Creating an Object, That will be Used to Get All Reviews 
        public ReviewCategorySpecifications(ReviewCategorySpecParams specParams)
        : base(P => (string.IsNullOrEmpty(specParams.Search) || (P.NameEN.ToLower() + P.NameAR.ToLower()).Contains(specParams.Search.ToLower())))
        //&& (!specParams.BrandId.HasValue || P.BrandId == specParams.BrandId)
        //&& (!specParams.CategoryId.HasValue || P.CategoryId == specParams.CategoryId))
        {
            
            AddSort(specParams.Sort);
            AddPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
        }
        public ReviewCategorySpecifications(int id)
        : base(P => P.Id == id)
        {
           
        }
        public void AddSort(string? Sort)
        {
            if (!string.IsNullOrEmpty(Sort))
            {
                switch (Sort)
                {
                    case "NameENAsc":
                        AddOrderBy(P => P.NameEN);
                        break;
                    case "NameENDesc":
                        AddOrderByDescending(P => P.NameEN);
                        break;
                    case "NameARAsc":
                        AddOrderBy(P => P.NameAR);
                        break;
                    case "NameARDesc":
                        AddOrderByDescending(P => P.NameAR);
                        break;
                    default:
                        AddOrderBy(P => P.NameEN);
                        break;
                }
            }
            else
                AddOrderBy(P => P.NameEN);
        }
     
    }
}
