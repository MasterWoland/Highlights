using System;

namespace Aion.Highlights.Data.Stream
{
    [Serializable]
    public enum HighlightMomentType
    {
        Unknown = 0,
        HighPressure = 1,
        // LineBreakingPass = 2
    }
    
    [Serializable]
    public class HighlightMoment
    {
        public HighlightMomentType HighlightMomentType;
        public int StartFrameIndex; // the moment we have to start showing the event
        public HighlightMomentPlayer[] HighlightMomentPlayers;
    }
}