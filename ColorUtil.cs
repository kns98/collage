using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollageMaker
{
    class ColorUtil
    {
        public enum ColorDistanceType
        {
            Euclidean,
            DeltaE
        }

        public struct LAB
        {
            public double L;
            public double A;
            public double B;
        }

        /// <summary>
        /// Finds the average Color of the image.
        /// The average color is the sum of each channel of each pixel divided by number of pixels.
        /// 
        /// Stolen from http://stackoverflow.com/a/1068404
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Color CalculateAverageColor(Bitmap bitmap)
        {
#if TIMEIT
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
#endif
            long[] totals = new long[] { 0, 0, 0 };
            BitmapData srcData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            int stride = srcData.Stride;
            IntPtr Scan0 = srcData.Scan0;

            int bppModifier = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        int idx = (y * stride) + x * bppModifier;
                        totals[2] += p[idx + 2];
                        totals[1] += p[idx + 1];
                        totals[0] += p[idx];
                    }
                }
            }

            bitmap.UnlockBits(srcData);

            int avgR = (int)(totals[2] / (bitmap.Width * bitmap.Height));
            int avgG = (int)(totals[1] / (bitmap.Width * bitmap.Height));
            int avgB = (int)(totals[0] / (bitmap.Width * bitmap.Height));

#if TIMEIT
            stopwatch.Stop();
            Trace.WriteLine("CalculateAverageColor Time elapsed: " + (stopwatch.ElapsedMilliseconds / 1000.0).ToString());
