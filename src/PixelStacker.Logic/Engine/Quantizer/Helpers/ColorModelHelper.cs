﻿using System;
using System.Collections.Generic;
using System.Drawing;
using PixelStacker.Logic.Engine.Quantizer.ColorCaches.Common;

namespace PixelStacker.Logic.Engine.Quantizer.Helpers
{
    public class ColorModelHelper
    {
        #region | Constants |

        private const int X = 0;
        private const int Y = 1;
        private const int Z = 2;

        private const float Epsilon = 1E-05f;
        private const float OneThird = 1.0f / 3.0f;
        private const float TwoThirds = 2.0f * OneThird;
        public const double HueFactor = 1.4117647058823529411764705882353;

        private static readonly float[] XYZWhite = new[] { 95.05f, 100.00f, 108.90f };

        #endregion

        #region | -> RGB |

        private static int GetColorComponent(float v1, float v2, float hue)
        {
            float preresult;

            if (hue < 0.0f) hue++;
            if (hue > 1.0f) hue--;

            if (6.0f * hue < 1.0f)
            {
                preresult = v1 + (v2 - v1) * 6.0f * hue;
            }
            else if (2.0f * hue < 1.0f)
            {
                preresult = v2;
            }
            else if (3.0f * hue < 2.0f)
            {
                preresult = v1 + (v2 - v1) * (TwoThirds - hue) * 6.0f;
            }
            else
            {
                preresult = v1;
            }

            return Convert.ToInt32(255.0f * preresult);
        }

        public static Color HSBtoRGB(float hue, float saturation, float brightness)
        {
            // initializes the default black
            int red = 0;
            int green = 0;
            int blue = 0;

            // only if there is some brightness; otherwise leave it pitch black
            if (brightness > 0.0f)
            {
                // if there is no saturation; leave it gray based on the brightness only
                if (Math.Abs(saturation - 0.0f) < Epsilon)
                {
                    red = green = blue = Convert.ToInt32(255.0f * brightness);
                }
                else // the color is more complex
                {
                    // converts HSL cylinder to one slice (its factors)
                    float factorHue = hue / 360.0f;
                    float factorA = brightness < 0.5f ? brightness * (1.0f + saturation) : brightness + saturation - brightness * saturation;
                    float factorB = 2.0f * brightness - factorA;

                    // maps HSL slice to a RGB cube
                    red = GetColorComponent(factorB, factorA, factorHue + OneThird);
                    green = GetColorComponent(factorB, factorA, factorHue);
                    blue = GetColorComponent(factorB, factorA, factorHue - OneThird);
                }
            }

            int argb = 255 << 24 | red << 16 | green << 8 | blue;
            return Color.FromArgb(argb);
        }

        #endregion

        #region | RGB -> |

        public static void RGBtoLab(int red, int green, int blue, out float l, out float a, out float b)
        {
            RGBtoXYZ(red, green, blue, out float x, out float y, out float z);
            XYZtoLab(x, y, z, out l, out a, out b);
        }

        public static void RGBtoXYZ(int red, int green, int blue, out float x, out float y, out float z)
        {
            // normalize red, green, blue values
            double redFactor = red / 255.0;
            double greenFactor = green / 255.0;
            double blueFactor = blue / 255.0;

            // convert to a sRGB form
            double sRed = redFactor > 0.04045 ? Math.Pow((redFactor + 0.055) / (1 + 0.055), 2.2) : redFactor / 12.92;
            double sGreen = greenFactor > 0.04045 ? Math.Pow((greenFactor + 0.055) / (1 + 0.055), 2.2) : greenFactor / 12.92;
            double sBlue = blueFactor > 0.04045 ? Math.Pow((blueFactor + 0.055) / (1 + 0.055), 2.2) : blueFactor / 12.92;

            // converts
            x = Convert.ToSingle(sRed * 0.4124 + sGreen * 0.3576 + sBlue * 0.1805);
            y = Convert.ToSingle(sRed * 0.2126 + sGreen * 0.7152 + sBlue * 0.0722);
            z = Convert.ToSingle(sRed * 0.0193 + sGreen * 0.1192 + sBlue * 0.9505);
        }

        #endregion

        #region | XYZ -> |

        private static float GetXYZValue(float value)
        {
            return value > 0.008856f ? (float)Math.Pow(value, OneThird) : 7.787f * value + 16.0f / 116.0f;
        }

        public static void XYZtoLab(float x, float y, float z, out float l, out float a, out float b)
        {
            l = 116.0f * GetXYZValue(y / XYZWhite[Y]) - 16.0f;
            a = 500.0f * (GetXYZValue(x / XYZWhite[X]) - GetXYZValue(y / XYZWhite[Y]));
            b = 200.0f * (GetXYZValue(y / XYZWhite[Y]) - GetXYZValue(z / XYZWhite[Z]));
        }

        #endregion

        #region | Methods |

        public static long GetColorEuclideanDistance(ColorModel colorModel, Color requestedColor, Color realColor)
        {
            GetColorComponents(colorModel, requestedColor, realColor, out float componentA, out float componentB, out float componentC);
            return (long)(componentA * componentA + componentB * componentB + componentC * componentC);
        }

