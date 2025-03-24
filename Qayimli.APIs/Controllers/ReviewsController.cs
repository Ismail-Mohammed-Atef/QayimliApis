using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Qayimli.APIs.Dtos.Requests;
using Qayimli.APIs.Dtos.Responses;
using Qayimli.APIs.Errors;
using Qayimli.APIs.Helpers;
using Qayimli.Core;
using Qayimli.Core.Entities;
using Qayimli.Core.Entities.Identity;
using Qayimli.Core.Service;
using Qayimli.Core.Specifications;
using Qayimli.Core.Specifications.ReviewDetailSpecs;
using Qayimli.Core.Specifications.ReviewSpecs;
using Qayimli.Service;
using System.Security.Claims;

namespace Qayimli.APIs.Controllers
{
    public class ReviewsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper; 
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileService _fileService;

        public ReviewsController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _fileService = fileService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReviewResponseDto>> AddNewReview(ReviewRequestDto model)
        {
            var reviewType = EnumHelper.GetEnumValueFromEnumMember<ReviewType>(model.ReviewType);
            if (reviewType == null)
            {
                return BadRequest(new ApiResponse(400, "There is no review with that type"));
            }
            if (reviewType.Value == ReviewType.ImageFile)
            {
                string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];
                foreach (var detail in model.ReviewDetails)
                {
                    if (string.IsNullOrEmpty(detail.ReviewContent))
                    {
                        return BadRequest(new ApiResponse(400, "Error in Image File"));
                    }
                    if (detail.ReviewContent.Length > 3 * 1024 * 1024 * 4 / 3)
                    {
                        return BadRequest(new ApiResponse(400, "File size should not exceed 3 MB"));
                    }
                    string createdImageName = await _fileService.SaveBase64FileAsync(detail.ReviewContent, allowedFileExtentions, "Images/ReviewDetails");
                    detail.ReviewContent = createdImageName;
                }
            }
            var reviewOwnerEmail = User.FindFirstValue(ClaimTypes.Email);
            if (reviewOwnerEmail == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid Email"));
            }

            var review = _mapper.Map<Review>(model);
            review.PostDate = DateTime.Now;
            review.ReviewOwnerEmail = reviewOwnerEmail;
            review.ReviewType = reviewType.Value;

