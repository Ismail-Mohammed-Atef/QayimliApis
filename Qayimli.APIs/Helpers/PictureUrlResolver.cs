using AutoMapper;
using Microsoft.Extensions.Configuration;
using Qayimli.Core.Entities;
using System;

namespace Qayimli.APIs.Helpers
{
    public class PictureUrlResolver<TSource, TDestination> : IValueResolver<TSource, TDestination, string>
    {
        private readonly IConfiguration _configuration;

        public PictureUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(TSource source, TDestination destination, string destMember, ResolutionContext context)
        {
            var pictureUrlValue = "";
            if (source != null)
            {
                if (source is ReviewDetail)
                {
                    var reviewDetail = source as ReviewDetail;
                    if (reviewDetail != null)
                    {
                        if (reviewDetail.ReviewContent != null)
                        {
                            pictureUrlValue = reviewDetail.ReviewContent;
                            if (!string.IsNullOrEmpty(pictureUrlValue) && !pictureUrlValue.StartsWith("https:"))
                            {
                                pictureUrlValue = $"{(reviewDetail.Review.ReviewType == ReviewType.ImageFile ? $"{_configuration["BaseURL"]}/Images/ReviewDetails/" : "")}{pictureUrlValue}";
                            }
                        }
                    }
                }
                else
                {
                    var pictureUrlProperty = source.GetType().GetProperty("PictureUrl");
                    if (pictureUrlProperty != null)
                    {
                        pictureUrlValue = pictureUrlProperty.GetValue(source) as string;
                        if (!string.IsNullOrEmpty(pictureUrlValue) && !pictureUrlValue.StartsWith("https:"))
                        {
                            pictureUrlValue = $"{_configuration["BaseURL"]}/Images/{pictureUrlValue}";
                        }
                    }
                }
            }
            return pictureUrlValue;
        }
    }
}
