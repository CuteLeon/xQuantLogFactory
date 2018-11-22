﻿using System.Collections.Generic;

namespace xQuantLogFactory.Model.Monitor
{
    /// <summary>
    /// 监视规则接口
    /// </summary>
    public interface IMonitor
    {
        /// <summary>
        /// 规则名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 监控项目树根节点列表
        /// </summary>
        List<MonitorItem> MonitorTreeRoots { get; set; }

        /// <summary>
        /// 是否有子监控项目
        /// </summary>
        bool HasChildren { get; }
    }
}
