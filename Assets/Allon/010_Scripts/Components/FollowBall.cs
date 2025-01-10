using Aion.Highlights.Data;
using Aion.Highlights.Events;
using Aion.Highlights.Utils;
using simpleDI.Injection.Allon;
using UnityEngine;

namespace Aion.Highlights.Components
{
	public class FollowBall : MonoBehaviour
	{
		// [InjectID(Id = ID.E_MATCH_OBJECTS_INITIALIZED)] private BaseEvent _matchObjectsInitializedEvent;
		private Transform _transform;
		private Transform _ballTransform;
		private float _minDistanceX = 7.5f; // is the X-distance between ball and cam is larger, the cam starts moving 
		private bool _isRunning = false;

		private void Awake()
		{
			// DIBinder.Injector.Inject(this);
			DIBinder.Injector.InjectID(this);
			_transform = transform;
		}

		private void Update()
		{
			if (!_isRunning) return;
			if (!_ballTransform) return;
			
			Vector3 ballPos = _ballTransform.position;
			Vector3 camPos = _transform.position;
			float distanceX = ballPos.x - camPos.x;
			distanceX = Mathf.Abs(distanceX);

			if (!(distanceX > _minDistanceX)) return;

			// move
			camPos.x = Mathf.Lerp(camPos.x, ballPos.x, Time.deltaTime);
			_transform.position = camPos;
		}

#region EVENTS
		private void OnEnable()
		{
			// _matchObjectsInitializedEvent.Handler += OnBallSpawned; 
		}

		private void OnDisable()
		{
			// _matchObjectsInitializedEvent.Handler -= OnBallSpawned;
			// CancelInvoke();
		}

		private void OnBallSpawned()
		{
			_ballTransform = GameObject.FindWithTag(AionConstants.TAG_BALL).transform;

			// Debug.Log("[ Follow ] ball found: " + _ballTransform.name);
			_isRunning = true;
			// InvokeRepeating(nameof(Follow), 0f, 0.05f);
		}
	}
#endregion
}