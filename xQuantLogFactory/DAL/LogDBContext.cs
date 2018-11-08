using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

using xQuantLogFactory.Model;

namespace xQuantLogFactory.DAL
{
    /// <summary>
    /// 日志数据库交互类
    /// </summary>
    public class LogDBContext : DbContext
    {
        /// <summary>
        /// 任务表
        /// </summary>
        public DbSet<TaskArgument> TaskArguments { get; set; }

        /// <summary>
        /// 系统信息表
        /// </summary>
        public DbSet<SystemInfo> SystemInfos { get; set; }

        /// <summary>
        /// 监视规则表
        /// </summary>
        public DbSet<MonitorItem> MonitorItems { get; set; }

        /// <summary>
        /// 日志文件表
        /// </summary>
        public DbSet<LogFile> LogFiles { get; set; }

        /// <summary>
        /// 监视日志解析结果表
        /// </summary>
        public DbSet<MonitorResult> MonitorResults { get; set; }

        /// <summary>
        /// 中间件日志解析结果表
        /// </summary>
        public DbSet<MiddlewareResult> MiddlewareResults { get; set; }

        /// <summary>
        /// 分析结果表
        /// </summary>
        public DbSet<AnalysisResult> AnalysisResults { get; set; }

        /// <summary>
        /// 全局静态数据库交互对象
        /// </summary>
        public volatile static LogDBContext UnityDBContext = new LogDBContext();

        private LogDBContext() : base("LogFactoryDB") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //必须的代码
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.AddFromAssembly(typeof(LogDBContext).Assembly);

            //初始化数据种子，用于CodeFirst模式自动创建或修改数据库
            Database.SetInitializer(new LogDBSeed(modelBuilder));
        }

    }
}
