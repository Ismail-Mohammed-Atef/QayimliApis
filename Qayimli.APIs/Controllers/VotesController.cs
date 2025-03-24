using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Qayimli.APIs.Dtos.Requests;
using Qayimli.APIs.Dtos.Responses;
using Qayimli.APIs.Errors;
using Qayimli.Core;
using Qayimli.Core.Entities;
using Qayimli.Core.Entities.Identity;
using System.Security.Claims;

namespace Qayimli.APIs.Controllers
{
    
    public class VotesController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public VotesController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<ActionResult<ReviewResponseDto>> AddVote(VoteRequestDto voteRequest)
        {
            string voteOwnerEmail = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.Email) : null;

            var vote = _mapper.Map<Vote>(voteRequest);
            vote.VoteDate = DateTime.Now;
            vote.VoteOwnerEmail = voteOwnerEmail;

            await _unitOfWork.Repository<Vote>().AddAsync(vote);
            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0) return StatusCode(500, "An error occurred while saving the vote.");
            var reviewController = new ReviewsController(_unitOfWork, _mapper, _userManager, null); // Manually create the controller
            var reviewResponse = await reviewController.GetReviewByReviewDetailId(vote.ReviewDetailId);
            if (reviewResponse.Result is NotFoundObjectResult)
            {
                return NotFound(new ApiResponse(404, "Review not found after adding the vote."));
            }

            return Ok(((Microsoft.AspNetCore.Mvc.ObjectResult)reviewResponse.Result).Value);
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ReviewResponseDto>> UpdateVote(int id, VoteRequestDto voteRequest)
        {
            // Get the email of the authenticated user
            string voteOwnerEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(voteOwnerEmail))
            {
                return Unauthorized(new ApiResponse(401, "User is not authorized."));
            }

            // Retrieve the existing vote from the database
            var vote = await _unitOfWork.Repository<Vote>().GetAsync(id);
            if (vote == null)
            {
                return NotFound(new ApiResponse(404, "Vote not found."));
            }

            // Check if the authenticated user is the owner of the vote
            if (vote.VoteOwnerEmail != voteOwnerEmail)
            {
                return Forbid(); // Return 403 Forbidden if the user is not the owner of the vote
            }

            vote.VoteDate = DateTime.Now;

            _unitOfWork.Repository<Vote>().Update(vote);

            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0) return StatusCode(500, "An error occurred while updating the vote.");

            var reviewController = new ReviewsController(_unitOfWork, _mapper, _userManager, null); // Manually create the controller
            var reviewResponse = await reviewController.GetReviewByReviewDetailId(vote.ReviewDetailId);
            if (reviewResponse.Result is NotFoundObjectResult)
            {
                return NotFound(new ApiResponse(404, "Review not found after updating the vote."));
            }

            return Ok(((Microsoft.AspNetCore.Mvc.ObjectResult)reviewResponse.Result).Value);
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteVote(int id)
        {
            var vote = await _unitOfWork.Repository<Vote>().GetAsync(id);
            if (vote == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid Vote"));

            }

            _unitOfWork.Repository<Vote>().Delete(vote);

            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0) return StatusCode(500, "An error occurred while deleting the vote.");

            return Ok();
        }
    }
}
