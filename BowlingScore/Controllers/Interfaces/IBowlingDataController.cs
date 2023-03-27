using BowlingScore.Generic;
using BowlingScore.Models;
using System.Collections.Generic;

namespace BowlingScore.Controller.Interfaces
{
    public interface IBowlingDataController
    {
        Result GetBowlingScoreModelsFromFile(string filePath, IFileReader fileReader);
        List<BowlingScoreModel> AddBowlingScoreModelsToList(List<KeyValuePair<string, List<int>>> nameScoreKvp);
    }
}
