﻿using PixelStacker.Extensions;
using PixelStacker.Logic.IO.Config;
using PixelStacker.Logic.Model;
using PixelStacker.Resources;
using SkiaSharp;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PixelStacker.UI.Controls
{
    public partial class CanvasEditor
    {
        private bool IsPainting = false;

        [System.Diagnostics.DebuggerStepThrough]
        private void timerPaint_Tick(object sender, EventArgs e)
        {
            if (this.RepaintRequested)
            {
                if (!IsPainting)
                {
                    IsPainting = true;
                    // Force repaint
                    skiaControl.Refresh();
                    this.RepaintRequested = false;
                }
            }
        }

        private void skiaControl_PaintSurface(object sender, Controls.GenericSKPaintSurfaceEventArgs e)
        {
            IsPainting = true;
            SKSurface surface = e.Surface;
            var g = surface.Canvas;
            var bgImg = UIResources.bg_imagepanel.BitmapToSKBitmap();

            // Background shaders
            SKShader bgShader = SKShader.CreateBitmap(bgImg, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);
            using (SKPaint paint = new SKPaint())
            {
                paint.Shader = bgShader;
                paint.FilterQuality = SKFilterQuality.High;
                paint.IsDither = true;
                g.DrawRect(e.Rect, paint);
                g.DrawBitmap(bgImg, 0, 0);
            }

            // Render the image they are looking at.
            var painter = this.Painter;
            if (painter == null) return;

            var pz = this.PanZoomSettings;
            if (pz == null) return;

            var canvas = this.Canvas;
            if (canvas == null) return;

            var options = this.Options;

            if (options == null || options.ViewerSettings == null) return;
            painter.PaintSurface(g, new SKSize(this.Width, this.Height), pz, options.ViewerSettings);

            Point cursorLoc = CanvasEditor.GetPointOnPanel(CanvasEditor.GetPointOnImage(previousCursorPosition, pz, EstimateProp.Floor), pz);

            
            {
                bool usesBrWidth = this.CurrentTool?.UsesBrushWidth ?? false;
                int brushWidth = usesBrWidth ? Options.Tools.BrushWidth : 1;
                using SKPaint pTest = new SKPaint();
                pTest.Color = new SKColor(255, 255, 255, 128);
                pTest.StrokeWidth = 4;
                pTest.IsStroke = true;

                g.DrawRect(cursorLoc.X - (brushWidth / 2 * (float)pz.zoomLevel),
                    cursorLoc.Y - (brushWidth / 2 * (float)pz.zoomLevel),
                    (float)(brushWidth * pz.zoomLevel),
                    (float)(brushWidth * pz.zoomLevel),
                    pTest);
                
                pTest.StrokeWidth = 1;
                pTest.Color = new SKColor(0, 0, 0, 255);

                g.DrawRect(cursorLoc.X - (brushWidth / 2 * (float)pz.zoomLevel),
                    cursorLoc.Y - (brushWidth / 2 * (float)pz.zoomLevel),
                    (float)(brushWidth * pz.zoomLevel),
                    (float)(brushWidth * pz.zoomLevel),
                    pTest);
            }

            IsPainting = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.DesignMode)
            {
                Graphics g = e.Graphics;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                using Brush bgBrush = new TextureBrush(Resources.UIResources.bg_imagepanel);
                g.FillRectangle(bgBrush, 0, 0, this.Width, this.Height);
                base.OnPaint(e);
                return;
            }
        }
    }
}
