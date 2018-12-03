using System;
using System.Collections.Generic;
using System.Linq;

using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.Model.Extensions
{
    /// <summary>
    /// 分析结果扩展
    /// </summary>
    /// <remarks>间接实现一种多继承的方案：让多个基类不同的派生类继承同一接口，对接口实现静态扩展方法即可</remarks>
    public static class IAnalysisResultExtension
    {
        /// <summary>
        /// 获取所有子分析结果及其子分析结果
        /// </summary>
        /// <param name="targetResult"></param>
        /// <returns></returns>
        /// <remarks>IEnumerable<>对象即使储存为变量，每次访问依然会进入此方法，若要减少计算量，需要将此方法返回数据 .ToList()</remarks>
        public static IEnumerable<GroupAnalysisResult> GetAnalysisResults(this IAnalysisResult targetResult)
        {
            if (targetResult == null)
            {
                throw new ArgumentNullException(nameof(targetResult));
            }

            Stack<IAnalysisResult> resultRoots = new Stack<IAnalysisResult>();
            IAnalysisResult currentResult = targetResult;
            while (true)
            {
                if (currentResult.HasChildren())
                {
                    foreach (var result in currentResult.AnalysisResultRoots
                        .AsEnumerable().Reverse())
                    {
                        resultRoots.Push(result);
                    }
                }

                if (resultRoots.Count > 0)
                {
                    currentResult = resultRoots.Pop();
                    yield return currentResult as GroupAnalysisResult;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 是否有子监控项目
        /// </summary>
        /// <param name="targetResult"></param>
        /// <returns></returns>
        public static bool HasChildren(this IAnalysisResult targetResult)
        {
            if (targetResult == null)
            {
                throw new ArgumentNullException(nameof(targetResult));
            }

            return targetResult.AnalysisResultRoots != null && targetResult.AnalysisResultRoots.Count > 0;
        }
    }
}
