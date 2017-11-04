using System;
using System.Drawing;

namespace TagsCloudVisualization
{
    public class CircleFinder
    {
        private readonly CircularCloudLayouter layouter;

        public CircleFinder(CircularCloudLayouter layouter)
        {
            this.layouter = layouter;
        }

        private double GetSquaredLengthToCenter(int x, int y)
        {
            return Math.Pow(Math.Abs(layouter.Center.X - x), 2) +
                   Math.Pow(Math.Abs(layouter.Center.Y - y), 2);
        }

        private double GetSquaredCircleRadius(Rectangle lastRect)
        {
            return Math.Max(Math.Max(Math.Max(
                        GetSquaredLengthToCenter(lastRect.Left, lastRect.Top),
                        GetSquaredLengthToCenter(lastRect.Left, lastRect.Bottom)),
                    GetSquaredLengthToCenter(lastRect.Right, lastRect.Top)),
                GetSquaredLengthToCenter(lastRect.Right, lastRect.Bottom));
        }

        /// <summary>
        /// Returns the minimal radius of сircumscribed a circle, which contains every rectangle
        /// </summary>
        /// <param name="lastRectangle">The last added rectangle</param>
        /// <returns></returns>
        public double GetCircleArea(Rectangle lastRectangle)
        {
            return GetSquaredCircleRadius(lastRectangle) * Math.PI;
        }
    }
}
