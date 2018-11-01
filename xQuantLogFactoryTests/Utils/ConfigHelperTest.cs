
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xQuantLogFactory.Utils;

namespace xQuantLogFactoryTests.Utils
{
    [TestClass()]
    public class ConfigHelperTest
    {
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
                "CltLog_Info.xt",
                "CltLog_Warn.tt",
                "CltLog_Debug.tx",
            };
            //有效文件名
            string[] validlogFiles = new string[]
            {
                "SrvLog_Debug.txt",
                "SrvLog_Trace.txt",
                "SrvLog_Error.txt",
                "SrvLog_Info.txt",
                "SrvLog_Warn.txt",
                "CltLog_Debug.txt",
                "CltLog_Trace.txt",
                "CltLog_Error.txt",
                "CltLog_Info.txt",
                "CltLog_Warn.txt",

                "SrvLog_Debug.txt.1",
                "SrvLog_Trace.txt.5",
                "SrvLog_Error.txt.25",
                "SrvLog_Info.txt.625",
                "SrvLog_Warn.txt.1500",
                "CltLog_Debug.txt.1",
                "CltLog_Trace.txt.5",
                "CltLog_Error.txt.25",
                "CltLog_Info.txt.625",
                "CltLog_Warn.txt.1500",
            };

            Regex logFileRegex = new Regex(ConfigHelper.LogFileNameFormat, RegexOptions.IgnoreCase);

            Assert.IsTrue(invalidlogFiles.All(file => !logFileRegex.IsMatch(file)));
            Assert.IsTrue(validlogFiles.All(file => logFileRegex.IsMatch(file)));
        }
    }
}
