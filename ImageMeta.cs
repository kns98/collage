using System;
using System.Drawing;
using System.Drawing.Imaging;

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
        /// Can be null if the image is not stored on disk.
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
            const int BITS_IN_PIXEL = 4;
            BitmapData srcData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            int stride = srcData.Stride;

            IntPtr Scan0 = srcData.Scan0;

            long[] totals = new long[] { 0, 0, 0 };

            int width = bitmap.Width;
            int height = bitmap.Height;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                for (int y = 0; y < bitmap.Width; y++)
                {
                    for (int x = 0; x < bitmap.Height; x++)
                    {
                        for (int color = 0; color < 3; color++)
                        {
                            int idx = (y * stride) + x * BITS_IN_PIXEL + color;
                            totals[color] += p[idx];
                        }
                    }
                }
            }

            bitmap.UnlockBits(srcData);

            long avgB = totals[0] / (bitmap.Width * bitmap.Height);
            long avgG = totals[1] / (bitmap.Width * bitmap.Height);
            long avgR = totals[2] / (bitmap.Width * bitmap.Height);

            return Color.FromArgb((int)avgR, (int)avgG, (int)avgB);

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
