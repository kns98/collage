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

            IEnumerable<FileInfo> fileList1 = info.GetFiles("*.bmp",
                SearchOption.AllDirectories);
            IEnumerable<FileInfo> fileList2 = info.GetFiles("*.gif",
                SearchOption.AllDirectories);
            IEnumerable<FileInfo> fileList3 = info.GetFiles("*.exif",
                SearchOption.AllDirectories);
            IEnumerable<FileInfo> fileList4 = info.GetFiles("*.jpg",
                SearchOption.AllDirectories);
            IEnumerable<FileInfo> fileList5 = info.GetFiles("*.jpeg",
               SearchOption.AllDirectories);
            IEnumerable<FileInfo> fileList6 = info.GetFiles("*.png",
                SearchOption.AllDirectories);
            IEnumerable<FileInfo> fileList7 = info.GetFiles("*.tiff",
                SearchOption.AllDirectories);

            IEnumerable<FileInfo> _fileQuery1 =
                from file in fileList1
                where file.Length > 0
                orderby file.Name
                select file;

            var fileQuery1 = _fileQuery1.ToArray();

            IEnumerable<FileInfo> _fileQuery2 =
                from file in fileList2
                where file.Length > 0
                orderby file.Name
                select file;

            var fileQuery2 = _fileQuery2.ToArray();

            IEnumerable<FileInfo> _fileQuery3 =
                from file in fileList3
                where file.Length > 0
                orderby file.Name
                select file;

            var fileQuery3 = _fileQuery3.ToArray();

            IEnumerable<FileInfo> _fileQuery4 =
                from file in fileList4
                where file.Length > 0
                orderby file.Name
                select file;

            var fileQuery4 = _fileQuery4.ToArray();

            IEnumerable<FileInfo> _fileQuery5 =
                from file in fileList5
                where file.Length > 0
                orderby file.Name
                select file;

            var fileQuery5 = _fileQuery5.ToArray();

            IEnumerable<FileInfo> _fileQuery6 =
                from file in fileList6
                where file.Length > 0
                orderby file.Name
                select file;

            var fileQuery6 = _fileQuery6.ToArray();

            IEnumerable<FileInfo> _fileQuery7 =
                from file in fileList7
                where file.Length > 0
                orderby file.Name
                select file;

            var fileQuery7 = _fileQuery7.ToArray();

            var len1 = Math.Min(20, fileQuery1.Length / 10);
            var len2 = Math.Min(20, fileQuery2.Length / 10);
            var len3 = Math.Min(20, fileQuery3.Length / 10);
            var len4 = Math.Min(20, fileQuery4.Length / 10);
            var len5 = Math.Min(20, fileQuery5.Length / 10);
            var len6 = Math.Min(20, fileQuery6.Length / 10);
            var len7 = Math.Min(20, fileQuery7.Length / 10);

            var bag1 = Add(new FileInfo[len1], new Random(), fileQuery1);
            var bag2 = Add(new FileInfo[len2], new Random(), fileQuery2);
            var bag3 = Add(new FileInfo[len3], new Random(), fileQuery3);
            var bag4 = Add(new FileInfo[len4], new Random(), fileQuery4);
            var bag5 = Add(new FileInfo[len5], new Random(), fileQuery5);
            var bag6 = Add(new FileInfo[len6], new Random(), fileQuery6);
            var bag7 = Add(new FileInfo[len7], new Random(), fileQuery7);

            return
                new ConcurrentBag<FileInfo>(bag1.Union(bag2).Union(bag3).
                Union(bag4).Union(bag5).Union(bag6).Union(bag7)

                );
        }

    static void Main(string[] args)
        {
            var files = GetFiles().ToArray();
            var filenames = from f in files select f.FullName;
            var filenames_arr = filenames.ToArray();

            var rndm = new Random().Next(0, filenames_arr.Length);

            Collage collage = new Collage(filenames_arr[rndm] , filenames_arr, new Size(10000, 10000));
            Collage.ResizeType resizeType = Collage.ResizeType.Fit;
            ColorUtil.ColorDistanceType colorDistanceType = ColorUtil.ColorDistanceType.DeltaE;
            collage.SortCells(colorDistanceType);
            Bitmap collageBitmap = collage.ToImage(resizeType);
            collageBitmap.Save(@"d:\output.png", ImageFormat.Png);


            Console.ReadKey();
        }
    }
}