#endif

            return Color.FromArgb(avgR, avgG, avgB);
        }

        public static float EuclidianColorDistance(Color color1, Color color2)
        {
            return Math.Abs(color1.R - color2.R) + Math.Abs(color1.G - color2.G) + Math.Abs(color1.B - color2.B);
        }

        /// <summary>
        /// XYZ to L*a*b* transformation function.
        /// </summary>
        private static double Fxyz(double t)
        {
            return ((t > 0.008856) ? Math.Pow(t, (1.0 / 3.0)) : (7.787 * t + 16.0 / 116.0));
        }

        /// <summary>
        /// Converts RGB to CIE XYZ (CIE 1931 color space)
        /// Completely ripped from http://www.codeproject.com/Articles/19045/Manipulating-colors-in-NET-Part
        /// </summary>
        public static LAB RGBtoLAB(Color color)
        {
            // normalize red, green, blue values
            double rLinear = (double)color.R / 255.0;
            double gLinear = (double)color.G / 255.0;
            double bLinear = (double)color.B / 255.0;

            // convert to a sRGB form
            double r = (rLinear > 0.04045) ? Math.Pow((rLinear + 0.055) / (
                1 + 0.055), 2.2) : (rLinear / 12.92);
            double g = (gLinear > 0.04045) ? Math.Pow((gLinear + 0.055) / (
                1 + 0.055), 2.2) : (gLinear / 12.92);
            double b = (bLinear > 0.04045) ? Math.Pow((bLinear + 0.055) / (
                1 + 0.055), 2.2) : (bLinear / 12.92);

            // get x y z
            double x = r * 0.4124 + g * 0.3576 + b * 0.1805;
            double y = r * 0.2126 + g * 0.7152 + b * 0.0722;
            double z = r * 0.0193 + g * 0.1192 + b * 0.9505;

            LAB lab = new LAB();
            double D65X = 0.9505;
            double D65Y = 1.0;
            double D65Z = 1.0890;
            lab.L = 116.0 * Fxyz(y / D65Y) - 16;
            lab.A = 500.0 * (Fxyz(x / D65X) - Fxyz(y / D65Y));
            lab.B = 200.0 * (Fxyz(y / D65Y) - Fxyz(z / D65Z));

            return lab;
        }

        private static double DegreeToRad(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }

        private static double RadToDegree(double rads)
        {
            return rads * (180.0 / Math.PI);
        }

        private static double twentyFiveToSeventh = Math.Pow(25, 7);
        private static double k_L = 1;
        private static double k_C = 1;
        private static double k_H = 1;
        public static double DeltaE(Color color1, Color color2)
        {
            LAB lab1 = RGBtoLAB(color1);
            LAB lab2 = RGBtoLAB(color2);
            //All values are stored as rads.
            double CStar1 = Math.Sqrt(Math.Pow(lab1.A, 2.0) + Math.Pow(lab1.B, 2));
            double CStar2 = Math.Sqrt(Math.Pow(lab2.A, 2.0) + Math.Pow(lab2.B, 2));
            double CHat = (CStar1 + CStar2) / 2.0;
            double CHatSevenBlock = Math.Sqrt(Math.Pow(CHat, 7.0) / (Math.Pow(CHat, 7.0) + twentyFiveToSeventh));
            double aPrime2 = lab2.A + ((lab2.A / 2.0) * (1.0 - CHatSevenBlock));
            double aPrime1 = lab1.A + ((lab1.A / 2.0) * (1.0 - CHatSevenBlock));
            double hPrime2 = Math.Atan2(lab2.B, aPrime2) % DegreeToRad(360.0);
            double hPrime1 = Math.Atan2(lab1.B, aPrime1) % DegreeToRad(360.0);
            double CPrime2 = Math.Sqrt(Math.Pow(aPrime2, 2.0) + Math.Pow(lab2.B, 2.0));
            double CPrime1 = Math.Sqrt(Math.Pow(aPrime1, 2.0) + Math.Pow(lab1.B, 2.0));
            double CHatPrime = (CPrime1 + CPrime2) / 2.0;
            double deltaCPrime = CPrime2 - CPrime1;
            double HHatPrime = Math.Abs(hPrime1 - hPrime2) > DegreeToRad(180.0) ?
                (hPrime1 + hPrime2 + DegreeToRad(360.0)) / 2.0 :
                (hPrime1 + hPrime2) / 2.0;
            double R_T = -2.0 * CHatSevenBlock * Math.Sin(DegreeToRad(60.0) * Math.Exp(-Math.Pow((HHatPrime - DegreeToRad(275.0)) / DegreeToRad(25.0), 2.0)));
            double T = 1.0 - 0.17 * Math.Cos(HHatPrime - DegreeToRad(30.0)) +
                0.24 * Math.Cos(2.0 * HHatPrime) +
                0.32 * Math.Cos(3.0 * HHatPrime + DegreeToRad(6.0)) -
                0.20 * Math.Cos(4.0 * HHatPrime - DegreeToRad(63.0));
            double S_H = 1.0 + 0.015 * CHatPrime * T;
            double S_C = 1.0 + 0.045 * CHatPrime;
            double LHat = (lab1.L + lab2.L) / 2.0;
            double S_L = 1.0 + (0.015 * Math.Pow(LHat - 50.0, 2.0)) / Math.Sqrt(20.0 + Math.Pow(LHat - 50.0, 2.0));
            double deltahPrime = hPrime2 - hPrime1;
            double deltaHPrime = 2.0 * Math.Sqrt(CPrime1 * CPrime2) * Math.Sin(deltahPrime / 2.0);
            if (Math.Abs(hPrime1 - hPrime2) > DegreeToRad(180.0))
                deltahPrime += (hPrime2 <= hPrime1 ? DegreeToRad(360.0) : DegreeToRad(-360.0));

            double deltaLPrime = lab2.L - lab1.L;
            double deltaE = Math.Sqrt(
                Math.Pow((deltaLPrime / (k_L * S_L)), 2.0) +
                Math.Pow((deltaCPrime / (k_C * S_C)), 2.0) +
                Math.Pow((deltaHPrime / (k_H * S_H)), 2.0) +
                (R_T * (deltaCPrime / (k_C * S_C)) * (deltaHPrime / (k_H * S_H))));
            return deltaE;
        }
    }
}
