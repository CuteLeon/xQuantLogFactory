using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using xQuantLogFactory.Model;
using xQuantLogFactory.Utils;

namespace xQuantLogFactory
{
    //TODO: 移除和排除 using
    //TODO: 编写单元测试

    class Program
    {
        /// <summary>
        /// 全局任务参数
        /// </summary>
        public static TaskArgument UnityArgument = null;

        /// <summary>
        /// 全局追踪器
        /// </summary>
        public static ITrace Tracer = new Trace();

        /* 启动参数：{string_日志文件目录} {string.Format(,)_监控的项目名称列表} "{datetime_日志开始时间}" "[datetime_日志截止时间 =DateTime.Now]" [boolean_包含系统信息 =false] [boolean_包含客户端信息 =false] [reportmodes_报告导出模式 =RepostModes.Html]
         * 注意：
         *  1. 任何参数值内含有空格时需要在值外嵌套英文双引号；
         *  2. 不允许离散地省略参数，只可以选择省略某可选参数及其后所有的参数
         * 参数介绍：
         * {string_日志文件目录}：目录含有空格时需要在值外嵌套英文双引号；如：C:\TEST_DIR 或 "C:\TEST DIR" 
         * {string.Format(,)_监控的项目名称列表}：程序监控的项目名称列表；不允许为空；当存在多个值时，值间以英文逗号分隔(不加空格)；含有空格时需要在值外嵌套英文双引号，如：监控项目_Demo 或 监控项目_0,监控项目_1 或 "监控项目_0,监控项目 1"
         * {datetime_日志开始时间}：以格式化日期时间传入；采用24小时制；格式如：yyyy-MM-dd HH:mm:ss
         * {datetime_日志截止时间 =DateTime.Now}：格式同日志开始时间；采用24小时制；可省略，默认值为当前时间
         * {boolean_包含系统信息 =false}：可省略，默认值为 false；可取值为：{false/true}，可忽略大小写
         * {boolean_包含客户端信息 =false}：可省略，默认值为 false；可取值为：{false/true}，可忽略大小写
         * {reportmodes_报告导出模式 =RepostModes.Html}：可省略，默认值为 Html；可取值为：{html/word/excel}，可忽略大小写
         */

        /// <summary>
        /// 入口
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.Title = $"xQuant 日志分析工具 - {Application.ProductVersion}";
            Tracer.WriteLine($"{Console.Title} 已启动...");
            Tracer.WriteLine($"启动参数：\n\t{string.Join("\n\t", args)}");

            Tracer.WriteLine("开始创建任务参数对象...");
            try
            {
                UnityArgument = TaskArgument.Parse(args);
            }
            catch (Exception ex)
            {
                Tracer.WriteLine($"创建任务参数对象失败：{ex.Message}");
                Exit(1);
            }
            Tracer.WriteLine("创建任务参数对象成功");

            //TODO: so much todo ...

            Exit(0);
        }

        /// <summary>
        /// 程序结束
        /// </summary>
        /// <param name="code">程序退出代码</param>
        public static void Exit(int code)
        {
            Console.WriteLine("\n————————\n按任意键退出此程序... (￣▽￣)／");
            Console.Read();
            Environment.Exit(code);
        }

    }
}
