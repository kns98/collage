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
        /// Gets the distance between the overall color of this and another image.
        /// </summary>
        /// <param name="other">The ImageMeta object to compare to.</param>
        /// <returns></returns>
        public float Distance(ImageMeta other, ColorUtil.ColorDistanceType type)
        {
            switch (type)
            {
                case ColorUtil.ColorDistanceType.Euclidean:
                    return ColorUtil.EuclidianColorDistance(this.OverallColor, other.OverallColor);
                case ColorUtil.ColorDistanceType.DeltaE:
                    return (float)ColorUtil.DeltaE(this.OverallColor, other.OverallColor);
                default:
                    return 1.0F;
            }
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
            this._overallColor = ColorUtil.CalculateAverageColor(sourceImage);
        }
    }
}
