using System.Collections.Generic;

namespace xQuantLogFactory.Model.Monitor
{
    /// <summary>
    /// 监视规则容器接口
    /// </summary>
    public interface IMonitorContainer
    {
        /// <summary>
        /// 初始化客户端和服务端监视规则树
        /// </summary>
        void InitTerminalMonitorTree();

        /// <summary>
        /// 初始化Performance监视规则树
        /// </summary>
        void InitPerformanceMonitorTree();

        /// <summary>
        /// 扫描客户端和服务端监视规则树
        /// </summary>
        /// <returns></returns>
        IEnumerable<TerminalMonitorItem> GetTerminalMonitorItems();

        /// <summary>
        /// 扫描Performance监视规则树
        /// </summary>
        /// <returns></returns>
        IEnumerable<PerformanceMonitorItem> GetPerformanceMonitorItems();
    }
}
