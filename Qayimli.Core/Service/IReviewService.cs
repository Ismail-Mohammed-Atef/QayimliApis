using Qayimli.Core.Entities;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qayimli.Core.Service
{
    public interface IReviewService
    {
        Task<Review?> CreateReviewAsync(string buyerEmail);
        Task<IReadOnlyList<Review>> GetReviewsForSpecificUserAsync(string buyerEmail);
        Task<Review> GetReviewByIdForSpecificUserAsync(string buyerEmail, int reviewId);
    }
}
