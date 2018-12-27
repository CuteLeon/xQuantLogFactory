using System;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using xQuantLogFactory.BIZ.FileFinder;
using xQuantLogFactory.Model.Fixed;

namespace xQuantLogFactory.Model.Extensions.Tests
{
    [TestClass()]
    public class TaskArgumentExtensionTests
    {
        [TestMethod()]
        public void CheckTaskTime()
        {
            TaskArgument argument = new TaskArgument();

            // 检查文件时间
            argument.LogStartTime = null;
            argument.LogFinishTime = null;
            Assert.IsTrue(argument.CheckLogFileTime(DateTime.MinValue, DateTime.MaxValue));

            argument.LogStartTime = new DateTime(1000);
            argument.LogFinishTime = null;
            Assert.IsFalse(argument.CheckLogFileTime(new DateTime(500), new DateTime(800)));
            Assert.IsTrue(argument.CheckLogFileTime(new DateTime(500), new DateTime(1500)));
            Assert.IsTrue(argument.CheckLogFileTime(new DateTime(2000), new DateTime(3000)));

            argument.LogStartTime = null;
            argument.LogFinishTime = new DateTime(2000);
            Assert.IsTrue(argument.CheckLogFileTime(new DateTime(1000), new DateTime(1500)));
            Assert.IsTrue(argument.CheckLogFileTime(new DateTime(1500), new DateTime(2500)));
            Assert.IsFalse(argument.CheckLogFileTime(new DateTime(2500), new DateTime(3000)));

            argument.LogStartTime = new DateTime(1000);
            argument.LogFinishTime = new DateTime(2000);
            Assert.IsFalse(argument.CheckLogFileTime(new DateTime(500), new DateTime(800)));
            Assert.IsTrue(argument.CheckLogFileTime(new DateTime(500), new DateTime(1500)));
            Assert.IsTrue(argument.CheckLogFileTime(new DateTime(500), new DateTime(2500)));
            Assert.IsTrue(argument.CheckLogFileTime(new DateTime(1500), new DateTime(1800)));
            Assert.IsTrue(argument.CheckLogFileTime(new DateTime(1500), new DateTime(2500)));
            Assert.IsFalse(argument.CheckLogFileTime(new DateTime(2500), new DateTime(3000)));

            // 检查日志时间
            argument.LogStartTime = null;
            argument.LogFinishTime = null;
            Assert.IsTrue(argument.CheckLogTime(DateTime.MinValue));
            Assert.IsTrue(argument.CheckLogTime(DateTime.MaxValue));

            argument.LogStartTime = new DateTime(1000);
            argument.LogFinishTime = null;
            Assert.IsFalse(argument.CheckLogTime(new DateTime(500)));
            Assert.IsTrue(argument.CheckLogTime(new DateTime(1500)));

            argument.LogStartTime = null;
            argument.LogFinishTime = new DateTime(2000);
            Assert.IsTrue(argument.CheckLogTime(new DateTime(1000)));
            Assert.IsFalse(argument.CheckLogTime(new DateTime(2500)));

            argument.LogStartTime = new DateTime(1000);
            argument.LogFinishTime = new DateTime(2000);
            Assert.IsFalse(argument.CheckLogTime(new DateTime(500)));
            Assert.IsTrue(argument.CheckLogTime(new DateTime(1500)));
            Assert.IsFalse(argument.CheckLogTime(new DateTime(2500)));
        }

