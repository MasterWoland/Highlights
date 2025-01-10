using System;
using UnityEngine;

namespace Aion.Highlights.Data.Stream
{
	public enum HighlightObjectType // MRA: rename this?
	{
		Unknown = -1,
		PlayerHome = 0,
		PlayerAway = 1,
		Referee = 2,
		GoalkeeperHome = 3,
		GoalkeeperAway = 4,
		Ball = 5
	}
	
	public enum PositionID
	{
		Unknown = 0,
		Striker = 1,
		Midfielder = 2,
		Defender = 3,
		Goalkeeper = 4,
		WingBack = 5,
		Substitute = 6
	}
	
	// MRA: still necessary?
	public enum PositionSide
	{
		Unknown = 0,
		Centre = 1,
		LeftCentre = 2,
		CentreRight = 3,
		Left = 4,
		Right = 5,
		WingBack = 6,
		Substitute = 7
	}
	
	[Serializable]
	public class HighlightObject
	{
		public string ObjectID;
		public HighlightObjectType Type;
		public long Timestamp;
		public float PositionX;
		public float PositionY;
		public float PositionZ;
		public Vector3 Position; // Will be calculated from PositionY and PositionZ (NewtonSoft cannot Deserialize Vector3)
		public PositionID PositionID;
		public int ShirtNumber = -1;
		public string FirstName = string.Empty;
		public string LastName = string.Empty; 
	}
}