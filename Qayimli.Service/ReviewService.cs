using Qayimli.Core;
using Qayimli.Core.Entities;
using Qayimli.Core.Service;
using Qayimli.Core.Specifications.ReviewSpecs;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qayimli.Service
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Task<Review?> CreateReviewAsync(string buyerEmail)
        {
            throw new NotImplementedException();
        }

        public Task<Review> GetReviewByIdForSpecificUserAsync(string buyerEmail, int reviewId)
        {
          //  var spec = new ReviewSpecifications(buyerEmail, reviewId);
         //  var reviews = _unitOfWork.Repository<Review>().GetEntityWithSpecAsync(spec);
            return null;
        }

        public Task<IReadOnlyList<Review>> GetReviewsForSpecificUserAsync(string buyerEmail)
        {
            throw new NotImplementedException();
        }
    }
}
