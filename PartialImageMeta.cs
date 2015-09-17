using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CollageMaker
{
    class PartialImageMeta : ImageMeta
    {
        protected Point _start;

        PartialImageMeta(Bitmap sourceImage, string path, Point start, Size size) : base(sourceImage.Clone(new Rectangle(start, size), sourceImage.PixelFormat))
        {
            this._start = start;
        }
    }
}
