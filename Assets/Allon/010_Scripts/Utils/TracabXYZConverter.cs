using System;
using System.Collections.Generic;
using System.Globalization;
using Aion.Highlights.Data;
using Aion.Highlights.Data.Stream;
using UnityEngine;

namespace Aion.Highlights.Utils
{
	/// <summary>
	/// MRA: temp class
	/// </summary>
	public class BaseFrame
	{
		public Dictionary<string, HighlightObject> MatchObjects;
		// public Dictionary<int, HighlightObject> MatchObjects;

		public BaseFrame()
		{
			MatchObjects = new Dictionary<string, HighlightObject>();
			// MatchObjects = new Dictionary<int, HighlightObject>();
		}
	}

	public static class TracabXYZConverter
	{
		// private Dictionary<int, MatchObject> _tempCurrentFrameData;

		public static BaseFrame GetBaseFrame(string data)
		{
			BaseFrame baseFrame = new BaseFrame();


			// OPT: string.Split() is too heavy (nearly 10KB)
			string[] timeAndPlayersAndBall = data.Split(':'); // split in 3 strings, time, players & ball-data

			long timeStamp = long.Parse(timeAndPlayersAndBall[0]); // MRA: magic number alert
			// Debug.Log("[XYZ Converter] timeStamp: "+timeStamp);
			HighlightObject ballBaseObject = GetBallAsMatchObject(timeAndPlayersAndBall[2], timeStamp);
			ballBaseObject.Timestamp = timeStamp;

			baseFrame.MatchObjects.Add(ballBaseObject.ObjectID, ballBaseObject);

			foreach (string tempData in timeAndPlayersAndBall[1].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
			{
				HighlightObject playerBaseObject = GetPlayerAsMatchObject(tempData);

				if (playerBaseObject != null)
				{
					playerBaseObject.Timestamp = timeStamp;
					baseFrame.MatchObjects.Add(playerBaseObject.ObjectID, playerBaseObject);
				}
			}
			return baseFrame;
		}

#region HELPER METHODS
		private static HighlightObject GetBallAsMatchObject(string data, float timeStamp)
		{
			// Debug.Log("[ XYZ ] GetBallAsMatchObject > "+data);

			HighlightObject highlightObject = new HighlightObject();

			string[] array = data.Split(',');

			highlightObject.ObjectID = AionConstants.BALL_ID; // Ball Id is always 0. --> opslaan in static AionConstants class, net als ball radius (0.5 * 0.225) 
			highlightObject.Position = new Vector3(float.Parse(array[0], CultureInfo.InvariantCulture.NumberFormat) / 100,
				0, //0.1125f, 
				float.Parse(array[1], CultureInfo.InvariantCulture.NumberFormat) / 100);
			// baseObject.Timestamp = StaticFunctions.MilliSecondsToSecondsBasedOnFrameRate(timeStamp, framerate);

			return highlightObject;
		}

		private static HighlightObject GetPlayerAsMatchObject(string data)
		{
			HighlightObject highlightObject = new HighlightObject();

			string[] array = data.Split(',');

			int objectTypeNumber = int.Parse(array[0]);
			int jerseyNumber = int.Parse(array[2]);
			// Debug.Log("[ XYZ ] teamSide: "+objectTypeNumber);

			highlightObject.Type = (HighlightObjectType)objectTypeNumber;
			highlightObject.ObjectID = (objectTypeNumber * 100 + jerseyNumber).ToString();
			highlightObject.Position = new Vector3(float.Parse(array[3], CultureInfo.InvariantCulture.NumberFormat) / 100,
				0f,
				float.Parse(array[4], CultureInfo.InvariantCulture.NumberFormat) / 100);
			// matchObject.Timestamp = StaticFunctions.MilliSecondsToSecondsBasedOnFrameRate(timeStamp, framerate);

			if (objectTypeNumber == 3)
			{
				highlightObject.ObjectID = ((jerseyNumber != -1) ? (300 + jerseyNumber) : (300 + objectTypeNumber)).ToString();
			}
			else if (jerseyNumber == -1 || objectTypeNumber == -1)
			{
				highlightObject = null;
			}

			return highlightObject;
		}
#endregion
	}
}