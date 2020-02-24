using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;

namespace ias.Rebens.Helper
{
    public static class ScratchcardHelper
    {
        public static void GenerateNoPrize(List<string> images, string path, string name)
        {
            var tmp = images.OrderBy(x => Guid.NewGuid()).Take(9).ToArray();
            using (Bitmap bmp = new Bitmap(600, 600))
            {
                var img1 = Image.FromFile(Path.Combine(path, tmp[0]));
                var img2 = Image.FromFile(Path.Combine(path, tmp[1]));
                var img3 = Image.FromFile(Path.Combine(path, tmp[2]));
                var img4 = Image.FromFile(Path.Combine(path, tmp[3]));
                var img5 = Image.FromFile(Path.Combine(path, tmp[4]));
                var img6 = Image.FromFile(Path.Combine(path, tmp[5]));
                var img7 = Image.FromFile(Path.Combine(path, tmp[6]));
                var img8 = Image.FromFile(Path.Combine(path, tmp[7]));
                var img9 = Image.FromFile(Path.Combine(path, tmp[8]));
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(img1, 0, 0, 200, 200);
                    g.DrawImage(img2, 0, 200, 200, 200);
                    g.DrawImage(img3, 200, 0, 200, 200);
                    g.DrawImage(img4, 200, 200, 200, 200);
                    g.DrawImage(img5, 200, 400, 200, 200);
                    g.DrawImage(img6, 400, 0, 200, 200);
                    g.DrawImage(img7, 400, 200, 200, 200);
                    g.DrawImage(img8, 400, 400, 200, 200);
                    g.DrawImage(img9, 0, 400, 200, 200);
                }
                bmp.Save(Path.Combine(path, $"{name}.png"));
            }
        }

        public static void GeneratePrize(string prize, List<string> images, string path, string name)
        {
            var temp = new List<string>() { prize, prize, prize };
            foreach (var s in images.OrderBy(x => Guid.NewGuid()).Take(6))
                temp.Add(s);

            var tmp = temp.OrderBy(x => Guid.NewGuid()).ToArray();
            using (Bitmap bmp = new Bitmap(600, 600))
            {
                var img1 = Image.FromFile(Path.Combine(path, tmp[0]));
                var img2 = Image.FromFile(Path.Combine(path, tmp[1]));
                var img3 = Image.FromFile(Path.Combine(path, tmp[2]));
                var img4 = Image.FromFile(Path.Combine(path, tmp[3]));
                var img5 = Image.FromFile(Path.Combine(path, tmp[4]));
                var img6 = Image.FromFile(Path.Combine(path, tmp[5]));
                var img7 = Image.FromFile(Path.Combine(path, tmp[6]));
                var img8 = Image.FromFile(Path.Combine(path, tmp[7]));
                var img9 = Image.FromFile(Path.Combine(path, tmp[8]));
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(img1, 0, 0, 200, 200);
                    g.DrawImage(img2, 0, 200, 200, 200);
                    g.DrawImage(img3, 200, 0, 200, 200);
                    g.DrawImage(img4, 200, 200, 200, 200);
                    g.DrawImage(img5, 200, 400, 200, 200);
                    g.DrawImage(img6, 400, 0, 200, 200);
                    g.DrawImage(img7, 400, 200, 200, 200);
                    g.DrawImage(img8, 400, 400, 200, 200);
                    g.DrawImage(img9, 0, 400, 200, 200);
                }
                bmp.Save(Path.Combine(path, $"{name}.png"));
            }
        }
    }
}
