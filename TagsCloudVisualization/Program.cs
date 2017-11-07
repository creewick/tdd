using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TagsCloudVisualization
{
    class MyForm : Form
    {
        private readonly CircularCloudLayouter layouter = new CircularCloudLayouter(new Point(350, 350));

        public MyForm()
        {
            Size = new Size(700, 700);
            Shown += (sender, args) =>
            {
                PutRectangles();
                SaveAsImage();
                Console.WriteLine(GetPercentage());
            };
        }
        

        private void SaveAsImage()
        {
            var image = new Bitmap(700, 700);
            var graphics = Graphics.FromImage(image);
            foreach (var rect in layouter.Rectangles)
                graphics.DrawRectangle(Pens.Blue, rect);
            image.Save(Path.Combine(Directory.GetCurrentDirectory(), "1.bmp"));
        }

        private double GetPercentage()
        {
            var actualArea = 0;
            foreach (var rect in layouter.Rectangles)
                actualArea += rect.Width * rect.Height;
            var expectedArea = new CircleFinder(layouter).GetCircleArea(layouter.Rectangles[layouter.Rectangles.Count - 1]);

            return actualArea / expectedArea;
        }

        private void PutRectangles()
        {
            for (var i = 0; i < 1000; i++)
            {
                layouter.PutNextRectangle(new Size(
                    i % 2 == 0 ? 50 : 5,
                    5
                ));
                Refresh();
            }

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.Clear(Color.White);
            
            foreach (var rect in layouter.Rectangles)
                graphics.DrawRectangle(Pens.Blue, rect);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Application.Run(new MyForm());
        }
    }
}
