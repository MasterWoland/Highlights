using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Aion.Highlights.Components;
using Aion.Highlights.Data;
using Aion.Highlights.Data.Stream;
using Aion.Highlights.Events;
using Newtonsoft.Json;
using simpleDI.Injection;
using simpleDI.Injection.Allon;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Aion.Highlights.Managers
{
    /// <summary>
    /// Reads and assigns the incoming data.
    /// Manages the updates and data flow.
    /// </summary>
    public class DataManager : MonoBehaviour
    {
        // Events
        [InjectID(Id = ID.E_PREPARE_NEW_DATA)] private DataFileInfoEvent _prepareNewDataEvent_Main;
        [InjectID(Id = ID.E_START_HIGHLIGHT)] private BaseEvent _startHighlight_Main;
        [InjectID(Id = ID.E_PREPARE_NEXT_FRAME)] private BaseEvent _prepareNextFrame_Main;
        
        // Event Dispatchers
        [InjectID(Id = ID.E_DATA_PREPARED)] private BaseEvent _dataPreparedEventDispatcher;
        [InjectID(Id = ID.E_POSITIONS_UPDATED)] private BaseEvent _positionsUpdatedEventDispatcher;
        [InjectID(Id = ID.E_OUT_OF_ENTRIES)] private BaseEvent _outOfEntriesEventDispatcher;
      
        // Data
        [Inject] [ShowInInspector, ReadOnly]  private AllEntries _allEntries;
        [Inject] [ShowInInspector, ReadOnly]  private TrackingDataPerEntry _currentTrackingDataPerEntry; 
        [Inject] [ShowInInspector, ReadOnly] private HighlightInfo_SO _highlightInfo_SO;
        
        private string _jsonAsString;
        [ShowInInspector, ReadOnly] private int _frameIndex; // INFO: exposed for testing purposes
        private DataFileInfo _currentDataFileInfo; // the data that is currently being processed
        [ShowInInspector, ReadOnly] private HighlightInfo _highlightInfo;
        private Dictionary<string, HighlightObject>[] _allTrackingData;// We only need this one, the injected SO is only for Inspector viewing
        private CancellationTokenSource _cancellationTokenSource;
        
        private void Awake()
        {
            DIBinder.Injector.Inject(this);
            DIBinder.Injector.InjectID(this);

            _cancellationTokenSource = new CancellationTokenSource();
        }

        private async Task<string> ReadJsonAsync(CancellationToken token)
        {
            Task<string> task = File.ReadAllTextAsync(_currentDataFileInfo.FilePath, token);
            await task;

            return task.Result;
        }

#region EVENTS
        private void OnEnable()
        {
            _prepareNewDataEvent_Main.Handler += OnPrepareNewData;
            _startHighlight_Main.Handler += UpdatePositions;
            _prepareNextFrame_Main.Handler += UpdatePositions;
        }

        private void OnDisable()
        {
            _cancellationTokenSource.Cancel();// This will cancel any running tasks when the application stops
            
            _prepareNewDataEvent_Main.Handler -= OnPrepareNewData;
            _startHighlight_Main.Handler -= UpdatePositions;
            _prepareNextFrame_Main.Handler -= UpdatePositions;
        }

        private async void OnPrepareNewData(DataFileInfo info)
        {
            if (_currentDataFileInfo != info)
            {
                // new file
                Reset();
                
                _currentDataFileInfo = info;
                var cancellationToken = _cancellationTokenSource.Token;
                Task<string> readJsonTask = ReadJsonAsync(cancellationToken);
                await readJsonTask;
                
                _jsonAsString = readJsonTask.Result;
                _highlightInfo = JsonConvert.DeserializeObject<HighlightInfo>(_jsonAsString);

                _highlightInfo_SO.HighlightInfo = _highlightInfo;
                
                ObtainHighlightData(); // fill the dictionaries
            }

            _dataPreparedEventDispatcher?.Dispatch();
        }

        private void UpdatePositions()
        {
            if (_allTrackingData.Length > _frameIndex)
            {
                _currentTrackingDataPerEntry.HighlightObjectsByID = _allTrackingData[_frameIndex];
                
                // TESTING
                _currentTrackingDataPerEntry.CurrentIndex = _frameIndex;
                // ------
                
                _positionsUpdatedEventDispatcher?.Dispatch();
                _frameIndex++;
            }
            else
            {
                Debug.Log("[ DM ] UpdatePos() We ran out of entries: " + _allTrackingData.Length + ", frame index = " + _frameIndex);
                _frameIndex = 0;
                _outOfEntriesEventDispatcher?.Dispatch();
            }
        }
#endregion

#region HELPER METHODS
        private void Reset()
        {
            // DataManager resets for each new file 
            _frameIndex = 0;
            _jsonAsString = string.Empty;
            
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }
        
        private void ObtainHighlightData()
        {
            _allTrackingData = new Dictionary<string, HighlightObject>[_highlightInfo.HighlightObjectsByIDArray.Length];
            _allTrackingData = _highlightInfo.HighlightObjectsByIDArray;
            
            _currentTrackingDataPerEntry.HighlightObjectsByID = _allTrackingData[_frameIndex];
            
            // TESTING
            _currentTrackingDataPerEntry.CurrentIndex = _frameIndex;
            _currentTrackingDataPerEntry.TotalAmount = _highlightInfo.HighlightObjectsByIDArray.Length;
            // -----
            
            Debug.Log("[ DM ] _allTrackingData Length: " + _allTrackingData.Length);

            // only for checking values in the inspector
            int numEntries = _highlightInfo.HighlightObjectsByIDArray.Length;
            _allEntries.HighlightObjectsByIDArray = new Dictionary<string, HighlightObject>[numEntries];
            _allEntries.HighlightObjectsByIDArray = _highlightInfo.HighlightObjectsByIDArray;
        }
#endregion
    }
}