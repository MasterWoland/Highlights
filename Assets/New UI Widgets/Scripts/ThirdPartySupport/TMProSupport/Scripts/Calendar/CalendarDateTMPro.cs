#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UnityEngine;

	/// <summary>
	/// CalendarDate TMPro.
	/// Display date.
	/// </summary>
	public class CalendarDateTMPro : CalendarDateBase
	{
		/// <summary>
		/// Text component to display Day.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with DayAdapter")]
		protected TextMeshProUGUI Day;

		/// <inheritdoc/>
		public override void Upgrade()
		{
			base.Upgrade();
#pragma warning disable 0618
			Utilities.GetOrAddComponent(Day, ref dayAdapter);
#pragma warning restore 0618
		}
	}
}
#endif