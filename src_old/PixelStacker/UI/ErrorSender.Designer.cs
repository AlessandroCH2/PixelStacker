﻿namespace PixelStacker
{
    partial class ErrorSender
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cbxIncludeImage = new System.Windows.Forms.CheckBox();
            this.cbxIsUploadSavedSettings = new System.Windows.Forms.CheckBox();
            this.cbxIsStackTraceEnabled = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.btnYes = new System.Windows.Forms.Button();
            this.btnNo = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // cbxIncludeImage
            // 
            this.cbxIncludeImage.AutoSize = true;
            this.cbxIncludeImage.Checked = true;
            this.cbxIncludeImage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxIncludeImage.Location = new System.Drawing.Point(12, 217);
            this.cbxIncludeImage.Name = "cbxIncludeImage";
            this.cbxIncludeImage.Size = new System.Drawing.Size(457, 21);
            this.cbxIncludeImage.TabIndex = 0;
            this.cbxIncludeImage.Text = global::PixelStacker.Resources.Text.ErrorSender_IncludeImage;
            this.cbxIncludeImage.UseVisualStyleBackColor = true;
            // 
            // cbxIsUploadSavedSettings
            // 
            this.cbxIsUploadSavedSettings.AutoSize = true;
            this.cbxIsUploadSavedSettings.Checked = true;
            this.cbxIsUploadSavedSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxIsUploadSavedSettings.Enabled = false;
            this.cbxIsUploadSavedSettings.Location = new System.Drawing.Point(12, 117);
            this.cbxIsUploadSavedSettings.Name = "cbxIsUploadSavedSettings";
            this.cbxIsUploadSavedSettings.Size = new System.Drawing.Size(338, 21);
            this.cbxIsUploadSavedSettings.TabIndex = 1;
            this.cbxIsUploadSavedSettings.Text = global::PixelStacker.Resources.Text.ErrorSender_SavedOptions;
            this.cbxIsUploadSavedSettings.UseVisualStyleBackColor = true;
            // 
            // cbxIsStackTraceEnabled
            // 
            this.cbxIsStackTraceEnabled.AutoSize = true;
            this.cbxIsStackTraceEnabled.Checked = true;
            this.cbxIsStackTraceEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxIsStackTraceEnabled.Enabled = false;
            this.cbxIsStackTraceEnabled.Location = new System.Drawing.Point(12, 144);
            this.cbxIsStackTraceEnabled.Name = "cbxIsStackTraceEnabled";
            this.cbxIsStackTraceEnabled.Size = new System.Drawing.Size(335, 21);
            this.cbxIsStackTraceEnabled.TabIndex = 2;
            this.cbxIsStackTraceEnabled.Text = global::PixelStacker.Resources.Text.ErrorSender_StackTrace;
            this.cbxIsStackTraceEnabled.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Underline);
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(283, 31);
            this.label1.TabIndex = 3;
            this.label1.Text = global::PixelStacker.Resources.Text.ErrorSender_ErrorOccurred;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(443, 51);
            this.label2.TabIndex = 4;
            this.label2.Text = global::PixelStacker.Resources.Text.ErrorSender_SendMeSomeInfo;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Enabled = false;
            this.checkBox1.Location = new System.Drawing.Point(12, 171);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(280, 21);
            this.checkBox1.TabIndex = 5;
            this.checkBox1.Text = global::PixelStacker.Resources.Text.ErrorSender_Version;
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // btnYes
            // 
            this.btnYes.Location = new System.Drawing.Point(12, 259);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(205, 98);
            this.btnYes.TabIndex = 6;
            this.btnYes.Text = global::PixelStacker.Resources.Text.ErrorSender_Yes;
            this.btnYes.UseVisualStyleBackColor = true;
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            // 
            // btnNo
            // 
            this.btnNo.Location = new System.Drawing.Point(235, 259);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(220, 98);
            this.btnNo.TabIndex = 7;
            this.btnNo.Text = global::PixelStacker.Resources.Text.ErrorSender_No;
            this.btnNo.UseVisualStyleBackColor = true;
            this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(72, 83);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(91, 17);
            this.linkLabel1.TabIndex = 10;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = global::PixelStacker.Resources.Text.ErrorSender_Github;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // ErrorSender
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 372);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.btnYes);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbxIsStackTraceEnabled);
            this.Controls.Add(this.cbxIsUploadSavedSettings);
            this.Controls.Add(this.cbxIncludeImage);
            this.Icon = global::PixelStacker.Resources.UIResources.wool;
            this.Name = "ErrorSender";
            this.Text = global::PixelStacker.Resources.Text.ErrorSender_Title;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbxIncludeImage;
        private System.Windows.Forms.CheckBox cbxIsUploadSavedSettings;
        private System.Windows.Forms.CheckBox cbxIsStackTraceEnabled;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.Button btnNo;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}