using System;
using System.Linq;
using System.Reflection;

namespace xQuantLogFactory.Utils.Extensions
{
    /// <summary>
    /// 深度克隆
    /// </summary>
    public static class DeepCloneExtension
    {
        /// <summary>
        /// 深度克隆
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetInstance">待克隆实例</param>
        /// <returns></returns>
        public static T DeepClone<T>(this T targetInstance)
        {
            if (targetInstance == null)
            {
                throw new ArgumentNullException(nameof(targetInstance));
            }

            T cloneInstance = Activator.CreateInstance<T>();
            Type targetType = typeof(T);

            // 拷贝属性
            foreach (var property in targetType
                .GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(property => property.GetSetMethod(true) != null))
            {
                property.SetValue(cloneInstance, property.GetValue(targetInstance, null), null);
            }

            return cloneInstance;
        }
    }
}
