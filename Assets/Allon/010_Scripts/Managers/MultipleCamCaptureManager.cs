using Aion.Highlights.Data;
using Aion.Highlights.Events;
using simpleDI.Injection.Allon;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Aion.Highlights.Managers
{
    public class MultipleCamCaptureManager : CaptureManager
    {
        // We need these to capture with different cameras in a row ---------------------
        [InjectID(Id = ID.E_FINISHED_ALL_ENTRIES)] protected BaseEvent _finishedAllEntriesEvent_Main;
        [InjectID(Id = ID.E_CAM_CAPTURE_COMPLETE)] protected BaseEvent _camCaptureCompleteEventDispatcher; // capture by 1 camera
        [ShowInInspector, ReadOnly] private int _currentCamIndex; // INFO: we want to check this in the Inspector
        // ------------------------------------------------------------------------------
        
#region EVENTS
        protected override void OnEnable()
        {
            _finishedAllEntriesEvent_Main.Handler += OnFinishedAllEntries;
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            _finishedAllEntriesEvent_Main.Handler -= OnFinishedAllEntries;
            base.OnEnable();
        }

        protected override void OnNewData(DataFileInfo dataFileInfo)
        {
            // New DataFile: we set the file and directory name
            _fileName = dataFileInfo.FileName;
            _directoryName = dataFileInfo.FileName + "_CAM_" + _currentCaptureCamera + "\\";
            string path = _pathBase + TEMP_DIRECTORY + _directoryName;
            CheckDirectory(path);
        }

        // Check if we need to capture with a new camera or if all captures are finished
        private void OnFinishedAllEntries()
        {
            // Here we check if all cameras have finished capturing or only the previous one
            MoveFinishedScreenshots();

            // MRA: check if we need to capture from another camera or if all cameras have captured the current data
            if (_cameras.Length <= ++_currentCamIndex)
            {
                _captureCompleteEventDispatcher?.Dispatch();

                Reset();
            }
            else
            {
                // we need to capture the same file with a different camera
                _frameNumber = 0;
                // disable all cameras
                foreach (var cam in _cameras)
                {
                    cam.gameObject.SetActive(false);
                }

                _currentCaptureCamera = (CaptureCamera)_currentCamIndex;
                Debug.Log("[ MCM ] _currentCaptureCamera: "+_currentCaptureCamera);
                
                _cameras[_currentCamIndex].gameObject.SetActive(true);
                _camCaptureCompleteEventDispatcher?.Dispatch();
            }
        }
#endregion

#region HELPER METHODS
        protected override void Reset()
        {
            _currentCamIndex = 0;
            _frameNumber = 0;
            _currentCaptureCamera = (CaptureCamera)_currentCamIndex;

            // disable all cameras
            foreach (var cam in _cameras)
            {
                cam.gameObject.SetActive(false);
            }

            // enable first camera
            _cameras[_currentCamIndex].gameObject.SetActive(true);
        }
#endregion
    }
}