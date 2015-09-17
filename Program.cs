using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollageMaker
{
    class Program
    {
        const string OUTPATH = "out.png";

        static void Main(string[] args)
        {
            // args[0] is the base image.
            // args[1] is the directory containing cell images.
            // args[2] is the resolution in pixels (width)
            // args[3] is the resolution in pixels (height)
            // args[4] is the resizeType (fit or stretch)
            if (args.Length != 5)
            {
                Console.WriteLine("Usage: CollageMaker.exe baseImage cellDirectory width height");
                Console.WriteLine();
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
                return;
            }

            Collage collage = new Collage(args[0], Directory.GetFiles(args[1]), new Size(int.Parse(args[2]), int.Parse(args[3])));
            Collage.ResizeType resizeType = args[4].Equals("stretch", StringComparison.CurrentCultureIgnoreCase) ? Collage.ResizeType.Stretch : Collage.ResizeType.Fit;
            collage.ToImage(resizeType).Save(Path.Combine(Directory.GetCurrentDirectory(), OUTPATH), ImageFormat.Png);

            GC.Collect();

            Console.WriteLine("Output written to {0}.  Press any key to continue.", OUTPATH);
            Console.ReadKey();
        }
    }
}
