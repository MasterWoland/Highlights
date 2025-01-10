namespace UIWidgets.Examples.VHAR
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ItemView component.
	/// </summary>
	public class ItemViewLayoutElement : ItemView
	{
		/// <summary>
		/// Texture.
		/// </summary>
		[SerializeField]
		public RawImage Texture;

		/// <summary>
		/// Texture LayoutElement.
		/// </summary>
		LayoutElement textureLayoutElement;

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public TextAdapter Name;

		/// <summary>
		/// Text.
		/// </summary>
		[SerializeField]
		public TextAdapter Text;

		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				Foreground = new Graphic[]
				{
					UtilitiesUI.GetGraphic(Name),
					UtilitiesUI.GetGraphic(Text),
				};
				GraphicsForegroundVersion = 1;
			}
		}

		/// <inheritdoc/>
		public override void Init()
		{
			base.Init();

			textureLayoutElement = Utilities.GetOrAddComponent<LayoutElement>(Texture);
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public override void SetData(Item item)
		{
			name = item.Name;
			Texture.texture = item.Texture;

			var ar = item.Texture != null
				? item.Texture.width / item.Texture.height
				: 0f;
			textureLayoutElement.preferredHeight = ar != 0f ? Texture.rectTransform.rect.width / ar : 0f;
			Name.text = item.Name;
			Text.text = item.Text;
		}
	}
}