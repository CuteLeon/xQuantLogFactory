using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BatchHost.Utils;

namespace BatchHost
{
    public partial class BatchHostForm
    {
        public void InitExecuteTabPage()
        {
            this.FindDirTextBox.Text = UnityUtils.BuildDirectory;
        }

        /// <summary>
        /// 选择批处理文件查找目录
        /// </summary>
        private void SelectFindDir()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog() { Description = "请选择批处理文件存放目录 ..." })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.FindDirTextBox.Text = dialog.SelectedPath;

                    this.FindBatches(dialog.SelectedPath);
                }
            }
        }

        /// <summary>
        /// 查找批处理文件
        /// </summary>
        /// <param name="targetDir"></param>
        /// <param name="predicate"></param>
        private void FindBatches(string targetDir, string predicate = "*")
        {
            this.BatchesListBox.Items.Clear();

            try
            {
                // 未使用通配字符时，自动嵌套星号
                predicate = $"*{predicate}*";

                this.BatchesListBox.Items.AddRange(
                    Directory.GetFiles(targetDir, predicate, SearchOption.AllDirectories)
                    // .Select(path => Path.GetFileName(path))
                    .ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"查找批处理文件遇到错误：\n{ex.Message}", "查找批处理文件遇到错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 显示批处理预览
        /// </summary>
        /// <param name="path"></param>
        private void ApplyBatchPreview(string path)
        {
            try
            {
                this.PreviewTextBox.Text = File.ReadAllText(path, Encoding.Default);
            }
            catch (Exception ex)
            {
                this.PreviewTextBox.Text = ex.Message;
            }
        }
    }
}
