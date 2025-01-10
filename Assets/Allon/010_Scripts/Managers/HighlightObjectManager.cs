using System.Collections.Generic;
using System.Linq;
using Aion.Highlights.Components;
using Aion.Highlights.Controllers;
using Aion.Highlights.Data;
using Aion.Highlights.Data.Stream;
using Aion.Highlights.Events;
using Aion.Highlights.Utils;
using simpleDI.Injection;
using simpleDI.Injection.Allon;
using UnityEngine;
using UnityEngine.Rendering;

namespace Aion.Highlights.Managers
{
    public class HighlightObjectManager : MonoBehaviour
    {
        // Inspector
        [SerializeField] private GameObject _ballPrefab;
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private Transform _highlightObjectHolder;

        // Events
        [InjectID(Id = ID.E_NEW_TRACKING_DATA_ENTRY)] private BaseEvent _newTrackingDataEntryEvent_Main;
        [InjectID(Id = ID.E_PREPARE_NEW_DATA)] private DataFileInfoEvent _prepareNewDataEvent_Main;
        [InjectID(Id = ID.E_START_HIGHLIGHT_MOMENT)] private HighlightMomentEvent _startHighlightMoment_Main;
        [InjectID(Id = ID.E_RESUME_PLAY)] private BaseEvent _resumePlay_Main;
        [InjectID(Id = ID.E_FINISHED_RENDERING_UPDATED_FRAME)] private BaseEvent _finishedRenderingUpdatedFrameDispatcher;

        // Data
        [Inject] private TrackingDataPerEntry _currentTrackingDataPerEntry;
        private Dictionary<string, PlayerController> _playersByKey = new Dictionary<string, PlayerController>();

        // Misc
        private BallController _ballController;
        private bool _positionsAreUpdated = false;
        private bool _isBall = false;

        // TEMP
        [SerializeField] private List<PlayerController> _momentPlayers = new List<PlayerController>(); // exposed for testing
        
        private void Awake()
        {
            DIBinder.Injector.Inject(this);
            DIBinder.Injector.InjectID(this);
        }

        private void InitBall()
        {
            if (!_currentTrackingDataPerEntry.HighlightObjectsByID.ContainsKey(AionConstants.BALL_ID)) return;

            _ballController = Instantiate(_ballPrefab, _highlightObjectHolder).GetComponent<BallController>();
            _isBall = true;
        }

#region EVENTS
        private void OnEnable()
        {
            _newTrackingDataEntryEvent_Main.Handler += OnNewTrackingDataEntry;
            _prepareNewDataEvent_Main.Handler += OnPrepareNewData;
            _startHighlightMoment_Main.Handler += OnStartHighlightMoment;
            _resumePlay_Main.Handler += OnResumePlay;
            RenderPipelineManager.endContextRendering += OnEndContextRendering;
        }

        private void OnDisable()
        {
            _newTrackingDataEntryEvent_Main.Handler -= OnNewTrackingDataEntry;
            _prepareNewDataEvent_Main.Handler -= OnPrepareNewData;
            _startHighlightMoment_Main.Handler -= OnStartHighlightMoment;
            _resumePlay_Main.Handler -= OnResumePlay;
            RenderPipelineManager.endContextRendering += OnEndContextRendering;
        }

        private void OnResumePlay()
        {
            foreach (var kvp in _playersByKey)
            {
                kvp.Value.FinishHighlightMoment();
                kvp.Value.ToggleAnimation(true);
            }
        }

        private void OnStartHighlightMoment(HighlightMoment highlightMoment)
        {
            _momentPlayers.Clear();
            TogglePlayerAnimation(false);

            // MRA: the problem here is that due to faulty data, the necessary players may not be available
            foreach (var player in highlightMoment.HighlightMomentPlayers)
            {
                // Debug.Log("[ MoM ] MOMENT player: " + player.LastName + ", ID = " + player.PlayerID);

                if (_playersByKey.ContainsKey(player.PlayerID))
                {
                    _playersByKey[player.PlayerID].PrepareForHighlightMoment(player);
                    _momentPlayers.Add(_playersByKey[player.PlayerID]);
                }
            }

            // MRA: we need to order this list by distance to the camera
            Vector3 camPos = Camera.main.transform.position;
            
            foreach (var player in _momentPlayers)
            {
                player.DistanceToCamera = (camPos - player.transform.position).magnitude;
                // Debug.Log("[ "+player.name+" ] distance to cam = "+player.DistanceToCamera);
            }          
            
            _momentPlayers = _momentPlayers.OrderBy(player => player.DistanceToCamera).ToList();
            // _momentPlayers = _momentPlayers.OrderBy(player => player.transform.position.z).ToList(); // MRA: previous solution
            
            for (int i = 0; i < _momentPlayers.Count; i++)
            {
                _momentPlayers[i].ArrangeInfoView(i);
            }
        }

