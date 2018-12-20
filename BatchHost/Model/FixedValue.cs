namespace BatchHost.Model
{
    public static class FixedValue
    {

        /// <summary>
        /// 时间单位
        /// </summary>
        public enum TimeUnits
        {
            /// <summary>
            /// 天
            /// </summary>
            Day = 0,
            /// <summary>
            /// 小时
            /// </summary>
            Hour = 1,
            /// <summary>
            /// 分钟
            /// </summary>
            Minute = 2,
        }

        /// <summary>
        /// 日志等级
        /// </summary>
        public enum LogLevels
        {
            /// <summary>
            /// Debug
            /// </summary>
            Debug = 0,

            /// <summary>
            /// Trace
            /// </summary>
            Trace = 1,

            /// <summary>
            /// Warn
            /// </summary>
            Warn = 2,

            /// <summary>
            /// Error
            /// </summary>
            Error = 3,

            /// <summary>
            /// Pref
            /// </summary>
            Pref = 4,
        }

        /// <summary>
        /// 界面状态
        /// </summary>
        public enum PageStates
        {
            /// <summary>
            /// 准备状态
            /// </summary>
            Prepare = 0,
            /// <summary>
            /// 工作状态
            /// </summary>
            Working = 1,
            /// <summary>
            /// 取消状态
            /// </summary>
            Cancel = 2,
            /// <summary>
            /// 任务结束
            /// </summary>
            Finish = 3,
        }

    }
}
