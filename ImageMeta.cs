using System;
using System.Drawing;
using System.Drawing.Imaging;
#if TIMEIT
using System.Diagnostics;
#endif

namespace CollageMaker
{
    class ImageMeta
    {
        protected Color _overallColor;
        protected Size _size;
        protected string _path;
        
        /// <summary>
        /// A Color representing the average of each pixel in the Image.
        /// </summary>
        public Color OverallColor
        {
            get { return this._overallColor; }
        }

        /// <summary>
        /// A Size representing the width and height of the Image.
        /// </summary>
        public Size Size
        {
            get { return this._size; }
        }

        /// <summary>
        /// A string representing the path to the image, such that Image.FromFile could open it.
        /// </summary>
        public string Path
        {
            get { return this._path; }
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

        /// <summary>
        /// Gets the distance between the overall color of this and another image.
        /// </summary>
        /// <param name="other">The ImageMeta object to compare to.</param>
        /// <returns></returns>
        public float Distance(ImageMeta other)
        {
            return EuclidianColorDistance(this.OverallColor, other.OverallColor);
        }

        private float EuclidianColorDistance(Color color1, Color color2)
        {
            return Math.Abs(color1.R - color2.R) + Math.Abs(color1.G - color2.G) + Math.Abs(color1.B - color2.B);
        }
        
        /// <summary>
        /// Constructs a new ImageMeta object using the given image and file path.
        /// </summary>
        /// <param name="sourceImage">Source Bitmap image</param>
        /// <param name="path">File path (can be null)</param>
        public ImageMeta(Bitmap sourceImage, string path = null)
        {
            this._path = path;
            this._size = sourceImage.Size;
            this._overallColor = ImageMeta.CalculateAverageColor(sourceImage);
        }
    }
}
