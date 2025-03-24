using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Qayimli.Core.Entities
{
    public enum ReviewType
    {
        [EnumMember(Value = "Image File")]
        ImageFile,
        [EnumMember(Value = "Image Link")]
        ImageLink,
        [EnumMember(Value = "Text")]
        Text
    }
}