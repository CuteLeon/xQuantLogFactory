using LogFactory.Model.Fixed;

namespace BatchHost.Model
{
    /// <summary>
    /// 预设任务
    /// </summary>
    public class PresetTask
    {
        /// <summary>
        /// 预设任务
        /// </summary>
        /// <param name="name"></param>
        /// <param name="logLevels"></param>
        /// <param name="monitors"></param>
        public PresetTask(string name, LogLevels logLevels, string[] monitors)
        {
            this.Name = name;
            this.LogLevel = logLevels;
            this.Monitors = monitors;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 日志等级
        /// </summary>
        public LogLevels LogLevel { get; set; }

        /// <summary>
        /// 监视规则文件名称集合
        /// </summary>
        public string[] Monitors { get; set; }
    }
}
