using Aion.Highlights.Data;
using Aion.Highlights.Events;
using simpleDI.Injection;
using simpleDI.Injection.Allon;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Aion.Highlights.Managers
{
    public class SingleCamCaptureManager : CaptureManager
    {
        // We need these to find out which camera should be used to capture -------------
        [Inject] [ShowInInspector, ReadOnly] private HighlightInfo_SO _highlightInfo_SO; // needed to obtain correct camera
        [InjectID(Id = ID.E_START_HIGHLIGHT)] private BaseEvent _startHighlight_Main;
        [InjectID(Id = ID.E_FINISHED_ALL_ENTRIES)] protected BaseEvent _finishedAllEntriesEvent_Main;
        private DataFileInfo _dataFileInfo;
        // ------------------------------------------------------------------------------

#region EVENTS
        protected override void OnEnable()
        {
            _startHighlight_Main.Handler += OnStartHighlight;
            _finishedAllEntriesEvent_Main.Handler += OnFinishedAllEntries;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            _startHighlight_Main.Handler -= OnStartHighlight;
            _finishedAllEntriesEvent_Main.Handler -= OnFinishedAllEntries;
            base.OnDisable();
        }

        private void OnStartHighlight()
        {
            // On this event we have the current info in _highlight_SO
            _currentCaptureCamera = _highlightInfo_SO.HighlightInfo.CaptureCamera;
            // disable all cameras
            foreach (var cam in _cameras)
            {
                cam.gameObject.SetActive(false);
            }

            int index = (int) _currentCaptureCamera;
            _cameras[index].gameObject.SetActive(true);

            _fileName = _dataFileInfo.FileName;
            _directoryName = _dataFileInfo.FileName + "_CAM_" + _currentCaptureCamera + "\\";
            string path = _pathBase + TEMP_DIRECTORY + _directoryName;
            CheckDirectory(path);
        }

        private void OnFinishedAllEntries()
        {
            Reset(); // Must happen before dispatching the event
            MoveFinishedScreenshots();
            _captureCompleteEventDispatcher?.Dispatch();
        }

        protected override void OnNewData(DataFileInfo dataFileInfo)
        {
            // MRA: on this event we don't know yet what the targeted Capture Camera is
            _dataFileInfo = dataFileInfo; // new DataFileInfo(dataFileInfo.FilePath, dataFileInfo.FileName);
        }
#endregion

#region HELPER METHODS
        protected override void Reset()
        {
            _dataFileInfo = null;
            _frameNumber = 0;
        }
#endregion
    }
}