using System.Collections.Generic;

namespace BowlingScore.Generic
{
    public class Result
    {
        public bool IsSuccess { get; set; } = true;
        public List<string> Messages { get; set; } = new List<string>();
        public object ResultObject { get; set; }

        public void AddError(string errorMessage)
        {
            IsSuccess = false;
            Messages.Add(errorMessage);
        }

        public void AddInfo(string infoMessage)
        {
            Messages.Add(infoMessage);
        }
    }
}
