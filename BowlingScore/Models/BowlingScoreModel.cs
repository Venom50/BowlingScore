using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlingScore.Models
{
    public class BowlingScoreModel
    {
        public string Name { get; set; }
        public List<int> ThrowsScores { get; set; }
        public int TotalScore { get; set; }
    }
}
