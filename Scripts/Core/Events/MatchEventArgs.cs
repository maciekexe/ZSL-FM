using System;
using LacznoscFM.Core.Models;

namespace LacznoscFM.Core.Events
{
    /// <summary>
    /// Argumenty zdarzenia związanego z przebiegiem meczu.
    /// Można tu przekazywać np. gola, kartkę lub zmianę.
    /// </summary>
    public class MatchEventArgs : EventArgs
    {
        public Match Match { get; }
        public string EventDescription { get; }

        public MatchEventArgs(Match match, string eventDescription)
        {
            Match = match;
            EventDescription = eventDescription;
        }

    }
}
