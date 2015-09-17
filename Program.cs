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
        static void Main(string[] args)
        {
            // args[0] is the base image.
            // args[1] is the directory containing cell images.
            // args[2] is the resolution in pixels (width)
            // args[3] is the resolution in pixels (height)
            if (args.Length != 4)
            {
                Console.WriteLine("Usage: CollageMaker.exe baseImage cellDirectory width height");
                Console.WriteLine();
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
                return;
            }

            Collage collage = new Collage(args[0], Directory.GetFiles(args[1]), new Size(int.Parse(args[2]), int.Parse(args[3])));
            collage.ToImage().Save(Path.Combine(Directory.GetCurrentDirectory(), "out.png"), ImageFormat.Png);

            Console.WriteLine("Output written to out.jpg.  Press any key to continue.");
            Console.ReadKey();
        }
    }
}
