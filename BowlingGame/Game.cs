using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace BowlingGame
{
	public class Game
	{
	    private int scores;

	    private int pinsLeft = 10;
	    private int triesUsed;
	    private int frameNo = 1;
	    private int extra;

	    private int bonusesTTL1;
	    private int bonusesTTL2;

        public Game Roll(int pins)
		{
            if (pins < 0 || pins > pinsLeft)
                throw new ArgumentException();
		    if (frameNo > 10)
		        throw new IllegalStateException();
		    triesUsed++;
		    pinsLeft -= pins;
		    AddScores(pins);
            if (frameNo != 10)
    		    CheckBonuses(pins);
		    if (frameNo == 10 && pinsLeft == 0)
		    {
		        pinsLeft = 10;
		        extra = 1;
		    }
		    if (pinsLeft == 0 || triesUsed == 2 + extra)
		        ResetFrame();
		    return this;
		}

	    private void AddScores(int pins)
	    {
	        scores += pins * (1 + (
	            frameNo <= 10
	                ? bonusesTTL1 + bonusesTTL2
	                : 0));
	        bonusesTTL1 = bonusesTTL2;
	        bonusesTTL2 = 0;
        }

	    private void ResetFrame()
	    {
	        pinsLeft = 10;
	        triesUsed = 0;
	        frameNo++;
	    }

	    private void CheckBonuses(int pins)
	    {
	        if (pins == 10)
	            bonusesTTL2++;
	        else if (pinsLeft == 0)
	            bonusesTTL1++;
        }


	    public int GetScore()
		{
		    return scores;
		}
	}

    public class IllegalStateException : Exception
    {
    }


    [TestFixture]
	public class Game_should : ReportingTest<Game_should>
	{
		// ReSharper disable once UnusedMember.Global
		public static string Names = "5 Исламов Юхатский"; // Ivanov Petrov

	    private Game game;

	    [SetUp]
	    public void SetUp()
	    {
	        game = new Game();
	    }


		[Test]
		public void HaveZeroScore_BeforeAnyRolls()
		{
			new Game()
				.GetScore()
				.Should().Be(0);
		}

	    [Test]
	    public void RollOnce_ScoreHowManyBit()
	    {
	        game.Roll(4).GetScore().Should().Be(4);
	    }

	    [Test]
	    public void RollFrame_Scores()
	    {
	        game.Roll(4).Roll(5).GetScore().Should().Be(9);
	    }

	    [Test]
	    public void RollSpare_ThirdRoll_CorrectScores()
	    {
	        game.Roll(4).Roll(6).Roll(5).GetScore().Should().Be(20);
	    }

	    [Test]
	    public void RollSpare_FourthRoll_CorrectScores()
	    {
	        game.Roll(5).Roll(5).Roll(5).Roll(5).GetScore().Should().Be(25);
	    }

	    [Test]
	    public void RollNegative()
	    {
	        Action action = () => game.Roll(-1);
	        action.ShouldThrow<ArgumentException>();
	    }

	    [Test]
	    public void RollTwiceSumGreaterThanTen_ThrowsException()
	    {
	        Action act = () => game.Roll(9).Roll(2);
	        act.ShouldThrow<ArgumentException>();
	    }

	    [Test]
	    public void Strike_ResetPins()
	    {
	        Action action = () => game.Roll(10).Roll(1);
            action.ShouldNotThrow();
	    }

	    [Test]
	    public void TwoStrikesInRow_ThirdRollTripleCounted()
	    {
	        game.Roll(10).Roll(10).Roll(5).GetScore().Should().Be(45);
	    }

	    [Test]
	    public void MaximumScores()
	    {
	        for (int i = 0; i < 12; i++)
	            game.Roll(10);
	        game.GetScore().Should().Be(300);
	    }

	    [Test]
	    public void GameOverAfterTenFrames()
	    {
	        for (int i = 0; i < 12; i++)
	            game.Roll(10);
	        Action act = () => game.Roll(1);
	        act.ShouldThrow<IllegalStateException>();
	    }

        [Test]
        public void TwoExtraThrowsAfterStrikeInFrameTen()
        {
            for (int i = 0; i < 10; i++)
                game.Roll(10);
            Action act = () => game.Roll(1).Roll(1);
            act.ShouldNotThrow();
        }
    }
}
