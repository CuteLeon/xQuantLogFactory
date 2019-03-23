using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using xQuantLogFactory.BIZ.Processer;
using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Utils;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Exporter
{
    /// <summary>
    /// 图表报告导出器
    /// </summary>
    public class ChartsReportExporter : LogProcesserBase, ILogReportExporter
    {
        #region 数据

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
        #endregion

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
                new ChartContainer("客户端启动", "clientLaunch", this.RenderClientLaunch),
                new ChartContainer("中间件启动", "serverLaunch", this.RenderServerLaunch),
                new ChartContainer("事项", "monitor", this.RenderMonitor),
                new ChartContainer("请求", "performance", this.RenderPerformance),
            };
        }

        #region 框架

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

        /// <summary>
        /// 获取标题
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private string GetTitle(TaskArgument argument)
            => $"xQuant图标报告-{Path.GetFileName(argument.LogDirectory)}_{argument.MonitorContainerRoot.Name}";
        #endregion

        #region 渲染组件

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
            builder.AppendLine($@"<div class=""report-container container-fluid"" id=""container_{container.Target}"" style=""display: none"">");
            container.Render?.Invoke(builder, argument);
            builder.AppendLine("</div>");
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
            string header = $@"
    <header>
        <nav class=""navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3"">
            <div class=""container"">
                <a class=""navbar-brand"">xQuant 图表报告</a>
                <button class=""navbar-toggler"" type=""button"" data-toggle=""collapse"" data-target="".navbar-collapse"" aria-controls=""navbarSupportedContent"" aria-expanded=""false"" aria-label=""Toggle navigation"">
                    <span class=""navbar-toggler-icon""></span>
                </button>
                <div class=""navbar-collapse collapse d-sm-inline-flex flex-sm-row-reverse"">
                    <ul class=""navbar-nav flex-grow-1"">
                        {string.Join(
                            "\n",
                            chartContainers.Select(container => $@"
                            <li class=""nav-item"">
                                <a class=""nav-link text-dark"" data-target=""{container.Target}"">{container.Text}</a>
                            </li>
                        "))}
                    </ul>
                </div>
            </div>
        </nav>
    </header>
    ";
            builder.Append(header);
        }
        #endregion

        #region 渲染图表

        /// <summary>
        /// 渲染内存
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="argument"></param>
        private void RenderMemory(StringBuilder builder, TaskArgument argument)
        {
            var results = argument.TerminalAnalysisResults.Where(r => r.MonitorItem.Memory).ToList();
            string memory = $@"
<div id=""canvas_memory"" class=""container-fluid rounded text-center text-muted"" style=""height:500px;width:800px;padding:0px""></div>

<script type=""text/javascript"">
    let memoryChart = echarts.init(document.getElementById('canvas_memory'));
    $(window).resize(function () {{
        memoryChart.resize();
    }});
    try {{
        memoryChart.showLoading();

        option = {{
            title: {{
                text: '内存'
            }},
            tooltip: {{
                trigger: 'axis'
            }},
            legend: {{
                data: ['{string.Join("', '", results.Select(r => r.Client).Distinct().OrderBy(ip => ip))}']
            }},
            grid: {{
                left: '3%',
                right: '4%',
                bottom: '3%',
                containLabel: true
            }},
            toolbox: {{
                feature: {{
                    dataZoom: {{
                        yAxisIndex: 'none'
                    }},
                    restore: {{}},
                    saveAsImage: {{}}
                }}
            }},
            dataZoom: [{{
                type: 'inside',
            }}, {{
                handleIcon: 'M10.7,11.9v-1.3H9.3v1.3c-4.9,0.3-8.8,4.4-8.8,9.4c0,5,3.9,9.1,8.8,9.4v1.3h1.3v-1.3c4.9-0.3,8.8-4.4,8.8-9.4C19.5,16.3,15.6,12.2,10.7,11.9z M13.3,24.4H6.7V23h6.6V24.4z M13.3,19.6H6.7v-1.4h6.6V19.6z',
                handleStyle: {{
                    color: '#fff',
                    shadowBlur: 3,
                    shadowColor: 'rgba(0, 0, 0, 0.6)',
                    shadowOffsetY: 2
                }}
            }}],
            xAxis: {{
                type: 'category',
                boundaryGap: false,
                data: ['{string.Join("', '", results.Select(r => r.LogTime))}']
            }},
            yAxis: {{
                type: 'value'
            }},
            series: [
                {string.Join(
                    ",",
                    results.GroupBy(r => r.Client).OrderBy(g => g.Key).Select(g => $@"
                {{
                    name: '{g.Key ?? "空"}',
                    type: 'line',
                    stack: '内存',
                    data: [{string.Join(", ", g.Select(r => r.AnalysisDatas.TryGetValue(FixedDatas.MEMORY_CONSUMED, out object m) ? m : 0.0))}],
                    markPoint : {{
                        data: [
                            {{type : 'max', name: '最大值'}},
                            {{type: 'min', name: '最小值'}}
                        ]
                    }},
                    markLine : {{
                        data: [
                            {{type : 'average', name: '平均值'}}
                        ]
                    }}
                }}"))}
            ]
        }};

        memoryChart.setOption(option);
    }} catch (err) {{
        $('#canvas_memory').html(""加载出错，请刷新页面重试 ..."");
    }} finally {{
        memoryChart.hideLoading();
    }}
</script>
";
            builder.Append(memory);
        }

        /// <summary>
        /// 渲染客户端启动
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="argument"></param>
        private void RenderClientLaunch(StringBuilder builder, TaskArgument argument)
        {
            var monitor = argument.MonitorContainerRoot.TerminalMonitorTreeRoots.Find(m => m.Name == "客户端启动");
            if (monitor == null)
            {
                builder.AppendLine("不包含名称为 \"客户端启动\" 的监视规则");
                return;
            }

            var results = monitor.AnalysisResults.Where(r => r.IsIntactGroup()).ToList();
            var groups = results.GroupBy(r => r.FinishMonitorResult.Version).OrderBy(g => g.Key);
            foreach (var group in groups)
            {
                builder.Append($@"
<div id=""canvas_clientLaunch"" class=""container-fluid rounded text-center text-muted"" style=""height:500px;width:800px;padding:0px""></div>

<script type=""text/javascript"">
    let clientLaunchChart = echarts.init(document.getElementById('canvas_clientLaunch'));
    $(window).resize(function () {{
        clientLaunchChart.resize();
    }});
    try {{
        clientLaunchChart.showLoading();

        option = {{
            title: {{
                text: '客户端启动耗时'
            }},
            xAxis: {{
                data: ['{string.Join("', '", groups.Select(g => g.Key))}'],
            }},
            yAxis: {{
            }},
            series: [
                {{
                    type: 'bar',
                    data: [{string.Join(", ", group.Average(r => r.ElapsedMillisecond))}]
                }}
            ]
        }};

        clientLaunchChart.setOption(option);
    }} catch (err) {{
        $('#canvas_clientLaunch').html(""加载出错，请刷新页面重试 ..."");
    }} finally {{
        clientLaunchChart.hideLoading();
    }}
</script>
");

                builder.Append($@"
            <div class=""card"">
              <div class=""card-header"">
                <kbd>{group.Key}</kbd> 版本客户端启动-统计
              </div>
              <div class=""card-body text-left"">
                <h5 class=""card-title"">启动次数：{group.Count()} 次</h5>
                <div class=""card-text"">
                    <ul>
                        <li>平均耗时：{group.Average(r => r.ElapsedMillisecond / 1000).ToString("N2")} 秒</li>
                        <li>最大耗时：{group.Max(r => r.ElapsedMillisecond / 1000).ToString("N2")} 秒</li>
                        <li>最小耗时：{group.Min(r => r.ElapsedMillisecond / 1000).ToString("N2")} 秒</li>
                    </ul>
                </div>
                {string.Join(
                    string.Empty,
                    group.Select(launch =>
                        $@"
                        <div class=""card-body"">
                            <h5 class=""card-title"">耗时Top：</h5>
                            <div class=""card-text"">
                                {string.Join(
                                    string.Empty,
                                    launch.GetAnalysisResults()
                                        .OrderByDescending(r => r.ElapsedMillisecond)
                                        .Take(10).Select(r =>
                                            $@"<div class=""alert alert-danger"" role=""alert"">
                                                    {r.MonitorItem.Name} - {r.ElapsedMillisecond}
                                            </div>"))}
                            </div>
                        </div>"
                    ))}
              </div>
            </div>
");
            }

        }

        /// <summary>
        /// 渲染中间件启动
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="argument"></param>
        private void RenderServerLaunch(StringBuilder builder, TaskArgument argument)
        {
            builder.AppendLine("<h1>服务端启动</h1>");
        }

        /// <summary>
        /// 渲染监事事项
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="argument"></param>
        private void RenderMonitor(StringBuilder builder, TaskArgument argument)
        {
            builder.AppendLine("<h1>监视</h1>");
        }

        /// <summary>
        /// 渲染请求事项
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="argument"></param>
        private void RenderPerformance(StringBuilder builder, TaskArgument argument)
        {
            builder.AppendLine("<h1>请求</h1>");
        }
        #endregion
    }
}
