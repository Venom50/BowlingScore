using System.Collections.Generic;

namespace BowlingScore.Calculator
{
    public class BowlingScoreCalculator
    {
        private const int LAST_FRAME_INDEX = 18;

        public int CalculateScore(List<int> scores)
        {
            var totalScore = 0;

            for (int i = 0; i < LAST_FRAME_INDEX; i+=2)
            {
                var frameScore = scores[i] + scores[i + 1];

                if (scores[i] == 10)
                {
                    int additionalScoreAfterStrike;

                    if (scores[i + 2] == 10 && i + 2 != LAST_FRAME_INDEX)
                    {
                        additionalScoreAfterStrike = scores[i + 4];
                    } 
                    else
                    {
                        additionalScoreAfterStrike = scores[i + 3];
                    }

                    totalScore += frameScore + scores[i + 2] + additionalScoreAfterStrike;
                    continue;
                }
                else if (frameScore == 10 && scores[i] != 10)
                {
                    totalScore += frameScore + scores[i + 2];
                    continue;
                }

                totalScore += frameScore;
            }

            foreach (var score in scores.GetRange(LAST_FRAME_INDEX, scores.Count - LAST_FRAME_INDEX))
            {
                totalScore += score;
            }

            return totalScore;
        }
    }
}
