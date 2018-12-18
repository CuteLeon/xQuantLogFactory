using System;
using System.Threading;
using System.Windows.Forms;

namespace xQuantLogFactory.Model.Factory
{
    /// <summary>
    /// GUI-任务参数对象工厂
    /// </summary>
    public class GUITaskArgumentFactory : ITaskArgumentFactory
    {
        private static Lazy<GUITaskArgumentFactory> factory = new Lazy<GUITaskArgumentFactory>();

        /// <summary>
        /// 工厂异常对象在线程交互对象
        /// </summary>
        private Exception factoryException = null;

        /// <summary>
        /// 任务参数实例
        /// </summary>
        private TaskArgument targetTaskArgument = null;

        /// <summary>
        /// Gets 任务参数工厂实例
        /// </summary>
        public static GUITaskArgumentFactory Intance
        {
            get => factory.Value;
        }

        /// <summary>
        /// 根据工具启动参数创建任务参数对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns>任务参数对象</returns>
        public TaskArgument CreateTaskArgument<T>(T source = null)
            where T : class
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Thread guiThread = new Thread(new ParameterizedThreadStart(this.ShowGUIFactory));
            guiThread.SetApartmentState(ApartmentState.STA);
            guiThread.Start(source);
            guiThread.Join();

            if (this.factoryException != null)
            {
                throw this.factoryException;
            }

            return this.targetTaskArgument;
        }

        private void ShowGUIFactory(object taskArgument = null)
        {
            // 初始化
            this.factoryException = null;

            try
            {
                using (CreateTaskArgumentForm factoryForm = new CreateTaskArgumentForm())
                {
                    // 将已有任务参数对象赋值为窗体
                    if (taskArgument is TaskArgument argument)
                    {
                        factoryForm.TargetTaskArgument = argument;
                    }

                    if (factoryForm.ShowDialog() != DialogResult.OK)
                    {
                        throw new OperationCanceledException();
                    }

                    this.targetTaskArgument = factoryForm.TargetTaskArgument;
                }
            }
            catch (Exception ex)
            {
                this.factoryException = ex;
            }
        }
    }
}
