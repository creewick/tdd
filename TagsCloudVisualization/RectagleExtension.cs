using System.Drawing;

namespace TagsCloudVisualization
{
    public static class RectangleExtension
    {
        public static Point Center(this Rectangle rect)
        {
            return new Point(
                rect.X + rect.Width / 2,
                rect.Y + rect.Height / 2
            );
        }
    }
}