        private void OnNewTrackingDataEntry()
        {
            // we update the positions of the existing match objects
            // we instantiate the match objects that are new
            // we destroy the match objects that are no longer used

            DestroyRemovedPlayerControllers();

            bool isBallCurrently = false;

            foreach (var keyValuePair in _currentTrackingDataPerEntry.HighlightObjectsByID)
            {
                // Ball
                if (keyValuePair.Value.ObjectID == AionConstants.BALL_ID)
                {
                    TryUpdateBall(keyValuePair);
                    isBallCurrently = true;
                    continue;
                }

                // We instantiate a player if we haven't already got him
                if (!_playersByKey.ContainsKey(keyValuePair.Key))
                {
                    InitPlayer(keyValuePair);
                }

                // Update PlayerController
                if (_playersByKey.TryGetValue(keyValuePair.Key, out var playerController))
                {
                    UpdatePlayer(keyValuePair, playerController);
                }
            }

            // We need to remove the ball if it has disappeared
            if (!isBallCurrently && _isBall) RemoveBall();

            _positionsAreUpdated = true;
        }

        private void RemoveBall()
        {
            _isBall = false;
            Destroy(_ballController.gameObject);
        }

        private void OnPrepareNewData(DataFileInfo dataFileInfo)
        {
            Reset();
        }

        // If we have applied the new positions and the frame has finished rendering, we want to
        // pause the updating and start capturing the frame
        private void OnEndContextRendering(ScriptableRenderContext arg1, List<Camera> arg2)
        {
            if (!_positionsAreUpdated) return;

            _finishedRenderingUpdatedFrameDispatcher?.Dispatch();
            _positionsAreUpdated = false;
        }
#endregion

#region HELPER METHODS
        private void TryUpdateBall(KeyValuePair<string, HighlightObject> keyValuePair)
        {
            if (!_isBall) InitBall();

            Vector3 pos = new Vector3(keyValuePair.Value.PositionX - AionConstants.HALF_PITCH_LENGTH,
                0,
                keyValuePair.Value.PositionZ - AionConstants.HALF_PITCH_WIDTH);
            _ballController.UpdatePosition(pos);
        }

        private void DestroyRemovedPlayerControllers()
        {
            for (int i = 0; i < _playersByKey.Count; i++)
            {
                string key = _playersByKey.ElementAt(i).Key;
                if (_currentTrackingDataPerEntry.HighlightObjectsByID.ContainsKey(key)) continue;

                // This player has been removed
                if (!_playersByKey.TryGetValue(key, out var playerController)) continue;

                Destroy(playerController.gameObject);
                _playersByKey.Remove(key);
            }
        }

        private void InitPlayer(KeyValuePair<string, HighlightObject> keyValuePair)
        {
            Transform player = Instantiate(_playerPrefab, _highlightObjectHolder).transform;
            PlayerController controller = player.GetComponent<PlayerController>();
            controller.AssignPlayerHighlightObject(keyValuePair.Value as HighlightObject);
            _playersByKey.Add(keyValuePair.Key, controller);
        }

        private void UpdatePlayer(KeyValuePair<string, HighlightObject> keyValuePair, PlayerController playerController)
        {
            float posX = keyValuePair.Value.PositionX - AionConstants.HALF_PITCH_LENGTH;
            float posZ = keyValuePair.Value.PositionZ - AionConstants.HALF_PITCH_WIDTH;
            Vector3 pos = new Vector3(posX, 0, posZ);
            
            if (_isBall) playerController.SetBallPosition(_ballController.transform.position);
            playerController.UpdatePosition(pos, _isBall);
            playerController.UpdateAnimator();
        }

        private void TogglePlayerAnimation(bool doAnimate)
        {
            for (int i = 0; i < _playersByKey.Count; i++)
            {
                string key = _playersByKey.ElementAt(i).Key;
                _playersByKey.TryGetValue(key, out var playerController);
                playerController.ToggleAnimation(doAnimate);
            }
        }

        private void Reset()
        {
            _isBall = false;
            _positionsAreUpdated = false;
            _playersByKey.Clear();
            _playersByKey = new Dictionary<string, PlayerController>();

            foreach (Transform child in _highlightObjectHolder.transform)
            {
                Destroy(child.gameObject);
            }
        }
#endregion
    }
}