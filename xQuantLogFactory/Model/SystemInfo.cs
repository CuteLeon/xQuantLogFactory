using System;

namespace xQuantLogFactory.Model
{
    /// <summary>
    /// 系统信息
    /// </summary>
    public class SystemInfo
    {
        /// <summary>
        /// Gets or sets 系统信息ID
        /// </summary>
        public int SystemInfoID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 是否为64位系统
        /// </summary>
        public bool Is64BitOperatingSystem { get; set; } = Environment.Is64BitOperatingSystem;

        /// <summary>
        /// Gets or sets a value indicating whether 是否为64位进程
        /// </summary>
        public bool Is64BitProcess { get; set; } = Environment.Is64BitProcess;

        /// <summary>
        /// Gets or sets 主机名称
        /// </summary>
        public string MachineName { get; set; } = Environment.MachineName;

        /// <summary>
        /// Gets or sets 平台ID
        /// </summary>
        public string Platform { get; set; } = Environment.OSVersion.Platform.ToString();

        /// <summary>
        /// Gets or sets SP版本
        /// </summary>
        public string ServicePack { get; set; } = Environment.OSVersion.ServicePack;

        /// <summary>
        /// Gets or sets 系统版本号
        /// </summary>
        public string OSVersion { get; set; } = Environment.OSVersion.VersionString;

        /// <summary>
        /// Gets or sets 处理器数
        /// </summary>
        public int ProcessorCount { get; set; } = Environment.ProcessorCount;

        /// <summary>
        /// Gets or sets 系统目录
        /// </summary>
        public string SystemDirectory { get; set; } = Environment.SystemDirectory;

        /// <summary>
        /// Gets or sets 系统页大小
        /// </summary>
        public int SystemPageSize { get; set; } = Environment.SystemPageSize;

        /// <summary>
        /// Gets or sets 启已动毫秒数
        /// </summary>
        public int TickCount { get; set; } = Environment.TickCount;

        /// <summary>
        /// Gets or sets 网络域名
        /// </summary>
        public string UserDomainName { get; set; } = Environment.UserDomainName;

        /// <summary>
        /// Gets or sets a value indicating whether 交互模式
        /// </summary>
        public bool UserInteractive { get; set; } = Environment.UserInteractive;

        /// <summary>
        /// Gets or sets 用户名称
        /// </summary>
        public string UserName { get; set; } = Environment.UserName;

        /// <summary>
        /// Gets or sets 逻辑驱动器
        /// </summary>
        public string[] LogicalDrives { get; set; } = Environment.GetLogicalDrives();

        public override string ToString()
        {
            return $"\t主机名称：{this.MachineName}\n\t用户名称：{this.UserName}\n\t系统版本：{this.OSVersion}";
        }
    }
}
