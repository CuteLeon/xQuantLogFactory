﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LogFactory.BIZ.Analysiser;
using LogFactory.BIZ.Analysiser.DirectedAnalysiser.Terminal;
using LogFactory.BIZ.Processer;
using LogFactory.Model;
using LogFactory.Model.Fixed;
using LogFactory.Model.Result;
using LogFactory.Utils;
using LogFactory.Utils.Trace;

namespace LogFactory.BIZ.Exporter
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
                new ChartContainer("缓存", "cache", this.RenderCache),
                new ChartContainer("事项", "monitor", this.RenderMonitor),
                new ChartContainer("请求", "performance", this.RenderPerformance),
                new ChartContainer("SQL", "sql", this.RenderSQL),
                new ChartContainer("指标", "quota", this.RenderQuota),
                new ChartContainer("异步服务", "coreservice", this.RenderCoreService),
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
                    this.RenderBody,
                    this.RenderScript);

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
            => new StringBuilder("<!-- Log Factory -->\n");

        /// <summary>
        /// 写入布局
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="argument"></param>
        /// <param name="getTitle"></param>
        /// <param name="renderScriptStyle"></param>
        /// <param name="renderBody"></param>
        /// <param name="renderScript"></param>
        /// <returns></returns>
        private StringBuilder RenderLayout(
            StringBuilder builder,
            TaskArgument argument,
            Func<TaskArgument, string> getTitle,
            Action<StringBuilder> renderScriptStyle,
            Action<StringBuilder, TaskArgument> renderBody,
            Action<StringBuilder> renderScript)
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
            builder.AppendLine(@"<a class=""btn btn-sm btn-outline-primary"" z-index="" - 1"" style =""position:fixed;right:20px;bottom:20px"" href=""#header"">返回顶部</a>");

            try
            {
                renderScript(builder);
            }
            catch (Exception ex)
            {
                builder.AppendLine($"<h4>渲染脚本部分出错：{ex.Message}</h4>");
            }

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
            => $"图标报告-{Path.GetFileName(argument.LogDirectory)}_{argument.MonitorContainerRoot.Name}";
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
        /// 渲染脚本
        /// </summary>
        /// <param name="builder"></param>
        private void RenderScript(StringBuilder builder)
        {
            builder.AppendLine(@"<script type=""text/javascript"">
    $(function () {{
        $('[data-toggle=""popover""]').popover()
    }});
</script>");
        }

        /// <summary>
        /// 渲染主体
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="argument"></param>
        private void RenderBody(StringBuilder builder, TaskArgument argument)
        {
            this.RenderHeader(builder, argument, this.chartContainers);

            this.RenderIndex(builder, argument, this.chartContainers);

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
    <header id=""header"">
        <nav class=""navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3"">
            <div class=""container"">
                <a class=""my-nav-button navbar-brand"" data-target=""index"">图表报告</a>
                <button class=""navbar-toggler"" type=""button"" data-toggle=""collapse"" data-target="".navbar-collapse"" aria-controls=""navbarSupportedContent"" aria-expanded=""false"" aria-label=""Toggle navigation"">
                    <span class=""navbar-toggler-icon""></span>
                </button>
                <div class=""navbar-collapse collapse d-sm-inline-flex flex-sm-row-reverse"">
                    <ul class=""navbar-nav flex-grow-1"">
                        {string.Join(
                            "\n",
                            chartContainers.Select(container => $@"<li class=""nav-item"">
                                <a class=""my-nav-button nav-link text-dark"" data-target=""{container.Target}"">{container.Text}</a>
                            </li>"))}
                    </ul>
                </div>
            </div>
        </nav>
    </header>
    ";
            builder.Append(header);
        }

        /// <summary>
        /// 渲染主页
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="argument"></param>
        /// <param name="chartContainers"></param>
        private void RenderIndex(StringBuilder builder, TaskArgument argument, IEnumerable<ChartContainer> chartContainers)
        {
            builder.Append($@"
            <div class=""report-container container-fluid"" id=""container_index"">
                <p class=""lead text-muted"">日志分析工具 图表报告</p>
                <hr />
                <div class=""container-fluid justify-content-between"">
                        {string.Join("\n", chartContainers.Select(c =>
                            $@"<input type=""button"" class=""my-nav-button btn shadow btn-outline-primary"" data-target=""{c.Target}"" style=""margin:10px"" value=""{c.Text}"" />"))}
                </div>
            </div>
");
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
                data: [{string.Join(", ", results.Select(r => (r.Client, r.Version)).Distinct().Where(t => !string.IsNullOrEmpty(t.Client) && !string.IsNullOrEmpty(t.Version)).OrderBy(t => (t.Client, t.Version)).Select(t => $"'{t.Client}-{t.Version}'"))}]
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
                data: [{string.Join(", ", results.Select(r => $"'{r.LogTime}'"))}]
            }},
            yAxis: {{
                type: 'value',
                scale: true,
            }},
            series: [
                {string.Join(
                    ",",
                    results.GroupBy(r => (r.Client, r.Version)).Where(g => !string.IsNullOrEmpty(g.Key.Client) && !string.IsNullOrEmpty(g.Key.Version)).OrderBy(g => (g.Key.Client, g.Key.Version)).Select(g => $@"
                {{
                    name: '{$"{g.Key.Client}-{g.Key.Version}"}',
                    type: 'line',
                    stack: '{$"{g.Key.Client}-{g.Key.Version}"}',
                    data: [{string.Join(", ", g.Select(r => $"['{r.LogTime}', {(r.AnalysisDatas.TryGetValue(FixedDatas.MEMORY_CONSUMED, out object m) ? m : 0.0)}]"))}],
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
            tooltip: {{
                trigger: 'axis'
            }},
            legend: {{
                data: [{string.Join(", ", groups.Select(g => $"'{g.Key}'"))}]
            }},
            xAxis: {{
                type: 'category',
                data: [{string.Join(", ", results.Select(r => $"'{r.LogTime}'"))}],
            }},
            yAxis: {{
                type: 'value'
            }},
            series: [
            {string.Join(
                    ",\n",
                    groups.Select(g => $@"
                    {{
                        name: '{$"{g.Key}"}',
                        type: 'bar',
                        stack: '{$"{g.Key}"}',
                        data: [{string.Join(", ", g.Select(r => $"['{r.LogTime}', {r.ElapsedMillisecond}]"))}],
                        markLine : {{
                            data: [
                                {{type : 'average', name: '平均值'}}
                            ]
                        }}
                    }}"))}
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
<div class=""container-fluid"">
    <div class=""row"">
        {string.Join("\n", groups.Select(g => $@"
        <div class=""col-sm-6"">
            <div class=""card text-left"">
                <div class=""card-header"">
                    <kbd class=""bg-success"">{g.Key}</kbd> 版本客户端启动-统计
                </div>
                <div class=""card-body"">
                    <h5 class=""card-title"">启动次数：{g.Count()} 次</h5>
                    <div class=""card-text"">
                        <ul>
                        <li>平均耗时：{g.Average(r => r.ElapsedMillisecond / 1000).ToString("N2")} 秒</li>
                        <li>最大耗时：{g.Max(r => r.ElapsedMillisecond / 1000).ToString("N2")} 秒</li>
                        <li>最小耗时：{g.Min(r => r.ElapsedMillisecond / 1000).ToString("N2")} 秒</li>
                        </ul>
                    </div>
                    <hr />
                    <h5 class=""card-title""><span class=""badge badge-warning"">启动耗时Top：</span></h5>
                    <div class=""card-text"">
                        <ul>
                            {string.Join("\n", g.OrderByDescending(r => r.ElapsedMillisecond).Take(10).Select(r => $@"<li>{r.MonitorItem.Name} <span class=""badge badge-success"">时间：{r.LogTime}</span> <span class=""badge badge-warning"">耗时：{r.ElapsedMillisecond}</span></li>"))}
                        </ul>
                    </div>
                    {string.Join("\n", g.Select(r => $@"
                    <hr />
                    <h5 class=""card-title""><span class=""badge badge-info"">{r.LogTime}</span> 启动耗时Top：</h5>
                    <div class=""card-text"">
                        <ul>
                            {string.Join("\n", r.GetAnalysisResults()
                                .OrderByDescending(c => c.ElapsedMillisecond)
                                .Take(10).Select(c =>
                                    $@"<li>{c.MonitorItem.Name} <span class=""badge badge-danger"">耗时：{c.ElapsedMillisecond}</span></li>"))}
                        </ul>
                    </div>
                    "))}
                </div>
            </div>
        </div>"))}
    </div>
</div>
");
        }

        /// <summary>
        /// 渲染中间件启动
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="argument"></param>
        private void RenderServerLaunch(StringBuilder builder, TaskArgument argument)
        {
            var monitor = argument.MonitorContainerRoot.TerminalMonitorTreeRoots.Find(m => m.Name == "中间件启动");
            if (monitor == null)
            {
                builder.AppendLine("不包含名称为 \"中间件启动\" 的监视规则");
                return;
            }

            var results = monitor.AnalysisResults.Where(r => r.IsIntactGroup()).ToList();
            var groups = results.GroupBy(r => r.FinishMonitorResult.Version).OrderBy(g => g.Key);

            builder.Append($@"
<div id=""canvas_serverLaunch"" class=""container-fluid rounded text-center text-muted"" style=""height:500px;width:800px;padding:0px""></div>

<script type=""text/javascript"">
    let serverLaunchChart = echarts.init(document.getElementById('canvas_serverLaunch'));
    $(window).resize(function () {{
        serverLaunchChart.resize();
    }});
    try {{
        serverLaunchChart.showLoading();

        option = {{
            title: {{
                text: '中间件启动耗时'
            }},
            tooltip: {{
                trigger: 'axis'
            }},
            legend: {{
                data: [{string.Join(", ", groups.Select(g => $"'{g.Key}'"))}]
            }},
            xAxis: {{
                type: 'category',
                data: [{string.Join(", ", results.Select(r => $"'{r.LogTime}'"))}],
            }},
            yAxis: {{
                type: 'value'
            }},
            series: [
                {string.Join(
                ",\n",
                groups.Select(g => $@"
                    {{
                        name: '{$"{g.Key}"}',
                        type: 'bar',
                        stack: '{$"{g.Key}"}',
                        data: [{string.Join(", ", g.Select(r => $"['{r.LogTime}', {r.ElapsedMillisecond}]"))}],
                        markLine : {{
                            data: [
                                {{type : 'average', name: '平均值'}}
                            ]
                        }}
                    }}"))}
            ]
        }};

        serverLaunchChart.setOption(option);
    }} catch (err) {{
        $('#canvas_serverLaunch').html(""加载出错，请刷新页面重试 ..."");
    }} finally {{
        serverLaunchChart.hideLoading();
    }}
</script>
");
            builder.Append($@"
<div class=""container-fluid"">
    <div class=""row"">
        {string.Join("\n", groups.Select(g => $@"
        <div class=""col-sm-6"">
            <div class=""card text-left"">
                <div class=""card-header"">
                    <kbd class=""bg-success"">{g.Key}</kbd> 版本服务端启动-统计
                </div>
                <div class=""card-body"">
                    <h5 class=""card-title"">启动次数：{g.Count()} 次</h5>
                    <div class=""card-text"">
                        <ul>
                        <li>平均耗时：{g.Average(r => r.ElapsedMillisecond / 1000).ToString("N2")} 秒</li>
                        <li>最大耗时：{g.Max(r => r.ElapsedMillisecond / 1000).ToString("N2")} 秒</li>
                        <li>最小耗时：{g.Min(r => r.ElapsedMillisecond / 1000).ToString("N2")} 秒</li>
                        </ul>
                    </div>
                    <hr />
                    <h5 class=""card-title""><span class=""badge badge-warning"">启动耗时Top：</span></h5>
                    <div class=""card-text"">
                        <ul>
                            {string.Join("\n", g.OrderByDescending(r => r.ElapsedMillisecond).Take(10).Select(r => $@"<li>{r.MonitorItem.Name} <span class=""badge badge-success"">时间：{r.LogTime}</span> <span class=""badge badge-warning"">耗时：{r.ElapsedMillisecond}</span></li>"))}
                        </ul>
                    </div>
                    {string.Join("\n", g.Select(r => $@"
                    <hr />
                    <h5 class=""card-title""><span class=""badge badge-info"">{r.LogTime}</span> 启动耗时Top：</h5>
                    <div class=""card-text"">
                        <ul>
                            {string.Join("\n", r.GetAnalysisResults()
                                .OrderByDescending(c => c.ElapsedMillisecond)
                                .Take(10).Select(c =>
                                    $@"<li>{c.MonitorItem.Name} <span class=""badge badge-danger"">耗时：{c.ElapsedMillisecond}</span></li>"))}
                        </ul>
                    </div>
                    "))}
                </div>
            </div>
        </div>"))}
    </div>
</div>
");
        }

        /// <summary>
        /// 渲染缓存
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="argument"></param>
        private void RenderCache(StringBuilder builder, TaskArgument argument)
        {
            string[] targets = new[]
            {
                "Model.XPO.XPInstrument",
                "Model.XPO.XPBond",
                "Model.XPO.OTC.XPTradeGroup",
                "Model.XPO.OTC.XPCounterParty",
                "Model.XPO.XPOtcPoolComp",
                "Model.XPO.OTC.XPOTCTrade",
            };
            var results = argument.TerminalAnalysisResults
                .Where(r => r.MonitorItem.DirectedAnalysiser == TerminalDirectedAnalysiserTypes.CacheSize &&
                                    r.AnalysisDatas.TryGetValue(FixedDatas.RESOURCE_NAME, out object name) ? targets.Contains(name as string) : false)
                .ToList();

            string cache = $@"
<div id=""canvas_cache"" class=""container-fluid rounded text-center text-muted"" style=""height:500px;width:800px;padding:0px""></div>

<script type=""text/javascript"">
    let cacheChart = echarts.init(document.getElementById('canvas_cache'));
    $(window).resize(function () {{
        cacheChart.resize();
    }});
    try {{
        cacheChart.showLoading();

        option = {{
            title: {{
                text: '缓存'
            }},
            tooltip: {{
                trigger: 'axis'
            }},
            legend: {{
                data: [{string.Join(", ", results.Select(r => r.MonitorItem.Name).Distinct().OrderBy(n => n).Select(n => $"'{n}'"))}]
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
                data: [{string.Join(", ", results.Select(r => $"'{r.LogTime}'").Distinct())}]
            }},
            yAxis: {{
                type: 'value'
            }},
            series: [
                {string.Join(
                    ",",
                    results.GroupBy(r => r.MonitorItem.Name).OrderBy(g => g.Key).Select(g => $@"
                {{
                    name: '{$"{g.Key}"}',
                    type: 'line',
                    stack: '{$"{g.Key}"}',
                    data: [{string.Join(", ", g.Select(r => $"['{r.LogTime}', {(r.AnalysisDatas.TryGetValue(FixedDatas.COUNT, out object n) ? n : 0)}]"))}],
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

        cacheChart.setOption(option);
    }} catch (err) {{
        $('#canvas_cache').html(""加载出错，请刷新页面重试 ..."");
    }} finally {{
        cacheChart.hideLoading();
    }}
</script>
";
            builder.Append(cache);
        }

        /// <summary>
        /// 渲染监事事项
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="argument"></param>
        private void RenderMonitor(StringBuilder builder, TaskArgument argument)
        {
            string[] targets = new[]
            {
                "客户端启动",
                "中间件启动",
                "第一阶段初始化",
                "第二阶段初始化",
                "加载清算所债券估值",
                "加载中证债券估值",
                "加载股票估值",
                "加载债券行情估值Series",
                "加载金融工具债券估值",
                "加载中债登债券估值",
                "VSTK",
                "TFND",
                "TBND",
                "TTRD_ACC_BALANCE_CASH",
                "TTRD_ACC_BALANCE_SECU",
                "TTRD_OTC_COUNTERPARTY",
                "TTRD_OTC_POOL",
                "TTRD_OTC_POOL_COMPONENT",
                "TTRD_OTC_TRADE",
                "启动服务",
                "数据加载服务",
                "COM初始化",
                "加载后续持仓债券现金流",
                "交易对手、金融工具出入池",
                "自动出入池刷新",
            };
            var results = argument.TerminalAnalysisResults
                .Where(r => targets.Contains(r.MonitorItem.Name))
                .ToList();

            string monitor = $@"
<div id=""canvas_monitor"" class=""container-fluid rounded text-center text-muted"" style=""height:500px;width:800px;padding:0px""></div>

<script type=""text/javascript"">
    let monitorChart = echarts.init(document.getElementById('canvas_monitor'));
    $(window).resize(function () {{
        monitorChart.resize();
    }});
    try {{
        monitorChart.showLoading();

        option = {{
            title: {{
                text: '监视事项'
            }},
            tooltip: {{
                trigger: 'axis'
            }},
            legend: {{
                data: [{string.Join(", ", results.Select(r => r.MonitorItem.Name).Distinct().OrderBy(n => n).Select(n => $"'{n}'"))}]
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
                data: [{string.Join(", ", results.Select(r => $"'{r.LogTime}'").Distinct())}]
            }},
            yAxis: {{
                type: 'value'
            }},
            series: [
                {string.Join(
                    ",",
                    results.GroupBy(r => r.MonitorItem.Name).OrderBy(g => g.Key).Select(g => $@"
                {{
                    name: '{$"{g.Key}"}',
                    type: 'line',
                    stack: '{$"{g.Key}"}',
                    data: [{string.Join(", ", g.Select(r => $"['{r.LogTime}', {r.ElapsedMillisecond}]"))}],
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

        monitorChart.setOption(option);
    }} catch (err) {{
        $('#canvas_monitor').html(""加载出错，请刷新页面重试 ..."");
    }} finally {{
        monitorChart.hideLoading();
    }}
</script>
";
            builder.Append(monitor);
        }

        /// <summary>
        /// 渲染请求
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="argument"></param>
        private void RenderPerformance(StringBuilder builder, TaskArgument argument)
        {
            var groups = argument.PerformanceParseResults
                .GroupBy(r => (r.RequestURI, r.MethodName))
                .ToArray();

            builder.AppendLine(@"<div class=""container-fluid"">");
            builder.AppendLine($@"  <div class=""row align-content-center"">{this.RenderValue("danger", "排行榜排序字段")} {this.RenderValue("warning", "平均耗时")} {this.RenderValue("info", "方法调用次数")}</div>");
            builder.AppendLine(@"   <hr />");
            builder.AppendLine(@"    <div class=""row"">");

            var topGroups = groups.OrderByDescending(g => g.Sum(r => r.Elapsed)).Take(20).ToArray();
            this.RenderRankingList(builder, "总耗时-排行榜", topGroups, g => this.RenderPopover(g.Key.MethodName, "方法详情", g.Key.RequestURI), g => this.RenderValue("danger", g.Sum(r => r.Elapsed).ToString("N0")), g => this.RenderValue("info", g.Count().ToString("N0")));
            topGroups = groups.OrderByDescending(g => g.Average(r => r.Elapsed)).Take(20).ToArray();
            this.RenderRankingList(builder, "平均耗时-排行榜", topGroups, g => this.RenderPopover(g.Key.MethodName, "方法详情", g.Key.RequestURI), g => this.RenderValue("warning", g.Average(r => r.Elapsed).ToString("N2")), g => this.RenderValue("info", g.Count().ToString("N0")));
            topGroups = groups.OrderByDescending(g => g.Max(r => r.Elapsed)).Take(20).ToArray();
            this.RenderRankingList(builder, "最大耗时-排行榜", topGroups, g => this.RenderPopover(g.Key.MethodName, "方法详情", g.Key.RequestURI), g => this.RenderValue("danger", g.Max(r => r.Elapsed).ToString("N0")), g => this.RenderValue("warning", g.Average(r => r.Elapsed).ToString("N2")), g => this.RenderValue("info", g.Count().ToString("N0")));

            topGroups = groups.OrderByDescending(g => g.Sum(r => r.RequestStreamLength)).Take(20).ToArray();
            this.RenderRankingList(builder, "总请求流长度-排行榜", topGroups, g => this.RenderPopover(g.Key.MethodName, "方法详情", g.Key.RequestURI), g => this.RenderValue("danger", g.Sum(r => r.RequestStreamLength).ToString("N0")), g => this.RenderValue("warning", g.Average(r => r.Elapsed).ToString("N2")), g => this.RenderValue("info", g.Count().ToString("N0")));
            topGroups = groups.OrderByDescending(g => g.Average(r => r.RequestStreamLength)).Take(20).ToArray();
            this.RenderRankingList(builder, "平均请求流长度-排行榜", topGroups, g => this.RenderPopover(g.Key.MethodName, "方法详情", g.Key.RequestURI), g => this.RenderValue("danger", g.Average(r => r.RequestStreamLength).ToString("N2")), g => this.RenderValue("warning", g.Average(r => r.Elapsed).ToString("N2")), g => this.RenderValue("info", g.Count().ToString("N0")));
            topGroups = groups.OrderByDescending(g => g.Max(r => r.RequestStreamLength)).Take(20).ToArray();
            this.RenderRankingList(builder, "最大请求流长度-排行榜", topGroups, g => this.RenderPopover(g.Key.MethodName, "方法详情", g.Key.RequestURI), g => this.RenderValue("danger", g.Max(r => r.RequestStreamLength).ToString("N0")), g => this.RenderValue("warning", g.Average(r => r.Elapsed).ToString("N2")), g => this.RenderValue("info", g.Count().ToString("N0")));

            topGroups = groups.OrderByDescending(g => g.Sum(r => r.ResponseStreamLength)).Take(20).ToArray();
            this.RenderRankingList(builder, "总响应流长度-排行榜", topGroups, g => this.RenderPopover(g.Key.MethodName, "方法详情", g.Key.RequestURI), g => this.RenderValue("danger", g.Sum(r => r.ResponseStreamLength).ToString("N0")), g => this.RenderValue("warning", g.Average(r => r.Elapsed).ToString("N2")), g => this.RenderValue("info", g.Count().ToString("N0")));
            topGroups = groups.OrderByDescending(g => g.Average(r => r.ResponseStreamLength)).Take(20).ToArray();
            this.RenderRankingList(builder, "平均响应流长度-排行榜", topGroups, g => this.RenderPopover(g.Key.MethodName, "方法详情", g.Key.RequestURI), g => this.RenderValue("danger", g.Average(r => r.ResponseStreamLength).ToString("N2")), g => this.RenderValue("warning", g.Average(r => r.Elapsed).ToString("N2")), g => this.RenderValue("info", g.Count().ToString("N0")));
            topGroups = groups.OrderByDescending(g => g.Max(r => r.ResponseStreamLength)).Take(20).ToArray();
            this.RenderRankingList(builder, "最大响应流长度-排行榜", topGroups, g => this.RenderPopover(g.Key.MethodName, "方法详情", g.Key.RequestURI), g => this.RenderValue("danger", g.Max(r => r.ResponseStreamLength).ToString("N0")), g => this.RenderValue("warning", g.Average(r => r.Elapsed).ToString("N2")), g => this.RenderValue("info", g.Count().ToString("N0")));

            topGroups = groups.OrderByDescending(g => g.Count()).Take(20).ToArray();
            this.RenderRankingList(builder, "调用次数-排行榜", topGroups, g => this.RenderPopover(g.Key.MethodName, "方法详情", g.Key.RequestURI), g => this.RenderValue("info", g.Count().ToString("N0")), g => this.RenderValue("warning", g.Average(r => r.Elapsed).ToString("N2")));

            builder.AppendLine(@"    </div>");
            builder.AppendLine(@"</div>");
        }

        /// <summary>
        /// 渲染SQL
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="argument"></param>
        private void RenderSQL(StringBuilder builder, TaskArgument argument)
        {
            var groups = argument.TerminalAnalysisResults
                .Where(r => r.MonitorItem.DirectedAnalysiser == TerminalDirectedAnalysiserTypes.SQL)
                .GroupBy<TerminalAnalysisResult, (string Database, string SQLHash)>(r => (r.AnalysisDatas.TryGetValue(FixedDatas.DATABASE, out object database) ? database as string : string.Empty, r.AnalysisDatas.TryGetValue(FixedDatas.HASH, out object hash) ? hash as string : string.Empty))
                .ToArray();
            List<string> sqlHashs = new List<string>();

            builder.AppendLine(@"<div class=""container-fluid"">");
            builder.AppendLine($@"  <div class=""row align-content-center"">{this.RenderValue("danger", "SQL总耗时")} {this.RenderValue("warning", "SQL平均耗时")} {this.RenderValue("info", "调用次数")}</div>");
            builder.AppendLine(@"   <hr />");
            builder.AppendLine(@"    <div class=""row"">");

            var topGroups = groups.OrderByDescending(g => g.Sum(r => r.ElapsedMillisecond)).Take(100).ToArray();
            sqlHashs = sqlHashs.Union(topGroups.Select(r => r.Key.SQLHash)).ToList();
            this.RenderRankingList(builder, "总耗时-排行榜", topGroups, g => CreateSQLHashLink(g.Key.Database, g.Key.SQLHash), g => this.RenderValue("danger", g.Sum(r => r.ElapsedMillisecond).ToString("N0")), g => this.RenderValue("info", g.Count().ToString("N0")));

            topGroups = groups.OrderByDescending(g => g.Average(r => r.ElapsedMillisecond)).Take(100).ToArray();
            sqlHashs = sqlHashs.Union(topGroups.Select(r => r.Key.SQLHash)).ToList();
            this.RenderRankingList(builder, "平均耗时-排行榜", topGroups, g => CreateSQLHashLink(g.Key.Database, g.Key.SQLHash), g => this.RenderValue("warning", g.Average(r => r.ElapsedMillisecond).ToString("N2")), g => this.RenderValue("info", g.Count().ToString("N0")));

            topGroups = groups.OrderByDescending(g => g.Count()).Take(100).ToArray();
            sqlHashs = sqlHashs.Union(topGroups.Select(r => r.Key.SQLHash)).ToList();
            this.RenderRankingList(builder, "执行次数-排行榜", topGroups, g => CreateSQLHashLink(g.Key.Database, g.Key.SQLHash), g => this.RenderValue("info", g.Count().ToString("N0")), g => this.RenderValue("warning", g.Average(r => r.ElapsedMillisecond).ToString("N")));

            string CreateSQLHashLink(string database, string sqlhash)
                => $@"<a class=""link-sqlhash text-primary"" data-sqlhash=""{sqlhash}""><span class=""badge badge-success font-weight-bold"">{database}</span> {(SQLAnalysiser.HashDescriptions.TryGetValue(sqlhash, out string description) ? description : sqlhash)}</a>";

            builder.AppendLine(@"    </div>");
            builder.Append($@"<div class=""row"">
        <div class=""col"" style=""margin-bottom:10px"">
            <div class=""card text-left font-weight-bold"">
                <div class=""card-header"">
                    SQL-Hash 对应关系 (双击展开或折叠SQL卡片)
                </div>
                <div class=""card-body"">
                    <div class=""card-text"">
                        {string.Join("\n", sqlHashs.Select(sqlhash => $@"
                        <div class=""sqlcard alert alert-success"" data-target=""#sqlhash_{sqlhash}"" role=""alert"">
                            {sqlhash}
                            <div class=""collapse font-weight-normal"" id=""sqlhash_{sqlhash}"">
                                <xmp>{(SQLAnalysiser.SQLHashs.TryGetValue(sqlhash, out string sql) ? sql : "<未找到 Hash 对应的 SQL 命令！>")}</xmp>
                            </div>
                        </div>"))}
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type=""text/javascript"">
        (function () {{
            $("".link-sqlhash"").click((e) => {{
                let target = $(e.target);
                let sqlhash = target.data('sqlhash');
                let sqlelementid = `#sqlhash_${{sqlhash}}`;
                let sqlelement = $(sqlelementid);
                sqlelement.collapse('show');
                location.href = sqlelementid;
            }});
            $("".sqlcard"").dblclick((e) => {{
                let target = $(e.target).data('target');
                if (target !== undefined)
                {{
                    $(target).collapse('toggle');
                }}
            }});
        }})();
    </script>");
            builder.AppendLine(@"</div>");
        }

        /// <summary>
        /// 渲染指标
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="argument"></param>
        private void RenderQuota(StringBuilder builder, TaskArgument argument)
        {
            var quotaAnalysiser = new QuotaAnalysiser();
            var monitors = quotaAnalysiser.GetMonitorsWithQuote(argument).ToList();
            int exported = 0;

            builder.AppendLine(@"<div class=""container-fluid"">");

            foreach (var monitor in monitors)
            {
                var results = quotaAnalysiser.GetAnalysiserResultExceedQuota(monitor).ToList();
                if (results.Count > 0)
                {
                    exported++;
                }

                builder.AppendLine($@"
            <div class=""card text-left"">
                <div class=""card-header"">
                    <strong>{monitor.Name}</strong>
                    <span class=""badge badge-primary"">耗时指标：{monitor.Quota}</span>
                    <span class=""badge badge-success"">平均耗时：{monitor.AnalysisResults.Average(result => result.ElapsedMillisecond).ToString("N2")}</span>
                    <span class=""badge badge-info"">最小耗时：{monitor.AnalysisResults.Min(result => result.ElapsedMillisecond).ToString()}</span>
                    <span class=""badge badge-warning"">最大耗时：{monitor.AnalysisResults.Max(result => result.ElapsedMillisecond).ToString()}</span>
                    <span class=""badge badge-danger"">超额结果占比：{results.Count}/{monitor.AnalysisResults.Count}={((double)results.Count / monitor.AnalysisResults.Count).ToString("P2")}</span>
                </div>
                <div class=""card-body"">
                    <table class=""table table-sm table-hover"">
                      <thead>
                        <tr>
                          <th scope=""col"">开始时间</th>
                          <th scope=""col"">结束时间</th>
                          <th scope=""col"">耗时</th>
                          <th scope=""col"">超额比例</th>
                          <th scope=""col"">开始行号</th>
                          <th scope=""col"">结束行号</th>
                          <th scope=""col"">日志文件</th>
                        </tr>
                      </thead>
                      <tbody>
                        {string.Join("", results.Select(result =>
                        $@"<tr>
                          <td>{result.StartMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}</td>
                          <td>{result.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}</td>
                          <td>{result.ElapsedMillisecond}</td>
                          <td>{((result.ElapsedMillisecond / monitor.Quota) - 1).ToString("P2")}</td>
                          <td>{result.StartMonitorResult?.LineNumber}</td>
                          <td>{result.FinishMonitorResult?.LineNumber}</td>
                          <td>{result.LogFile?.RelativePath}</td>
                        </tr>"))}
                      </tbody>
                    </table>
                </div>
            </div>
            <div class=""w-100"" style=""margin:6px""></div>");
            }

            if (exported == 0)
            {
                builder.AppendLine("不存在性能指数超过指标的结果");
            }

            builder.AppendLine(@"</div>");
        }

        /// <summary>
        /// 渲染异步服务
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="argument"></param>
        private void RenderCoreService(StringBuilder builder, TaskArgument argument)
        {
            var monitor = argument.MonitorContainerRoot.TerminalMonitorTreeRoots.Find(m => m.Name == "Core服务");
            if (monitor == null)
            {
                builder.AppendLine("不包含名称为 \"Core服务\" 的监视规则");
                return;
            }

            var groups = monitor.AnalysisResults.Where(r => r.IsIntactGroup())
                            .GroupBy(r => r.AnalysisDatas[FixedDatas.CORE_SERVICE_NAME])
                            .ToArray();

            builder.AppendLine(@"<div class=""container-fluid"">");
            builder.AppendLine($@"  <div class=""row align-content-center"">{this.RenderValue("danger", "排行榜排序字段")} {this.RenderValue("warning", "平均耗时")} {this.RenderValue("info", "服务调用次数")}</div>");
            builder.AppendLine(@"   <hr />");
            builder.AppendLine(@"    <div class=""row"">");

            var topGroups = groups.OrderByDescending(g => g.Sum(r => r.ElapsedMillisecond)).Take(20).ToArray();
            this.RenderRankingList(builder, "总耗时-排行榜", topGroups, g => g.Key, g => this.RenderValue("danger", g.Sum(r => r.ElapsedMillisecond).ToString("N0")), g => this.RenderValue("info", g.Count().ToString("N0")));
            topGroups = groups.OrderByDescending(g => g.Average(r => r.ElapsedMillisecond)).Take(20).ToArray();
            this.RenderRankingList(builder, "平均耗时-排行榜", topGroups, g => g.Key, g => this.RenderValue("warning", g.Average(r => r.ElapsedMillisecond).ToString("N2")), g => this.RenderValue("info", g.Count().ToString("N0")));
            topGroups = groups.OrderByDescending(g => g.Max(r => r.ElapsedMillisecond)).Take(20).ToArray();
            this.RenderRankingList(builder, "最大耗时-排行榜", topGroups, g => g.Key, g => this.RenderValue("danger", g.Max(r => r.ElapsedMillisecond).ToString("N0")), g => this.RenderValue("warning", g.Average(r => r.ElapsedMillisecond).ToString("N2")), g => this.RenderValue("info", g.Count().ToString("N0")));

            builder.AppendLine(@"    </div>");

            builder.AppendLine(@"<hr/>");
            builder.AppendLine(@"<h4>异常异步服务</h4>");
            var results = monitor.AnalysisResults.Where(r => !r.IsIntactGroup()).ToList();
            if (results.Count == 0)
            {
                builder.AppendLine("不存在异常异步服务分析结果");
            }
            else
            {
                builder.AppendLine($@"<table class=""table table-sm table-hover"">
                      <thead>
                        <tr>
                          <th scope=""col"">服务名称</th>
                          <th scope=""col"">执行序号</th>
                          <th scope=""col"">触发</th>
                          <th scope=""col"">线程号</th>
                          <th scope=""col"">耗时</th>
                          <th scope=""col"">开始时间</th>
                          <th scope=""col"">结束时间</th>
                          <th scope=""col"">日志文件</th>
                          <th scope=""col"">开始行号</th>
                          <th scope=""col"">结束行号</th>
                        </tr>
                      </thead>
                      <tbody>
                        {string.Join("", results.Select(result =>
                        $@"<tr>
                          <td>{(result.AnalysisDatas.TryGetValue(FixedDatas.CORE_SERVICE_NAME, out object service) ? service : string.Empty)}</td>
                          <td>{(result.AnalysisDatas.TryGetValue(FixedDatas.EXECUTE_INDEX, out object index) ? index : string.Empty)}</td>
                          <td>{(result.AnalysisDatas.TryGetValue(FixedDatas.TRIGGER, out object trigger) ? trigger : string.Empty)}</td>
                          <td>{result.ThreadID}</td>
                          <td>{result.ElapsedMillisecond}</td>
                          <td>{result.StartMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}</td>
                          <td>{result.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}</td>
                          <td>{result.LogFile?.RelativePath}</td>
                          <td>{result.StartMonitorResult?.LineNumber}</td>
                          <td>{result.FinishMonitorResult?.LineNumber}</td>
                        </tr>"))}
                      </tbody>
                    </table>");

                builder.AppendLine(@"</div>");
            }
        }

        /// <summary>
        /// 渲染排行榜
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="title"></param>
        /// <param name="sources"></param>
        /// <param name="keySelector"></param>
        /// <param name="valueSelectors"></param>
        private void RenderRankingList<TSource, TKey, TValue>(
            StringBuilder builder,
            string title,
            IEnumerable<TSource> sources,
            Func<TSource, TKey> keySelector,
            params Func<TSource, TValue>[] valueSelectors)
        {
            builder.Append($@"
        <div class=""col-sm-4"" style=""margin-bottom:10px"">
            <div class=""card text-left font-weight-bold"">
                <div class=""card-header"">
                    {title}
                </div>
                <div class=""card-body"">
                    <div class=""card-text"">
                        <ul>
                            {string.Join("\n", sources.Select(element => $@"<li>{keySelector(element)} {string.Join(" ", valueSelectors.Select(vselector => vselector(element)))}</li>"))}
                        </ul>
                    </div>
                </div>
            </div>
        </div>
");
        }

        /// <summary>
        /// 渲染值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="style"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private string RenderValue<TValue>(string style, TValue value)
            => $@"<span class=""badge badge-{style}"">{value}</span>";

        /// <summary>
        /// 渲染提示框
        /// </summary>
        /// <typeparam name="TLabel"></typeparam>
        /// <param name="label"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private string RenderPopover<TLabel>(TLabel label, string title, string content)
            => $@"<nobr data-toggle=""popover"" title=""{title}"" data-content=""{content}"">{label}</nobr>";
        #endregion
    }
}
