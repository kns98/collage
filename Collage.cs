using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CollageMaker
{
    class Collage
    {
        private ImageMeta _baseImage;
        private PartialImageMeta[] _baseImageCells;
        private ImageMeta[] _cellImages;

        private void SortCells()
        {

        }

        public Collage(string baseImagePath, string[] cellImagePaths)
        {
            Bitmap baseBitmap = new Bitmap(baseImagePath);
            this._baseImage = new ImageMeta(baseBitmap, baseImagePath);
            this._cellImages = new PartialImageMeta[cellImagePaths.Length];
            for (int i = 0; i < cellImagePaths.Length; i++)
            {
                this._cellImages[i] = new ImageMeta(new Bitmap(cellImagePaths[i]), cellImagePaths[i]);
            }
            int numCols = (int)((baseBitmap.Width / (float)baseBitmap.Height) * Math.Sqrt(cellImagePaths.Length));
            int numRows = (int)((baseBitmap.Height / (float)baseBitmap.Width) * Math.Sqrt(cellImagePaths.Length));

            // Correct rounding
            while (numRows * numCols < cellImagePaths.Length)
                numRows++;

            this._baseImageCells = PartialImageMeta.ArrayFromImage(baseBitmap, baseImagePath, numRows, numCols);
        }
    }
}
