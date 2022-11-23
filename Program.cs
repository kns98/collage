using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace CollageMaker
{
    class Program
    {
        private static ConcurrentBag<FileInfo> Add(FileInfo[] sel, Random random, FileInfo[] query)
        {
            var bag = new ConcurrentBag<FileInfo>();

            for (var i = 0; i < sel.Length; i++)
            {
                var indexToGetImageFrom = random.Next(query.Length);
                try
                {
                    bag.Add(query[indexToGetImageFrom]);
                    
                }
                catch (Exception ex)
                {
                }
            }

            return bag;
        }

        private static ConcurrentBag<FileInfo> GetFiles(int count)
        {
            var info = new DirectoryInfo(@"d:\onedrive");

            //BMP, GIF, EXIF, JPG, PNG, and TIFF

            
            IEnumerable<FileInfo> fileList1 = info.GetFiles("*.jpg",
                SearchOption.AllDirectories);


            IEnumerable<FileInfo> _fileQuery1 =
                from file in fileList1
                where file.Length > 0
                orderby file.Name
                select file;

            var fileQuery1 = _fileQuery1.ToArray();
            var len1 = Math.Min(count, fileQuery1.Length);
            var bag1 = Add(new FileInfo[len1], new Random(), fileQuery1);

            return bag1;
                

                
        }

    static void Main(string[] args)
        {
            int seed =  int.Parse( args[0]);
            int count = int.Parse(args[1]); 
            int samples = int.Parse(args[2]);
            int i = 0;

            for (i=0; i < samples; i++)
            {
                var files = GetFiles(count).ToArray();
                var filenames = from f in files select f.FullName;
                var filenames_arr = filenames.ToArray();

                var rndm = new Random(seed).Next(0, filenames_arr.Length);

                Collage collage = new Collage(filenames_arr[rndm], filenames_arr, new Size(20000, 20000));
                Collage.ResizeType resizeType = Collage.ResizeType.Fit;
                ColorUtil.ColorDistanceType colorDistanceType = ColorUtil.ColorDistanceType.DeltaE;
                collage.SortCells(colorDistanceType);
                Bitmap collageBitmap = collage.ToImage(resizeType);

                collageBitmap.Save(@"d:\collage\output-"+ i + ".png", ImageFormat.Png);
            }

            Console.WriteLine(i + " samples created");
            Console.ReadKey();
        }
    }
}
