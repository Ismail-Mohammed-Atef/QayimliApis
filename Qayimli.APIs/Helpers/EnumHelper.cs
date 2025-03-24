using System.Runtime.Serialization;

namespace Qayimli.APIs.Helpers
{
    public static class EnumHelper
    {
        public static TEnum? GetEnumValueFromEnumMember<TEnum>(string value) where TEnum : struct
        {
            var type = typeof(TEnum);
            if (!type.IsEnum) throw new ArgumentException("Type must be an enum");

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                if (attribute != null && attribute.Value == value)
                {
                    return (TEnum)field.GetValue(null);
                }
                else if (field.Name.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    return (TEnum)field.GetValue(null);
                }
            }

            return null; // Return null if no match is found
        }
    }
}