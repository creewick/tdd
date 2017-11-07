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
        public List<Rectangle> Rectangles { get; private set; }
        public long TotalArea { get; private set; }
        public double SpiralArgument { get; private set; }
        private bool rearranging;

        public CircularCloudLayouter(Point center)
            {
                Center = center;
                Rectangles = new List<Rectangle>();
            }

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
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
            TotalArea += rect.Width * rect.Height;

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
            if (ListEquals(sorted, Rectangles)) return;
            
            rearranging = true;

            Rectangles = new List<Rectangle>();
            TotalArea = 0;
            SpiralArgument = 0;

            foreach (var rectangle in sorted)
                PutNextRectangle(rectangle.Size);

            rearranging = false;
        }

        private static bool ListEquals<T>(List<T> list1, List<T> list2)
        {
            if (list1.Count != list2.Count)
                return false;
            return !list1.Where((t, i) => !Equals(t, list2[i])).Any();
        }

        private bool LookingLikeCircle()
        {
            var circleArea = new CircleFinder(this).GetCircleArea(Rectangles[Rectangles.Count - 1]);
            return TotalArea / circleArea > 0.75;
        }

        private bool IsFreeRectangle(Rectangle rect)
        {
            return Rectangles.All(rectangle => !rectangle.IntersectsWith(rect));
        }
    }
}
