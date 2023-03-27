using BowlingScore.Calculator;
using BowlingScore.Controller.Interfaces;
using BowlingScore.FileReaders;
using BowlingScore.Generic;
using BowlingScore.Helpers;
using BowlingScore.Models;
using System.Collections.Generic;

namespace BowlingScore.Controller
{
    public class BowlingDataController : IBowlingDataController
    {
        private readonly BowlingScoreCalculator _bowlingScoreCalculator;

        public BowlingDataController(BowlingScoreCalculator bowlingScoreCalculator)
        {
            _bowlingScoreCalculator = bowlingScoreCalculator;
        }

        public Result GetBowlingScoreModelsFromFile(string filePath, IFileReader fileReader)
        {
            if (fileReader is null)
            {
                return new Result()
                {
                    IsSuccess = false,
                    Messages = new List<string> { "No matching file readers for this file's extension." }
                };
            }

            var readFileResult = fileReader.ReadFile(filePath);

            if (!readFileResult.IsSuccess)
            {
                return readFileResult;
            }

            var bowlingData = fileReader.GetBowlingData((string[])readFileResult.ResultObject);

            if (!bowlingData.IsSuccess)
            {
                return bowlingData;
            }

            return bowlingData;
        }

        public List<BowlingScoreModel> AddBowlingScoreModelsToList(List<KeyValuePair<string, List<int>>> nameScoreKvp)
        {
            var bowlingScoreModels = new List<BowlingScoreModel>();

            foreach (var item in nameScoreKvp)
            {
                bowlingScoreModels.Add(new BowlingScoreModel
                {
                    Name = item.Key,
                    ThrowsScores = item.Value,
                    TotalScore = _bowlingScoreCalculator.CalculateScore(item.Value)
                });
            }

            return bowlingScoreModels;
        }
    }
}
