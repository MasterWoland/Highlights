using Aion.Highlights.Data;
using Aion.Highlights.Events;
using simpleDI.Injection;
using simpleDI.Injection.Allon;
using UnityEngine;

namespace Aion.Highlights.Components
{
    public class DIBinder : MonoBehaviour
    {
        public static Injector Injector;

        // DATA
        [Header("Data")]
        [SerializeField] private TrackingDataPerEntry _currentTrackingDataPerEntry;
        [SerializeField] private AllEntries _allEntries;
        [SerializeField] private HighlightInfo_SO _highlightInfo_SO;

        // EVENTS
        [Header("Events")]
        [SerializeField] private DataFileInfoEvent _dataFileInfoEvent;
        [SerializeField] private BaseEvent _dataPreparedEvent;
        [SerializeField] private BaseEvent _positionsUpdatedEvent;
        [SerializeField] private BaseEvent _outOfEntriesEvent;
        [SerializeField] private BaseEvent _finishedRenderingUpdatedFrame;
        [SerializeField] private BaseEvent _screenshotCapturedEvent;
        [SerializeField] private BaseEvent _camCaptureCompleteEvent;
        [SerializeField] private BaseEvent _captureCompleteEvent;
        [SerializeField] private BaseEvent _frozenFrameCapturedEvent;
        [SerializeField] private BaseEvent _highlightMomentCaptureCompleteEvent;
        [SerializeField] private HighlightMomentEvent _highlightMomentDetected;
        
        // EVENTS from AppManager
        [SerializeField] private DataFileInfoEvent _prepareNewDataEvent_Main;
        [SerializeField] private BaseEvent _startHighlight_Main;
        [SerializeField] private BaseEvent _resumePlay_Main;
        [SerializeField] private BaseEvent _captureEvent_Main;
        [SerializeField] private BaseEvent _prepareNextFrame_Main;
        [SerializeField] private BaseEvent _newTrackingDataEntryEvent_Main;
        [SerializeField] private BaseEvent _finishedAllEntriesEvent_Main;
        [SerializeField] private HighlightMomentEvent _startHighlightMoment_Main;
        [SerializeField] private IntEvent _captureFrozenMoment_Main;
        [SerializeField] private BaseEvent _frozenFrameCapturedEvent_Main;
        
        private void Awake()
        {
            DontDestroyOnLoad(this);
            Injector = new Injector();
            Bind();
        }

        private void Bind()
        {
            // MRA: note: order is important!

            // DATA
            Injector.Bind(_currentTrackingDataPerEntry);
            Injector.Bind(_allEntries);
            Injector.Bind(_highlightInfo_SO);

            // EVENTS
            Injector.BindWithId(_dataFileInfoEvent, ID.E_DATA_FILE_INFO);
            Injector.BindWithId(_dataPreparedEvent, ID.E_DATA_PREPARED);
            Injector.BindWithId(_outOfEntriesEvent, ID.E_OUT_OF_ENTRIES);
            Injector.BindWithId(_positionsUpdatedEvent, ID.E_POSITIONS_UPDATED);
            Injector.BindWithId(_finishedRenderingUpdatedFrame, ID.E_FINISHED_RENDERING_UPDATED_FRAME);
            Injector.BindWithId(_screenshotCapturedEvent, ID.E_SCREENSHOT_CAPTURED);
            Injector.BindWithId(_camCaptureCompleteEvent, ID.E_CAM_CAPTURE_COMPLETE);
            Injector.BindWithId(_captureCompleteEvent, ID.E_CAPTURE_COMPLETE);
            Injector.BindWithId(_frozenFrameCapturedEvent, ID.E_FROZEN_FRAME_CAPTURED);
            Injector.BindWithId(_highlightMomentCaptureCompleteEvent, ID.E_HIGHLIGHT_MOMENT_CAPTURE_COMPLETE);
            Injector.BindWithId(_highlightMomentDetected, ID.E_HIGHLIGHT_MOMENT_DETECTED);

            Injector.BindWithId(_prepareNewDataEvent_Main, ID.E_PREPARE_NEW_DATA);
            Injector.BindWithId(_startHighlight_Main, ID.E_START_HIGHLIGHT);
            Injector.BindWithId(_resumePlay_Main, ID.E_RESUME_PLAY);
            Injector.BindWithId(_captureEvent_Main, ID.E_CAPTURE);
            Injector.BindWithId(_prepareNextFrame_Main, ID.E_PREPARE_NEXT_FRAME);
            Injector.BindWithId(_newTrackingDataEntryEvent_Main, ID.E_NEW_TRACKING_DATA_ENTRY);
            Injector.BindWithId(_finishedAllEntriesEvent_Main, ID.E_FINISHED_ALL_ENTRIES);
            Injector.BindWithId(_startHighlightMoment_Main, ID.E_START_HIGHLIGHT_MOMENT);
            Injector.BindWithId(_captureFrozenMoment_Main, ID.E_CAPTURE_FROZEN_MOMENT);
            Injector.BindWithId(_frozenFrameCapturedEvent_Main, ID.E_FROZEN_FRAME_CAPTURED_MAIN);
            
            Injector.PostBindings();
        }
    }
}