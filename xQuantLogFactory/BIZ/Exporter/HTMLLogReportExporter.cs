using System;
using System.IO;
using System.Linq;
using System.Text;

using xQuantLogFactory.BIZ.Processer;
using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.BIZ.Exporter
{

    /// <summary>
    /// HTML日志报告导出器
    /// </summary>
    public class HTMLLogReportExporter : LogProcesserBase, ILogReportExporter
    {
        /// <summary>
        /// 报告文本构建器
        /// </summary>
        protected Lazy<StringBuilder> HTMLBuilder = new Lazy<StringBuilder>();

        /// <summary>
        /// 导出日志报告
        /// </summary>
        /// <param name="reportPath">报告文件路径</param>
        /// <param name="argument">任务参数</param>
        public void ExportReport(string reportPath, TaskArgument argument)
        {
            this.HTMLBuilder.Value.AppendLine("<html>\n<head>");

            this.WriteCSS(@"body{margin:10 auto;width:80%}pre{color:#333;background-color:#f5f5f5;border:1px solid #ccc;border-radius:5px}h1{font-family:'微软雅黑';text-align:center}#tasktable{margin:0 auto;border:1px #fff solid}caption{text-align:left}hr{margin:10 auto;border:0;height:1px;background-image:linear-gradient(to right,rgba(0,0,0,0),rgba(75,75,75,0.5),rgba(0,0,0,0))}td.label{width:40%;font-size:15px;text-align:right}td.value{font-weight:bold;word-wrap:break-word;text-align:left}ul{list-style:none}#tab{padding:0;width:100%;height:auto;border:1px solid #ddd;box-shadow:0 0 2px #ddd;margin:0 auto;overflow:hidden}#tab-header{margin:0;padding:0;background-color:#f7f7f7;height:50px;text-align:center;position:relative}#tab-header ul{margin:0;padding:0;width:auto;position:absolute;left:-1px}#tab-header ul li{float:left;width:fit-content;height:50px;line-height:50px;padding:0 10 0 10;border-bottom:1px solid #ddd}#tab-header ul li.selected{background-color:white;font-weight:bolder;border-bottom:0;border-left:1px solid #ddd;border-right:1px solid #ddd}#tab-header ul li:hover{color:orangered}#tab-container .tabContent{min-height: 500px;display:none;padding:10px}.datatable{margin:0 auto;width:100%;border-collapse:collapse}.datatable th{line-height: 36px;border:1px #ccc solid;background-color:#eee}.datatable td{padding: 0 5 0 5;line-height: 30px;border:1px #ddd solid}");
            this.WriteCSS(@".card{margin: 10 36 10 36; auto;position:relative;display:-ms-flexbox;display:flex;-ms-flex-direction:column;flex-direction:column;min-width:0;word-wrap:break-word;background-color:#fff;background-clip:border-box;border:1px solid rgba(0,0,0,0.125);border-radius:.25rem}.card-body{-ms-flex:1 1 auto;flex:1 1 auto;padding: 1rem 1.5rem;}.card-title{margin:0}.card-header{padding:.5rem 1rem;margin-bottom:0;background-color:rgba(0,0,0,0.03);border-bottom:1px solid rgba(0,0,0,0.125)}.card-footer{padding:.5rem 1rem;background-color:rgba(0,0,0,0.03);border-top:1px solid rgba(0,0,0,0.125)}");
            //切换Tab容器JS
            this.WriteJS(@"function $(id){return typeof id === 'string' ? document.getElementById(id):id}window.onload = function(){var titles = $('tab-header').getElementsByTagName('li');var divs = $('tab-container').getElementsByClassName('tabContent');for(var i = 0;i < titles.length;i++){var li = titles[i];li.id = i;li.onmousemove = function(){for(var j = 0;j < titles.length;j++){titles[j].className = '';divs[j].style.display = 'none'}this.className = 'selected';divs[this.id].style.display = 'block'}}}");
            //显隐Card容器JS
            this.WriteJS(@"function onCardClick(){var card;if(event.target.className==""card-title""){card=event.target.parentNode.parentNode}else{if(event.target.className==""card-header""){card=event.target.parentNode}}var cardbody=card.getElementsByClassName(""card-body"")[0];cardbody.style.display=cardbody.style.display==""none""?""block"":""none""};");
            this.HTMLBuilder.Value.AppendFormat("<meta charset=\"UTF-8\">\n<title>xQuant-日志分析报告：{0}</title>\n", argument.TaskID);

            this.HTMLBuilder.Value.AppendLine("</head>\n<body>");

            this.WriteReportTitle("xQuant-日志分析报告");
            this.WriteHR();

            this.WriteTaskArgument(argument);
            this.WriteHR();

            this.WriteNodeTitle("日志分析结果：");
            this.HTMLBuilder.Value.AppendLine("<div id=\"tab\">");
            this.WriteTabTitles("监视规则", "客户端日志文件", "服务端日志文件", "中间件日志文件");
            this.HTMLBuilder.Value.AppendLine("<div id=\"tab-container\">");

            this.WriteMonitorItemTabContent(argument);
            this.WriteClientLogFileTabContent(argument);
            this.WriteServerLogFileTabContent(argument);
            this.WriteMiddlewareLogFileTabContent(argument);

            this.HTMLBuilder.Value.AppendLine("</div>\n</div>");
            this.HTMLBuilder.Value.AppendLine("</body>\n</html>");

            this.SaveReportFile(reportPath);

            //清理 StringBuilder 内数据
            this.HTMLBuilder.Value.Clear();
        }

        /// <summary>
        /// 写入监视规则查看容器
        /// </summary>
        /// <param name="argument"></param>
        private void WriteMonitorItemTabContent(TaskArgument argument)
        {
            this.HTMLBuilder.Value.AppendLine(@"<div class=""tabContent"" style=""display: block;"">");

            this.HTMLBuilder.Value.AppendLine(@"<table class=""datatable"">
<caption><h3>监视规则查看：</h3></caption>
<thead>
    <th>项目名称</th>
    <th>起始匹配模式</th>
    <th>结束匹配模式</th>
    <th>日志文件数量</th>
    <th>监视结果数量</th>
    <th>分析匹配组数</th>
    <th>结果总耗时(毫秒)</th>
    <th>匹配组平均耗时</th>
</thead>
<tbody>");
            foreach (var monitor in argument.MonitorItems
                )
            {
                this.HTMLBuilder.Value.AppendLine($@"<tr>
    <td>{monitor.Name.PadLeft(monitor.Name.Length + monitor.GetLayerDepth(), '+')}</td>
    <td>{monitor.StartPattern}</td>
    <td>{monitor.FinishPatterny}</td>
    <td>{argument.LogFiles.Count(logFile => logFile.MonitorResults.Any(result => result.MonitorItem == monitor))}</td>
    <td>{monitor.MonitorResults.Count}</td>
    <td>{monitor.AnalysisResults.Count}</td>
    <td>{monitor.ElapsedMillisecond}</td>
    <td>{monitor.AverageElapsedMillisecond.ToString("0.##")}</td>
</tr>");
            }
            this.HTMLBuilder.Value.AppendLine("</tbody>\n</table>");
            this.WriteHR();

            this.WriteSectionTitle("监视规则详情：");
            foreach (var monitor in argument.MonitorItems
                .OrderByDescending(monitor => monitor.ElapsedMillisecond))
            {
                this.WriteMonitorItemCard(monitor);
            }

            this.HTMLBuilder.Value.AppendLine("</div>");
        }

        /// <summary>
        /// 写入客户端或服务端日志文件卡片
        /// </summary>
        /// <param name="logFile"></param>
        private void WriteCSLogFileCard(LogFile logFile)
        {
            this.WriteCardHeader($"日志文件：<b>{logFile.FilePath}</b>");
            this.HTMLBuilder.Value.Append($"创建时间：<b>{logFile.CreateTime}</b><br>最后访问时间：<b>{logFile.LastWriteTime}</b><hr>匹配结果：");

            if (logFile.AnalysisResults.Count == 0)
            {
                this.HTMLBuilder.Value.Append("无");
            }
            else
            {
                foreach (var analysisResult in logFile.AnalysisResults
                    .OrderByDescending(result => result.ElapsedMillisecond)
                    )
                {
                    MonitorResult startResult = analysisResult.StartMonitorResult;
                    MonitorResult finishResult = analysisResult.FinishMonitorResult;
                    this.WriteCard(
                        $"耗时：<b>{analysisResult.ElapsedMillisecond} ms</b>",
                        $@"监视规则：{analysisResult.MonitorItem?.Name}<br><hr>
开始日志：{(startResult == null ? "无" : $"<b>{startResult.LogTime}</b> 行号: <b>{startResult.LineNumber}</b> 等级: <b>{startResult.LogLevel}</b> 内容: <b>{startResult.LogContent}")}</b><br>
结束日志：{(finishResult == null ? "无" : $"<b>{finishResult.LogTime}</b> 行号: <b>{finishResult.LineNumber}</b> 等级: <b>{finishResult.LogLevel}</b> 内容: <b>{finishResult.LogContent}</b>")}"
                        );
                }
            }

            this.WriteCardFooter($"监视结果总数：<b>{logFile.MonitorResults.Count.ToString()}</b> 个， 分析结果总数：<b>{logFile.AnalysisResults.Count().ToString()}</b> 组， 总耗时：<b>{logFile.ElapsedMillisecond}</b> 毫秒");
        }

        /// <summary>
        /// 写入监视规则卡片
        /// </summary>
        /// <param name="monitor"></param>
        private void WriteMonitorItemCard(MonitorItem monitor)
        {
            this.WriteCardHeader($"监视规则：<b>{monitor.Name}</b>");
            this.HTMLBuilder.Value.Append($"开始匹配：<b>{monitor.StartPattern ?? "无"}</b><br>结束匹配：<b>{monitor.FinishPatterny ?? "无"}</b><hr>匹配结果：");

            if (monitor.AnalysisResults.Count == 0)
            {
                this.HTMLBuilder.Value.Append("无");
            }
            else
            {
                foreach (var analysisResult in monitor.AnalysisResults
                    .OrderByDescending(result => result.ElapsedMillisecond)
                    )
                {
                    MonitorResult startResult = analysisResult.StartMonitorResult;
                    MonitorResult finishResult = analysisResult.FinishMonitorResult;
                    this.WriteCard(
                        $"耗时：<b>{analysisResult.ElapsedMillisecond} ms</b>",
                        $@"日志文件：{analysisResult.LogFile?.FilePath}<br><hr>
开始日志：{(startResult == null ? "无" : $"<b>{startResult.LogTime}</b> 行号: <b>{startResult.LineNumber}</b> 等级: <b>{startResult.LogLevel}</b> 内容: <b>{startResult.LogContent}")}</b><br>
结束日志：{(finishResult == null ? "无" : $"<b>{finishResult.LogTime}</b> 行号: <b>{finishResult.LineNumber}</b> 等级: <b>{finishResult.LogLevel}</b> 内容: <b>{finishResult.LogContent}</b>")}"
                        );
                }
            }

            this.WriteCardFooter($"监视结果总数：<b>{monitor.MonitorResults.Count.ToString()}</b> 个， 分析结果总数：<b>{monitor.AnalysisResults.Count().ToString()}</b> 组， 总耗时：<b>{monitor.ElapsedMillisecond}</b> 毫秒");
        }

        /// <summary>
        /// 写入卡片
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="body">主体</param>
        /// <param name="footer">底部</param>
        private void WriteCard(string title, string body, string footer = null)
        {
            //处理换行符转义
            //body = body.Replace("\r\n", "<br>").Replace("\n", "<br>").Replace("\r", "<br>");

            this.WriteCardHeader(title);
            this.HTMLBuilder.Value.AppendLine(body);
            this.WriteCardFooter(footer);
        }

        /// <summary>
        /// 写入卡片顶部（需要成对使用）
        /// </summary>
        /// <param name="title"></param>
        private void WriteCardHeader(string title)
        {
            this.HTMLBuilder.Value.AppendLine("<div class=\"card\">");
            this.HTMLBuilder.Value.AppendLine($"<div class=\"card-header\" onclick=\"onCardClick()\"><div class=\"card-title\">{title}</div></div>");
            this.HTMLBuilder.Value.AppendLine($"<div class=\"card-body\">");
        }

        /// <summary>
        /// 写入卡片底部（需要成对使用）
        /// </summary>
        /// <param name="footer"></param>
        private void WriteCardFooter(string footer = null)
        {
            this.HTMLBuilder.Value.AppendLine("</div>");
            if (footer?.Length > 0) this.HTMLBuilder.Value.AppendLine($"<div class=\"card-footer\">{footer}</div>");
            this.HTMLBuilder.Value.AppendLine($"</div>");
        }

        /// <summary>
        /// 写入客户端日志文件查看容器
        /// </summary>
        /// <param name="argument"></param>
        private void WriteClientLogFileTabContent(TaskArgument argument)
        {
            this.HTMLBuilder.Value.AppendLine(@"<div class=""tabContent"">");

            this.HTMLBuilder.Value.AppendLine(@"<table class=""datatable"">
<caption><h3>客户端日志文件查看：</h3></caption>
<thead>
    <th>文件路径</th>
    <th>创建时间</th>
    <th>最后写入时间</th>
    <th>匹配监视规则</th>
    <th>结果数量</th>
    <th>匹配组数</th>
    <th>匹配组总耗时</th>
</thead>
<tbody>");

            IOrderedEnumerable<LogFile> logFiles = argument.LogFiles
                .Where(logFile => logFile.LogFileType == LogFileTypes.Client)
                .OrderByDescending(logFile => logFile.ElapsedMillisecond);

            foreach (var logFile in logFiles)
            {
                this.HTMLBuilder.Value.AppendLine($@"<tr>
    <td>{logFile.FilePath}</td>
    <td>{logFile.CreateTime}</td>
    <td>{logFile.LastWriteTime}</td>
    <td>{string.Join("、", logFile.MonitorResults.Select(result => result.MonitorItem?.Name).Distinct())}</td>
    <td>{logFile.MonitorResults.Count}</td>
    <td>{logFile.AnalysisResults.Count}</td>
    <td><b>{logFile.ElapsedMillisecond}</b></td>
</tr>");
            }
            this.HTMLBuilder.Value.AppendLine("</tbody>\n</table>");
            this.WriteHR();

            this.WriteSectionTitle("日志文件详情：");
            foreach (var logFile in logFiles)
                this.WriteCSLogFileCard(logFile);

            this.HTMLBuilder.Value.AppendLine("</div>");
        }

        /// <summary>
        /// 写入服务端日志文件查看容器
        /// </summary>
        /// <param name="argument"></param>
        private void WriteServerLogFileTabContent(TaskArgument argument)
        {
            this.HTMLBuilder.Value.AppendLine(@"<div class=""tabContent"">");

            this.HTMLBuilder.Value.AppendLine(@"<table class=""datatable"">
<caption><h3>服务端日志文件查看：</h3></caption>
<thead>
    <th>文件路径</th>
    <th>创建时间</th>
    <th>最后写入时间</th>
    <th>匹配监视规则</th>
    <th>结果数量</th>
    <th>匹配组数</th>
    <th>匹配组总耗时</th>
</thead>
<tbody>");

            IOrderedEnumerable<LogFile> logFiles = argument.LogFiles
                .Where(logFile => logFile.LogFileType == LogFileTypes.Server)
                .OrderByDescending(logFile => logFile.MonitorResults.Count);

            foreach (var logFile in logFiles)
            {
                this.HTMLBuilder.Value.AppendLine($@"<tr>
    <td>{logFile.FilePath}</td>
    <td>{logFile.CreateTime}</td>
    <td>{logFile.LastWriteTime}</td>
    <td>{string.Join("、", logFile.MonitorResults.Select(result => result.MonitorItem?.Name).Distinct())}</td>
    <td><b>{logFile.MonitorResults.Count}</b></td>
    <td>{logFile.AnalysisResults.Count}</td>
    <td>{logFile.ElapsedMillisecond}</td>
</tr>");
            }
            this.HTMLBuilder.Value.AppendLine("</tbody>\n</table>");
            this.WriteHR();

            this.WriteSectionTitle("日志文件详情：");
            foreach (var logFile in logFiles)
                this.WriteCSLogFileCard(logFile);

            this.HTMLBuilder.Value.AppendLine("</div>");
        }

        /// <summary>
        /// 写入中间件日志文件卡片
        /// </summary>
        /// <param name="logFile"></param>
        private void WriteMiddlewareLogFileCard(LogFile logFile)
        {
            this.WriteCardHeader($"日志文件：<b>{logFile.FilePath}</b>");
            this.HTMLBuilder.Value.Append($"创建时间：<b>{logFile.CreateTime}</b><br>最后访问时间：<b>{logFile.LastWriteTime}</b><hr>匹配结果：");

            if (logFile.MiddlewareResults.Count == 0)
            {
                this.HTMLBuilder.Value.Append("无");
            }
            else
            {
                foreach (var requesURIResult in logFile.MiddlewareResults
                    .GroupBy(result => result.RequestURI))
                {
                    this.WriteCardHeader($"请求路径：<b>{requesURIResult.Key}</b>");

                    foreach (var methodNameResult in requesURIResult
                        .GroupBy(result => result.MethodName)
                        .OrderByDescending(result => result.Count()))
                    {
                        this.WriteCard(
                            $"方法名称：<b>{methodNameResult.Key}</b>",
                            $@"<b>方法调用次数：{methodNameResult.Count()}</b><br>
调用客户端数：<b>{methodNameResult.Select(result => result.Client).Distinct().Count()}</b><br>
调用用户数量：<b>{methodNameResult.Select(result => result.UserCode).Distinct().Count()}</b><br>
返回总流长度：<b>{methodNameResult.Sum(result => result.StreamLength)}</b><br>
流长度平均值：<b>{methodNameResult.Average(result => result.StreamLength).ToString("0.##")}</b>"
                            );
                    }

                    this.WriteCardFooter($"方法总数：<b>{requesURIResult.Select(result => result.MethodName).Distinct().Count()}</b> 个， 调用总次数：<b>{requesURIResult.Count()}</b> 个， 总耗时：<b>{requesURIResult.Sum(result => result.Elapsed)}</b> 毫秒");
                }
            }

            this.WriteCardFooter($"监视结果总数：<b>{logFile.MonitorResults.Count.ToString()}</b> 个， 分析结果总数：<b>{logFile.AnalysisResults.Count().ToString()}</b> 组， 总耗时：<b>{logFile.ElapsedMillisecond}</b> 毫秒");
        }

        /// <summary>
        /// 写入中间件日志文件查看容器
        /// </summary>
        /// <param name="argument"></param>
        private void WriteMiddlewareLogFileTabContent(TaskArgument argument)
        {
            this.HTMLBuilder.Value.AppendLine(@"<div class=""tabContent"">");

            this.HTMLBuilder.Value.AppendLine(@"<table class=""datatable"">
<caption><h3>中间件日志文件查看：</h3></caption>
<thead>
    <th>文件路径</th>
    <th>创建时间</th>
    <th>最后写入时间</th>
    <th>日志数量</th>
</thead>
<tbody>");

            IOrderedEnumerable<LogFile> logFiles = argument.LogFiles
                .Where(logFile => logFile.LogFileType == LogFileTypes.Middleware)
                .OrderByDescending(logFile => logFile.MiddlewareResults.Count);

            foreach (var logFile in logFiles)
            {
                this.HTMLBuilder.Value.AppendLine($@"<tr>
    <td>{logFile.FilePath}</td>
    <td>{logFile.CreateTime}</td>
    <td>{logFile.LastWriteTime}</td>
    <td><b>{logFile.MiddlewareResults.Count}</b></td>
</tr>");
            }
            this.HTMLBuilder.Value.AppendLine("</tbody>\n</table>");
            this.WriteHR();

            this.HTMLBuilder.Value.AppendLine(@"<table class=""datatable"">
<caption><h3>请求路径：</h3></caption>
<thead>
    <th>请求路径</th>
    <th>方法名称</th>
    <th>调用次数</th>
    <th>调用IP数量</th>
    <th>最小流长度</th>
    <th>最大流长度</th>
    <th>总流长度</th>
    <th>总耗时</th>
    <th>平均耗时</th>
</thead>
<tbody>");
            foreach (var resultGroup in argument.MiddlewareResults
                .GroupBy(result => (result.RequestURI, result.MethodName))
                .OrderBy(result => (result.Key.RequestURI, result.Key.MethodName))
                )
            {
                this.HTMLBuilder.Value.AppendLine($@"<tr>
    <td><b>{resultGroup.Key.RequestURI}</b></td>
    <td><b>{resultGroup.Key.MethodName}</b></td>
    <td>{resultGroup.Count()}</td>
    <td>{resultGroup.Select(result => result.Client).Distinct().Count()}</td>
    <td>{resultGroup.Min(result => result.StreamLength)}</td>
    <td>{resultGroup.Max(result => result.StreamLength)}</td>
    <td>{resultGroup.Sum(result => result.StreamLength)}</td>
    <td>{resultGroup.Sum(result => result.Elapsed)}</td>
    <td>{resultGroup.Average(result => result.Elapsed).ToString("0.##")}</td>
</tr>");
            }
            this.HTMLBuilder.Value.AppendLine("</tbody>\n</table>");
            this.WriteHR();

            this.HTMLBuilder.Value.AppendLine(@"<table class=""datatable"">
<caption><h3>请求耗时：</h3></caption>
<thead>
    <th>请求路径</th>
    <th>方法名称</th>
    <th>调用次数</th>
    <th>调用IP数量</th>
    <th>最小流长度</th>
    <th>最大流长度</th>
    <th>总耗时</th>
    <th>平均耗时</th>
</thead>
<tbody>");
            foreach (var resultGroup in argument.MiddlewareResults
                .GroupBy(result => (result.RequestURI, result.MethodName))
                .OrderByDescending(results => results.Sum(result => result.Elapsed))
                )
            {
                this.HTMLBuilder.Value.AppendLine($@"<tr>
    <td>{resultGroup.Key.RequestURI}</td>
    <td>{resultGroup.Key.MethodName}</td>
    <td>{resultGroup.Count()}</td>
    <td>{resultGroup.Select(result => result.Client).Distinct().Count()}</td>
    <td>{resultGroup.Min(result => result.StreamLength)}</td>
    <td>{resultGroup.Max(result => result.StreamLength)}</td>
    <td><b>{resultGroup.Sum(result => result.Elapsed)}</b></td>
    <td>{resultGroup.Average(result => result.Elapsed).ToString("0.##")}</td>
</tr>");
            }
            this.HTMLBuilder.Value.AppendLine("</tbody>\n</table>");
            this.WriteHR();

            this.HTMLBuilder.Value.AppendLine(@"<table class=""datatable"">
<caption><h3>客户端：</h3></caption>
<thead>
    <th>客户端IP</th>
    <th>请求路径</th>
    <th>方法名称</th>
    <th>调用次数</th>
    <th>最小流长度</th>
    <th>最大流长度</th>
    <th>总流长度</th>
    <th>总耗时</th>
</thead>
<tbody>");
            foreach (var resultGroup in argument.MiddlewareResults
                .GroupBy(result => (result.Client, result.RequestURI, result.MethodName))
                .OrderByDescending(results => results.Sum(result => result.Elapsed))
                )
            {
                this.HTMLBuilder.Value.AppendLine($@"<tr>
    <td>{resultGroup.Key.Client}</td>
    <td>{resultGroup.Key.RequestURI}</td>
    <td>{resultGroup.Key.MethodName}</td>
    <td>{resultGroup.Count()}</td>
    <td>{resultGroup.Min(result => result.StreamLength)}</td>
    <td>{resultGroup.Max(result => result.StreamLength)}</td>
    <td>{resultGroup.Sum(result => result.StreamLength)}</td>
    <td><b>{resultGroup.Sum(result => result.Elapsed)}</b></td>
</tr>");
            }
            this.HTMLBuilder.Value.AppendLine("</tbody>\n</table>");
            this.WriteHR();

            this.WriteSectionTitle("日志文件详情：");
            foreach (var logFile in logFiles)
                this.WriteMiddlewareLogFileCard(logFile);

            this.HTMLBuilder.Value.AppendLine("</div>");
        }

        /// <summary>
        /// 写入选项卡标题数组
        /// </summary>
        /// <param name="titles"></param>
        private void WriteTabTitles(params string[] titles)
        {
            this.HTMLBuilder.Value.AppendLine("<div id=\"tab-header\">\n<ul>");
            if (titles.Length > 0)
            {
                this.HTMLBuilder.Value.AppendLine($"<li class=\"selected\">{titles[0]}</li>");
            }
            if (titles.Length > 1)
            {
                titles.Skip(1).ToList().ForEach(title => this.HTMLBuilder.Value.AppendLine($"<li>{title}</li>"));
            }
            this.HTMLBuilder.Value.AppendLine("</ul>\n</div>");
        }

        /// <summary>
        /// 导出报告文件
        /// </summary>
        /// <param name="reportPath"></param>
        private void SaveReportFile(string reportPath)
        {
            try
            {
                File.WriteAllText(reportPath, this.HTMLBuilder.Value.ToString(), Encoding.UTF8);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 写入CSS
        /// </summary>
        /// <param name="css"></param>
        private void WriteCSS(string css)
        {
            this.HTMLBuilder.Value.AppendLine($"<style>\n{css}\n</style>");
        }

        /// <summary>
        /// 写入JS
        /// </summary>
        /// <param name="js"></param>
        private void WriteJS(string js)
        {
            this.HTMLBuilder.Value.AppendLine($"<script>\n{js}\n</script>\n");
        }

        /// <summary>
        /// 写入任务信息
        /// </summary>
        /// <param name="argument"></param>
        private void WriteTaskArgument(TaskArgument argument)
        {
            if (argument == null) throw new ArgumentNullException(nameof(argument));

            this.HTMLBuilder.Value.AppendLine($@"<table id =""tasktable"">
    <caption>
        <h2>任务信息：</h3>
    </caption>
    <tbody>
        <tr>
            <td class=""label"">任务ID：</td>
            <td class=""value"">{argument.TaskID}</td>
        </tr>
        <tr>
            <td class=""label"">日志目录：</td>
            <td class=""value"">{argument.LogDirectory}</td>
        </tr>
        <tr>
            <td class=""label"">任务开始时间：</td>
            <td class=""value"">{argument.TaskStartTime.ToString()}</td>
        </tr>
        <tr>
            <td class=""label"">任务结束时间：</td>
            <td class=""value"">{argument.TaskFinishTime.ToString()}</td>
        </tr>
        <tr>
            <td class=""label"">日志开始时间：</td>
            <td class=""value"">{argument.LogStartTime?.ToString() ?? "[不限制]"}</td>
        </tr>
        <tr>
            <td class=""label"">日志结束时间：</td>
            <td class=""value"">{argument.LogFinishTime?.ToString() ?? "[不限制]"}</td>
        </tr>
        <tr>
            <td class=""label"">包含系统信息：</td>
            <td class=""value"">{argument.IncludeSystemInfo.ToString()}</td>
        </tr>
        <tr>
            <td class=""label"">包含客户端信息：</td>
            <td class=""value"">{argument.IncludeClientInfo.ToString()}</td>
        </tr>
        <tr>
            <td class=""label"">监控规则数量：</td>
            <td class=""value"">{argument.MonitorItems.Count().ToString()}</td>
        </tr>
        <tr>
            <td class=""label"">监控规则名称：</td>
            <td class=""value"">{string.Join("、", argument.MonitorItems.Select(monitor => monitor.Name))}</td>
        </tr>
        <tr>
            <td class=""label"">日志文件数量：</td>
            <td class=""value"">{argument.LogFiles.Count().ToString()}</td>
        </tr>
        <tr>
            <td class=""label"">报告导出模式：</td>
            <td class=""value"">{argument.ReportMode.ToString()}</td>
        </tr>
        <tr>
            <td class=""label"">日志监视结果数量：</td>
            <td class=""value"">{argument.MonitorResults.Count()}</td>
        </tr>
        <tr>
            <td class=""label"">中间件日志结果数量：</td>
            <td class=""value"">{argument.MiddlewareResults.Count()}</td>
        </tr>
        <tr>
            <td class=""label"">监视分析结果数量：</td>
            <td class=""value"">{argument.AnalysisResults.Count()}</td>
        </tr>
    </tbody>
</table>");
        }

        /// <summary>
        /// 写入分割线
        /// </summary>
        private void WriteHR()
        {
            this.HTMLBuilder.Value.AppendLine("<hr>");
        }

        /// <summary>
        /// 写入报告标题
        /// </summary>
        /// <param name="title"></param>
        private void WriteReportTitle(string title)
        {
            this.HTMLBuilder.Value.AppendLine($"<pre><h1>{title}</h1></pre>");
        }

        /// <summary>
        /// 写入节点标题
        /// </summary>
        /// <param name="title"></param>
        private void WriteNodeTitle(string title)
        {
            this.HTMLBuilder.Value.AppendLine($"<h2>{title}</h2>");
        }

        /// <summary>
        /// 写入分段标题
        /// </summary>
        /// <param name="title"></param>
        private void WriteSectionTitle(string title)
        {
            this.HTMLBuilder.Value.AppendLine($"<h3>{title}</h3>");
        }

    }
}
