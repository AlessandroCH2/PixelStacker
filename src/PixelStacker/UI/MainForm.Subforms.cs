﻿using PixelStacker.Logic;
using PixelStacker.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PixelStacker
{
    partial class MainForm
    {
        private MaterialSelectWindow MaterialOptions { get; set; } = null;

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var about = new AboutForm();
            about.ShowDialog(this);
        }
        
        private void otherOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskManager.Get.CancelTasks(null);
            // Cancel from UI thread
            var about = new OtherOptionsWindow();
            about.ShowDialog(this);
        }

        private void prerenderOptions_Click(object sender, EventArgs e)
        {
            TaskManager.Get.CancelTasks(null);
            var about = new PreRenderOptionsForm(this);
            about.Show();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.MaterialOptions == null)
            {
                this.MaterialOptions = new MaterialSelectWindow();
            }

            TaskManager.Get.CancelTasks(null);
            this.MaterialOptions.ShowDialog(this);
        }
    }
}
