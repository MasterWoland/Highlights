using System.Collections.Generic;
using Aion.Highlights.Components;
using Aion.Highlights.Data;
using Aion.Highlights.Data.Stream;
using Aion.Highlights.Events;
using simpleDI.Injection;
using simpleDI.Injection.Allon;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Aion.Highlights.Managers
{
    public class HighlightMomentManager : MonoBehaviour
    {
        // Events
        [InjectID(Id = ID.E_NEW_TRACKING_DATA_ENTRY)] private BaseEvent _newTrackingDataEntryEvent_Main;
        [InjectID(Id = ID.E_START_HIGHLIGHT)] private BaseEvent _startHighlight_Main;
        [InjectID(Id = ID.E_PREPARE_NEW_DATA)] private DataFileInfoEvent _prepareNewDataEvent_Main;
        [InjectID(Id = ID.E_HIGHLIGHT_MOMENT_DETECTED)] private HighlightMomentEvent _highlightMomentDetected_Dispatcher;

        // Data
        [Inject] [ShowInInspector, ReadOnly] private HighlightInfo_SO _highlightInfo_SO;

        [ShowInInspector, ReadOnly] private List<int> _pressureEventIndices = new List<int>();
        [ShowInInspector, ReadOnly] private int _curFrameIndex;

        private void Awake()
        {
            DIBinder.Injector.Inject(this);
            DIBinder.Injector.InjectID(this);
        }

#region EVENTS
        private void OnEnable()
        {
            _newTrackingDataEntryEvent_Main.Handler += OnNewTrackingDataEntry;
            _startHighlight_Main.Handler += OnStartHighlight;
            _prepareNewDataEvent_Main.Handler += OnPrepareForNewData;
        }

        private void OnDisable()
        {
            _newTrackingDataEntryEvent_Main.Handler -= OnNewTrackingDataEntry;
            _startHighlight_Main.Handler -= OnStartHighlight;
            _prepareNewDataEvent_Main.Handler -= OnPrepareForNewData;
        }

        private void OnNewTrackingDataEntry()
        {
            // positions are updated
            _curFrameIndex++;
            
            for (int i = 0; i < _pressureEventIndices.Count; i++)
            {
                if (_curFrameIndex != _pressureEventIndices[i]) continue;

                // Debug.Log("[ HMM ] We have a High Pressure Event at: " + _curFrameIndex);

                // obtain corresponding HighlightMoment
                HighlightMoment moment = null;
                moment = GetHighlightMoment();

                if (moment != null) _highlightMomentDetected_Dispatcher?.Dispatch(moment);
            }
        }

        private void OnStartHighlight()
        {
            // new HighlightInfo data has been deserialized and is available
            _pressureEventIndices.Clear();

            foreach (var highlightEvent in _highlightInfo_SO.HighlightInfo.HighlightMoments)
            {
                if (highlightEvent.HighlightMomentType == HighlightMomentType.HighPressure)
                {
                    _pressureEventIndices.Add(highlightEvent.StartFrameIndex);
                }
            }
        }

        private void OnPrepareForNewData(DataFileInfo dataFileInfo)
        {
            _curFrameIndex = 0;
        }
#endregion

#region HELPER METHODS
        private HighlightMoment GetHighlightMoment()
        {
            for (int j = 0; j < _highlightInfo_SO.HighlightInfo.HighlightMoments.Length; j++)
            {
                if (_curFrameIndex != _highlightInfo_SO.HighlightInfo.HighlightMoments[j].StartFrameIndex) continue;

                HighlightMoment moment = _highlightInfo_SO.HighlightInfo.HighlightMoments[j];
                return moment;
            }

            return null;
        }
#endregion
    }
}