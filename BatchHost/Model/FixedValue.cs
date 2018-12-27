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
