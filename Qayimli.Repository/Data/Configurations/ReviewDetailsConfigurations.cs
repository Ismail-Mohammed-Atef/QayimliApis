using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qayimli.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qayimli.Repository.Data.Configurations
{
    public class ReviewDetailsConfigurations : IEntityTypeConfiguration<ReviewDetail>
    {
        public void Configure(EntityTypeBuilder<ReviewDetail> builder)
        {
            builder.Property(P => P.ReviewContent)
                .IsRequired();

            builder.HasOne(P => P.Review)
               .WithMany(R => R.ReviewDetails)
               .HasForeignKey(P => P.ReviewId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
