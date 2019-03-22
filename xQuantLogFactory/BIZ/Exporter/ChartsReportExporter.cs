using System;
using System.IO;
using System.Text;

using xQuantLogFactory.BIZ.Processer;
using xQuantLogFactory.Model;
using xQuantLogFactory.Utils;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Exporter
{
    /// <summary>
    /// 图表报告导出器
    /// </summary>
    public class ChartsReportExporter : LogProcesserBase, ILogReportExporter
    {
        /// <summary>
        /// 库文件目录名称
        /// </summary>
        private const string LibDirName = "lib";

        /// <summary>
        /// 库文件名称
        /// </summary>
        private readonly string[] libFileNames = new[]
        {
            "jquery-3.3.1.min.js",
            "bootstrap.bundle.min.js",
            "bootstrap.min.css",
            "echarts.min.js",
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartsReportExporter"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public ChartsReportExporter(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// 导出日志报告
        /// </summary>
        /// <param name="reportPath">报告文件路径</param>
        /// <param name="argument">任务参数</param>
        public void ExportReport(string reportPath, TaskArgument argument)
        {
            StringBuilder builder = null;

            try
            {
                this.CopyLibs();

                builder = this.GetStringBuilder();
                this.RenderLayout(
                    builder,
                    argument,
                    this.GetTitle,
                    this.RenderScriptStyle,
                    this.RenderBody);

                this.SaveReportFile(
                    reportPath,
                    builder.ToString());
            }
            finally
            {
                builder.Clear();
            }
        }

        /// <summary>
        /// 渲染脚本和样式
        /// </summary>
        /// <param name="builder"></param>
        private void RenderScriptStyle(StringBuilder builder)
        {
            string extension = string.Empty;
            string relationPath = string.Empty;

            foreach (var libFileName in this.libFileNames)
            {
                extension = Path.GetExtension(libFileName).ToLower();
                relationPath = Path.Combine(LibDirName, libFileName);

                switch (extension)
                {
                    case ".css":
                        {
                            builder.AppendLine($"<link rel=\"stylesheet\" href=\"{relationPath}\">");
                            break;
                        }

                    case ".js":
                        {
                            builder.AppendLine($"<script src=\"{relationPath}\"></script>");
                            break;
                        }

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 渲染主体
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="builder"></param>
        private void RenderBody(TaskArgument argument, StringBuilder builder)
        {
            builder.AppendLine("<input type=\"button\" class=\"btn btn-primary\"></input>");
        }

        /// <summary>
        /// 创建 StringBuilder
        /// </summary>
        /// <returns></returns>
        private StringBuilder GetStringBuilder()
            => new StringBuilder("<!-- xQuant Log Factory -->");

        /// <summary>
        /// 写入布局
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="argument"></param>
        /// <param name="getTitle"></param>
        /// <param name="renderBody"></param>
        /// <returns></returns>
        private StringBuilder RenderLayout(
            StringBuilder builder,
            TaskArgument argument,
            Func<TaskArgument, string> getTitle,
            Action<StringBuilder> renderScriptStyle,
            Action<TaskArgument, StringBuilder> renderBody)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            builder.AppendLine("<html>");
            builder.AppendLine("    <head>");
            builder.AppendLine("        <meta charset=\"UTF-8\">");
            builder.AppendLine($"        <title>{getTitle(argument)}</title>");

            try
            {
                renderScriptStyle(builder);
            }
            catch
            {
            }

            builder.AppendLine("    </head>");
            builder.AppendLine("    <body>");

            try
            {
                renderBody(argument, builder);
            }
            catch
            {
            }

            builder.AppendLine("    </body>");
            builder.AppendLine("</html>");

            return builder;
        }

        /// <summary>
        /// 获取标题
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private string GetTitle(TaskArgument argument)
            => $"xQuant图标报告-{Path.GetFileName(argument.LogDirectory)}_{argument.MonitorContainerRoot.Name}";

        /// <summary>
        /// 复制库文件
        /// </summary>
        private void CopyLibs()
        {
            string sourceDirectory = Path.Combine(ConfigHelper.ReportTempletDirectory, LibDirName);
            string targetDirectory = Path.Combine(ConfigHelper.ReportExportDirectory, LibDirName);
            if (!Directory.Exists(targetDirectory))
            {
                try
                {
                    this.Tracer?.WriteLine($"创建导出库文件目录：{targetDirectory}");
                    Directory.CreateDirectory(targetDirectory);
                }
                catch
                {
                    throw;
                }
            }

            string sourcePath = string.Empty;
            string targetPath = string.Empty;

            foreach (string libFileName in this.libFileNames)
            {
                targetPath = Path.Combine(targetDirectory, libFileName);
                if (!File.Exists(targetPath))
                {
                    sourcePath = Path.Combine(sourceDirectory, libFileName);

                    try
                    {
                        this.Tracer?.WriteLine($"复制库文件：{sourcePath} => {targetPath}");
                        File.Copy(sourcePath, targetPath);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 导出报告文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        private void SaveReportFile(string path, string content)
        {
            try
            {
                File.WriteAllText(path, content, Encoding.UTF8);
            }
            catch
            {
                throw;
            }
        }
    }
}
