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

        private static ConcurrentBag<FileInfo> GetFiles()
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
            var len1 = Math.Min(12, fileQuery1.Length / 10);
            var bag1 = Add(new FileInfo[len1], new Random(), fileQuery1);

            return bag1;
                

                
        }

    static void Main(string[] args)
        {
            for (int i = 0; i < 100; i++)
            {
                var files = GetFiles().ToArray();
                var filenames = from f in files select f.FullName;
                var filenames_arr = filenames.ToArray();

                var rndm = new Random().Next(0, filenames_arr.Length);

                Collage collage = new Collage(filenames_arr[rndm], filenames_arr, new Size(4000, 4000));
                Collage.ResizeType resizeType = Collage.ResizeType.Fit;
                ColorUtil.ColorDistanceType colorDistanceType = ColorUtil.ColorDistanceType.DeltaE;
                collage.SortCells(colorDistanceType);
                Bitmap collageBitmap = collage.ToImage(resizeType);

                collageBitmap.Save(@"d:\collage\output" + i + ".png", ImageFormat.Png);
            }

            Console.WriteLine("100 samples created");
            Console.ReadKey();
        }
    }
}
