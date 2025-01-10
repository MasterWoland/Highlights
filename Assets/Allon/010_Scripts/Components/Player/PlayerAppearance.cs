using Aion.Highlights.Data;
using Aion.Highlights.Data.Stream;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Aion.Highlights.Components.Player
{
	public class PlayerAppearance : MonoBehaviour
	{
		[SerializeField] private GameObject[] _gloves;

		[Header("Renderers")] [SerializeField] private Renderer _basePlayerRenderer;
		[SerializeField] private Renderer _hairTopRenderer;
		[SerializeField] private Renderer _hairBaseRenderer;
		[SerializeField] private Renderer _shirtRenderer; // Has two materials
		[SerializeField] private Renderer _shortsRenderer;
		[SerializeField] private Renderer _kneesRenderer;
		[SerializeField] private Renderer _sockLeftRenderer;
		[SerializeField] private Renderer _sockRightRenderer;
		[SerializeField] private Renderer _shoeLeftRenderer;
		[SerializeField] private Renderer _shoeRightRenderer;
		[SerializeField] private Renderer _gloveRendererLeft;
		[SerializeField] private Renderer _gloveRendererRight;

		// [Header("Player ObjectID")] 
		// [SerializeField] private GameObject _playerIDContainer;
		// [SerializeField] private TextMeshProUGUI _shirtNumber;
		// [SerializeField] private TextMeshProUGUI _playerName;

		[Header("Team kits")]
		[SerializeField] private TeamKit[] _teamKits;
		
		private int _teamKitIndex = -1;
		private bool _isGoalie = false;

		private static readonly int MATERIAL_COLOR_HASH = Shader.PropertyToID("_BaseColor");

		private void Awake()
		{
			foreach (var glove in _gloves)
			{
				glove.SetActive(false); // hidden by default
			}
		}

		public void SetAppearance(int teamID)
		{
			// Debug.Log("[ APP ] team ID = " + teamID);

			_teamKitIndex = teamID;

			if (_teamKitIndex < 0) _teamKitIndex = (int)HighlightObjectType.Referee; // in case we bump into an anomaly

			SetShirt();
			SetShorts();
			SetSocks();
			SetShoes();

			if (_isGoalie) SetGloves();
		}

#region Set Kit
		private void SetShirt()
		{
			_shirtRenderer.materials[0].SetColor(MATERIAL_COLOR_HASH, _teamKits[_teamKitIndex].ShirtColor);
			_shirtRenderer.materials[1].SetColor(MATERIAL_COLOR_HASH, _teamKits[_teamKitIndex].ShirtColor);
		}

		private void SetShorts()
		{
			_shortsRenderer.materials[0].SetColor(MATERIAL_COLOR_HASH, _teamKits[_teamKitIndex].ShortsColor);
		}

		private void SetSocks()
		{
			_sockLeftRenderer.materials[0].SetColor(MATERIAL_COLOR_HASH, _teamKits[_teamKitIndex].SocksColor);
			_sockRightRenderer.materials[0].SetColor(MATERIAL_COLOR_HASH, _teamKits[_teamKitIndex].SocksColor);
		}

		private void SetShoes()
		{
			_shoeLeftRenderer.materials[0].SetColor(MATERIAL_COLOR_HASH, _teamKits[_teamKitIndex].ShoesColor);
			_shoeRightRenderer.materials[0].SetColor(MATERIAL_COLOR_HASH, _teamKits[_teamKitIndex].ShoesColor);
		}

		private void SetGloves()
		{
			_gloveRendererLeft.materials[0].SetColor(MATERIAL_COLOR_HASH, _teamKits[_teamKitIndex].GlovesColor);
			_gloveRendererRight.materials[0].SetColor(MATERIAL_COLOR_HASH, _teamKits[_teamKitIndex].GlovesColor);
		}
#endregion
	}
}