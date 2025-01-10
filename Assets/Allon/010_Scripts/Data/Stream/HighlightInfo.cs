using System;
using System.Collections.Generic;
using Aion.Highlights.Managers;

namespace Aion.Highlights.Data.Stream
{
    // General information about the match
    [Serializable]
    public class HighlightInfo
    {
        public string MatchTitle; // Is niet altijd aanwezig
        public string MatchID; // is dit een int?
        public DateTime MatchDate; // DateTime string UTC format
        public CaptureCamera CaptureCamera = CaptureCamera.Center;
        public float PitchWidth;
        public float PitchLength;
        public HighlightMoment[] HighlightMoments;
        public Dictionary<string, HighlightObject>[] HighlightObjectsByIDArray;
    }
    
    // MRA: in CaptureManager.cs:
    // public enum CaptureCamera
    // {
    //     Center = 0,
    //     BirdsEye = 1,
    //     TowardsGoalLeft = 2,
    //     TowardsGoalRight = 3,
    //     BehindGoalLeft = 4,
    //     BehindGoalRight = 5
    // }
}