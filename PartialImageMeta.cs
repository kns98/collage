using System.Drawing;

namespace CollageMaker
{
    class PartialImageMeta : ImageMeta
    {
        protected Point _start;

        /// <summary>
        /// Divides an image into equal cells based on numRows and numCols.
        /// </summary>
        /// <param name="numRows">The number of rows of cells to divide the image into.</param>
        /// <param name="numCols">The number of cols of cells to divide the image into.</param>
        /// <returns>An array of PartialImageMetas</returns>
        public static PartialImageMeta[] ArrayFromImage(Bitmap sourceImage, string sourceImagePath, int numRows, int numCols)
        {
            PartialImageMeta[] cells = new PartialImageMeta[numRows * numCols];
            float cellWidth = sourceImage.Width / (float)numCols;
            float cellHeight = sourceImage.Height / (float)numRows;
            
            
            for (int y = 0; y < numRows; y++)
            {
                for (int x = 0; x < numCols; x++)
                {
                    cells[(y * numCols) + x] = new PartialImageMeta(sourceImage, sourceImagePath, new Point((int)(x * cellWidth),(int)(y * cellHeight)), new Size((int)cellWidth, (int)cellHeight));
                }
            }

            return cells;
        }

        PartialImageMeta(Bitmap sourceImage, string path, Point start, Size size) : base(sourceImage.Clone(new Rectangle(start, size), sourceImage.PixelFormat))
        {
            this._start = start;
        }
    }
}
