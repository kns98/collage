using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CollageMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            // args[0] is the base image.
            // args[1] is the directory containing cell images.
            // args[2] is the output file path.
            // args[3] is the resolution in pixels (width)
            // args[4] is the resolution in pixels (height)
            // args[5] is the resizeType (fit or stretch)
            // args[6] is the colorDistanceType (euclid or deltae)
            if (args.Length != 7)
            {
                Console.WriteLine("Usage: CollageMaker.exe baseImage cellDirectory outFilePath width height fit|stretch");
                Console.WriteLine();
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
                return;
            }

            Collage collage = new Collage(args[0], Directory.GetFiles(args[1]), new Size(int.Parse(args[3]), int.Parse(args[4])));
            Collage.ResizeType resizeType = args[5].Equals("stretch", StringComparison.CurrentCultureIgnoreCase) ? Collage.ResizeType.Stretch : Collage.ResizeType.Fit;
            ColorUtil.ColorDistanceType colorDistanceType = args[6].Equals("deltae", StringComparison.CurrentCultureIgnoreCase) ? ColorUtil.ColorDistanceType.DeltaE : ColorUtil.ColorDistanceType.Euclidean;
            collage.SortCells(colorDistanceType);
            Bitmap collageBitmap = collage.ToImage(resizeType);
            Console.WriteLine("Writing image to {0}.", args[2]);
            collageBitmap.Save(Path.Combine(Directory.GetCurrentDirectory(), args[2]), ImageFormat.Png);

            // Collect garbage and flush the input buffer (so it doesn't immediately exit if a key was pressed earlier).
            GC.Collect();
            while (Console.KeyAvailable)
                Console.ReadKey(true);

            Console.WriteLine("Done!  Press any key to continue.", args[2]);
            Console.ReadKey(true);
        }
    }
}
