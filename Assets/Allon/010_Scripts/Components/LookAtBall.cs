using System;
using Aion.Highlights.Data;
using Aion.Highlights.Events;
using Aion.Highlights.Utils;
using simpleDI.Injection.Allon;
using UnityEngine;

namespace Aion.Highlights.Components
{
    /// <summary>
    /// Used for Cameras
    /// </summary>
    public class LookAtBall : MonoBehaviour
    {
        [InjectID(Id = ID.E_NEW_TRACKING_DATA_ENTRY)] private BaseEvent _newTrackingDataEntryEvent_Main;
        
        private Transform _transform;
        private Quaternion _defaultRotation;
        private Transform _ballTransform;
        // private bool _isBall = false;
        
        private void Awake()
        {
            DIBinder.Injector.InjectID(this);
            
            _transform = transform;
            _defaultRotation = _transform.rotation;
        }

        private void Reset()
        {
            _transform.rotation = _defaultRotation;
        }
        
        #region EVENTS
        private void OnEnable()
        {
            _newTrackingDataEntryEvent_Main.Handler += OnPositionsUpdated;
        }
        
        private void OnDisable()
        {
            _newTrackingDataEntryEvent_Main.Handler -= OnPositionsUpdated;
        }

        private void OnPositionsUpdated()
        {
            if (_ballTransform == null)
            {
                TryGetBall(); // will be for next time
                return;
            }

            Vector3 targetPos = new Vector3(_ballTransform.position.x, _ballTransform.position.y, _ballTransform.position.z);
            _transform.LookAt(targetPos);
        }

        private void TryGetBall()
        {
            if (!GameObject.FindWithTag(AionConstants.TAG_BALL)) return;
            
            _ballTransform = GameObject.FindWithTag(AionConstants.TAG_BALL).transform;
            // if (_ballTransform) _isBall = true;
        }
#endregion
    }
}