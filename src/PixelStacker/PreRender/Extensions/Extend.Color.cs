﻿using PixelStacker.Logic;
using SimplePaletteQuantizer.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelStacker.PreRender.Extensions
{
    // Sat: 0...1
    // Hue: 0...360
    // Brightness: 0...1
    public static class ExtendColor
    {
        public static float GetDegreeDistance(float alpha, float beta)
        {
            float phi = Math.Abs(beta - alpha) % 360;       // This is either the distance or 360 - distance
            float distance = phi > 180 ? 360 - phi : phi;
            return distance;
        }

        /// <summary>
        /// Custom color matching algorithm
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static float GetColorDistance(this Color c, Color toMatch)
        {
            int dR = (c.R - toMatch.R);
            int dG = (c.G - toMatch.G);
            int dB = (c.B - toMatch.B);
            float dHue = GetDegreeDistance(c.GetHue(), toMatch.GetHue());

            float diff = (
                (dR * dR)
                + (dG * dG)
                + (dB * dB)
                + (float) Math.Pow(dHue, 1.5)
                //+ ((c.GetSaturation() - c.GetSaturation()) * 100).Pow2()
                );

            return diff;
            //double diffd = 1000 *
            //   (
            //   //Math.Pow(Math.Abs(c.R - toMatch.R), Constants.rgbPower)
            //   //+ Math.Pow(Math.Abs(c.G - toMatch.G), Constants.rgbPower)
            //   //+ Math.Pow(Math.Abs(c.B - toMatch.B), Constants.rgbPower)
            //   //+ Math.Pow(GetDegreeDistance(c.GetHue(), toMatch.GetHue()) / 2, Constants.huePower)
            //   //+ Math.Pow(Math.Abs(c.GetSaturation() - toMatch.GetSaturation()), Constants.satPower)
        }

        /// <summary>
        /// Does not normalize alpha channels
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Color Normalize(this Color c)
        {
            int F = Constants.ColorFragmentSize;
            int R = (int) Math.Round(Convert.ToDecimal(c.R) / F, 0) * F;
            int G = (int) Math.Round(Convert.ToDecimal(c.G) / F, 0) * F;
            int B = (int) Math.Round(Convert.ToDecimal(c.B) / F, 0) * F;

            return Color.FromArgb(c.A, R, G, B);
        }

        public static IEnumerable<TSource> OrderByColor<TSource>(this IEnumerable<TSource> source, Func<TSource, Color> colorSelector)
        {
            source.Select(x => colorSelector(x));
            var grayscale = source.Where(x => colorSelector(x).GetSaturation() <= 0.20
            || colorSelector(x).GetBrightness() <= 0.15
            || colorSelector(x).GetBrightness() >= 0.85)
            .OrderByDescending(x => colorSelector(x).GetBrightness());
            const int numHueFragments = 18;

            var colorsInOrder = grayscale.ToList();

            //// Sat: 0...1
            //// Hue: 0...360
            //// Brightness: 0...1
            bool isAscendingBrightness = false;
            source.Except(grayscale)
                .GroupBy(x => (int)Math.Round(colorSelector(x).GetHue()) / numHueFragments)
                .OrderBy(x => x.Key)
                .ToList().ForEach(grouping =>
                {
                    isAscendingBrightness = !isAscendingBrightness;

                    if (isAscendingBrightness)
                    {
                        colorsInOrder.AddRange(grouping.OrderBy(g => colorSelector(g).GetBrightness()));
                    }
                    else
                    {
                        colorsInOrder.AddRange(grouping.OrderByDescending(g => colorSelector(g).GetBrightness()));
                    }
                });

            return colorsInOrder;
        }
    }
}
