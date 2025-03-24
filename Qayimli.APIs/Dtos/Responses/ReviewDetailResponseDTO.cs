using Qayimli.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Qayimli.APIs.Dtos.Responses
{
    public class ReviewDetailResponseDto
    {
        public int Id { get; set; }
        public string ReviewContent { get; set; }
        public string? Description { get; set; }
        public List<VoteResponseDto>? Votes{ get; set; }
        public double AverageRate => Votes != null && Votes.Any()
                                 ? Votes.Average(vote => vote.Rate)
                                 : 0;
    }
}
