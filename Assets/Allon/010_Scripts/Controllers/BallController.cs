using Aion.Highlights.Utils;
using UnityEngine;

namespace Aion.Highlights.Controllers
{
	public class BallController : MonoBehaviour
	{
		private Transform _transform;
		[SerializeField] private Transform _blobShadowTF;
		private float _blobShadowPosY;
		
		private void Awake()
		{
			tag = AionConstants.TAG_BALL;
			_transform = this.transform;

			_blobShadowPosY =_blobShadowTF.position.y;
		}

#region PUBLIC
		public void UpdatePosition(Vector3 position)
		{
			_transform.position = position;

			// make sure blob shadow remains on the ground
			Vector3 blobShadowPosition = _transform.position;
			blobShadowPosition.y = _blobShadowPosY;
			_blobShadowTF.position = blobShadowPosition;
		}
#endregion
	}
}