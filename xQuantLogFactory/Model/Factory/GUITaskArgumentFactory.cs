using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace xQuantLogFactory.Model.Factory
{
    /// <summary>
    /// GUI-任务参数对象工厂
    /// </summary>
    public class GUITaskArgumentFactory
    {

        /// <summary>
        /// 根据工具启动参数创建任务参数对象
        /// </summary>
        /// <returns>任务参数对象</returns>
        public TaskArgument CreateTaskArgument()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (CreateTaskArgumentForm factoryForm = new CreateTaskArgumentForm())
            {
                if (factoryForm.ShowDialog() == DialogResult.OK)
                {

                }

                return default;
            }
        }

    }
}
