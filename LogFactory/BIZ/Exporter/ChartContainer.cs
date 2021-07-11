using System;
using System.Text;

using LogFactory.Model;

namespace LogFactory.BIZ.Exporter
{
    /// <summary>
    /// 图表容器类
    /// </summary>
    public class ChartContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartContainer"/> class.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="target"></param>
        /// <param name="render"></param>
        public ChartContainer(string text, string target, Action<StringBuilder, TaskArgument> render)
        {
            this.Text = text;
            this.Target = target;
            this.Render = render;
        }

        /// <summary>
        /// Gets or sets 文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets 目标
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Gets or sets 渲染方法
        /// </summary>
        public Action<StringBuilder, TaskArgument> Render { get; set; }
    }
}
