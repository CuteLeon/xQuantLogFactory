using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatchHost
{
    public partial class BatchHostForm
    {
        private void BuildControlPanel_Resize(object sender, EventArgs e)
        {
            this.BuildDirTextBox.Width = this.BuildButton.Left - this.BuildDirTextBox.Left - 20;
            this.BuildDirTextBox.TextBoxWidth = this.BuildDirTextBox.Width - 60;
        }
    }
}
