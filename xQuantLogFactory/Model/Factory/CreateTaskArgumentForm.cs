using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace xQuantLogFactory.Model.Factory
{
    public partial class CreateTaskArgumentForm : Form
    {
        /*
         * logdir={string_日志文件目录}：目录含有空格时需要在值外嵌套英文双引号；如：C:\TEST_DIR 或 "C:\TEST DIR" 
         * monitor={string_监视规则文件名称}：可省略，默认为所有监视规则；程序监控的项目名称列表；如：监控项目.xml"
         * start={datetime_日志开始时间}：可省略，以格式化日期时间传入；采用24小时制；格式如：yyyy-MM-dd HH:mm:ss
         * finish={datetime_日志截止时间 =DateTime.Now}：可省略，默认值为当前时间；格式同日志开始时间；采用24小时制；
         * sysinfo={boolean_包含系统信息 =false}：可省略，默认值为 false；可取值为：{false/true}，可忽略大小写
         * cltinfo={boolean_包含客户端信息 =false}：可省略，默认值为 false；可取值为：{false/true}，可忽略大小写
         * report={reportmodes_报告导出模式 =RepostModes.Html}：可省略，默认值为 Html；可取值为：{html/word/excel}，可忽略大小写
         */

        public CreateTaskArgumentForm()
        {
            this.InitializeComponent();
            this.Icon = UnityResource.xQuantLogFactoryIcon;
        }

        /// <summary>
        /// 创建任务参数对象
        /// </summary>
        public TaskArgument TragetTaskArgument { get; protected set; }

        private void OKButton_Click(object sender, EventArgs e)
        {

        }

        private void CancelButton_Click(object sender, EventArgs e)
        {

        }

    }
}
