using System;
using System.Linq;
using xQuantLogFactory.BIZ.Processer;
using xQuantLogFactory.Model;
using xQuantLogFactory.Utils;

namespace xQuantLogFactory.BIZ.Analysiser
{
    /// <summary>
    /// 日志分析器
    /// </summary>
    public class LogAnalysiser : LogProcesserBase, ILogAnalysiser
    {
        public LogAnalysiser() { }

        public LogAnalysiser(ITrace trace) : base(trace) { }

        /// <summary>
        /// 分析日志
        /// </summary>
        /// <param name="argument">任务参数</param>
        public void Analysis(TaskArgument argument)
        {
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            //对日志结果按文件分组，以保证日志解析结果有序
            foreach (var fileResult in argument.MonitorResults.GroupBy(result => result.LogFile))
            {
                this.Trace?.WriteLine("——————————————————————");
                this.Trace?.WriteLine(fileResult.Key.FilePath);

                //对日志结果按监视规则分组，以匹配同一监视规则解析的日志解析结果
                foreach (var monitorResult in fileResult.GroupBy(result => result.MonitorItem))
                {
                    this.Trace?.WriteLine("———————————");
                    this.Trace?.WriteLine(monitorResult.Key.Name);

                    //TODO: 查询文件和规则约束下成对的日志解析结果，并计算执行时间等；注意监视规则只存在开始条件或结束条件而无法匹配成对的情况（也创建一个分析结果对象，但其中一个结果对象为空）
                    foreach (var result in monitorResult)
                    {
                        this.Trace?.WriteLine($"{result.ResultType} {result.LogContent}");
                    }
                }
            }

            /*
            argument.LogFiles.Where(file =>
                (file.LogFileType == LogFileTypes.Client || file.LogFileType == LogFileTypes.Server) && file.MonitorResults.Count > 0
                ).AsParallel().ForAll(logFile =>
            {
                this.Trace?.WriteLine($"开始分析日志文件：(ID: {logFile.FileID}, Type: {logFile.LogFileType}) {logFile.FilePath}");

                this.Trace?.WriteLine($"当前日志文件(ID: {logFile.FileID})解析完成\n————————");
            });
             */
        }
    }
}
