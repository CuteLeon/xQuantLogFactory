
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xQuantLogFactory.Utils;

namespace xQuantLogFactoryTests.Utils
{
    [TestClass()]
    public class ConfigHelperTests
    {
        [TestMethod()]
        public void LogFileNameFormatTest()
        {
            ConfigHelper.LogFileLevel = "(DEBUG|TRACE|ERROR|INFO|WARN|)";

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
                "SRVLog_TrAce.txt",
                "SrvLOG_ERROR.txt",
                "SrVLoG_InFo.txt",
                "SRVLOg_WArn.txt",
                "CLTLOG_DebUg.txt",
                "CltLOG_TraCe.txt",
                "CLTLog_ErrOr.txt",
                "CLtLog_InFo.txt",
                "ClTLog_WaRn.txt",

                "SRVLoG_DEBug.txt.1",
                "SrVLog_TrAce.Txt.5",
                "SRvLoG_ErrOr.tXt.25",
                "SrVLog_Info.txT.625",
                "SrvLOg_WarN.tXT.1500",
                "CltLog_DebUG.TXt.1",
                "CltLoG_TraCE.txt.5",
                "CLtLOg_ErrOr.txt.25",
                "CltLoG_InfO.txt.625",
                "ClTLOg_Warn.txt.1500",
            };

            Regex logFileRegex = new Regex(ConfigHelper.LogFileNameFormat, RegexOptions.IgnoreCase);

            Assert.IsTrue(invalidlogFiles.All(file => !logFileRegex.IsMatch(file)));
            foreach (string file in validlogFiles_0)
                Assert.IsTrue(logFileRegex.IsMatch(file));
            Assert.IsTrue(validlogFiles_0.All(file => logFileRegex.IsMatch(file)));

            ConfigHelper.LogFileLevel = ConfigHelper.MiddlewareLogFileAlias;
            logFileRegex = new Regex(ConfigHelper.LogFileNameFormat, RegexOptions.IgnoreCase);

            foreach (string file in validlogFiles_1)
                Assert.IsTrue(logFileRegex.IsMatch(file));
            Assert.IsTrue(validlogFiles_1.All(file => logFileRegex.IsMatch(file)));

        }
    }
}
