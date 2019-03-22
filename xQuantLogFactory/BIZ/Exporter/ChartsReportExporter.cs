using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using RazorEngine;
using RazorEngine.Templating;

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
            "report.js",
        };

        /// <summary>
        /// 图表容器
        /// </summary>
        private readonly IEnumerable<ChartContainer> chartContainers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartsReportExporter"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public ChartsReportExporter(ITracer tracer)
            : base(tracer)
        {
            this.chartContainers = new ChartContainer[]
            {
                new ChartContainer("内存", "memory", this.RenderMemory),
                new ChartContainer("客户端启动", "clientLaunch", this.RenderMemory),
                new ChartContainer("中间件启动", "serverLaunch", this.RenderMemory),
                new ChartContainer("事项", "monitor", this.RenderMemory),
                new ChartContainer("请求", "performance", this.RenderMemory),
            };
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
                            builder.AppendLine($"<link rel=\"stylesheet\" href=\"{relationPath}\" />");
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
        /// <param name="builder"></param>
        /// <param name="argument"></param>
        private void RenderBody(StringBuilder builder, TaskArgument argument)
        {
            this.RenderHeader(builder, argument, this.chartContainers);

            foreach (var container in this.chartContainers)
            {
                this.RenderContainer(builder, argument, container);
            }
        }

        /// <summary>
        /// 渲染容器
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="argument"></param>
        /// <param name="container"></param>
        private void RenderContainer(
            StringBuilder builder,
            TaskArgument argument,
            ChartContainer container)
        {
            builder.AppendLine($@"<div class=""report-container container-fluid bg-light"" id=""container_{container.Target}"" >");
            container.Render?.Invoke(builder, argument);
            builder.AppendLine("</div>");
        }

        /// <summary>
        /// 渲染内存
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="argument"></param>
        private void RenderMemory(StringBuilder builder, TaskArgument argument)
        {
            builder.AppendLine("<h1>内存</h1>");
        }

        /// <summary>
        /// 渲染头部
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="argument"></param>
        /// <param name="chartContainers"></param>
        private void RenderHeader(
            StringBuilder builder,
            TaskArgument argument,
            IEnumerable<ChartContainer> chartContainers)
        {
            string header = @"
    <header>
        <nav class=""navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3"">
            <div class=""container"">
                <a class=""navbar-brand"">xQuant 图表报告</a>
                <button class=""navbar-toggler"" type=""button"" data-toggle=""collapse"" data-target="".navbar-collapse"" aria-controls=""navbarSupportedContent"" aria-expanded=""false"" aria-label=""Toggle navigation"">
                    <span class=""navbar-toggler-icon""></span>
                </button>
                <div class=""navbar-collapse collapse d-sm-inline-flex flex-sm-row-reverse"">
                    <ul class=""navbar-nav flex-grow-1"">
                        @foreach(var container in Model)
                        {
                            <li class=""nav-item"">
                                <a class=""nav-link text-dark"" data-target=""@container.Target"">@container.Text</a>
                            </li>
                        }
                    </ul>
                </div>
            </div>
        </nav>
    </header>
    ";
            header = Engine.Razor.RunCompile(header, "header", null, chartContainers);
            builder.Append(header);
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
            Action<StringBuilder, TaskArgument> renderBody)
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
            builder.AppendLine("        <div class=\"text-center\">");

            try
            {
                renderBody(builder, argument);
            }
            catch (Exception ex)
            {
                builder.AppendLine($@"<h3>渲染主体部分出错：{ex.Message}</h3>");
            }

            builder.AppendLine("        </div>");
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
                File.WriteAllText(path, content, System.Text.Encoding.UTF8);
            }
            catch
            {
                throw;
            }
        }
    }
}
