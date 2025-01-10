using System;
using System.Collections.Generic;
using Aion.Highlights.Data.Stream;
using simpleDI.Injection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Aion.Highlights.Data
{
	// Only used for checking values in the inspector
	[CreateAssetMenu(fileName = "AllEntries", menuName = "Data/AllEntries")]
	[Serializable]
	public class AllEntries : SerializedScriptableObject, IInjectable
	{
		public Dictionary<string, HighlightObject>[] HighlightObjectsByIDArray;
	}
}