using System;
using System.Collections.Generic;
using Aion.Highlights.Data.Stream;
using simpleDI.Injection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Aion.Highlights.Data
{
	[CreateAssetMenu(fileName = "TrackingDataPerEntry", menuName = "Data/TrackingDataPerEntry")]
	[Serializable]
	public class TrackingDataPerEntry : SerializedScriptableObject, IInjectable
	{
		// TESTING
		public int CurrentIndex;
		public int TotalAmount;
		// -----
		
		public Dictionary<string, HighlightObject> HighlightObjectsByID;
		
		public TrackingDataPerEntry()
		{
			HighlightObjectsByID = new Dictionary<string, HighlightObject>();
		}
	}
}