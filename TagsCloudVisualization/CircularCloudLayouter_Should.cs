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
        public void PutNextRectangle_ThousandSquares_FasterThanSecond()
        {
            for (var i = 0; i < 100; i++)
                layouter.PutNextRectangle(squareSize);
        }

        [Test, Timeout(1000)]
        public void PutNextRectangle_ThousandRectangles_FasterThanSecond()
        {
            for (var i = 0; i < 100; i++)
                layouter.PutNextRectangle(rectangleSize);
        }

        [Test, Timeout(1000)]
        public void PutNextRectangle_ThousandSquaresAndRectangles_FasterThanSecond()
        {
            for (var i = 0; i < 100; i++)
                layouter.PutNextRectangle(i % 2 == 0 ? squareSize : rectangleSize);
        }

        [Test]
        public void PutNextRectangle_BigAmount_NoIntersection()
        {
            for (var i = 1; i < 100; i++)
            {
                var newRect = layouter.PutNextRectangle(squareSize);
                foreach (var rect in layouter.Rectangles)
                    foreach (var rect2 in layouter.Rectangles)
                        if (rect != rect2)
                            rect.IntersectsWith(rect2).Should().BeFalse();
            }
        }

        [Test]
        public void PutNextRectangle_Squares_LooksLikeCircle()
        {
            var count = 100;

            var rect = layouter.PutNextRectangle(squareSize);
            for (var i = 1; i < count; i++)
                rect = layouter.PutNextRectangle(squareSize);

            var expectedArea = new CircleFinder(layouter).GetCircleArea(rect);
            var actualArea = squareSize.Height * squareSize.Width * count;

            (actualArea / expectedArea).Should().BeGreaterThan(0.6);
        }

        [Test]
        public void PutNextRectangle_Rectangles_LooksLikeCircle()
        {
            var count = 100;

            var rect = layouter.PutNextRectangle(rectangleSize);
            for (var i = 1; i < count; i++)
                rect = layouter.PutNextRectangle(rectangleSize);

            var expectedArea = new CircleFinder(layouter).GetCircleArea(rect);
            var actualArea = rectangleSize.Height * rectangleSize.Width * count;

            (actualArea / expectedArea).Should().BeGreaterThan(0.6);
        }

        [Test]
        public void PutNextRectangle_SquaresAndRectangles_LooksLikeCircle()
        {
            var count = 100;

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

            (actualArea / expectedArea).Should().BeGreaterThan(0.6);
        }

        [Test]
        public void PutNextRectangle_LooksNotLikeCircle_Rearrange()
        {
            var firstRect = layouter.PutNextRectangle(squareSize);
            for (var i = 1; i < 500; i++)
                layouter.PutNextRectangle(squareSize);
            for (var i = 1; i < 500; i++)
                layouter.PutNextRectangle(rectangleSize);
            
            layouter.Rectangles[0].Should().NotBe(firstRect);
        }
    }
}