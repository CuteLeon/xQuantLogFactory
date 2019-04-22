using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.VisualBasic.FileIO;
using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser.DirectedAnalysiser.Terminal
{
    /// <summary>
    /// SQL分析器
    /// </summary>
    public class SQLAnalysiser : DirectedLogAnalysiserBase
    {
        /// <summary>
        /// SQL-Hash 对应字典
        /// </summary>
        public static ConcurrentDictionary<string, string> SQLHashs = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Hash-描述 对应字典
        /// </summary>
        public static ConcurrentDictionary<string, string> HashDescriptions = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLAnalysiser"/> class.
        /// </summary>
        public SQLAnalysiser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLAnalysiser"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public SQLAnalysiser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets or sets 分析正则
        /// </summary>
        public override Regex AnalysisRegex { get; protected set; } = new Regex(
            @"^.*?=>(?<DBUser>.*?)\s(?<SQLHash>.*?)\s(?<RowCount>-?\d*?)\s(?<StartTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}.\d{3})\s(?<FinishTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}.\d{3})\s(?<Elapsed>\d*?)\s(?<Params>.*?)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 分析
        /// </summary>
        /// <param name="argument"></param>
        public override void Analysis(TaskArgument argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            this.Tracer?.WriteLine($"执行 SQL定向分析器 ....");
            argument.TerminalAnalysisResults
                .Where(result => result.MonitorItem.DirectedAnalysiser == TerminalDirectedAnalysiserTypes.SQL)
                .GroupBy(result => result.MonitorItem)
                .AsParallel().ForAll(resultGroup =>
                {
                    TerminalMonitorItem targetMonitor = resultGroup.Key;
                    TerminalMonitorResult firstResult = null;
                    Match analysisMatch = null;

                    this.Tracer?.WriteLine($">>>正在分析监视规则：{targetMonitor.Name}，结果数量：{resultGroup.Count()}");
                    foreach (var analysisResult in resultGroup)
                    {
                        firstResult = analysisResult.FirstResultOrDefault();
                        if (firstResult == null)
                        {
                            continue;
                        }

                        lock (this.AnalysisRegex)
                        {
                            analysisMatch = this.AnalysisRegex.Match(firstResult.LogContent);
                        }

                        if (analysisMatch.Success)
                        {
                            analysisResult.AnalysisDatas[FixedDatas.DATABASE] = analysisMatch.Groups["DBUser"].Success ? analysisMatch.Groups["DBUser"].Value : string.Empty;
                            analysisResult.AnalysisDatas[FixedDatas.HASH] = analysisMatch.Groups["SQLHash"].Success ? analysisMatch.Groups["SQLHash"].Value : string.Empty;
                            analysisResult.AnalysisDatas[FixedDatas.RESULT_COUNT] = Math.Max(analysisMatch.Groups["RowCount"].Success && int.TryParse(analysisMatch.Groups["RowCount"].Value, out int count) ? count : 0, 0);
                            analysisResult.AnalysisDatas[FixedDatas.PARAMS] = analysisMatch.Groups["Params"].Success ? analysisMatch.Groups["Params"].Value : string.Empty;

                            analysisResult.ElapsedMillisecond = analysisMatch.Groups["Elapsed"].Success && double.TryParse(analysisMatch.Groups["Elapsed"].Value, out double elapsed) ? elapsed : 0;
                        }
                    }
                });

            string[] sqlHashDirs = argument.TerminalLogFiles
                .Where(file => file.LogFileType == LogFileTypes.Server && file.LogLevel == LogLevels.SQL)
                .Select(file => Path.GetDirectoryName(file.FilePath))
                .Distinct().ToArray();

            if (sqlHashDirs.Length > 0)
            {
                this.ParseSQLHashCSVs(sqlHashDirs);
            }

            this.ParseSQLHashDescription();
        }

        /// <summary>
        /// 解析SQL哈希描述
        /// </summary>
        protected void ParseSQLHashDescription()
        {
            string filePath = Path.Combine(ConfigHelper.ReportTempletDirectory, FixedDatas.SQLHashDescriptionFileName);
            if (File.Exists(filePath))
            {
                try
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(filePath);
                    var sqlHashs = document.GetElementsByTagName("SQLHashs").Item(0);
                    if (sqlHashs == null ||
                        sqlHashs.ChildNodes.Count == 0)
                    {
                        return;
                    }

                    string hash = string.Empty, description = string.Empty;
                    foreach (XmlNode sqlHash in sqlHashs.ChildNodes)
                    {
                        if ("SQLHash".Equals(sqlHash.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            hash = sqlHash.Attributes["Hash"]?.Value;
                            description = sqlHash.Attributes["Description"]?.Value;

                            if (!string.IsNullOrEmpty(hash) &&
                                !string.IsNullOrEmpty(description))
                            {
                                HashDescriptions[hash] = description;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Tracer?.WriteLine($"解析 SQLHash 描述遇到异常：{ex.Message}");
                }
            }
        }

        /// <summary>
        /// 解析所有 SQLHash CSV 文件
        /// </summary>
        /// <param name="sqlLogDirs"></param>
        protected void ParseSQLHashCSVs(string[] sqlLogDirs)
        {
            this.Tracer?.WriteLine($"查找 SQL-Hash 对应关系 ....");
            sqlLogDirs.Select(dir => Path.Combine(dir, FixedDatas.SQLHashFileName)).AsParallel().ForAll(sqlHashFile =>
              {
                  if (File.Exists(sqlHashFile))
                  {
                      this.Tracer?.WriteLine($"解析 SQL-Hash ：{sqlHashFile}");
                      try
                      {
                          foreach (var (hash, sql) in this.ParseSQLHashCSV(sqlHashFile))
                          {
                              SQLHashs[hash] = sql;
                          }
                      }
                      catch (Exception ex)
                      {
                          this.Tracer?.WriteLine($"解析 SQL-Hash 时遇到异常：{ex.Message}");
                      }
                  }
              });
        }

        /// <summary>
        /// 解析 SQL-Hash CSV 文件
        /// </summary>
        /// <param name="csvPath"></param>
        /// <returns></returns>
        protected IEnumerable<(string hash, string sql)> ParseSQLHashCSV(string csvPath)
        {
            using TextFieldParser parser = new TextFieldParser(csvPath)
            {
                Delimiters = new[] { "," },
                TextFieldType = FieldType.Delimited,
                HasFieldsEnclosedInQuotes = true,
            };

            string[] values = null;
            while (!parser.EndOfData)
            {
                values = parser.ReadFields();
                if (values.Length >= 2)
                {
                    yield return (values[0], values[1]);
                }
            }
        }
    }
}
