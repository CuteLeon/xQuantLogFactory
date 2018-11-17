using System;
using System.Threading;
using System.Windows.Forms;

namespace xQuantLogFactory.Model.Factory
{
    /// <summary>
    /// GUI-任务参数对象工厂
    /// </summary>
    public class GUITaskArgumentFactory
    {
        /// <summary>
        /// 工厂异常对象在线程交互对象
        /// </summary>
        private Exception FactoryException = null;

        /// <summary>
        /// 任务参数实例
        /// </summary>
        private TaskArgument TargetTaskArgument = null;

        private static Lazy<GUITaskArgumentFactory> factory = new Lazy<GUITaskArgumentFactory>();
        /// <summary>
        /// 任务参数工厂实例
        /// </summary>
        public static GUITaskArgumentFactory Intance
        {
            get => factory.Value;
        }

        /// <summary>
        /// 根据工具启动参数创建任务参数对象
        /// </summary>
        /// <returns>任务参数对象</returns>
        public TaskArgument CreateTaskArgument()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Thread GUIThread = new Thread(new ThreadStart(this.ShowGUIFactory));
            GUIThread.SetApartmentState(ApartmentState.STA);
            GUIThread.Start();
            GUIThread.Join();

            if (this.FactoryException != null)
                throw this.FactoryException;

            return this.TargetTaskArgument;
        }

        private void ShowGUIFactory()
        {
            //初始化
            this.FactoryException = null;

            try
            {
                using (CreateTaskArgumentForm factoryForm = new CreateTaskArgumentForm())
                {
                    if (factoryForm.ShowDialog() != DialogResult.OK)
                    {
                        throw new OperationCanceledException();
                    }

                    this.TargetTaskArgument = factoryForm.TargetTaskArgument;
                }
            }
            catch (Exception ex)
            {
                this.FactoryException = ex;
            }
        }

    }
}
