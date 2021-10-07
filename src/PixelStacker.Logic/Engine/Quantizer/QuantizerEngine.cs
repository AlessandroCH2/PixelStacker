﻿using PixelStacker.Extensions;
using PixelStacker.IO.Config;
using PixelStacker.Logic.Engine.Quantizer.Enums;
using PixelStacker.Logic.Model;
using SimplePaletteQuantizer.ColorCaches;
using SimplePaletteQuantizer.ColorCaches.Common;
using SimplePaletteQuantizer.ColorCaches.EuclideanDistance;
using SimplePaletteQuantizer.ColorCaches.LocalitySensitiveHash;
using SimplePaletteQuantizer.ColorCaches.Octree;
using SimplePaletteQuantizer.Ditherers;
using SimplePaletteQuantizer.Ditherers.ErrorDiffusion;
using SimplePaletteQuantizer.Ditherers.Ordered;
using SimplePaletteQuantizer.Helpers;
using SimplePaletteQuantizer.Quantizers;
using SimplePaletteQuantizer.Quantizers.DistinctSelection;
using SimplePaletteQuantizer.Quantizers.MedianCut;
using SimplePaletteQuantizer.Quantizers.NeuQuant;
using SimplePaletteQuantizer.Quantizers.Octree;
using SimplePaletteQuantizer.Quantizers.OptimalPalette;
using SimplePaletteQuantizer.Quantizers.Popularity;
using SimplePaletteQuantizer.Quantizers.Uniform;
using SimplePaletteQuantizer.Quantizers.XiaolinWu;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;

namespace SimplePaletteQuantizer
{
    public static class QuantizerEngine
    {
        public static string[] GetQuantizerAlgorithms() => QuantizerAlgorithm.Values;

        private static IColorQuantizer GetQuantizerByAlgorithmName(string algo)
        {
            if (string.IsNullOrEmpty(algo))
                throw new ArgumentNullException(nameof(algo));

            switch (algo)
            {
                case QuantizerAlgorithm.HslDistinctSelection:
                    return new DistinctSelectionQuantizer();
                case QuantizerAlgorithm.MedianCut:
                    return new MedianCutQuantizer();
                case QuantizerAlgorithm.Neural:
                    return new NeuralColorQuantizer();
                case QuantizerAlgorithm.Octree:
                    return new OctreeQuantizer();
                case QuantizerAlgorithm.OptimalPalette:
                    return new OptimalPaletteQuantizer();
                case QuantizerAlgorithm.Popularity:
                    return new PopularityQuantizer();
                case QuantizerAlgorithm.UniformQuantizer:
                    return new UniformQuantizer();
                case QuantizerAlgorithm.WuColor:
                    return new WuColorQuantizer();
                default:
                    throw new ArgumentOutOfRangeException(nameof(algo), algo, "Algorithm must be from supported list of values: [" + string.Join(", ", QuantizerAlgorithm.Values) + "]");
            }
        }

        public static QuantizerAlgorithmOptions GetQuantizerAlgorithmOptions(string algo)
        {
            var opts = new QuantizerAlgorithmOptions();
            IColorQuantizer q = GetQuantizerByAlgorithmName(algo);
            if (q is BaseColorCacheQuantizer)
            {
                opts.ColorCacheList = new OrderedDictionary<string, IColorCache>
                {
                    { "Euclidean distance", new EuclideanDistanceColorCache() },
                    { "Locality-sensitive hash", new LshColorCache () },
                    { "Octree search", new OctreeColorCache() }
                };
            }

            if (q is not WuColorQuantizer)
            {
                opts.DithererList = new OrderedDictionary<string, IColorDitherer>
                {
                    { "No dithering", null },
                    { "--[ Ordered ]--", null },
                    { "Bayer dithering (4x4)", new BayerDitherer4() },
                    { "Bayer dithering (8x8)", new BayerDitherer8() },
                    { "Clustered dot (4x4)", new ClusteredDotDitherer() },
                    { "Dot halftoning (8x8)", new DotHalfToneDitherer() },
                    { "--[ Error diffusion ]--", null },
                    { "Fan dithering (7x3)", new FanDitherer() },
                    { "Shiau dithering (5x3)", new ShiauDitherer() },
                    { "Sierra dithering (5x3)", new SierraDitherer() },
                    { "Stucki dithering (5x5)", new StuckiDitherer() },
                    { "Burkes dithering (5x3)", new BurkesDitherer() },
                    { "Atkinson dithering (5x5)", new AtkinsonDithering() },
                    { "Two-row Sierra dithering (5x3)", new TwoRowSierraDitherer() },
                    { "Floyd–Steinberg dithering (3x3)", new FloydSteinbergDitherer() },
                    { "Jarvis-Judice-Ninke dithering (5x5)", new JarvisJudiceNinkeDitherer() },
                };
            }

            if (q.AllowParallel)
            {
                opts.MaxParallelProcessesList = new List<int>()
                {
                    1, 2, 4, 8, 16, 32, 64
                };
            }

            if (!(q is UniformQuantizer || q is NeuralColorQuantizer || q is OptimalPaletteQuantizer))
            {
                opts.MaxColorCountsList = new List<int>()
                {
                    2, 4, 8, 16, 32, 64, 128, 256
                };
            }

            return opts;
        }

        /// <summary>
        ///  SOURCE IMAGE SHOULD BE 32bbpARGB
        /// </summary>
        /// <param name="sourceImage"></param>
        /// <exception cref="OperationCanceledException"></exception>
        /// <returns></returns>
        public static Bitmap RenderImage(CancellationToken? _worker, Bitmap sourceImage, QuantizerSettings settings, QuantizerAlgorithmOptions opts = null)
        {
            /// prepares quantizer
            opts ??= GetQuantizerAlgorithmOptions(settings.Algorithm);
            if (!settings.IsValid(opts, !Constants.IsDevMode))
                throw new Exception("Invalid settings. Verify all settings are correct before moving forward!");

            // tries to retrieve an image based on HSB quantization
            var activeQuantizer = GetQuantizerByAlgorithmName(settings.Algorithm);
            if (activeQuantizer is BaseColorCacheQuantizer)
            {
                var colorCacheProvider = opts.ColorCacheList[settings.ColorCache];
                ((BaseColorCacheQuantizer)activeQuantizer).ChangeCacheProvider(colorCacheProvider);
            }

            var activeDitherer = opts.DithererList[settings.DitherAlgorithm];
            int parallelTaskCount = activeQuantizer.AllowParallel ? settings.MaxParallelProcesses : 1;
            int colorCount = settings.MaxColorCount;
            
            try
            {
                // For some reason the super quick algo failed. Need to fail over to this super safe one.
                using (Image targetImage = ImageBuffer.QuantizeImage(sourceImage, activeQuantizer, activeDitherer, colorCount, parallelTaskCount))
                {
                    using (Bitmap formattedBM = targetImage.To32bppBitmap())
                    {
                        var returnVal = sourceImage.ToMergeStream(formattedBM, _worker, (x, y, o, n) => {
                            if (o.A < 32) return Color.Transparent;
                            else return n;
                        });

                        return returnVal;
                    }
                }
            }
            catch (Exception)
            {
                // Throw THIS type if cancellation caused the issue
                _worker.SafeThrowIfCancellationRequested();
                throw; // Throw whatever type was already there if it is something else.
            }

        }
    }
}
