namespace EasyLayoutNS
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Settings for the staggered layout.
	/// </summary>
	[Serializable]
	public class EasyLayoutEllipseSettings : IObservable, INotifyPropertyChanged
	{
		[SerializeField]
		private bool widthAuto = true;

		/// <summary>
		/// Calculate with or not.
		/// </summary>
		public bool WidthAuto
		{
			get
			{
				return widthAuto;
			}

			set
			{
				Change(ref widthAuto, value, "WidthAuto");
			}
		}

		[SerializeField]
		private float width;

		/// <summary>
		/// Width.
		/// </summary>
		public float Width
		{
			get
			{
				return width;
			}

			set
			{
				Change(ref width, value, "Width");
			}
		}

		[SerializeField]
		private bool heightAuto = true;

		/// <summary>
		/// Calculate height or not.
		/// </summary>
		public bool HeightAuto
		{
			get
			{
				return heightAuto;
			}

			set
			{
				Change(ref heightAuto, value, "HeightAuto");
			}
		}

		[SerializeField]
		private float height;

		/// <summary>
		/// Height.
		/// </summary>
		public float Height
		{
			get
			{
				return height;
			}

			set
			{
				Change(ref height, value, "Height");
			}
		}

		[SerializeField]
		private float angleStart;

		/// <summary>
		/// Angle for the display first element.
		/// </summary>
		public float AngleStart
		{
			get
			{
				return angleStart;
			}

			set
			{
				Change(ref angleStart, value, "AngleStart");
			}
		}

		[SerializeField]
		private bool angleStepAuto;

		/// <summary>
		/// Calculate or not AngleStep.
		/// </summary>
		public bool AngleStepAuto
		{
			get
			{
				return angleStepAuto;
			}

			set
			{
				Change(ref angleStepAuto, value, "AngleStepAuto");
			}
		}

		[SerializeField]
		private float angleStep = 20f;

		/// <summary>
		/// Angle distance between elements.
		/// </summary>
		public float AngleStep
		{
			get
			{
				return angleStep;
			}

			set
			{
				Change(ref angleStep, value, "AngleStep");
			}
		}

		[SerializeField]
		private EllipseFill fill = EllipseFill.Closed;

		/// <summary>
		/// Fill type.
		/// </summary>
		public EllipseFill Fill
		{
			get
			{
				return fill;
			}

			set
			{
				Change(ref fill, value, "Fill");
			}
		}

		[SerializeField]
		private float arcLength = 360f;

		/// <summary>
		/// Arc length.
		/// </summary>
		public float ArcLength
		{
			get
			{
				return arcLength;
			}

			set
			{
				Change(ref arcLength, value, "Length");
			}
		}

		[SerializeField]
		private EllipseAlign align;

		/// <summary>
		/// Align.
		/// </summary>
		public EllipseAlign Align
		{
			get
			{
				return align;
			}

			set
			{
				Change(ref align, value, "Align");
			}
		}

		[SerializeField]
		[HideInInspector]
		private float angleScroll;

		/// <summary>
		/// Angle padding.
		/// </summary>
		public float AngleScroll
		{
			get
			{
				return angleScroll;
			}

			set
			{
				Change(ref angleScroll, value, "AngleScroll");
			}
		}

		[SerializeField]
		[HideInInspector]
		private float angleFiller;

		/// <summary>
		/// Angle filler.
		/// </summary>
		public float AngleFiller
		{
			get
			{
				return angleFiller;
			}

			set
			{
				Change(ref angleFiller, value, "AngleFiller");
			}
		}

		[SerializeField]
		private bool elementsRotate = true;

		/// <summary>
		/// Rotate elements.
		/// </summary>
		public bool ElementsRotate
		{
			get
			{
				return elementsRotate;
			}

			set
			{
				Change(ref elementsRotate, value, "ElementsRotate");
			}
		}

		[SerializeField]
		private float elementsRotationStart;

		/// <summary>
		/// Start rotation for elements.
		/// </summary>
		public float ElementsRotationStart
		{
			get
			{
				return elementsRotationStart;
			}

			set
			{
				Change(ref elementsRotationStart, value, "ElementsRotationStart");
			}
		}

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event OnChange OnChange;

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Change value.
		/// </summary>
		/// <typeparam name="T">Type of field.</typeparam>
		/// <param name="field">Field value.</param>
		/// <param name="value">New value.</param>
		/// <param name="propertyName">Property name.</param>
		protected void Change<T>(ref T field, T value, string propertyName)
		{
			if (!EqualityComparer<T>.Default.Equals(field, value))
			{
				field = value;
				NotifyPropertyChanged(propertyName);
			}
		}

		/// <summary>
		/// Property changed.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		protected void NotifyPropertyChanged(string propertyName)
		{
			var c_handlers = OnChange;
			if (c_handlers != null)
			{
				c_handlers();
			}

			var handlers = PropertyChanged;
			if (handlers != null)
			{
				handlers(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Get debug information.
		/// </summary>
		/// <param name="sb">String builder.</param>
		public virtual void GetDebugInfo(System.Text.StringBuilder sb)
		{
			sb.Append("\tWidth Auto: ");
			sb.Append(WidthAuto);
			sb.AppendLine();

			sb.Append("\tWidth: ");
			sb.Append(Width);
			sb.AppendLine();

			sb.Append("\tHeight Auto: ");
			sb.Append(HeightAuto);
			sb.AppendLine();

			sb.Append("\tHeight: ");
			sb.Append(Height);
			sb.AppendLine();

			sb.Append("\tAngle Start: ");
			sb.Append(AngleStart);
			sb.AppendLine();

			sb.Append("\tAngle Step Auto: ");
			sb.Append(AngleStepAuto);
			sb.AppendLine();

			sb.Append("\tAngle Step: ");
			sb.Append(AngleStep);
			sb.AppendLine();

			sb.Append("\tAlign: ");
			sb.Append(EnumHelper<EllipseAlign>.ToString(Align));
			sb.AppendLine();

			sb.Append("\tElements Rotate: ");
			sb.Append(ElementsRotate);
			sb.AppendLine();

			sb.Append("\tElements Rotation Start: ");
			sb.Append(ElementsRotationStart);
			sb.AppendLine();

			sb.AppendLine("\t#####");

			sb.Append("\tFill: ");
			sb.Append(EnumHelper<EllipseFill>.ToString(Fill));
			sb.AppendLine();

			sb.Append("\tAngle Filler: ");
			sb.Append(AngleFiller);
			sb.AppendLine();

			sb.Append("\tAngle Scroll: ");
			sb.Append(AngleScroll);
			sb.AppendLine();

			sb.Append("\tArc Length: ");
			sb.Append(ArcLength);
			sb.AppendLine();
		}
	}
}