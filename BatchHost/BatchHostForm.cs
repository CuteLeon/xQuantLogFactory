using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BatchHost
{
    public partial class BatchHostForm : Form
    {
        public BatchHostForm()
        {
            this.InitializeComponent();

            this.Icon = UnityResource.BatchHostIcon;
        }
    }
}
