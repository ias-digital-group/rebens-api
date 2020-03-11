using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;

namespace ias.Rebens.Helper
{
    public static class ScratchcardHelper
    {
        public static void GenerateNoPrize(List<string> images, string path, string fileName)
        {
            var tmp = images.OrderBy(x => Guid.NewGuid()).Take(9).ToArray();
            GenerateImage(tmp, path, fileName);
        }

        private static void GenerateImage(string[] imgList, string path, string fileName)
        {
            int spaceSize = 6;
            int imgSize = 192;
            using (Bitmap bmp = new Bitmap(600, 600))
            {
                var img1 = Image.FromFile(Path.Combine(path, imgList[0]));
                var img2 = Image.FromFile(Path.Combine(path, imgList[1]));
                var img3 = Image.FromFile(Path.Combine(path, imgList[2]));
                var img4 = Image.FromFile(Path.Combine(path, imgList[3]));
                var img5 = Image.FromFile(Path.Combine(path, imgList[4]));
                var img6 = Image.FromFile(Path.Combine(path, imgList[5]));
                var img7 = Image.FromFile(Path.Combine(path, imgList[6]));
                var img8 = Image.FromFile(Path.Combine(path, imgList[7]));
                var img9 = Image.FromFile(Path.Combine(path, imgList[8]));
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(img1, spaceSize, spaceSize, imgSize, imgSize);
                    g.DrawImage(img2, spaceSize, (2 * spaceSize + imgSize), imgSize, imgSize);
                    g.DrawImage(img3, (2 * spaceSize + imgSize), spaceSize, imgSize, imgSize);
                    g.DrawImage(img4, (2 * spaceSize + imgSize), (2 * spaceSize + imgSize), imgSize, imgSize);
                    g.DrawImage(img5, (2 * spaceSize + imgSize), (3 * spaceSize + 2 * imgSize), imgSize, imgSize);
                    g.DrawImage(img6, (3 * spaceSize + 2 * imgSize), spaceSize, imgSize, imgSize);
                    g.DrawImage(img7, (3 * spaceSize + 2 * imgSize), (2 * spaceSize + imgSize), imgSize, imgSize);
                    g.DrawImage(img8, (3 * spaceSize + 2 * imgSize), (3 * spaceSize + 2 * imgSize), imgSize, imgSize);
                    g.DrawImage(img9, spaceSize, (3 * spaceSize + 2 * imgSize), imgSize, imgSize);
                }
                bmp.Save(Path.Combine(path, fileName));
            }
        }

        public static void GeneratePrize(string prize, List<string> otherPrizeImages, List<string> noPrizeImages, string path, string fileName)
        {
            var temp = new List<string>() { prize, prize, prize };
            temp.AddRange(otherPrizeImages.OrderBy(x => Guid.NewGuid()).Take(6));
            temp.AddRange(noPrizeImages.Take(9 - temp.Count));

            if(temp.Count < 9)
            {
                for(int i = temp.Count; i < 9; i++)
                {
                    temp.Add(noPrizeImages.First());
                }
            }

            var tmp = temp.OrderBy(x => Guid.NewGuid()).ToArray();
            GenerateImage(tmp, path, fileName);
        }
    }
}
