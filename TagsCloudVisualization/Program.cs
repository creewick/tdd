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
        private readonly List<Rectangle> rectangles = new List<Rectangle>();

        public MyForm()
        {
            Size = new Size(700, 700);
            PutRectangles();
            SaveAsImage();
            Console.WriteLine(GetPercentage());
        }

        private void SaveAsImage()
        {
            var image = new Bitmap(700, 700);
            var graphics = Graphics.FromImage(image);
            foreach (var rect in rectangles)
                graphics.DrawRectangle(Pens.Blue, rect);
            image.Save(Path.Combine(Directory.GetCurrentDirectory(), "1.bmp"));
        }

        private double GetPercentage()
        {
            var actualArea = 0;
            foreach (var rect in rectangles)
                actualArea += rect.Width * rect.Height;
            var expectedArea = new CircleFinder(layouter).GetCircleArea(rectangles[rectangles.Count - 1]);

            return actualArea / expectedArea;
        }

        private void PutRectangles()
        {
            for (var i = 0; i < 200; i++)
            {
                var a = new Random();
                var k = a.Next(1, 5);
                rectangles.Add(layouter.PutNextRectangle(new Size(
                    k * a.Next(10, 50),
                    k * a.Next(5, 10)
                )));
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.Clear(Color.White);

            
            foreach (var rect in rectangles)
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
