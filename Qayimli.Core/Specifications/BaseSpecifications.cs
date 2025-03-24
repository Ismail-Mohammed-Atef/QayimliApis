using Microsoft.EntityFrameworkCore.Query;
using Qayimli.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Qayimli.Core.Specifications
{
    public class BaseSpecifications<T> : ISpecifications<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> Criteria { get; set; } = null;
        public List<Func<IQueryable<T>, IIncludableQueryable<T, object>>> Includes { get; set; } = new List<Func<IQueryable<T>, IIncludableQueryable<T, object>>>();
        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDescending { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool IsPaginationEnabled { get; set; }

        public BaseSpecifications()
        {
            //  Criteria = null
        }

        public BaseSpecifications(Expression<Func<T, bool>> criteriaExp)
        {
            Criteria = criteriaExp;
        }

        public void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        public void AddOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }

        public void AddPagination(int skip, int take)
        {
            Take = take;
            Skip = skip;
            IsPaginationEnabled = true;
        }

        public void AddInclude(Func<IQueryable<T>, IIncludableQueryable<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
    }
}
