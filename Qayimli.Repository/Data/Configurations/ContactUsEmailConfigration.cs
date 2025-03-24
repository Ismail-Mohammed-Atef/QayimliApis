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
    public class ContactUsEmailConfigration : IEntityTypeConfiguration<ContactUsEmail>
    {

        public void Configure(EntityTypeBuilder<ContactUsEmail> builder)
        {
            


    }
    }
}