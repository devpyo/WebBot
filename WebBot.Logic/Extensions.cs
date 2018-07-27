using System;
using System.Linq;
using System.Reflection;

namespace WebBot.Logic
{
    public static class CommonExtensions
    {
        public static AttributeType GetAttribute<AttributeType>(this object thisObject) where AttributeType : Attribute
        {
            return GetAttribute<AttributeType>(thisObject.GetType());
        }

        public static AttributeType GetAttribute<AttributeType>(this Type thisType) where AttributeType : Attribute
        {
            return thisType.GetCustomAttributes(typeof(AttributeType), inherit: false).FirstOrDefault() as AttributeType;
        }

        public static AttributeType GetAttribute<AttributeType>(this FieldInfo thisInfo) where AttributeType : Attribute
        {
            return thisInfo.GetCustomAttribute<AttributeType>(inherit: false);
        }
    }
}
