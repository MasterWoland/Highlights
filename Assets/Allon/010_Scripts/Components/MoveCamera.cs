using System;
using Aion.Highlights.Data;
using Aion.Highlights.Data.Stream;
using Aion.Highlights.Events;
using Aion.Highlights.Utils;
using simpleDI.Injection;
using simpleDI.Injection.Allon;
using UnityEngine;

namespace Aion.Highlights.Components
{
    [RequireComponent(typeof(Camera))]
    public class MoveCamera : MonoBehaviour
    {
        // [Inject] private TrackingDataPerEntry _currentTrackingDataPerEntry;
        [InjectID(Id = ID.E_START_HIGHLIGHT_MOMENT)] private HighlightMomentEvent _startHighlightMoment_Main;
        // [InjectID(Id = ID.E_CAPTURE_FROZEN_MOMENT)] private IntEvent _captureFrozenMoment_Main;
        [InjectID(Id = ID.E_FROZEN_FRAME_CAPTURED_MAIN)] protected BaseEvent _frozenFrameCapturedEvent_Main;
        [InjectID(Id = ID.E_RESUME_PLAY)] private BaseEvent _resumePlay_Main;

        private Camera _camera;
        private Transform _ballTransform;
        private int _numFrames;
        private int _curFrame;
        private Vector3 _defaultPos;
        private Quaternion _defaultRotation;
        private Quaternion _targetRotation;
        private float _defaultFOV;
        private float _targetFOV;
        private float _zoomStep;
        private int _numFramesForZoom = 25; // MRA: this is a temporary solution

        private void Awake()
        {
            DIBinder.Injector.Inject(this);
            DIBinder.Injector.InjectID(this);

            _camera = GetComponent<Camera>();
            _numFrames = (int) (AionConstants.FREEZE_TIME * AionConstants.FPS);

            _defaultPos = _camera.transform.position;
            _defaultRotation = _camera.transform.rotation;
            _defaultFOV = _camera.fieldOfView;

            _targetFOV = _defaultFOV * 0.5f;
            _zoomStep = (_defaultFOV - _targetFOV) / _numFramesForZoom;

            Debug.Log("[ CAM ] num frames: " + _numFrames);
            Debug.Log("[ CAM ] zoom step: " + _zoomStep);
        }

#region EVENTS
        private void OnEnable()
        {
            _startHighlightMoment_Main.Handler += OnStartHighlightMoment;
            _frozenFrameCapturedEvent_Main.Handler += OnNextFrame;
            _resumePlay_Main.Handler += OnHighlightMomentComplete;
        }

        private void OnDisable()
        {
            _startHighlightMoment_Main.Handler -= OnStartHighlightMoment;
            _frozenFrameCapturedEvent_Main.Handler -= OnNextFrame;
            _resumePlay_Main.Handler -= OnHighlightMomentComplete;
        }

        private void OnStartHighlightMoment(HighlightMoment moment)
        {
            _curFrame = 0;
            _ballTransform = GameObject.FindWithTag(AionConstants.TAG_BALL).transform;
            if (_ballTransform == null) return; // in case there is no ball
            _targetRotation = Quaternion.LookRotation(_ballTransform.position - _camera.transform.position, Vector3.up);
        }

        private void OnNextFrame()
        {
            if (_curFrame < _numFramesForZoom)
            {
                _camera.fieldOfView -= _zoomStep;
                float lerp = ((float)_curFrame + 1.0f) / (float)_numFramesForZoom;
                // Debug.Log("[ CAM ] zoom lerp = " + lerp);
                if (_ballTransform == null) return; // in case there is no ball
                _camera.transform.rotation = Quaternion.Lerp(_camera.transform.rotation, _targetRotation, lerp);
            }
            else if (_curFrame >= (_numFrames - _numFramesForZoom))
            {
                if (_camera.fieldOfView < _defaultFOV) _camera.fieldOfView += _zoomStep;

                float lerp = ((float)_numFrames - (float)_curFrame) / (float)_numFramesForZoom;
                // Debug.Log("[ CAM ] zoom lerp = " + lerp);
                _camera.transform.rotation = Quaternion.Lerp(_defaultRotation,  _camera.transform.rotation, lerp);
            }

            _curFrame++;
        }

        private void OnHighlightMomentComplete()
        {
            Debug.Log("[ CAM ] Finished moment");
            _camera.fieldOfView = _defaultFOV;
            _camera.transform.rotation = _defaultRotation;
            _curFrame = 0;
        }
#endregion

#region HELPER METHODS
        private void Reset()
        {
            _curFrame = 0;
            _targetRotation = _defaultRotation;
        }
#endregion
    }
}