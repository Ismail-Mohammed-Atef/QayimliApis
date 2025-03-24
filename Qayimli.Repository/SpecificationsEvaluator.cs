using Microsoft.EntityFrameworkCore;
using Qayimli.Core.Entities;
using Qayimli.Core.Specifications;
using System.Linq;

namespace Qayimli.Repository
{
    internal static class SpecificationsEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecifications<TEntity> spec)
        {
            var query = inputQuery;

            // Apply Criteria
            if (spec.Criteria is not null)
                query = query.Where(spec.Criteria);

            // Apply Includes and ThenIncludes
            foreach (var include in spec.Includes)
            {
                query = include(query);
            }

            // Apply OrderBy and OrderByDescending
            if (spec.OrderBy is not null)
                query = query.OrderBy(spec.OrderBy);

            if (spec.OrderByDescending is not null)
                query = query.OrderByDescending(spec.OrderByDescending);

            // Apply Pagination
            if (spec.IsPaginationEnabled)
                query = query.Skip(spec.Skip).Take(spec.Take);

            return query;
        }
    }
}
