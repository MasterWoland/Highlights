using System;
using System.Collections.Generic;
using System.Globalization;
using Aion.Highlights.Data;
using Aion.Highlights.Data.Stream;
using UnityEngine;

namespace Aion.Highlights.Utils
{
	// [Serializable]
	// public class MatchObjectsPerEntry : SerializedScriptableObject 
	// {
	// 	// MRA: we use a SerializedScriptableObject so we can check the Dictionary in the Inspector
	// 	// MRA: once we no longer need to do this, we may turn this into a regular data class
	// 	public Dictionary<int, MatchObject> MatchObjectsByID;
	//
	// 	public MatchObjectsPerEntry()
	// 	{
	// 		MatchObjectsByID = new Dictionary<int, MatchObject>();
	// 	}
	// }

	public static class OptaDataConverter
	{
		private static Dictionary<string, HighlightObject> _matchObjectsByID;
		// private static Dictionary<int, HighlightObject> _matchObjectsByID;
		
		
		public static Dictionary<string, HighlightObject> GetTrackingDataEntry(string data)
		{
			_matchObjectsByID = new Dictionary<string, HighlightObject>();
			// _matchObjectsByID = new Dictionary<int, HighlightObject>();
			// MatchObjectsPerEntry entry = ScriptableObject.CreateInstance<MatchObjectsPerEntry>();

			// OPT: string.Split() is too heavy (nearly 10KB)
			string[] rawEntry = data.Split(':'); // split in 3 strings: time/half info, players & ball-data

			var timeStampAndHalfInfo = rawEntry[0].Split(";");

			// MRA: time stamp value can be too high for a float, so we have to use a double
			long timeStamp = long.Parse(timeStampAndHalfInfo[0], CultureInfo.InvariantCulture.NumberFormat);
			// MRA: we probably won't need to do much with this, so we leave this as a string 
			string halfInfo = timeStampAndHalfInfo[1];

			HighlightObject ballHighlightObject = GetBallAsMatchObject(rawEntry[2], timeStamp);
			ballHighlightObject.Timestamp = timeStamp;
			_matchObjectsByID.Add(ballHighlightObject.ObjectID.ToString(), ballHighlightObject);

			// rawEntry[]
			// 0 = timestamp
			// 1 = half information
			// 2 = players
			// 3 = ball


			// MRA: not clear to me why originally a new[] was used for the split
			// foreach (string tempData in rawEntry[1].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
			foreach (string tempData in rawEntry[1].Split(';', StringSplitOptions.RemoveEmptyEntries))
			{
				// Debug.Log("[ C ] Player Temp Data: " + tempData);

				HighlightObject playerHighlightObject = GetPlayerAsMatchObject(tempData);

				if (playerHighlightObject != null)
				{
					playerHighlightObject.Timestamp = timeStamp;
					// Debug.Log("[ C ] Adding player with ObjectID: " + playerMatchObject.ObjectID);
					_matchObjectsByID.Add(playerHighlightObject.ObjectID, playerHighlightObject);
				}
			}

			return _matchObjectsByID;
		}

#region HELPER METHODS
		private static HighlightObject GetBallAsMatchObject(string data, double timeStamp)
		{
			HighlightObject highlightObject = new HighlightObject();

			data = data.Remove(data.Length - 1, 1); // Removing the ";" at the end
			string[] array = data.Split(','); // separate x, y & z values 

			// z-value is our y-position
			highlightObject.Position = new Vector3(float.Parse(array[0], CultureInfo.InvariantCulture.NumberFormat),
				float.Parse(array[2], CultureInfo.InvariantCulture.NumberFormat),
				float.Parse(array[1], CultureInfo.InvariantCulture.NumberFormat));
			
			highlightObject.Position = AdjustPositionFromOptaTXT(highlightObject.Position);
		
			// Debug.Log("[ Converter ] Ball position: " + matchObject.Position);

			// MRA: perhaps we will need this later, when calculating the correct update rate
			// baseObject.Timestamp = StaticFunctions.MilliSecondsToSecondsBasedOnFrameRate(timeStamp, framerate);

			return highlightObject;
		}

		private static HighlightObject GetPlayerAsMatchObject(string data)
		{
			HighlightObject highlightObject = new HighlightObject();
			string[] array = data.Split(',');

			highlightObject.Type = (HighlightObjectType)int.Parse(array[0]);
			// highlightObject.ObjectID = int.Parse(array[1]); // = PlayerID
			highlightObject.ObjectID = array[1]; // = PlayerID
			highlightObject.ShirtNumber = int.Parse(array[2]);

			highlightObject.Position = new Vector3(float.Parse(array[3], CultureInfo.InvariantCulture.NumberFormat),
				0f,
				float.Parse(array[4], CultureInfo.InvariantCulture.NumberFormat));

			highlightObject.Position = AdjustPositionFromOptaTXT(highlightObject.Position);
			
			// matchObject.Timestamp = StaticFunctions.MilliSecondsToSecondsBasedOnFrameRate(timeStamp, framerate);

			// MRA: may be redundant
			// if (objectTypeNumber == 3)
			// {
			// 	matchObject.ObjectID = ((jerseyNumber != -1) ? (300 + jerseyNumber) : (300 + objectTypeNumber));
			// }
			// else if (jerseyNumber == -1 || objectTypeNumber == -1)
			// {
			// 	Debug.LogError("[OptaDataConverter] ShirtNumber or Object Type equals -1");
			// 	matchObject = null;
			// }

			return highlightObject;
		}

		private static Vector3 AdjustPositionFromOptaTXT(Vector3 position)
		{
			position.x -= AionConstants.PITCH_LENGTH * 0.5f;
			position.z -= AionConstants.PITCH_WIDTH * 0.5f;
			return position;
		}
#endregion
	}
}