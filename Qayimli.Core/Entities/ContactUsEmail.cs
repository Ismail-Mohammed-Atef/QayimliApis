using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qayimli.Core.Entities
{
    public class ContactUsEmail: BaseEntity
    {
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Message { get; set; }
        public DateTime MessageDate { get; set; }= DateTime.Now;

    }
}
