using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace TagsCloudVisualization
{
    public class CircularCloudLayouter
    {
        public readonly Point Center;
        public double SpiralArgument { get; private set; }
        private const double Step = Math.PI / 180;
        public readonly List<Rectangle> Rectangles;

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
            return rect;
        }

        private Point ArchimedeSpiral(double argument, double parameter)
        {
            return new Point(
                Center.X + (int)Math.Round(parameter * argument * Math.Cos(argument)),
                Center.Y + (int)Math.Round(parameter * argument * Math.Sin(argument))
            );
        }

        private bool IsFreeRectangle(Rectangle rect)
        {
            return Rectangles.All(rectangle => !rectangle.IntersectsWith(rect));
        }
    }
}
