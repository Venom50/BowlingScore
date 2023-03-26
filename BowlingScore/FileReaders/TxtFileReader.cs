using BowlingScore.FileReaders.Interfaces;
using BowlingScore.Generic;
using System.Collections.Generic;
using System.Linq;

namespace BowlingScore
{
    public class TxtFileReader : IFileReader
    {
        private const int LAST_THROW = 20;
        private const int PENULTIMATE_THROW = 19;

        private readonly List<KeyValuePair<string, List<int>>> _nameScoreKvp;
        private readonly IFileWrapper _fileWrapper;

        public TxtFileReader(IFileWrapper fileWrapper)
        {
            _nameScoreKvp = new List<KeyValuePair<string, List<int>>>();
            _fileWrapper = fileWrapper;
        }

        public Result ReadFile(string filePath)
        {
            var result = new Result();

            if (!IsValidFile(filePath))
            {
                result.AddError("File does not exist.");
                return result;
            }

            var lines = _fileWrapper.ReadAllLines(filePath);

            if (!IsValidFileStructure(lines.Length))
            {
                result.AddError("Incorrect file structure.");
                return result;
            }

            result.ResultObject = lines;
            return result;
        }

        public Result GetBowlingData(string[] lines)
        {
            var result = new Result();

            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex += 2)
            {
                var message = string.Empty;

                if (!IsValidName(lines[lineIndex], lineIndex, ref message))
                {
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        result.AddError(message);
                        break;
                    }
                }

                var name = lines[lineIndex];

                string[] scoreStrings = lines[lineIndex + 1].Split(',');

                if (!IsValidScoresLength(scoreStrings.Length))
                {
                    result.AddError($"Incorrect amount of throws in line {lineIndex + 1} in file.");
                    break;
                }

                var scores = new List<int>();

                for (int scoreIndex = 0; scoreIndex < scoreStrings.Length; scoreIndex++)
                {
                    if (!IsValidScore(scoreStrings[scoreIndex], lineIndex + 1, ref message))
                    {
                        result.AddError(message);
                        break;
                    }

                    var score = int.Parse(scoreStrings[scoreIndex]);

                    if (scoreIndex % 2 == 1)
                    {
                        if (!IsValidFrame(score, scores[scoreIndex - 1], lineIndex, scoreIndex, ref message))
                        {
                            result.AddError(message);
                            break;
                        }

                        if (scoreIndex == PENULTIMATE_THROW)
                        {
                            if (!IsValidLastFrame(score, scores[scoreIndex - 1], lineIndex, scoreStrings.Length, ref message))
                            {
                                result.AddError(message);
                                break;
                            }
                        }
                    }

                    scores.Add(score);
                }

                if (!result.IsSuccess)
                {
                    break;
                }

                _nameScoreKvp.Add(new KeyValuePair<string, List<int>>(name, scores));
            }

            result.ResultObject = _nameScoreKvp.Count > 0 ? _nameScoreKvp : null;

            return result;
        }

        private bool IsValidFileStructure(int length)
        {
            return length % 2 == 0;
        }

        private bool IsValidFile(string filePath)
        {
            return _fileWrapper.Exists(filePath);
        }

        private bool IsValidName(string name, int index, ref string message)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                message = $"Name missing in line {index} in file.";
                return false;
            }
            if (!name.Any(c => char.IsLetter(c)))
            {
                message = $"Incorrect name in line {index} in file.";
                return false;
            }

            return true;
        }

        private bool IsValidScore(string scoreString, int index, ref string message)
        {
            if (!int.TryParse(scoreString, out int score))
            {
                message = $"Incorrect score format in line {index} in file.";
                return false;
            }
            if (score < 0 || score > 10)
            {
                message = $"Score out of range in line {index} in file.";
                return false;
            }

            return true;
        }

        private bool IsValidFrame(int score1, int score2, int lineIndex, int scoreIndex, ref string message)
        {
            if (!IsValidScoreAfterStrike(score1, score2, scoreIndex))
            {
                message = $"Incorrect value of score after strike in line {lineIndex + 1}, value number {scoreIndex + 1} in file.";
                return false;
            }

            if (!IsValidScoreInFrame(score1, score2, scoreIndex))
            {
                message = $"Incorrect value of frame in line {lineIndex + 1}, value number {scoreIndex} and {scoreIndex + 1} in file.";
                return false;
            }

            return true;
        }

        private bool IsValidLastFrame(int score1, int score2, int lineIndex, int scoreStringsLength, ref string message)
        {
            if (scoreStringsLength != LAST_THROW + 1)
            {
                if (score1 + score2 == 10 && score2 != 10)
                {
                    message = $"There was a spare in last frame of the game in line {lineIndex + 1} in file. Value from additional throw is required.";
                    return false;
                }
                if (score2 == 10)
                {
                    message = $"There was a strike in first throw of last frame of the game in line {lineIndex + 1} in file. Value from additional throw is required.";
                    return false;
                }
            }
            else
            {
                if (score1 + score2 < 10 || score2 != 10)
                {
                    message = $"There was no strike or spare in last frame of the game in line {lineIndex + 1} in file. Value from additional throw should not be provided.";
                    return false;
                }
            }

            return true;
        }

        private bool IsValidScoreAfterStrike(int score, int strikeScore, int index)
        {
            if (strikeScore == 10 && (index != PENULTIMATE_THROW && index != LAST_THROW))
            {
                if (score != 0)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsValidScoreInFrame(int score1, int score2, int index)
        {
            if (score1 + score2 > 10 && (index != PENULTIMATE_THROW && index != LAST_THROW))
            {
                return false;
            }

            return true;
        }

        private bool IsValidScoresLength(int length)
        {
            return length >= 20 && length <= 21;
        }
    }
}
