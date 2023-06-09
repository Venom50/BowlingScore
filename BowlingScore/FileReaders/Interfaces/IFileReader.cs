﻿using BowlingScore.Generic;

namespace BowlingScore
{
    public interface IFileReader
    {
        Result ReadFile(string filePath);
        Result GetBowlingData(string[] data);
    }
}