            await _unitOfWork.Repository<Review>().AddAsync(review);
            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0) return StatusCode(500, "An error occurred while saving the review.");

            var reviewDto = _mapper.Map<ReviewResponseDto>(review);
            return Ok(reviewDto);
        }

        [ProducesResponseType(typeof(ReviewResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<Pagination<ReviewResponseDto>>> GetAllReviews([FromQuery] ReviewSpecParams specParams)
        {
            var spec = new ReviewSpecifications(specParams);
            var reviews = await _unitOfWork.Repository<Review>().GetAllAsyncWithSpecAsync(spec);

            var countSpec = new ReviewWithFiltrationForCountAsync(specParams);
            var count = await _unitOfWork.Repository<Review>().GetCountAsyncWithSpecAsync(countSpec);
            var reviewDtos = new List<ReviewResponseDto>();
            foreach (var review in reviews)
            {
                var user = await _userManager.FindByEmailAsync(review.ReviewOwnerEmail); 

                var reviewDto = _mapper.Map<ReviewResponseDto>(review);

                reviewDto.ReviewOwner = _mapper.Map<UserResponseDto>(user);
                reviewDto.ReviewCategory.Name = RequestHeaderHelper.GetLanguageFromHeader(Request) == "ar" ? reviewDto.ReviewCategory.NameAR : reviewDto.ReviewCategory.NameEN;
                foreach (var reviewDetail in review.ReviewDetails)
                {
                    // Collect votes to replace the original ones
                    var updatedVotes = new List<VoteResponseDto>();

                    foreach (var vote in reviewDetail.Votes)
                    {
                        if (!string.IsNullOrEmpty(vote.VoteOwnerEmail))
                        {
                            var voteOwner = await _userManager.FindByEmailAsync(vote.VoteOwnerEmail);
                            if (voteOwner != null)
                            {
                                // Map the vote to VoteResponseDto
                                var voteDto = _mapper.Map<VoteResponseDto>(vote);
                                voteDto.VoteOwner = _mapper.Map<UserResponseDto>(voteOwner); // Map VoteOwner

                                // Add the updated voteDto to the list of updated votes
                                updatedVotes.Add(voteDto);
                            }
                        }
                        else
                        {
                            // If no VoteOwner, map and keep the original vote
                            updatedVotes.Add(_mapper.Map<VoteResponseDto>(vote));
                        }
                    }

                    // Now that we're done iterating, replace the votes in the DTO
                    var reviewDetailDto = reviewDto.ReviewDetails.FirstOrDefault(rd => rd.Id == reviewDetail.Id);
                    if (reviewDetailDto != null)
                    {
                        // Clear the existing votes to avoid duplicates
                        reviewDetailDto.Votes.Clear();

                        // Add the updated votes back in
                        reviewDetailDto.Votes.AddRange(updatedVotes);
                    }
                }

                reviewDtos.Add(reviewDto);
            }

            return Ok(new Pagination<ReviewResponseDto>(specParams.PageIndex, specParams.PageSize, reviewDtos, count));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewResponseDto>> GetReviewById(int id)
        {
            // Step 1: Fetch the review by its ID using the ReviewSpecifications
            var spec = new ReviewSpecifications(id);
            var review = await _unitOfWork.Repository<Review>().GetEntityWithSpecAsync(spec);

            // Step 2: Return a 404 response if the review is not found
            if (review == null)
                return NotFound(new ApiResponse(404, "Invalid Review Id"));

            // Step 3: Fetch the user who owns the review
            var user = await _userManager.FindByEmailAsync(review.ReviewOwnerEmail);
            if (user == null)
                return NotFound(new ApiResponse(404, "User not found"));

            // Step 4: Map the review to a ReviewResponseDto
            var reviewDto = _mapper.Map<ReviewResponseDto>(review);

            // Step 5: Map the ReviewOwner (user) to the response DTO
            reviewDto.ReviewOwner = _mapper.Map<UserResponseDto>(user);

            // Step 6: Set the appropriate language for the ReviewCategory name
            reviewDto.ReviewCategory.Name = RequestHeaderHelper.GetLanguageFromHeader(Request) == "ar"
                ? reviewDto.ReviewCategory.NameAR
                : reviewDto.ReviewCategory.NameEN;

            // Step 7: Loop through each ReviewDetail and its Votes to map VoteOwner and add it to the response
            foreach (var reviewDetail in review.ReviewDetails)
            {
                // Collect votes to replace the original ones
                var updatedVotes = new List<VoteResponseDto>();

                foreach (var vote in reviewDetail.Votes)
                {
                    if (!string.IsNullOrEmpty(vote.VoteOwnerEmail))
                    {
                        var voteOwner = await _userManager.FindByEmailAsync(vote.VoteOwnerEmail);
                        if (voteOwner != null)
                        {
                            // Map the vote to VoteResponseDto
                            var voteDto = _mapper.Map<VoteResponseDto>(vote);
                            voteDto.VoteOwner = _mapper.Map<UserResponseDto>(voteOwner); // Map VoteOwner

                            // Add the updated voteDto to the list of updated votes
                            updatedVotes.Add(voteDto);
                        }
                    }
                    else
                    {
                        // If no VoteOwner, map and keep the original vote
                        updatedVotes.Add(_mapper.Map<VoteResponseDto>(vote));
                    }
                }

                // Now that we're done iterating, replace the votes in the DTO
                var reviewDetailDto = reviewDto.ReviewDetails.FirstOrDefault(rd => rd.Id == reviewDetail.Id);
                if (reviewDetailDto != null)
                {
                    // Clear the existing votes to avoid duplicates
                    reviewDetailDto.Votes.Clear();

                    // Add the updated votes back in
                    reviewDetailDto.Votes.AddRange(updatedVotes);
                }
            }

            // Step 8: Return the fully mapped review DTO
            return Ok(reviewDto);
        }

        [HttpGet("detail/{id}")]
        public async Task<ActionResult<ReviewResponseDto>> GetReviewByReviewDetailId(int id)
        {
            // Step 1: Fetch the ReviewDetail by its ID using ReviewDetailSpecifications (or equivalent)
            var reviewDetailSpec = new ReviewDetailSpecifications(id); // You would need to create this specification if not already present
            var getReviewDetail = await _unitOfWork.Repository<ReviewDetail>().GetEntityWithSpecAsync(reviewDetailSpec);

            // Step 2: Return a 404 response if the review detail is not found
            if (getReviewDetail == null)
                return NotFound(new ApiResponse(404, "Invalid Review Detail Id"));
            // Step 1: Fetch the review by its ID using the ReviewSpecifications
            var spec = new ReviewSpecifications(getReviewDetail.ReviewId);
            var review = await _unitOfWork.Repository<Review>().GetEntityWithSpecAsync(spec);

            // Step 2: Return a 404 response if the review is not found
            if (review == null)
                return NotFound(new ApiResponse(404, "Invalid Review Id"));

            // Step 3: Fetch the user who owns the review
            var user = await _userManager.FindByEmailAsync(review.ReviewOwnerEmail);
            if (user == null)
                return NotFound(new ApiResponse(404, "User not found"));

            // Step 4: Map the review to a ReviewResponseDto
            var reviewDto = _mapper.Map<ReviewResponseDto>(review);

            // Step 5: Map the ReviewOwner (user) to the response DTO
            reviewDto.ReviewOwner = _mapper.Map<UserResponseDto>(user);

            // Step 6: Set the appropriate language for the ReviewCategory name
            reviewDto.ReviewCategory.Name = RequestHeaderHelper.GetLanguageFromHeader(Request) == "ar"
                ? reviewDto.ReviewCategory.NameAR
                : reviewDto.ReviewCategory.NameEN;

            // Step 7: Loop through each ReviewDetail and its Votes to map VoteOwner and add it to the response
            foreach (var reviewDetail in review.ReviewDetails)
            {
                // Collect votes to replace the original ones
                var updatedVotes = new List<VoteResponseDto>();

                foreach (var vote in reviewDetail.Votes)
                {
                    if (!string.IsNullOrEmpty(vote.VoteOwnerEmail))
                    {
                        var voteOwner = await _userManager.FindByEmailAsync(vote.VoteOwnerEmail);
                        if (voteOwner != null)
                        {
                            // Map the vote to VoteResponseDto
                            var voteDto = _mapper.Map<VoteResponseDto>(vote);
                            voteDto.VoteOwner = _mapper.Map<UserResponseDto>(voteOwner); // Map VoteOwner

                            // Add the updated voteDto to the list of updated votes
                            updatedVotes.Add(voteDto);
                        }
                    }
                    else
                    {
                        // If no VoteOwner, map and keep the original vote
                        updatedVotes.Add(_mapper.Map<VoteResponseDto>(vote));
                    }
                }

                // Now that we're done iterating, replace the votes in the DTO
                var reviewDetailDto = reviewDto.ReviewDetails.FirstOrDefault(rd => rd.Id == reviewDetail.Id);
                if (reviewDetailDto != null)
                {
                    // Clear the existing votes to avoid duplicates
                    reviewDetailDto.Votes.Clear();

                    // Add the updated votes back in
                    reviewDetailDto.Votes.AddRange(updatedVotes);
                }
            }

            // Step 8: Return the fully mapped review DTO
            return Ok(reviewDto);
        }

    }
}
