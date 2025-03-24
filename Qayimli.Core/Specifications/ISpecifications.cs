using Microsoft.EntityFrameworkCore.Query;
using Qayimli.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Qayimli.Core.Specifications
{
    public interface ISpecifications<T> where T : BaseEntity
    {
        public Expression<Func<T,bool >> Criteria { get; set; }// P => P.Id=id
        List<Func<IQueryable<T>, IIncludableQueryable<T, object>>> Includes { get; }
        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDescending { get; set; }

        public int Take { get; set; }

        public int Skip { get; set; }

        public bool IsPaginationEnabled { get; set; }
    }
}
