using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BatchHost.Utils;

namespace BatchHost
{
    public partial class BatchHostForm
    {
        public void InitExecuteTabPage()
        {
            this.FindDirTextBox.Text = UnityUtils.BuildDirectory;
        }
    }
}
