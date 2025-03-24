using Qayimli.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qayimli.Repository.Data.Configurations
{
    internal class ReviewConfigurations : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.Property(O => O.ReviewType)
                .HasConversion(OStatus => OStatus.ToString(), OStatus => (ReviewType)Enum.Parse(typeof(ReviewType), OStatus));

            builder.Property(P => P.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(P => P.PostDate)
                .IsRequired();
        }
    }
}
