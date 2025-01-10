using System;

namespace Aion.Highlights.Data.Stream
{
    [Serializable]
    public enum PressureLevel
    {
        None = 0,
        Low = 1,
        Med = 2,
        High = 3
    }
    
    [Serializable]
    public class HighlightMomentPlayer
    {
        // public int PlayerID;
        public string PlayerID;
        public PressureLevel PressureLevel;
        public int ShirtNumber;
        public string LastName;
        public bool IsMainActor;
        public float MainActorPositionX; // The cone of non-main actors needs to point towards the main actor
        public float MainActorPositionZ; // The cone of non-main actors needs to point towards the main actor
    }
}