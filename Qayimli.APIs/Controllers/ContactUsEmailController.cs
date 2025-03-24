using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qayimli.APIs.Dtos.Requests;
using Qayimli.Core;

namespace Qayimli.APIs.Controllers
{
    public class ContactUsEmailController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ContactUsEmailController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<IActionResult> AddContactUsEmail(ContactUsEmailRequestDTO model)
        {
            var contactUsEmail = _mapper.Map<Qayimli.Core.Entities.ContactUsEmail>(model);
            await _unitOfWork.Repository<Qayimli.Core.Entities.ContactUsEmail>().AddAsync(contactUsEmail);
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0) return StatusCode(500, "An error occurred while saving the ContactUsEmail.");
            return Ok();
        }

    }
    
}
