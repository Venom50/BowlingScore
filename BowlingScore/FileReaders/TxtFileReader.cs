using BowlingScore.Generic;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BowlingScore
{
    public class TxtFileReader : IFileReader
    {
        private readonly List<KeyValuePair<string, List<string>>> _nameScoreKvp;

        public TxtFileReader()
        {
            _nameScoreKvp = new List<KeyValuePair<string, List<string>>>();
        }

        public Result ReadFile(string filePath)
        {
            var result = new Result();

            if (!IsValidFile(filePath))
            {
                result.AddError("File does not exist.");
                return result;
            }

            var lines = File.ReadAllLines(filePath);

            if (!IsValidFileStructure(lines.Length))
            {
                result.AddError("Incorrect file structure.");
                return result;
            }

            for (int i = 0; i < lines.Length; i += 2)
            {
                var message = string.Empty;

                if (!IsValidName(lines[i], i, ref message))
                {
                    if (!string.IsNullOrEmpty(message))
                    {
                        result.AddError(message);
                        break;
                    }
                }

                var name = lines[i];

                string[] scoreStrings = lines[i + 1].Split(',');

                if (!IsValidScoresLength(scoreStrings.Length))
                {
                    result.AddError($"Incorrect amount of throws in line {i + 1} in file.");
                    break;
                }

                var scores = new List<string>();

                foreach (var scoreString in scoreStrings)
                {
                    if (!IsValidScore(scoreString, i + 1, ref message))
                    {
                        result.AddError(message);
                        break;
                    }

                    scores.Add(scoreString);
                }

                if (!result.IsSuccess)
                {
                    break;
                }

                _nameScoreKvp.Add(new KeyValuePair<string, List<string>>(name, scores));
            }

            result.ResultObject = _nameScoreKvp;

            return result;
        }

        private bool IsValidFileStructure(int length)
        {
            return length % 2 == 0;
        }

        private bool IsValidFile(string filePath)
        {
            return File.Exists(filePath);
        }

        private bool IsValidName(string name, int index, ref string message)
        {
            if (string.IsNullOrEmpty(name))
            {
                message = $"Name missing in line {index} in file.";
                return false;
            }
            if (name.Any(c => char.IsDigit(c)))
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

        private bool IsValidScoresLength(int length)
        {
            return length <= 21;
        }
    }
}
