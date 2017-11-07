using System;
using System.Drawing;
using FluentAssertions;
using NUnit.Framework;


namespace TagsCloudVisualization
{
    public class CircularCloudLayouter_Should
    {
        private CircularCloudLayouter layouter;
        private readonly Size squareSize = new Size(100, 100);
        private readonly Size rectangleSize = new Size(1000, 100);
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
            layouter.TotalArea.Should().Be(0);
            layouter.Rectangles.Count.Should().Be(0);
        }

        [Test]
        public void PutNextRectangle_FirstOnCenter()
        {
            var rect = layouter.PutNextRectangle(squareSize);

            rect.Center().ShouldBeEquivalentTo(center);
        }

        [Test]
        public void PutNextRectangle_TotalAreaInc()
        {
            var rect = layouter.PutNextRectangle(squareSize);

            layouter.TotalArea.Should().Be(rect.Height * rect.Width);
        }

        [Test]
        public void PutNextRectangle_AddToList()
        {
            layouter.PutNextRectangle(squareSize);

            layouter.Rectangles.Count.Should().Be(1);
        }

        [Test]
        public void PutNextRectangle_SecondRectangle_NoIntersect()
        {
            var firstRect = layouter.PutNextRectangle(squareSize);
            var secondRect = layouter.PutNextRectangle(squareSize);

            firstRect.IntersectsWith(secondRect).Should().BeFalse();
        }

        [Test, Timeout(1000)]
        public void PutNextRectangle_BigAmount_FasterThanSecond()
        {
            for (var i = 0; i < 1000; i++)
                layouter.PutNextRectangle(squareSize);
        }

        [Test]
        public void PutNextRectangle_BigAmount_NoIntersection()
        {
            for (var i = 1; i < 100; i++)
            {
                var newRect = layouter.PutNextRectangle(squareSize);

                foreach (var oldRect in layouter.Rectangles)
                    if (oldRect != newRect)
                        oldRect.IntersectsWith(newRect).Should().BeFalse();
            }
        }

        [Test]
        public void PutNextRectangle_LotsOfSquares_LooksLikeCircle()
        {
            var count = 1000;

            var rect = layouter.PutNextRectangle(squareSize);
            for (var i = 1; i < count; i++)
                rect = layouter.PutNextRectangle(squareSize);

            var expectedArea = new CircleFinder(layouter).GetCircleArea(rect);
            var actualArea = squareSize.Height * squareSize.Width * count;

            (actualArea / expectedArea).Should().BeGreaterThan(0.75);
        }

        [Test]
        public void PutNextRectangle_LotsOfWideRectangles_LooksLikeCircle()
        {
            var count = 1000;

            var rect = layouter.PutNextRectangle(rectangleSize);
            for (var i = 1; i < count; i++)
                rect = layouter.PutNextRectangle(rectangleSize);

            var expectedArea = new CircleFinder(layouter).GetCircleArea(rect);
            var actualArea = rectangleSize.Height * rectangleSize.Width * count;

            (actualArea / expectedArea).Should().BeGreaterThan(0.75);
        }

        [Test]
        public void PutNextRectangle_LotsOfSquaresAndRectangles_LooksLikeCircle()
        {
            var count = 1000;

            var rect = layouter.PutNextRectangle(squareSize);
            var actualArea = rect.Width * rect.Height;
            for (var i = 1; i < count; i++)
            {
                rect = layouter.PutNextRectangle(i % 2 == 0
                    ? squareSize
                    : rectangleSize
                );
                actualArea += rect.Width * rect.Height;
            }
            var expectedArea = new CircleFinder(layouter).GetCircleArea(rect);

            (actualArea / expectedArea).Should().BeGreaterThan(0.75);
        }

        [Test]
        public void PutNextRectangle_LooksNotLikeCircle_Rearrange()
        {
            var firstRect = layouter.PutNextRectangle(squareSize);
            for (var i = 1; i < 1000; i++)
                layouter.PutNextRectangle(squareSize);
            layouter.PutNextRectangle(rectangleSize);

            layouter.Rectangles[0].Should().NotBe(firstRect);
        }
    }
}