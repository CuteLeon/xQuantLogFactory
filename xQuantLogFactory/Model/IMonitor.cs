using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xQuantLogFactory.Model
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
        /// 监控项目列表
        /// </summary>
        List<MonitorItem> MonitorItems { get; set; }

        /// <summary>
        /// 是否有子监控项目
        /// </summary>
        bool HasChildren { get; }
    }
}
