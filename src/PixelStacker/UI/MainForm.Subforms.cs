﻿using PixelStacker.Logic.Utilities;
using PixelStacker.UI.Forms;
using System;

namespace PixelStacker.UI
{
    public partial class MainForm
    {
        private CanvasTools canvasEditorToolbox { get; set; } = null;
        private MaterialSelectWindow MaterialOptions { get; set; } = null;
        private ColorReducerForm ColorReducerForm { get; set; } = null;
        public MaterialPickerForm MaterialPickerForm { get; private set; }

        private void gridOptionsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            using var form = new GridSettingsForm();
            form.Options = this.Options;
            this.snapManager.RegisterChild(form);
            form.ShowDialog(this);
        }

        private void selectMaterialsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.MaterialOptions == null)
            {
                this.MaterialOptions = new MaterialSelectWindow(this.Options);

                this.MaterialOptions.OnColorPaletteRecompileRequested = (token) =>
                {
                    ProgressX.Report(40, Resources.Text.Progress_CompilingColorMap);
                    this.ColorMapper.SetSeedData(this.Palette.ToValidCombinationList(this.Options), this.Palette, this.Options.Preprocessor.IsSideView);
                    ProgressX.Report(100, Resources.Text.Progress_CompiledColorMap);
                };
            }

            this.MaterialOptions.ShowDialog(this);
        }

        private void canvasEditorToolsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (this.canvasEditorToolbox == null || this.canvasEditorToolbox.IsDisposed)
            {
                this.canvasEditorToolbox = new CanvasTools();
                this.snapManager.RegisterChild(this.canvasEditorToolbox);
                this.canvasEditor.SetCanvasToolboxEvents(this.canvasEditorToolbox);
            }

            this.canvasEditorToolbox.Show();
            this.canvasEditorToolbox.BringToFront();
        }

        private void contributorsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            var form = new Credits();
            form.ShowDialog(this);
        }

        private void sizingToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            var form = new SizeForm(this.Options);
            form.ShowDialog(this);
        }

        private void swatchToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (this.MaterialPickerForm == null || this.MaterialPickerForm.IsDisposed)
            {
                this.MaterialPickerForm = new MaterialPickerForm();
                this.snapManager.RegisterChild(this.MaterialPickerForm);
            }

            if (!this.MaterialPickerForm.Visible)
            {
                this.MaterialPickerForm.Show(this);
            }
            else
            {
                this.MaterialPickerForm.Hide();
            }
        }


        private void preprocessingToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (this.ColorReducerForm == null || this.ColorReducerForm.IsDisposed)
            {
                this.ColorReducerForm = new ColorReducerForm(this, this.Options);
                this.snapManager.RegisterChild(this.ColorReducerForm);
            }

            if (!this.ColorReducerForm.Visible)
            {
                this.ColorReducerForm.Show(this);
            }
        }
    }
}
