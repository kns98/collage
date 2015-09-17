﻿using System;
using System.Drawing;

namespace CollageMaker
{
    class PartialImageMeta : ImageMeta
    {
        protected Point _start;

        /// <summary>
        /// Divides an image into equally spaced cells based on numRows and numCols.
        /// </summary>
        /// <param name="numRows">The number of rows of PartialImageMeta</param>
        /// <param name="numCols">The number of cols of PartialImageMeta</param>
        /// <returns></returns>
        public static PartialImageMeta[] ArrayFromImage(Bitmap sourceImage, string sourceImagePath, int numRows, int numCols)
        {
            PartialImageMeta[] cells = new PartialImageMeta[numRows * numCols];
            int cellWidth = sourceImage.Width / numCols;
            int cellHeight = sourceImage.Height / numRows;
            
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    cells[(i * numCols) + j] = new PartialImageMeta(sourceImage, sourceImagePath, new Point(j * cellWidth, i * cellHeight), new Size(cellWidth, cellHeight));
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
