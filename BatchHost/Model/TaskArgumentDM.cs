using System;
using System.Collections.Generic;

using LogFactory.Model.Fixed;

using static BatchHost.Model.FixedValue;

namespace BatchHost.Model
{
    /// <summary>
    /// 任务参数数据模型
    /// </summary>
    public class TaskArgumentDM
    {
        private DateTime? logStartTime;
        private DateTime? logFinishTime;
        private int timeInterval;
        private TimeUnits timeIntervalUnit;

        /// <summary>
        /// 预计生成批处理文件数量变化事件
        /// </summary>
        public event Action<int> BatchesCountChanged;

        /// <summary>
        /// Gets or sets 日志文件目录
        /// </summary>
        public string LogDirectory { get; set; }

        /// <summary>
        /// Gets or sets 批处理文件目录
        /// </summary>
        public string BatchDirectory { get; set; }

        /// <summary>
        /// Gets or sets 日志开始时间
        /// </summary>
        public DateTime? LogStartTime
        {
            get => this.logStartTime;
            set
            {
                this.logStartTime = value;
                this.OnBatchesCountChanged();
            }
        }

        /// <summary>
        /// Gets or sets 日志截止时间
        /// </summary>
        public DateTime? LogFinishTime
        {
            get => this.logFinishTime;
            set
            {
                this.logFinishTime = value;
                this.OnBatchesCountChanged();
            }
        }

        /// <summary>
        /// Gets or sets 任务分隔时段长度
        /// </summary>
        public int TimeInterval
        {
            get => this.timeInterval;
            set
            {
                this.timeInterval = value;
                this.OnBatchesCountChanged();
            }
        }

        /// <summary>
        /// Gets or sets 分隔时段单位
        /// </summary>
        public TimeUnits TimeIntervalUnit
        {
            get => this.timeIntervalUnit;
            set
            {
                this.timeIntervalUnit = value;
                this.OnBatchesCountChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether 包含系统信息
        /// </summary>
        public bool IncludeSystemInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 包含客户端文件信息
        /// </summary>
        public bool IncludeClientInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 分析结束自动退出程序
        /// </summary>
        public bool AutoExit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 自动打开报告
        /// </summary>
        public bool AutoOpenReport { get; set; }

        /// <summary>
        /// Gets or sets 日志分析报告输出模式
        /// </summary>
        public ReportModes ReportMode { get; set; }

        /// <summary>
        /// Gets or sets 日志等级
        /// </summary>
        public LogLevels LogLevel { get; set; }

        /// <summary>
        /// Gets or sets 监视规则名称列表
        /// </summary>
        public List<string> MonitorNames { get; protected set; } = new List<string>();

        /// <summary>
        /// 是否分隔任务时段
        /// </summary>
        public bool SplitTaskTime
        {
            get => this.LogStartTime != null && this.LogFinishTime != null && this.TimeInterval > 0;
        }

        /// <summary>
        /// 触发批处理文件数量变化事件
        /// </summary>
        public void OnBatchesCountChanged()
        {
            this.BatchesCountChanged?.Invoke(this.BatchesCount);
        }

        /// <summary>
        /// 获取预计生成的批处理文件数
        /// </summary>
        /// <returns></returns>
        public int BatchesCount
        {
            get
            {
                // 计算时段数量
                int timeSpans = 1;
                if (this.LogStartTime.HasValue && this.LogFinishTime.HasValue)
                {
                    if (this.LogStartTime.Value < this.LogFinishTime.Value)
                    {
                        // 时间间隔大于 0 才处理
                        if (this.TimeInterval > 0)
                        {
                            TimeSpan span = this.LogFinishTime.Value - this.LogStartTime.Value;
                            switch (this.TimeIntervalUnit)
                            {
                                case TimeUnits.Day:
                                    {
                                        timeSpans = Convert.ToInt32(Math.Ceiling(span.TotalDays / this.TimeInterval));
                                        break;
                                    }
                                case TimeUnits.Hour:
                                    {
                                        timeSpans = Convert.ToInt32(Math.Ceiling(span.TotalHours / this.TimeInterval));
                                        break;
                                    }
                                case TimeUnits.Minute:
                                    {
                                        timeSpans = Convert.ToInt32(Math.Ceiling(span.TotalMinutes / this.TimeInterval));
                                        break;
                                    }
                                default:
                                    {
                                        return 0;
                                    }
                            }
                        }
                    }
                    else
                    {
                        // 日志开始时间晚于日志结束时间，返回0
                        return 0;
                    }
                }

                int monitorCount = this.MonitorNames.Count;

                return timeSpans * monitorCount;
            }
        }
    }
}
