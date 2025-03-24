using AutoMapper;
using Microsoft.Extensions.Configuration;
using Qayimli.APIs.Dtos.Requests;
using Qayimli.APIs.Dtos.Responses;
using Qayimli.Core.Entities;
using Qayimli.Core.Entities.Identity;

namespace Qayimli.APIs.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserAddress, UserAddressResponseDto>();
            CreateMap<UserAddressRequestDto, UserAddress>();

            CreateMap<AppUser, UserResponseDto>()
                .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom<PictureUrlResolver<AppUser, UserResponseDto>>()).ReverseMap();

            CreateMap<Review, ReviewResponseDto>()
                       .ForMember(dest => dest.ReviewType, opt => opt.MapFrom(src => src.ReviewType))
                       .ForMember(dest => dest.ReviewDetails, opt => opt.MapFrom(src => src.ReviewDetails));

            CreateMap<ReviewDetail, ReviewDetailResponseDto>()
                .ForMember(dest => dest.ReviewContent, opt =>
                    opt.MapFrom<PictureUrlResolver<ReviewDetail, ReviewDetailResponseDto>>());

            CreateMap<ReviewDetailRequestDto, ReviewDetail>();
            CreateMap<ReviewRequestDto, Review>()
                .ForMember(dest => dest.ReviewType, opt => opt.MapFrom(src => src.ReviewType.ToString()));

            CreateMap<ReviewCategory, ReviewCategoryResponseDto>();
            CreateMap<ReviewCategoryRequestDto, ReviewCategory>();

            CreateMap<Vote, VoteResponseDto>();
            CreateMap<VoteRequestDto, Vote>();

            CreateMap<ContactUsEmailRequestDTO, ContactUsEmail>();
        }
    }
}