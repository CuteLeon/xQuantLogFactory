using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using xQuantLogFactory.BIZ.Processer;
using xQuantLogFactory.Model;

namespace xQuantLogFactory.BIZ.Exporter
{
    //TODO: 显示最耗时的结果
    //TODO: HTML输出代码对内容格式化，防止导出结果太乱
    //TODO: 用户关注 监视规则：启动几次？ 耗时多久？

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

            this.WriteCSS(@"body{margin:10 auto;width:80%}pre{color:#333;background-color:#f5f5f5;border:1px solid #ccc;border-radius:5px}h1{font-family:'微软雅黑';text-align:center}#tasktable{margin:0 auto;border:1px #fff solid}caption{text-align:left}hr{margin:10 auto;border:0;height:1px;background-image:linear-gradient(to right,rgba(0,0,0,0),rgba(75,75,75,0.5),rgba(0,0,0,0))}td.label{width:40%;font-size:15px;font-weight:bold;text-align:right}td.value{word-wrap:break-word;text-align:left}ul{list-style:none}#tab{padding:0;width:100%;height:auto;border:1px solid #ddd;box-shadow:0 0 2px #ddd;margin:0 auto;overflow:hidden}#tab-header{margin:0;padding:0;background-color:#f7f7f7;height:50px;text-align:center;position:relative}#tab-header ul{margin:0;padding:0;width:auto;position:absolute;left:-1px}#tab-header ul li{float:left;width:fit-content;height:50px;line-height:50px;padding:0 10 0 10;border-bottom:1px solid #ddd}#tab-header ul li.selected{background-color:white;font-weight:bolder;border-bottom:0;border-left:1px solid #ddd;border-right:1px solid #ddd}#tab-header ul li:hover{color:orangered}#tab-container .tabContent{display:none;padding:10px}.datatable{margin:0 auto;width:100%;border-collapse:collapse}.datatable th{line-height: 36px;border:1px #ccc solid;background-color:#eee}.datatable td{line-height: 30px;border:1px #ddd solid}");
            this.WriteCSS(@".card{margin: 10 36 10 36; auto;position:relative;display:-ms-flexbox;display:flex;-ms-flex-direction:column;flex-direction:column;min-width:0;word-wrap:break-word;background-color:#fff;background-clip:border-box;border:1px solid rgba(0,0,0,0.125);border-radius:.25rem}.card-body{-ms-flex:1 1 auto;flex:1 1 auto;padding: 1rem 1.5rem;}.card-title{margin:0}.card-header{padding:.5rem 1rem;margin-bottom:0;background-color:rgba(0,0,0,0.03);border-bottom:1px solid rgba(0,0,0,0.125)}.card-footer{padding:.5rem 1rem;background-color:rgba(0,0,0,0.03);border-top:1px solid rgba(0,0,0,0.125)}");
            this.WriteJS(@"function $(id){return typeof id === 'string' ? document.getElementById(id):id}window.onload = function(){var titles = $('tab-header').getElementsByTagName('li');var divs = $('tab-container').getElementsByClassName('tabContent');for(var i = 0;i < titles.length;i++){var li = titles[i];li.id = i;li.onmousemove = function(){for(var j = 0;j < titles.length;j++){titles[j].className = '';divs[j].style.display = 'none'}this.className = 'selected';divs[this.id].style.display = 'block'}}}");
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
                        <th>结果总耗时(毫秒)</th>
                        <th>日志文件数量</th>
                        <th>监视结果数量</th>
                        <th>分析匹配组数</th>
                    </thead>
                    <tbody>");
            foreach (var monitor in argument.MonitorItems.OrderByDescending(monitor => monitor.ElapsedMillisecond))
            {
                this.HTMLBuilder.Value.AppendLine($@"<tr>
                    <td>{monitor.Name}</td>
                    <td>{monitor.StartPattern}</td>
                    <td>{monitor.FinishPatterny}</td>
                    <td>{monitor.ElapsedMillisecond}</td>
                    <td>{argument.LogFiles.Count(logFile => logFile.MonitorResults.Any(result => result.MonitorItem == monitor))}</td>
                    <td>{monitor.MonitorResults.Count} 个</td>
                    <td>{monitor.AnalysisResults.Count} 组</td>
                </tr>");
            }
            this.HTMLBuilder.Value.AppendLine("</tbody>\n</table>");
            this.WriteHR();

            this.WriteSectionTitle("监视规则详情：");
            foreach (var monitor in argument.MonitorItems
                .OrderByDescending(monitor => monitor.ElapsedMillisecond))
            {
                this.WriteCard();
            }

            this.HTMLBuilder.Value.AppendLine("</div>");
        }

