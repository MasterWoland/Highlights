using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Aion.Highlights.Components;
using Aion.Highlights.Data;
using Aion.Highlights.Events;
using simpleDI.Injection.Allon;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Aion.Highlights.Managers
{
    public enum CaptureCamera
    {
        Center = 0,
        BirdsEye = 1,
        TowardsGoalLeft = 2,
        TowardsGoalRight = 3,
        BehindGoalLeft = 4,
        BehindGoalRight = 5
    }

    /// <summary>
    /// Base class for Capturing
    /// </summary>
    public abstract class CaptureManager : MonoBehaviour
    {
        [InjectID(Id = ID.E_CAPTURE)] protected BaseEvent _captureEvent_Main; // MRA: must be rerouted through AppManager
        [InjectID(Id = ID.E_PREPARE_NEW_DATA)] protected DataFileInfoEvent _prepareNewDataEvent_Main;
        [InjectID(Id = ID.E_CAPTURE_FROZEN_MOMENT)] protected IntEvent _captureFrozenMoment_Main;
        [InjectID(Id = ID.E_SCREENSHOT_CAPTURED)] protected BaseEvent _screenshotCapturedEventDispatcher; // capture of single screenshot
        [InjectID(Id = ID.E_CAPTURE_COMPLETE)] protected BaseEvent _captureCompleteEventDispatcher; // capture by all cameras
        [InjectID(Id = ID.E_FROZEN_FRAME_CAPTURED)] protected BaseEvent _frozenFrameCapturedEventDispatcher;
        [InjectID(Id = ID.E_HIGHLIGHT_MOMENT_CAPTURE_COMPLETE)] protected BaseEvent _highlightMomentCaptureCompleteEventDispatcher;
        [SerializeField] protected Camera[] _cameras;
        
        // Constants
        protected const string TEMP_DIRECTORY = @"capture-temp\";
        private const string FINISHED_DIRECTORY = @"capture-finish\";
        private const string RESOLUTION = "_1920_1080";
        private const string FRAME_RATE = "_25_"; // INFO: for now we use a framerate of 25, because we have a new timestamp every 40ms
        private const string EXTENSION = ".png";
        private const string FORMAT = "00000000"; // 8 digits

        protected string _pathBase;
        private string _path;
        protected string _directoryName;
        protected string _fileName;
        private string _camName;
        protected int _frameNumber = 0;
        [ShowInInspector, ReadOnly] protected CaptureCamera _currentCaptureCamera;
        private CancellationTokenSource _cancellationTokenSource;

        private void Awake()
        {
            Debug.Log("[ CM ] Awake() "+this.name);
            DIBinder.Injector.Inject(this);
            DIBinder.Injector.InjectID(this);

            _cancellationTokenSource = new CancellationTokenSource();

#if UNITY_EDITOR
            _pathBase = @"F:\Unity\AionSports\Captures\";
#else
            _pathBase = @"C:\HighLights\";
#endif
        }

        private async Task AsyncCapture()
        {
            _path = _pathBase + TEMP_DIRECTORY + _directoryName + _fileName + RESOLUTION + FRAME_RATE + _camName +
                    _frameNumber.ToString(FORMAT) + EXTENSION;

            ScreenCapture.CaptureScreenshot(_path);
            _frameNumber++;
            
            // MRA: we may want to add a CancelAfter(maxTime) for this 
            while (!File.Exists(_path))
            {
                await Task.Yield();
            }
        }

#region EVENTS
        protected virtual void OnEnable()
        {
            _prepareNewDataEvent_Main.Handler += OnNewData;
            _captureEvent_Main.Handler += OnScreenCapture;
            _captureFrozenMoment_Main.Handler += OnCaptureHighlightMoment;
        }

        protected virtual void OnDisable()
        {
            _cancellationTokenSource.Cancel(); // This will cancel any running tasks when the application stops

            _prepareNewDataEvent_Main.Handler -= OnNewData;
            _captureEvent_Main.Handler -= OnScreenCapture;
            _captureFrozenMoment_Main.Handler -= OnCaptureHighlightMoment;
        }

        protected abstract void OnNewData(DataFileInfo dataFileInfo);

        protected virtual async void OnScreenCapture()
        {
            _camName = "cam_" + _currentCaptureCamera + "_";
            
            Task task = AsyncCapture();
            await task;

            if (!task.IsCompletedSuccessfully)
                Debug.LogError("[CaptureManager] Task not completed successfully. Status: " + task.Status.ToString());

            _screenshotCapturedEventDispatcher?.Dispatch(); // Because we awaited the Task, we know the Task has been completed
        }
        
        protected virtual async void OnCaptureHighlightMoment(int amount)
        {
            _camName = "cam_" + _currentCaptureCamera + "_";
            
            for (int i = 0; i < amount; i++)
            {
                Task task = AsyncCapture();
                await task;

                if (!task.IsCompletedSuccessfully)
                    Debug.LogError("[CaptureManager] Task not completed successfully. Status: " + task.Status.ToString());

                // Debug.Log("[ CM ] capturing frozen moment: "+i);
                _frozenFrameCapturedEventDispatcher?.Dispatch();
            }

            _highlightMomentCaptureCompleteEventDispatcher?.Dispatch();
        }
#endregion

#region HELPER METHODS
        protected abstract void Reset();

        protected void CheckDirectory(string directoryPath)
        {
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    Debug.Log("That path exists already.");
                    return;
                }

                DirectoryInfo di = Directory.CreateDirectory(directoryPath);
                Debug.Log("The directory was created successfully at " + Directory.GetCreationTime(directoryPath));
            }
            catch (Exception e)
            {
                Debug.LogError("The process failed: " + e.ToString());
            }
        }

        protected void MoveFinishedScreenshots()
        {
            string sourcePath = _pathBase + TEMP_DIRECTORY + _directoryName;
            string finishedPath = _pathBase + FINISHED_DIRECTORY + _fileName + "_CAM_" + _currentCaptureCamera;
            Directory.Move(sourcePath, finishedPath);
        }
#endregion
    }
}