        public static int GetEuclideanDistance(Color color, ColorModel colorModel, IList<Color> palette)
        {
            // initializes the best difference, set it for worst possible, it can only get better
            long leastDistance = long.MaxValue;
            int result = 0;

            if (palette != null)
            {
                for (int index = 0; index < palette.Count; index++)
                {
                    Color targetColor = palette[index];
                    long distance = GetColorEuclideanDistance(colorModel, color, targetColor);

                    // if a difference is zero, we're good because it won't get better
                    if (distance == 0)
                    {
                        result = index;
                        break;
                    }

                    // if a difference is the best so far, stores it as our best candidate
                    if (distance < leastDistance)
                    {
                        leastDistance = distance;
                        result = index;
                    }
                }
            }
            else
            {

            }

            return result;
        }

        public static int GetComponentA(ColorModel colorModel, Color color)
        {
            int result = 0;

            switch (colorModel)
            {
                case ColorModel.RedGreenBlue:
                    result = color.R;
                    break;

                case ColorModel.HueSaturationBrightness:
                    result = Convert.ToInt32(color.GetHue() / HueFactor);
                    break;

                case ColorModel.LabColorSpace:
                    float l;
                    RGBtoLab(color.R, color.G, color.B, out l, out _, out _);
                    result = Convert.ToInt32(l * 255.0f);
                    break;
            }

            return result;
        }

        public static int GetComponentB(ColorModel colorModel, Color color)
        {
            int result = 0;

            switch (colorModel)
            {
                case ColorModel.RedGreenBlue:
                    result = color.G;
                    break;

                case ColorModel.HueSaturationBrightness:
                    result = Convert.ToInt32(color.GetSaturation() * 255);
                    break;

                case ColorModel.LabColorSpace:
                    float a;
                    RGBtoLab(color.R, color.G, color.B, out _, out a, out _);
                    result = Convert.ToInt32(a * 255.0f);
                    break;
            }

            return result;
        }

        public static int GetComponentC(ColorModel colorModel, Color color)
        {
            int result = 0;

            switch (colorModel)
            {
                case ColorModel.RedGreenBlue:
                    result = color.B;
                    break;

                case ColorModel.HueSaturationBrightness:
                    result = Convert.ToInt32(color.GetBrightness() * 255);
                    break;

                case ColorModel.LabColorSpace:
                    float b;
                    RGBtoLab(color.R, color.G, color.B, out _, out _, out b);
                    result = Convert.ToInt32(b * 255.0f);
                    break;
            }

            return result;
        }

        public static void GetColorComponents(ColorModel colorModel, Color color, out float componentA, out float componentB, out float componentC)
        {
            componentA = 0.0f;
            componentB = 0.0f;
            componentC = 0.0f;

            switch (colorModel)
            {
                case ColorModel.RedGreenBlue:
                    componentA = color.R;
                    componentB = color.G;
                    componentC = color.B;
                    break;

                case ColorModel.HueSaturationBrightness:
                    componentA = color.GetHue();
                    componentB = color.GetSaturation();
                    componentC = color.GetBrightness();
                    break;

                case ColorModel.LabColorSpace:
                    RGBtoLab(color.R, color.G, color.B, out componentA, out componentB, out componentC);
                    break;

                case ColorModel.XYZ:
                    RGBtoXYZ(color.R, color.G, color.B, out componentA, out componentB, out componentC);
                    break;
            }
        }

        public static void GetColorComponents(ColorModel colorModel, Color color, Color targetColor, out float componentA, out float componentB, out float componentC)
        {
            componentA = 0.0f;
            componentB = 0.0f;
            componentC = 0.0f;

            switch (colorModel)
            {
                case ColorModel.RedGreenBlue:
                    componentA = color.R - targetColor.R;
                    componentB = color.G - targetColor.G;
                    componentC = color.B - targetColor.B;
                    break;

                case ColorModel.HueSaturationBrightness:
                    componentA = color.GetHue() - targetColor.GetHue();
                    componentB = color.GetSaturation() - targetColor.GetSaturation();
                    componentC = color.GetBrightness() - targetColor.GetBrightness();
                    break;

                case ColorModel.LabColorSpace:

                    float sourceL, sourceA, sourceB;
                    float targetL, targetA, targetB;

                    RGBtoLab(color.R, color.G, color.B, out sourceL, out sourceA, out sourceB);
                    RGBtoLab(targetColor.R, targetColor.G, targetColor.B, out targetL, out targetA, out targetB);

                    componentA = sourceL - targetL;
                    componentB = sourceA - targetA;
                    componentC = sourceB - targetB;

                    break;

                case ColorModel.XYZ:

                    float sourceX, sourceY, sourceZ;
                    float targetX, targetY, targetZ;

                    RGBtoXYZ(color.R, color.G, color.B, out sourceX, out sourceY, out sourceZ);
                    RGBtoXYZ(targetColor.R, targetColor.G, targetColor.B, out targetX, out targetY, out targetZ);

                    componentA = sourceX - targetX;
                    componentB = sourceY - targetY;
                    componentC = sourceZ - targetZ;

                    break;
            }
        }

        #endregion
    }
}