        [TestMethod()]
        public void LogFileNameFormatTest()
        {
            //无效文件名
            string[] invalidlogFiles = new string[]
            {
                "非法文件",
                "rvLog_Debug.txt",
                "SvLog_Trace.txt",
                "SrLog_Error.txt",
                "Srvog_Info.txt",
                "SrvLg_Warn.txt",
                "SrvLo_Debug.txt",
                "SrvLogTrace.txt",
                "SrvLog_rror.txt",
                "SrvLog_Ifo.txt",
                "SrvLog_Wan.txt",
                "SrvLog_Debg.txt",
                "SrvLog_Trac.txt",
                "SrvLog_Errortxt",
                "SrvLog_Info.xt",
                "SrvLog_Warn.tt",
                "SrvLog_Debug.tx",
                "ltLog_Debug.txt",
                "CtLog_Trace.txt",
                "ClLog_Error.txt",
                "Cltog_Info.txt",
                "CltLg_Warn.txt",
                "CltLo_Debug.txt",
                "CltLogTrace.txt",
                "CltLog_rror.txt",
                "CltLog_Ifo.txt",
                "CltLog_Wan.txt",
                "CltLog_Debg.txt",
                "CltLog_Trac.txt",
                "CltLog_Errortxt",
                "CltLog_Info.xtperformanceog20181031",
                "CltLog_Warn.tt",
                "CltLog_Debug.tx",
                "performanceog20181031.xt",
                "performanceLg20181031.txt",
                "performanceLo20181031.tt",
                "performanceLog0181031.txt",
                "performanceLog2181031.tt",
                "performanceLog2081031.tx",
                "performnceLog2011031.txt",
                "performanceLog2018031.txt",
                "erformanceLog2018131.txt",
                "prformancLog2018101.txt",
                "peformnceLog2018103.txt",
                "permaceLog2018031.txt",
                "perfrmnceLog2011031.txt",
                "perfomnceLog2011031.txt",
                "peroranceLog2018031.txt",
                "perfoceLog2018131.txt",
            };
            //有效文件名
            string[] validlogFiles_1 = new string[]
            {
                "pErformanceLog00000000.txt",
                "perFormanceLog20181031.txt",
                "perforManceLog20181031.TXT",
                "performAnceLog20181031.tXt",
                "performanCeLog20181031.txT",
                "perfOrmanceLOG20181031.Txt",
                "performanceLog20181031.txt",
                "performanceLog20181031.txt",
            };
            string[] validlogFiles_0 = new string[]
            {
                "SrvLog_DEbug.txt",
                "SRVLoG_DEBug.txt.1",
                "CLTLOG_DebUg.txt",
                "CltLog_DebUG.TXt.1",
                "wanglu11_hp-PC_主版本号_20181127173028_CltLog_Debug.txt",

                "SrVLoG_InFo.txt",
                "SrVLog_Info.txT.625",
                "CLtLog_InFo.txt",
                "CltLoG_InfO.txt.625",
                "wanglu11_hp-PC_主版本号_20181127173026_CltLog_Info.txt",

                "SRVLog_TrAce.txt",
                "SrVLog_TrAce.Txt.5",
                "CltLOG_TraCe.txt",
                "CltLoG_TraCE.txt.5",

                "SRVLOg_WArn.txt",
                "SrvLOg_WarN.tXT.1500",
                "ClTLog_WaRn.txt",
                "ClTLOg_Warn.txt.1500",

                "SrvLOG_ERROR.txt",
                "SRvLoG_ErrOr.tXt.25",
                "CLTLog_ErrOr.txt",
                "CLtLOg_ErrOr.txt.25",
            };

            TaskArgument argument = new TaskArgument();
            Regex regex = argument.GetLogLevelRegex();

            Assert.IsTrue(invalidlogFiles.All(file => !regex.IsMatch(file)));
            Assert.IsTrue(validlogFiles_0.Take(5).All(file => regex.IsMatch(file)));

            argument.LogLevel = LogLevels.Perf;
            regex = argument.GetLogLevelRegex();
            Assert.IsTrue(validlogFiles_1.All(file => regex.IsMatch(file)));

            LogFileFinder fileFinder = new LogFileFinder();
            Assert.IsTrue(validlogFiles_0.Take(2).All(name => fileFinder.GetLogFileType(name) == LogFileTypes.Server));
            Assert.IsTrue(validlogFiles_0.Skip(2).Take(3).All(name => fileFinder.GetLogFileType(name) == LogFileTypes.Client));
        }
    }
}