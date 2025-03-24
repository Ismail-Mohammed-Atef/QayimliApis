using Qayimli.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Qayimli.APIs.Dtos.Requests
{
    public class VoteRequestDto
    {
        public double Rate { get; set; }
        public string? Comment { get; set; }
        public int ReviewDetailId { get; set; }
    }
}
