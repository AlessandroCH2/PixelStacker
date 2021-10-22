﻿using PixelStacker.Logic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;

namespace PixelStacker.UI
{
    public partial class AboutForm : Form, ILocalized
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }

            MainForm.Self.konamiWatcher.ProcessKey(keyData);
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void lnkFontMaker_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://taylorlove.info/projects/fontmaker/");  // Adf.ly
            Process.Start(sInfo);
        }

        private void lnkWoolcityProject_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://lumengaming.com/index.php?threads/all-about-the-woolcity-project-freebuild.125/#post-427");  // Adf.ly
            Process.Start(sInfo);
        }

        private void lnkLinkedIn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://www.linkedin.com/in/tayloralove");  // Adf.ly
            Process.Start(sInfo);
        }

        private void lnkDonate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://www.paypal.me/TaylorLove");
            Process.Start(sInfo);
        }

        private void lnkDownload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://www.spigotmc.org/resources/pixelstacker.46812/");  // Adf.ly
            Process.Start(sInfo);
        }

        private void lnkMyWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://taylorlove.info");  // Adf.ly
            Process.Start(sInfo);
        }

        public void ApplyLocalization(CultureInfo locale)
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(AboutForm));
            foreach (Control c in this.Controls)
            {
                resources.ApplyResources(c, c.Name, locale);
            }
        }
    }
}
