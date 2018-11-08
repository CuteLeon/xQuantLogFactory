using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace xQuantLogFactory.Model
{
    /// <summary>
    /// 系统信息
    /// </summary>
    [Table("SystemInfos")]
    public class SystemInfo
    {
        /// <summary>
        /// 系统信息ID
        /// </summary>
        [Key]
        [Required]
        [DisplayName("系统信息ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SystemInfoID { get; set; }

        /// <summary>
        /// 是否为64位系统
        /// </summary>
        [DisplayName("是否为64位系统")]
        public bool Is64BitOperatingSystem { get; set; } = Environment.Is64BitOperatingSystem;

        /// <summary>
        /// 是否为64位进程
        /// </summary>
        [DisplayName("是否为64位进程")]
        public bool Is64BitProcess { get; set; } = Environment.Is64BitProcess;

        /// <summary>
        /// 主机名称
        /// </summary>
        [DisplayName("主机名称"), DataType(DataType.Text)]
        public string MachineName { get; set; } = Environment.MachineName;

        /// <summary>
        /// 平台ID
        /// </summary>
        [DisplayName("平台ID"), DataType(DataType.Text)]
        public string Platform { get; set; } = Environment.OSVersion.Platform.ToString();

        /// <summary>
        /// SP版本
        /// </summary>
        [DisplayName("SP版本"), DataType(DataType.Text)]
        public string ServicePack { get; set; } = Environment.OSVersion.ServicePack;

        /// <summary>
        /// 版本号
        /// </summary>
        [DisplayName("版本号"), DataType(DataType.Text)]
        public string Version { get; set; } = Environment.OSVersion.VersionString;

        /// <summary>
        /// 处理器数
        /// </summary>
        [DisplayName("处理器数")]
        public int ProcessorCount { get; set; } = Environment.ProcessorCount;

        /// <summary>
        /// 系统目录
        /// </summary>
        [DisplayName("系统目录"), DataType(DataType.Text)]
        public string SystemDirectory { get; set; } = Environment.SystemDirectory;

        /// <summary>
        /// 系统页大小
        /// </summary>
        [DisplayName("系统页大小")]
        public int SystemPageSize { get; set; } = Environment.SystemPageSize;

        /// <summary>
        /// 启已动毫秒数
        /// </summary>
        [DisplayName("启已动毫秒数")]
        public int TickCount { get; set; } = Environment.TickCount;

        /// <summary>
        /// 网络域名
        /// </summary>
        [DisplayName("网络域名"), DataType(DataType.Text)]
        public string UserDomainName { get; set; } = Environment.UserDomainName;

        /// <summary>
        /// 交互模式
        /// </summary>
        [DisplayName("交互模式")]
        public bool UserInteractive { get; set; } = Environment.UserInteractive;

        /// <summary>
        /// 用户名称
        /// </summary>
        [DisplayName("用户名称"), DataType(DataType.Text)]
        public string UserName { get; set; } = Environment.UserName;

        /// <summary>
        /// 逻辑驱动器
        /// </summary>
        [DisplayName("逻辑驱动器")]
        public string[] LogicalDrives { get; set; } = Environment.GetLogicalDrives();

        public override string ToString()
        {
            return $"\t主机名称：{this.MachineName}\t用户名称：{this.UserName}";
        }

    }
}
