using System;
using System.ComponentModel;
using System.Reflection;

namespace xQuantLogFactory.Utils.Extensions
{
    /// <summary>
    /// 枚举扩展类
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获取环境值
        /// </summary>
        /// <param name="value">枚举项</param>
        /// <returns></returns>
        public static string GetAmbientValue(this Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            AmbientValueAttribute[] attributes = (AmbientValueAttribute[])fieldInfo.GetCustomAttributes(typeof(AmbientValueAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Value.ToString() : string.Empty;
        }
    }
}