        //TODO: 写入卡片数据
        private void WriteCard()
        {
            this.HTMLBuilder.Value.AppendLine("<div class=\"card\">");
            this.HTMLBuilder.Value.AppendLine("<div class=\"card-header\">");
            this.HTMLBuilder.Value.AppendLine($"<div class=\"card-title\">{123}</div>");
            this.HTMLBuilder.Value.AppendLine($"</div>");
            this.HTMLBuilder.Value.AppendLine($"<div class=\"card-body\">{234}</div>");
            this.HTMLBuilder.Value.AppendLine($"<div class=\"card-footer\">{345}</div>");
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
                        <th>监视结果数量</th>
                        <th>分析匹配组数</th>
                    </thead>
                    <tbody>");
            foreach (var logFile in argument.LogFiles
                .Where(logFile => logFile.LogFileType == LogFileTypes.Client)
                .OrderByDescending(logFile => logFile.MonitorResults.Count)
                )
            {
                this.HTMLBuilder.Value.AppendLine($@"<tr>
                    <td>{logFile.FilePath}</td>
                    <td>{logFile.CreateTime}</td>
                    <td>{logFile.LastWriteTime}</td>
                    <td>{string.Join("、", logFile.MonitorResults.Select(result => result.MonitorItem.Name).Distinct())}</td>
                    <td>{logFile.MonitorResults.Count} 个</td>
                    <td>{logFile.AnalysisResults.Count} 组</td>
                </tr>");
            }
            this.HTMLBuilder.Value.AppendLine("</tbody>\n</table>");

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
                    </thead>
                    <tbody>");
            foreach (var logFile in argument.LogFiles
                .Where(logFile => logFile.LogFileType == LogFileTypes.Server)
                .OrderByDescending(logFile => logFile.MonitorResults.Count)
                )
            {
                this.HTMLBuilder.Value.AppendLine($@"<tr>
                    <td>{logFile.FilePath}</td>
                    <td>{logFile.CreateTime}</td>
                    <td>{logFile.LastWriteTime}</td>
                    <td>{string.Join("、", logFile.MonitorResults.Select(result => result.MonitorItem.Name).Distinct())}</td>
                    <td>{logFile.MonitorResults.Count} 个</td>
                    <td>{logFile.AnalysisResults.Count} 组</td>
                </tr>");
            }
            this.HTMLBuilder.Value.AppendLine("</tbody>\n</table>");

            this.HTMLBuilder.Value.AppendLine("</div>");
        }

        /// <summary>
        /// 写入中间件日志文件查看容器
        /// </summary>
        /// <param name="argument"></param>
        private void WriteMiddlewareLogFileTabContent(TaskArgument argument)
        {
            //TODO: 按用户代码分析异常请求

            this.HTMLBuilder.Value.AppendLine(@"<div class=""tabContent"">");

            this.HTMLBuilder.Value.AppendLine(@"<table class=""datatable"">
                    <caption><h3>服务端日志文件查看：</h3></caption>
                    <thead>
                        <th>文件路径</th>
                        <th>创建时间</th>
                        <th>最后写入时间</th>
                        <th>日志数量</th>
                    </thead>
                    <tbody>");
            foreach (var logFile in argument.LogFiles
                .Where(logFile => logFile.LogFileType == LogFileTypes.Middleware)
                .OrderByDescending(logFile => logFile.MiddlewareResults.Count)
                )
            {
                this.HTMLBuilder.Value.AppendLine($@"<tr>
                    <td>{logFile.FilePath}</td>
                    <td>{logFile.CreateTime}</td>
                    <td>{logFile.LastWriteTime}</td>
                    <td>{logFile.MiddlewareResults.Count} 个</td>
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
                        <th>调用用户数量</th>
                        <th>最小流长度</th>
                        <th>最大流长度</th>
                        <th>总耗时</th>
                    </thead>
                    <tbody>");
            foreach (var requestURIGroup in argument.MiddlewareResults
                .GroupBy(result => result.RequestURI)
                .OrderBy(result => result.Key)
                )
            {
                string requestURI = requestURIGroup.Key;
                foreach (var methodNameGroup in requestURIGroup
                    .GroupBy(result => result.MethodName)
                    .OrderBy(result => result.Key)
                    )
                {
                    string methodName = methodNameGroup.Key;
                    this.HTMLBuilder.Value.AppendLine($@"<tr>
                        <td>{requestURI}</td>
                        <td>{methodName}</td>
                        <td>{methodNameGroup.Count()}</td>
                        <td>{methodNameGroup.Select(result => result.UserCode).Distinct().Count()} 个</td>
                        <td>{methodNameGroup.Min(result => result.StreamLenth)}</td>
                        <td>{methodNameGroup.Max(result => result.StreamLenth)}</td>
                        <td>{methodNameGroup.Sum(result => result.Elapsed)}</td>
                    </tr>");
                }
            }
            this.HTMLBuilder.Value.AppendLine("</tbody>\n</table>");
            this.WriteHR();

            this.HTMLBuilder.Value.AppendLine(@"<table class=""datatable"">
                    <caption><h3>请求耗时：</h3></caption>
                    <thead>
                        <th>请求路径</th>
                        <th>方法名称</th>
                        <th>调用次数</th>
                        <th>调用用户数量</th>
                        <th>最小流长度</th>
                        <th>最大流长度</th>
                        <th>总耗时</th>
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
                    <td>{resultGroup.Select(result => result.UserCode).Distinct().Count()} 个</td>
                    <td>{resultGroup.Min(result => result.StreamLenth)}</td>
                    <td>{resultGroup.Max(result => result.StreamLenth)}</td>
                    <td>{resultGroup.Sum(result => result.Elapsed)}</td>
                </tr>");
            }
            this.HTMLBuilder.Value.AppendLine("</tbody>\n</table>");
            this.WriteHR();

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
                <td class=""value"">{argument.LogStartTime.ToString()}</td>
            </tr>
            <tr>
                <td class=""label"">日志结束时间：</td>
                <td class=""value"">{argument.LogFinishTime.ToString()}</td>
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
