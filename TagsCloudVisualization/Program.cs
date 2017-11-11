using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace TagsCloudVisualization
{
    class MyForm : Form
    {
        private CircularCloudLayouter layouter = new CircularCloudLayouter(new Point(350, 350));

        public MyForm()
        {
            Size = new Size(700, 700);
            Shown += (sender, args) =>
            {
                PutRectangles();
                SaveAsImage();
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
            while (true)
            {
                layouter = new CircularCloudLayouter(new Point(350, 350));
                for (var i = 0; i < 500; i++)
                {
                    var a = new Random();
                    layouter.PutNextRectangle(new Size(
                        a.Next(5, 50),
                        a.Next(5, 50)
                    ));
                    Refresh();
                    Thread.Sleep(100);
                }
            }

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.Clear(Color.White);
            
            foreach (var rect in layouter.Rectangles)
                graphics.DrawRectangle(Pens.Blue, rect);

            graphics.DrawString(GetPercentage().ToString(), DefaultFont, Brushes.Blue, new Point(0,0));
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
