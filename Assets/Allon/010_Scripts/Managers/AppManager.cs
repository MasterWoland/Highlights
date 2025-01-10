using System.Collections.Generic;
using Aion.Highlights.Components;
using Aion.Highlights.Data;
using Aion.Highlights.Data.Stream;
using Aion.Highlights.Events;
using Aion.Highlights.Utils;
using simpleDI.Injection;
using simpleDI.Injection.Allon;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Aion.Highlights.Managers
{
    public class AppManager : MonoBehaviour
    {
        // Data
        [Inject] [ShowInInspector, ReadOnly] private HighlightInfo_SO _highlightInfo_SO;

        // Events
        [InjectID(Id = ID.E_DATA_FILE_INFO)] private DataFileInfoEvent _dataFileInfoEvent;
        [InjectID(Id = ID.E_DATA_PREPARED)] private BaseEvent _dataPreparedEvent;
        [InjectID(Id = ID.E_POSITIONS_UPDATED)] private BaseEvent _positionsUpdatedEvent;
        [InjectID(Id = ID.E_OUT_OF_ENTRIES)] private BaseEvent _outOfEntriesEvent;
        [InjectID(Id = ID.E_CAPTURE_COMPLETE)] private BaseEvent _captureCompleteEvent; // capture of entire fragment
        [InjectID(Id = ID.E_CAM_CAPTURE_COMPLETE)] private BaseEvent _camCaptureCompleteEvent; // capture of entire fragment by 1 camera
        [InjectID(Id = ID.E_HIGHLIGHT_MOMENT_DETECTED)] private HighlightMomentEvent _highlightMomentDetected;
        [InjectID(Id = ID.E_SCREENSHOT_CAPTURED)] private BaseEvent _screenshotCapturedEvent;
        [InjectID(Id = ID.E_FINISHED_RENDERING_UPDATED_FRAME)] private BaseEvent _finishedRenderingUpdatedFrame;
        [InjectID(Id = ID.E_FROZEN_FRAME_CAPTURED)] protected BaseEvent _frozenFrameCapturedEvent;
        [InjectID(Id = ID.E_HIGHLIGHT_MOMENT_CAPTURE_COMPLETE)] protected BaseEvent _highlightMomentCaptureCompleteEvent;

        // Event Dispatchers
        [InjectID(Id = ID.E_PREPARE_NEW_DATA)] private DataFileInfoEvent _prepareNewData_Main;
        [InjectID(Id = ID.E_START_HIGHLIGHT)] private BaseEvent _startHighlight_Main;
        [InjectID(Id = ID.E_RESUME_PLAY)] private BaseEvent _resumePlay_Main;
        [InjectID(Id = ID.E_CAPTURE)] private BaseEvent _captureEvent_Main;
        [InjectID(Id = ID.E_PREPARE_NEXT_FRAME)] private BaseEvent _prepareNextFrame_Main;
        [InjectID(Id = ID.E_NEW_TRACKING_DATA_ENTRY)] private BaseEvent _newTrackingDataEntryEvent_Main;
        [InjectID(Id = ID.E_FINISHED_ALL_ENTRIES)] private BaseEvent _finishedAllEntriesEvent_Main;
        [InjectID(Id = ID.E_START_HIGHLIGHT_MOMENT)] private HighlightMomentEvent _startHighlightMoment_Main;
        [InjectID(Id = ID.E_CAPTURE_FROZEN_MOMENT)] private IntEvent _captureFrozenMoment_Main;
        [InjectID(Id = ID.E_FROZEN_FRAME_CAPTURED_MAIN)] protected BaseEvent _frozenFrameCapturedEvent_Main;

        [ShowInInspector, ReadOnly] private Queue<DataFileInfo> _dataFileInfoQueue = new Queue<DataFileInfo>();
        [ShowInInspector, ReadOnly] private DataFileInfo _currentDataFileInfo; // the data that is currently being processed
        private bool _isProcessingDataFileInfo = false;
       
        private bool _isMomentDetected = false;

        private void Awake()
        {
            Application.runInBackground = true;
            DIBinder.Injector.Inject(this);
            DIBinder.Injector.InjectID(this);
        }

        private void Reset()
        {
            _isProcessingDataFileInfo = false;
        }

#region EVENTS
        private void OnEnable()
        {
            _dataFileInfoEvent.Handler += OnNewDataFileInfo;
            _dataPreparedEvent.Handler += OnDataPrepared;
            _positionsUpdatedEvent.Handler += OnPositionsUpdated;
            _finishedRenderingUpdatedFrame.Handler += OnFinishedRenderingUpdatedFrame;
            _highlightMomentDetected.Handler += OnHighlightMomentDetected;
            _screenshotCapturedEvent.Handler += OnScreenshotCaptured;
            _camCaptureCompleteEvent.Handler += OnCameraCaptureComplete;
            _captureCompleteEvent.Handler += OnCaptureComplete;
            _frozenFrameCapturedEvent.Handler += OnFrozenFrameCaptured;
            _highlightMomentCaptureCompleteEvent.Handler += OnHighlightMomentCaptureComplete;
            _outOfEntriesEvent.Handler += OnOutOfEntries;
        }

        private void OnDisable()
        {
            _dataFileInfoEvent.Handler -= OnNewDataFileInfo;
            _dataPreparedEvent.Handler -= OnDataPrepared;
            _positionsUpdatedEvent.Handler -= OnPositionsUpdated;
            _finishedRenderingUpdatedFrame.Handler -= OnFinishedRenderingUpdatedFrame;
            _highlightMomentDetected.Handler -= OnHighlightMomentDetected;
            _screenshotCapturedEvent.Handler -= OnScreenshotCaptured;
            _camCaptureCompleteEvent.Handler -= OnCameraCaptureComplete;
            _captureCompleteEvent.Handler -= OnCaptureComplete;
            _frozenFrameCapturedEvent.Handler -= OnFrozenFrameCaptured;
            _highlightMomentCaptureCompleteEvent.Handler -= OnHighlightMomentCaptureComplete;
            _outOfEntriesEvent.Handler -= OnOutOfEntries;
        }

        private void OnFrozenFrameCaptured()
        {
            // Debug.Log("[ AM ] On Frozen Frame Captured");
            _frozenFrameCapturedEvent_Main?.Dispatch();
        }

        private void OnNewDataFileInfo(DataFileInfo info)
        {
            _dataFileInfoQueue.Enqueue(info);

            if (_isProcessingDataFileInfo) return;

            _isProcessingDataFileInfo = true;
            _currentDataFileInfo = _dataFileInfoQueue.Peek();
            _prepareNewData_Main?.Dispatch(_currentDataFileInfo); // Get the data ready for the highlight
        }

        private void OnDataPrepared()
        {
            _startHighlight_Main?.Dispatch(); // we are ready to start the highlight
        }

        private void OnPositionsUpdated()
        {
            _newTrackingDataEntryEvent_Main?.Dispatch();
        }

        private void OnFinishedRenderingUpdatedFrame()
        {
            _captureEvent_Main?.Dispatch();
        }

        private void OnHighlightMomentDetected(HighlightMoment moment)
        {
            _isMomentDetected = true; // We will act on this when we receive the ScreenshotCaptured Event
            _startHighlightMoment_Main?.Dispatch(moment); // Prepare the HighlightObjects
        }

        private void OnHighlightMomentCaptureComplete()
        {
            _resumePlay_Main?.Dispatch();
            _prepareNextFrame_Main?.Dispatch();
        }

        private void OnScreenshotCaptured()
        {
            if (_isMomentDetected)
            {
                //  Moment detected, do not PrepareNextFrame
                _isMomentDetected = false;
                int numFrames = (int) (AionConstants.FREEZE_TIME * AionConstants.FPS);
                _captureFrozenMoment_Main?.Dispatch(numFrames);
                // Debug.Log("[ AM ] capture " + numFrames + " for FREEZE");

                return;
            }

            _prepareNextFrame_Main?.Dispatch();
        }

        private void OnCameraCaptureComplete()
        {
            _prepareNewData_Main?.Dispatch(_currentDataFileInfo); // we need to capture the same file again
        }

        private void OnCaptureComplete()
        {
            _dataFileInfoQueue.Dequeue();
            if (_dataFileInfoQueue.Count > 0)
            {
                _currentDataFileInfo = _dataFileInfoQueue.Peek();
                _prepareNewData_Main?.Dispatch(_currentDataFileInfo);
            }
            else
            {
                Reset();
            }
        }

        private void OnOutOfEntries()
        {
            _finishedAllEntriesEvent_Main?.Dispatch();
        }
#endregion
    }
}