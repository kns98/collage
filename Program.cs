using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        private static ConcurrentBag<FileInfo> GetFiles(int count, Random rand)
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
            string s = args[0];
            int seed = s.GetHashCode();
            var rndm = new Random(seed);
            int samples = rndm.Next(1, 100);

            int i = 0;

            Parallel.For(0, samples, (sample =>
            {
                int count = rndm.Next(1, 100);
                var files = GetFiles(count, rndm).ToArray();
                var filenames = from f in files select f.FullName;
                var filenames_arr = filenames.ToArray();

                var skew = rndm.Next(0, filenames_arr.Length);

                int width = (int)Math.Sqrt(400 * 400 * count);

                Collage collage = new Collage(filenames_arr[skew], filenames_arr, new Size(width, width));
                Collage.ResizeType resizeType = Collage.ResizeType.Fit;
                ColorUtil.ColorDistanceType colorDistanceType = ColorUtil.ColorDistanceType.DeltaE;
                collage.SortCells(colorDistanceType, rndm);
                Bitmap collageBitmap = collage.ToImage(resizeType);

                collageBitmap.Save(@"d:\collage\output-" + sample + ".png", ImageFormat.Png);
            }));

            Console.WriteLine(i + " samples created");
            Console.ReadKey();
        }
    }
}
