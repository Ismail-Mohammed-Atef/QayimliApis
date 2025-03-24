using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Qayimli.APIs.Dtos.Requests;
using Qayimli.APIs.Helpers;
using Qayimli.Core;
using Qayimli.Core.Entities;
using Qayimli.Core.Specifications.ReviewSpecs;
using Qayimli.Core.Specifications;
using Qayimli.Repository.Data;
using Qayimli.APIs.Errors;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Qayimli.APIs.Dtos.Responses;

namespace Qayimli.APIs.Controllers
{
   
    public class ReviewCategoryController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewCategoryController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReviewCategoryResponseDto>> AddNewReviewCategory(ReviewCategoryRequestDto model)
        {

            var reviewOwnerEmail = User.FindFirstValue(ClaimTypes.Email);
            if (reviewOwnerEmail == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid Email"));
            }

            var reviewcategory = _mapper.Map<ReviewCategory>(model);
            reviewcategory.NameEN = model.NameEN;
            reviewcategory.NameAR = model.NameAR;

            await _unitOfWork.Repository<ReviewCategory>().AddAsync(reviewcategory);
            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0) return StatusCode(500, "An error occurred while saving the ReviewCategory.");

            var reviewcategoryDto = _mapper.Map<ReviewCategoryResponseDto>(reviewcategory);
            return Ok(reviewcategoryDto);
        }


        // GET: api/reviewcategory
        [HttpGet]
        public async Task<ActionResult<Pagination<ReviewCategoryResponseDto>>> GetAllReviewCategories([FromQuery] ReviewCategorySpecParams specParams)
        {
            var spec = new ReviewCategorySpecifications(specParams);
            var reviewCategories = await _unitOfWork.Repository<ReviewCategory>().GetAllAsyncWithSpecAsync(spec);

            var countSpec = new ReviewCategoryWithFiltrationForCountAsync(specParams);
            var count = await _unitOfWork.Repository<ReviewCategory>().GetCountAsyncWithSpecAsync(countSpec);

            var reviewCategoryDtos = reviewCategories.Select(rc =>
            {
                var dto = _mapper.Map<ReviewCategoryResponseDto>(rc);
                dto.Name = RequestHeaderHelper.GetLanguageFromHeader(Request) == "ar" ? dto.NameAR : dto.NameEN; // Set the Name property
                return dto;
            }).ToList();
            return Ok(new Pagination<ReviewCategoryResponseDto>(specParams.PageIndex, specParams.PageSize, reviewCategoryDtos, count));
        }

        // GET: api/reviewcategory/1
        [HttpGet("{Id}")]
        public async Task<ActionResult<ReviewCategoryResponseDto>> GetReviewCategoryById(int Id)
        {
            var spec = new ReviewCategorySpecifications(Id);
            var reviewCategory = await _unitOfWork.Repository<ReviewCategory>().GetEntityWithSpecAsync(spec);

            if (reviewCategory == null)
                return NotFound(new ApiResponse(404, "Invalid ReviewCategory Id"));

            var reviewcategoryDto = _mapper.Map<ReviewCategoryResponseDto>(reviewCategory);
            reviewcategoryDto.Name = RequestHeaderHelper.GetLanguageFromHeader(Request) == "ar" ? reviewcategoryDto.NameAR : reviewcategoryDto.NameEN;
            return Ok(reviewcategoryDto);
        }
    }
}
