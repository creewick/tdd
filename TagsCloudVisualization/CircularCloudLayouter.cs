using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace TagsCloudVisualization
{
    public class CircularCloudLayouter
    {
        private const double Step = Math.PI / 180;

        public readonly Point Center;
        private bool rearranging;
        public List<Rectangle> Rectangles { get; private set; }
        public List<double> Arguments { get; private set; }
        public long TotalArea => Rectangles.Sum(rect => rect.Width * rect.Height);

        public double SpiralArgument { get; private set; }

        public CircularCloudLayouter(Point center)
            {
                Center = center;
                Rectangles = new List<Rectangle>();
                Arguments = new List<double>();
            }

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            if (rectangleSize.Width <= 0 || rectangleSize.Height <= 0)
                throw new ArgumentException();
            Rectangle rect;
            do
            {
                var center = ArchimedeSpiral(SpiralArgument, 1);
                rect = new Rectangle(
                    center.X - rectangleSize.Width / 2,
                    center.Y - rectangleSize.Height / 2,
                    rectangleSize.Width,
                    rectangleSize.Height
                );
                SpiralArgument += Step;
            } while (!IsFreeRectangle(rect));

            Rectangles.Add(rect);
            Arguments.Add(SpiralArgument);

            if (!rearranging && !LookingLikeCircle())
                Rearrange();
            return rect;
        }

        private Point ArchimedeSpiral(double argument, double parameter)
        {
            return new Point(
                Center.X + (int)Math.Round(parameter * argument * Math.Cos(argument)),
                Center.Y + (int)Math.Round(parameter * argument * Math.Sin(argument))
            );
        }

        private void Rearrange()
        {
            var sorted = Rectangles.OrderByDescending(x => x.Width * x.Height).ToList();
            var index = 0;
            while (index < Rectangles.Count && Rectangles[index].Equals(sorted[index]))
                index++;
            if (index == Rectangles.Count) return;

            rearranging = true;

            var oldRectangles = Rectangles;
            Rectangles = Rectangles.GetRange(0, index);
            Arguments = Arguments.GetRange(0, index);
            SpiralArgument = Arguments.Count > 0 ? Arguments[Arguments.Count - 1] : 0;

            for (var i = index; i < oldRectangles.Count; i++)
                PutNextRectangle(sorted[i].Size);

            rearranging = false;
        }

        private bool LookingLikeCircle()
        {
            var circleArea = new CircleFinder(this).GetCircleArea(Rectangles[Rectangles.Count - 1]);
            return TotalArea / circleArea > 0.6;
        }

        private bool IsFreeRectangle(Rectangle rect)
        {
            return Rectangles.All(rectangle => !rectangle.IntersectsWith(rect));
        }
    }
}
