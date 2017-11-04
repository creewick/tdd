using System;
using System.Drawing;
using FluentAssertions;
using NUnit.Framework;


namespace TagsCloudVisualization
{
    public class CircularCloudLayouter_Should
    {
        private CircularCloudLayouter layouter;
        private readonly Point center = new Point(500, 500);

        [SetUp]
        public void SetUp()
        {
            layouter = new CircularCloudLayouter(center);
        }

        [Test]
        public void EmptyState()
        {
            layouter.Center.Should().Be(center);
            layouter.SpiralArgument.Should().Be(0);
            layouter.Rectangles.Count.Should().Be(0);
        }

        [Test]
        public void PutNextRectangle_FirstOnCenter()
        {
            var rect = layouter.PutNextRectangle(new Size(100, 200));

            rect.Center().ShouldBeEquivalentTo(center);
        }

        [Test]
        public void PutNextRectangle_AddToList()
        {
            layouter.PutNextRectangle(new Size(100, 200));

            layouter.Rectangles.Count.Should().Be(1);
        }

        [Test]
        public void PutNextRectangle_SecondRectangle_NoIntersect()
        {
            var firstRect = layouter.PutNextRectangle(new Size(100, 100));
            var secondRect = layouter.PutNextRectangle(new Size(100, 100));

            firstRect.IntersectsWith(secondRect).Should().BeFalse();
        }

        [Test, Timeout(1000)]
        public void PutNextRectangle_BigAmount_FasterThanSecond()
        {
            for (var i = 0; i < 1000; i++)
                layouter.PutNextRectangle(new Size(100, 100));
        }

        [Test]
        public void PutNextRectangle_BigAmount_NoIntersection()
        {
            for (var i = 1; i < 100; i++)
            {
                var newRect = layouter.PutNextRectangle(new Size(100, 100));

                foreach (var oldRect in layouter.Rectangles)
                    if (oldRect != newRect)
                        oldRect.IntersectsWith(newRect).Should().BeFalse();
            }
        }

        [Test]
        public void PutNextRectangle_LotsOfSquares_LooksLikeCircle()
        {
            var size = new Size(100, 100);
            var count = 1000;

            var rect = layouter.PutNextRectangle(size);
            for (var i = 1; i < count; i++)
                rect = layouter.PutNextRectangle(size);

            var expectedArea = new CircleFinder(layouter).GetCircleArea(rect);
            var actualArea = size.Height * size.Width * count;

            (actualArea / expectedArea).Should().BeGreaterThan(0.5);
        }

        [Test]
        public void PutNextRectangle_LotsOfWideRectangles_LooksLikeCircle()
        {
            var size = new Size(1000, 100);
            var count = 1000;

            var rect = layouter.PutNextRectangle(size);
            for (var i = 1; i < count; i++)
                rect = layouter.PutNextRectangle(size);

            var expectedArea = new CircleFinder(layouter).GetCircleArea(rect);
            var actualArea = size.Height * size.Width * count;

            (actualArea / expectedArea).Should().BeGreaterThan(0.5);
        }

        [Test]
        public void PutNextRectangle_LotsOfSquaresAndRectangles_LooksLikeCircle()
        {
            var squareSize = new Size(100, 100);
            var rectSize = new Size(1000, 100);
            var count = 1000;

            var rect = layouter.PutNextRectangle(squareSize);
            var actualArea = rect.Width * rect.Height;
            for (var i = 1; i < count; i++)
            {
                rect = layouter.PutNextRectangle(i % 2 == 0
                    ? squareSize
                    : rectSize
                );
                actualArea += rect.Width * rect.Height;
            }
            var expectedArea = new CircleFinder(layouter).GetCircleArea(rect);

            (actualArea / expectedArea).Should().BeGreaterThan(0.5);
        }
    }
}