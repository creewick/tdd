using System;
using System.Drawing;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;


namespace TagsCloudVisualization
{
    public class CircularCloudLayouter_Should
    {
        private CircularCloudLayouter layouter;
        private readonly Size squareSize = new Size(10, 10);
        private readonly Size rectangleSize = new Size(100, 10);
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
        public void FirstRectangle_PutOnCenter()
        {
            var rect = layouter.PutNextRectangle(squareSize);

            rect.Center().ShouldBeEquivalentTo(center);
        }

        [Test]
        public void NegativeSize_ArgumentException()
        {
            Action action = () => layouter.PutNextRectangle(new Size(-10, -10));
            action.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void NewRectangle_AddToList()
        {
            layouter.PutNextRectangle(squareSize);

            layouter.Rectangles.Count.Should().Be(1);
        }

        [Test]
        public void SecondRectangle_NoIntersect()
        {
            var firstRect = layouter.PutNextRectangle(squareSize);
            var secondRect = layouter.PutNextRectangle(squareSize);

            firstRect.IntersectsWith(secondRect).Should().BeFalse();
        }

        [Test, Timeout(1000)]
        public void ThousandSquares_FasterThanSecond()
        {
            for (var i = 0; i < 100; i++)
                layouter.PutNextRectangle(squareSize);
        }

        [Test, Timeout(1000)]
        public void ThousandRectangles_FasterThanSecond()
        {
            for (var i = 0; i < 100; i++)
                layouter.PutNextRectangle(rectangleSize);
        }

        [Test, Timeout(1000)]
        public void ThousandSquaresAndRectangles_FasterThanSecond()
        {
            for (var i = 0; i < 100; i++)
                layouter.PutNextRectangle(i % 2 == 0 ? squareSize : rectangleSize);
        }

        [Test]
        public void BigAmount_NoIntersection()
        {
            for (var i = 1; i < 100; i++)
            {
                layouter.PutNextRectangle(squareSize);
                foreach (var rect in layouter.Rectangles)
                    foreach (var rect2 in layouter.Rectangles)
                        if (rect != rect2)
                            rect.IntersectsWith(rect2).Should().BeFalse();
            }
        }

        [Test]
        public void Squares_LooksLikeCircle()
        {
            const int count = 100;

            var rect = layouter.PutNextRectangle(squareSize);
            for (var i = 1; i < count; i++)
                rect = layouter.PutNextRectangle(squareSize);

            var expectedArea = new CircleFinder(layouter).GetCircleArea(rect);
            var actualArea = squareSize.Height * squareSize.Width * count;

            (actualArea / expectedArea).Should().BeGreaterThan(0.6);
        }

        [Test]
        public void Rectangles_LooksLikeCircle()
        {
            const int count = 100;

            var rect = layouter.PutNextRectangle(rectangleSize);
            for (var i = 1; i < count; i++)
                rect = layouter.PutNextRectangle(rectangleSize);

            var expectedArea = new CircleFinder(layouter).GetCircleArea(rect);
            var actualArea = rectangleSize.Height * rectangleSize.Width * count;

            (actualArea / expectedArea).Should().BeGreaterThan(0.6);
        }

        [Test]
        public void SquaresAndRectangles_LooksLikeCircle()
        {
            const int count = 100;

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
        public void LooksNotLikeCircle_Rearrange()
        {
            var firstRect = layouter.PutNextRectangle(squareSize);
            for (var i = 1; i < 500; i++)
                layouter.PutNextRectangle(squareSize);
            for (var i = 1; i < 500; i++)
                layouter.PutNextRectangle(rectangleSize);
            
            layouter.Rectangles[0].Should().NotBe(firstRect);
        }

        [TearDown]
        public void TearDown()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status != TestStatus.Failed) return;

            var image = new Bitmap(1000, 1000);
            var graphics = Graphics.FromImage(image);
            foreach (var rect in layouter.Rectangles)
                graphics.DrawRectangle(Pens.Blue, rect);
            var path = Path.Combine(Directory.GetCurrentDirectory(),
                $"{TestContext.CurrentContext.Result.FailCount}.bmp");
            image.Save(path);
            Console.WriteLine($"Tag cloud visualization saved to file {path}");
        }

    }
}