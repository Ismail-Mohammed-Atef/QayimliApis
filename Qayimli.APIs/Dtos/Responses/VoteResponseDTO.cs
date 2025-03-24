using Qayimli.Core.Entities;
using Qayimli.Core.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace Qayimli.APIs.Dtos.Responses
{
    public class VoteResponseDto
    {
        public int Id { get; set; }
        public UserResponseDto VoteOwner { get; set; }
        public double Rate { get; set; }
        public string? Comment { get; set; }
        public DateTime VoteDate { get; set; }
        public int ReviewDetailId { get; set; }
    }
}
