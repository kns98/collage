using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace CollageMaker
{
    class Collage
    {
        private ImageMeta _baseImage;
        private PartialImageMeta[] _baseImageCells;
        private ImageMeta[] _cellImages;
        private Size _sizePixels;
        private Size _sizeImages;

        private void SortCells()
        {
            throw new NotImplementedException();
        }

        public Bitmap ToImage()
        {
            int cellWidth = this._sizePixels.Width / this._sizeImages.Width;
            int cellHeight = this._sizePixels.Height / this._sizeImages.Height;

            // Create a canvas to work on.
            Bitmap finalBitmap = new Bitmap(this._sizePixels.Width, this._sizePixels.Height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(finalBitmap);
            graphics.Clear(Color.Transparent);
            graphics.CompositingMode = CompositingMode.SourceCopy;

            for (int y = 0; y < this._sizeImages.Height; y++)
            {
                for (int x = 0; x < this._sizeImages.Width; x++)
                {
                    ImageMeta cellImage = this._cellImages[(y * this._sizeImages.Width) + x];

                    // Open and resize the bitmap of the cellImage.
                    int scalar = Math.Min(cellWidth / cellImage.Size.Width, cellHeight / cellImage.Size.Height);
                    Bitmap cellBitmap = new Bitmap(cellImage.Path);
                    cellBitmap = new Bitmap(cellBitmap, new Size(cellImage.Size.Width * scalar, cellImage.Size.Height * scalar));

                    // Find the offset needed (X or Y) to center the image on the square.
                    int offsetX = (cellBitmap.Width < cellWidth ? (cellWidth - cellBitmap.Width) / 2 : 0);
                    int offsetY = (cellBitmap.Height < cellHeight ? (cellHeight - cellBitmap.Height) / 2 : 0);

                    // Draw the cell onto the canvas.
                    graphics.DrawImage(cellBitmap, new Point((x * cellWidth) + offsetX, (y * cellHeight) + offsetY));
                }
            }

            return finalBitmap;
        }

        /// <summary>
        /// Creates a new collage given a baseImage (to mimic) and a collection of cell images to collage.
        /// </summary>
        /// <param name="baseImagePath">Path to the file containing the image to model.</param>
        /// <param name="cellImagePaths">Paths to the files containing the images to model with.</param>
        /// <param name="size">The final output size (in pixels) of the collage.</param>
        public Collage(string baseImagePath, string[] cellImagePaths, Size size)
        {
            this._sizePixels = size;
            Bitmap baseBitmap = new Bitmap(baseImagePath);
            this._baseImage = new ImageMeta(baseBitmap, baseImagePath);
            this._cellImages = new PartialImageMeta[cellImagePaths.Length];

            // Get ImageMetas from the cell images.
            for (int i = 0; i < cellImagePaths.Length; i++)
            {
                this._cellImages[i] = new ImageMeta(new Bitmap(cellImagePaths[i]), cellImagePaths[i]);
            }

            // Fit our images into the baseImage's aspect ratio.
            int numCols = (int)((baseBitmap.Width / (float)baseBitmap.Height) * Math.Sqrt(cellImagePaths.Length));
            int numRows = (int)((baseBitmap.Height / (float)baseBitmap.Width) * Math.Sqrt(cellImagePaths.Length));
            this._sizeImages = new Size(numCols, numRows);
            // Correct rounding
            while (numRows * numCols < cellImagePaths.Length)
                numRows++;

            // Split up the base image into PartialImageMeta cells.
            this._baseImageCells = PartialImageMeta.ArrayFromImage(baseBitmap, baseImagePath, numRows, numCols);
        }
    }
}
