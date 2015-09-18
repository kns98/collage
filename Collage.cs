using System;
using System.Collections.Generic;
using System.Linq;
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

        public enum ResizeType
        {
            Fit,
            Stretch
        };

        private void Shuffle(ref int[] arr)
        {
            Random gen = new Random();
            for (int i = arr.Length - 1; i > 0; i--)
            {
                int j = gen.Next(0, i);
                var temp = arr[j];
                arr[j] = arr[i];
                arr[i] = temp;
            }
        }

        public void SortCells(ColorUtil.ColorDistanceType distanceType)
        {
            Console.Write("Sorting cells... ");

            List<ImageMeta> cellImages = new List<ImageMeta>(this._cellImages);
            ImageMeta[] sortedCellImages = new ImageMeta[this._cellImages.Length];

            // Shuffle the range (to avoid gradient appearance)
            int[] range = Enumerable.Range(0, this._baseImageCells.Length).ToArray();
            Shuffle(ref range);

            foreach (int i in range)
            {
                float min = float.PositiveInfinity;
                int minIndex = -1;
                for (int j = 0; j < cellImages.Count; j++)
                {
                    float distance = cellImages[j].Distance(this._baseImageCells[i], distanceType);
                    if (distance < min)
                    {
                        min = distance;
                        minIndex = j;
                    }
                }
                sortedCellImages[i] = cellImages[minIndex];
                cellImages.RemoveAt(minIndex);
            }

            this._cellImages = sortedCellImages;

            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Compiles and returns the collage image.
        /// </summary>
        /// <param name="stretch">Whether to stretch or fit </param>
        /// <returns></returns>
        public Bitmap ToImage(ResizeType resizeType)
        {
            float cellWidth = this._sizePixels.Width / (float)this._sizeImages.Width;
            float cellHeight = this._sizePixels.Height / (float)this._sizeImages.Height;
            
            // Create a canvas to work on.
            Bitmap finalBitmap = new Bitmap(this._sizePixels.Width, this._sizePixels.Height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(finalBitmap);
            graphics.Clear(Color.Transparent);
            graphics.CompositingMode = CompositingMode.SourceCopy;

            for (int y = 0; y < this._sizeImages.Height; y++)
            {
                for (int x = 0; x < this._sizeImages.Width; x++)
                {
                    Console.Write("\rCompositing image {0} of {1}.", (y * this._sizeImages.Width) + x + 1, this._sizeImages.Width * this._sizeImages.Height);
                    ImageMeta cellImage = this._cellImages[(y * this._sizeImages.Width) + x];

                    // Open and resize the bitmap of the cellImage.
                    Bitmap cellBitmap = new Bitmap(cellImage.Path);
                    switch (resizeType)
                    {
                        case ResizeType.Fit:
                            float fitScalar = Math.Min(cellWidth / cellImage.Size.Width, cellHeight / cellImage.Size.Height);
                            cellBitmap = new Bitmap(cellBitmap, new Size((int)(cellImage.Size.Width * fitScalar), (int)(cellImage.Size.Height * fitScalar)));
                            break;
                        case ResizeType.Stretch:
                            float stretchScalarX = cellWidth / cellImage.Size.Width;
                            float stretchScalarY = cellHeight / cellImage.Size.Height;
                            cellBitmap = new Bitmap(cellBitmap, new Size((int)(cellImage.Size.Width * stretchScalarX), (int)(cellImage.Size.Height * stretchScalarY)));
                            break;
                    }

                    // Find the offset needed (X or Y) to center the image on the square.
                    float offsetX = (cellBitmap.Width < cellWidth ? (cellWidth - cellBitmap.Width) / 2 : 0);
                    float offsetY = (cellBitmap.Height < cellHeight ? (cellHeight - cellBitmap.Height) / 2 : 0);

                    // Draw the cell onto the canvas.
                    graphics.DrawImage(cellBitmap, new PointF((x * cellWidth) + offsetX, (y * cellHeight) + offsetY));
                }
            }

            Console.WriteLine();
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
            this._cellImages = new ImageMeta[cellImagePaths.Length];
            
            // Get ImageMetas from the cell images.
            for (int i = 0; i < cellImagePaths.Length; i++)
            {
                Console.Write("\rExtracting metadata from Image {0} of {1}", i + 1, cellImagePaths.Length);

                Bitmap cellBitmap;
                try
                {
                    cellBitmap = new Bitmap(cellImagePaths[i]);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("\nError processing image {0} (File is corrupt or does not exist)", cellImagePaths[i]);
                    continue;
                }
                this._cellImages[i] = new ImageMeta(cellBitmap, cellImagePaths[i]);
                cellBitmap.Dispose();
            }

            Console.WriteLine();

            this._cellImages = this._cellImages.Where(path => path != null).ToArray();

            // Fit our images into the baseImage's aspect ratio.
            int numCols = (int)((baseBitmap.Width / (float)baseBitmap.Height) * Math.Sqrt(cellImagePaths.Length));
            int numRows = (int)((baseBitmap.Height / (float)baseBitmap.Width) * Math.Sqrt(cellImagePaths.Length));

            // Correct rounding
            while (numRows * numCols < cellImagePaths.Length)
                numRows++;

            this._sizeImages = new Size(numCols, numRows);

            // Split up the base image into PartialImageMeta cells.
            this._baseImageCells = PartialImageMeta.ArrayFromImage(baseBitmap, baseImagePath, numRows, numCols);

            if (this._cellImages.Length < this._baseImageCells.Length) {
                int prevCellImagesLength = this._cellImages.Length;
                Array.Resize(ref this._cellImages, this._baseImageCells.Length);

                // Add filler cells as necessary.
                for (int i = prevCellImagesLength; i < this._baseImageCells.Length; i++)
                {
                    this._cellImages[i] = this._cellImages[i - prevCellImagesLength];
                }
            }
        }
    }
}
