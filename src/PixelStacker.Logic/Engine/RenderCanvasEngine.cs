﻿using PixelStacker.Extensions;
using PixelStacker.Logic.Collections.ColorMapper;
using PixelStacker.Logic.Extensions;
using PixelStacker.Logic.IO.Config;
using PixelStacker.Logic.Model;
using PixelStacker.Logic.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;
using SkiaSharp;
using PixelStacker.Logic.Engine.Quantizer;

namespace PixelStacker.Logic.Engine
{

    /// <summary>
    /// 1. Preprocess the image
    ///     a. Resize
    ///     b. Flatten colorspace
    ///     c. Quantize
    /// 2. Convert to "RenderedCanvas" or blueprint format.
    /// 3. Render the blueprint to screen so it can be viewed.
    /// </summary>
    public class RenderCanvasEngine
    {
        /// <summary>
        /// If NULL, it means image is too large.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="safetyMultiplier"></param>
        /// <returns></returns>
        public static int? CalculateTextureSize(int width, int height, int safetyMultiplier = 2)
        {
            int calculatedTextureSize = Constants.TextureSize;
            int bytesInSrcImage = (width * height * 32 / 8); // Still need to multiply by texture size (4 bytes per pixel / 8 bits per byte = 4 bytes)

            bool isSuccess = false;

            do
            {
                if (width * calculatedTextureSize >= 30000 || height * calculatedTextureSize >= 30000)
                {
                    calculatedTextureSize = Math.Max(1, calculatedTextureSize - 1);
                    continue;
                }

                int totalPixels = (width + 1) * height * calculatedTextureSize * calculatedTextureSize * 4;
                if (totalPixels >= int.MaxValue || totalPixels < 0)
                {
                    calculatedTextureSize = Math.Max(1, calculatedTextureSize - 1);
                    continue;
                }

                try
                {
                    int numMegaBytes = bytesInSrcImage // pixels in base image * bytes per pixel
                        * calculatedTextureSize * calculatedTextureSize // size of texture tile squared 
                        / 1024 / 1024       // convert to MB
                        * safetyMultiplier  // Multiply by safety buffer to plan for a bunch of these layers.
                        ;

                    if (numMegaBytes > 0)
                    {
                        using (var memoryCheck = new System.Runtime.MemoryFailPoint(numMegaBytes))
                        {
                        }
                    }

                    isSuccess = true;
                }
                catch (InsufficientMemoryException)
                {
                    calculatedTextureSize = Math.Max(1, calculatedTextureSize - 2);
                }
            } while (isSuccess == false && calculatedTextureSize > 1);

            if (!isSuccess) return null;
            else return calculatedTextureSize;
        }

        private int[] GetNewSize(int Wdt, int Hgt, int Max_W, int Max_H)
        {
            int W_Result = Wdt;
            int H_Result = Hgt;
            if (Wdt > Max_W || Hgt > Max_H)
            {
                if (Max_W * Hgt < Max_H * Wdt)
                {

                    W_Result = Max_W;
                    H_Result = Hgt * Max_W / Wdt;
                }
                else
                {

                    W_Result = Wdt * Max_H / Hgt;
                    H_Result = Max_H;
                }
            }
            return new int[] { W_Result, H_Result };
        }

        /// <summary>
        /// Processes an image prior to be matched up with existing material combinations.
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="LIM"></param>
        /// <param name="settings"></param>
        /// <exception cref="OperationCanceledException"></exception>
        /// <returns></returns>
        public Task<SkiaSharp.SKBitmap> PreprocessImageAsync(CancellationToken? worker, SkiaSharp.SKBitmap LIM, CanvasPreprocessorSettings settings)
        {
            var dims = LIM.Info.Rect;
            // Resize based on max size
            ProgressX.Report(5, "Pre-processing image. Resizing.");
            int mH = Math.Min(settings.MaxHeight ?? dims.Height, dims.Height);
            int mW = Math.Min(settings.MaxWidth ?? dims.Width, dims.Width);


            int[] WH = GetNewSize(dims.Width, dims.Height, mW, mH);

            int W = WH[0];
            int H = WH[1];
            // TODO: How to get "nearest neighboor" sampling selected?
            var resized = LIM.Resize(new SkiaSharp.SKImageInfo(W, H, SkiaSharp.SKColorType.Rgba8888), SkiaSharp.SKFilterQuality.Low);

            // Color bucket normalization
            int F = settings.RgbBucketSize;
            ProgressX.Report(25, $"Pre-processing image. Flattening into RGB buckets of size {F}");
            if (F > 1)
            {
                var tmp = resized.ToEditStream(worker, (x, y, c) => c.Normalize(F));
                resized.DisposeSafely();
                resized = tmp;
            }

            worker.SafeThrowIfCancellationRequested();
            if (settings.QuantizerSettings?.IsEnabled == true)
            {
                ProgressX.Report(50, $"Pre-processing image. Quantizing.");
                var quantized = QuantizerEngine.RenderImage(worker, resized, settings.QuantizerSettings);

                resized.DisposeSafely();
                ProgressX.Report(100, "Finished pre-processing the image.");
                return Task.FromResult(quantized);
            }

            ProgressX.Report(100, "Finished pre-processing the image.");
            return Task.FromResult(resized);
        }

        /// <summary>
        /// </summary>
        /// <param name="worker">A canceller token</param>
        /// <param name="preprocessedImage">Image to be used as the reference image later on.</param>
        /// <param name="mapper">Should be pre-loaded with the enabled material combos.</param>
        /// <param name="palette">The master lookup table for color palettes</param>
        /// <returns></returns>
        public Task<RenderedCanvas> RenderCanvasAsync(
            CancellationToken? worker,
            SKBitmap preprocessedImage,
            IColorMapper mapper,
            MaterialPalette palette
            )
        {
            preprocessedImage = preprocessedImage.Copy();
            RenderedCanvas canvas = new RenderedCanvas()
            {
                WorldEditOrigin = new PxPoint(0, preprocessedImage.Height - 1),
                IsCustomized = false,
                PreprocessedImage = preprocessedImage,
                MaterialPalette = palette,
                CanvasData = new CanvasData(palette, new int[preprocessedImage.Width, preprocessedImage.Height])
            };
            ProgressX.Report(0, Resources.Text.RenderEngine_ConvertingToBlocks);
            preprocessedImage.ToViewStream(worker, (x, y, c) =>
            {
                var mcId = mapper.FindBestMatch(c);
                canvas.CanvasData[x, y] = mcId;
            });

            return Task.FromResult(canvas);
        }
    }
}
