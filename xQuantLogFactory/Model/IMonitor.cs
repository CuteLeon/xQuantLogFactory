using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xQuantLogFactory.Model
{
    /// <summary>
    /// 监视接口
    /// </summary>
    public interface IMonitor
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }
    }
}
