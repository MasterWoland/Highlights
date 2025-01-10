using System;
using Aion.Highlights.Data.Stream;
using simpleDI.Injection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Aion.Highlights.Data
{
	/// <summary>
	/// Current HighlightInfo
	/// </summary>
	[CreateAssetMenu(fileName = "HighlightInfo_SO", menuName = "Data/HighlightInfo_SO")]
	[Serializable]
	public class HighlightInfo_SO : SerializedScriptableObject, IInjectable
	{
		public HighlightInfo HighlightInfo;
	}
}