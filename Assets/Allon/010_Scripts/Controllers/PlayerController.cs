using System.Collections.Generic;
using Aion.Highlights.Components.Player;
using Aion.Highlights.Data.Stream;
using Aion.Highlights.Utils;
using Aion.Highlights.Views;
using UnityEngine;

namespace Aion.Highlights.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        private const string SPEED_ANIMATION_VAR = "Speed";
        private static readonly int SPEED = Animator.StringToHash(SPEED_ANIMATION_VAR);

        [SerializeField] private Animator _animator;
        [SerializeField] private HighlightObject _playerHighlightObject; // contains data about the player
        [SerializeField] private PlayerAppearance _appearance;
        [SerializeField] private PlayerInfoView _infoView;
        [SerializeField] private HeadLookController _headLookController;

        // Movement and rotation
        private Transform _transform;
        private Vector3 _ballPosition = Vector3.zero;
        private const int QUEUE_SIZE = 15; // MRA: eventueel 25 van maken --> 1 seconde
        private Queue<Vector3> _positions = new Queue<Vector3>(QUEUE_SIZE);
        private Vector3 _velocity;
        // [SerializeField] private float _averageSpeed; // MRA: exposed for testing purposes
        [SerializeField] private List<float> _speedList = new List<float>(); // MRA: exposed for testing purposes
        [SerializeField] private List<float> _angleList = new List<float>(); // MRA: exposed for testing purposes
        private Dictionary<int, float> _angleDictionary = new Dictionary<int, float>();
        [SerializeField] private List<Vector3> _normalizedVelocityList = new List<Vector3>(); // MRA: exposed for testing purposes
        // we need a player model object, containing Name, Team, Shirt Number, Role etc.

        [HideInInspector] public float DistanceToCamera;

        private void Awake()
        {
            _transform = this.transform;
        }

        public void ToggleAnimation(bool doAnimate)
        {
            _animator.enabled = doAnimate;
        }

        public void AssignPlayerHighlightObject(HighlightObject playerHighlightObject)
        {
            _playerHighlightObject = playerHighlightObject;
            _appearance.SetAppearance((int) _playerHighlightObject.Type);
            _infoView.SetInfo(_playerHighlightObject);
            _infoView.Hide();
            gameObject.name = _playerHighlightObject.ShirtNumber.ToString("00") + "__" + _playerHighlightObject.LastName;
        }

        public void PrepareForHighlightMoment(HighlightMomentPlayer momentPlayer)
        {
            _infoView.gameObject.SetActive(true);
            _infoView.PrepareForHighlightMoment(momentPlayer);
        }

        public void ArrangeInfoView(int multiplier)
        {
            // the farther away a player is, the higher the InfoView must be

            _infoView.SetHeight(multiplier);
        }

        public void FinishHighlightMoment()
        {
            if (!_infoView.gameObject.activeSelf) return;

            _infoView.ResetHeight();
            _infoView.Hide();
            _infoView.gameObject.SetActive(false);
        }

        public void SetBallPosition(Vector3 pos)
        {
            _ballPosition = pos;
        }

#region POSITION & ROTATION
        public void UpdatePosition(Vector3 position, bool isBall)
        {
            MoveToPosition(position); // Called Move() in Pro
            UpdateVelocity();

            SetHeadLookTarget();

            if (isBall) UpdateBodyRotation(); // only if there is a ball

            // if (_playerHighlightObject.LastName.Contains("Sotoca"))
            // {
            //     Debug.Log("[ PC ] Sotoca > is moving towards ball? : " + IsMovingTowardsBall());
            // }
        }

        private void MoveToPosition(Vector3 position)
        {
            if (_positions.Count >= QUEUE_SIZE)
            {
                _positions.Dequeue();
            }

            _positions.Enqueue(position);

            _transform.position = position;
        }

        private void UpdateVelocity()
        {
            if (_positions.Count < QUEUE_SIZE)
            {
                _velocity = Vector3.zero;
                return;
            }

            ClearLists();

            Vector3[] positionsArray = _positions.ToArray(); // OPT: This causes a memory leak. We may want a better way.
            float timeBetweenPositions = 1f / AionConstants.FPS;

            for (int i = 0; i < QUEUE_SIZE - 1; i++)
            {
                Vector3 velocityBetweenPositions = (positionsArray[i + 1] - positionsArray[i]) / timeBetweenPositions;
                _speedList.Add(velocityBetweenPositions.magnitude);

                if (i == 0) continue;
                // MRA: angle is always 1.570796 radians !!!!!!!!!!!!!!!!!!!!! ==> which is 90 degrees
                // MRA: are we using the correct vectors? Which angle should we calculate here????
                float angle = Mathf.Acos(Vector3.Dot(Vector3.zero, velocityBetweenPositions.normalized));
                // Debug.Log("[ PC ] VBPosition.norm = "+velocityBetweenPositions.normalized);
                // Debug.Log("[ PC ] angle = " + angle);
                _angleDictionary.Add(i - 1, angle);
                _angleList.Add(angle);
                _normalizedVelocityList.Add(velocityBetweenPositions.normalized);
            }

            _velocity = GetAverageSpeed() * GetAverageDirection(); //_averageDirection;

            // if (_playerHighlightObject.LastName.Contains("Sotoca"))
            // {
            //     Debug.Log("[ PC ] Sotoca > velocity : " + _velocity);
            // }
        }

        // MRA: later!
        private void SetHeadLookTarget()
        {
            _headLookController.target = _ballPosition;
        }

        private void UpdateBodyRotation()
        {
            // _transform.LookAt(_ballPosition);

            // Rename BallDirection
            Vector3 ballDirection = (new Vector3(_ballPosition.x, 0f, _ballPosition.z) -
                                     new Vector3(_transform.localPosition.x, 0f, _transform.localPosition.z)).normalized;
            Vector3 playerDirection = _velocity.normalized;
            Vector3 currentDirection = _transform.localRotation * Vector3.forward;

            // MRA: do this differently --> test when we have a keeper !!!!!
            UpdateBodyRotationKeeper(playerDirection);

            if (IsPlayerIdle())
            {
                UpdateBodyRotationIdlePlayer();
                // MRA: What if goalkeeper is idle? 
                // In PRO we use idleRotation, which checks the distance of the Idle Player to the ball.
                // Below and above this threshold, the rotation is fictional, so we might as well apply the
                // current playerDirection
                _transform.localRotation = Quaternion.LookRotation(playerDirection);
            }

            if (playerDirection != Vector3.zero)
            {
                _transform.localRotation = Quaternion.LookRotation(playerDirection);
            }
        }

        private void UpdateBodyRotationIdlePlayer()
        {
            // Debug.Log("[ PC ] "+_playerHighlightObject.LastName+" --> is idle");
        }

        private void UpdateBodyRotationKeeper(Vector3 playerDirection)
        {
            // We don't know yet what to use to identify a keeper, because the data didn't feature one
            if (_playerHighlightObject.Type == HighlightObjectType.GoalkeeperAway ||
                _playerHighlightObject.Type == HighlightObjectType.GoalkeeperHome ||
                _playerHighlightObject.PositionID == PositionID.Goalkeeper)
            {
                playerDirection = (IsMovingTowardsBall()) ? playerDirection : -playerDirection;

                // Debug.Log("[ PC ] " + _playerHighlightObject.LastName + " --> is GOAL KEEPER");
            }

            if (playerDirection != Vector3.zero)
            {
                _transform.localRotation = Quaternion.LookRotation(playerDirection);
            }
        }

        public void UpdateAnimator()
        {
            float speed = GetAverageSpeed();

            // MRA: copied from PRO > SoccerPlayerBehaviour. Perhaps check for keeper moving away from ball (* -1f)
            _animator.SetFloat(SPEED, speed);
            if (speed > 6)
            {
                _animator.speed = 1.5f;
            }
            else if (speed < 1)
            {
                _animator.speed = 0.75f;
            }
            else
            {
                _animator.speed = 1.0f;
            }
        }
#endregion

#region HELPER METHODS
        private void ClearLists()
        {
            _speedList.Clear();
            _angleDictionary.Clear();
            _angleList.Clear();
            _normalizedVelocityList.Clear();
        }

        private float GetAverageSpeed()
        {
            float averageSpeed = 0f;
            int iterations = _speedList.Count - 1;

            for (int i = 0; i < iterations; i++)
            {
                averageSpeed += _speedList[i];
            }

            averageSpeed /= iterations;
            // Debug.Log("[  ] Average Speed = "+averageSpeed);
            return averageSpeed;
        }

        private Vector3 GetAverageDirection()
        {
            Vector3 averageDirection = Vector3.zero;

            foreach (var velocity in _normalizedVelocityList)
            {
                averageDirection += velocity;
            }

            return averageDirection.normalized;
        }

        private bool IsMovingTowardsBall()
        {
            Vector3 distance = _ballPosition - _transform.position; //_transform.localPosition;
            distance.y = _transform.position.y; // _transform.localPosition.y;
            distance.Normalize();

            return Vector3.Dot(distance, _velocity.normalized) > 0f;
        }

        private bool IsPlayerIdle()
        {
            // MRA: check if we can use a constant here!!!!
            return _velocity.sqrMagnitude < Mathf.Pow(0.45f, 2f); // 20cm check so small movements still count as idle.
        }

        // MRA: if we don't want a speed list, we can use this method
        // private float ObtainAverageSpeed()
        // {
        //     Vector3[] posArray = _positions.ToArray(); // We need an index, Queues do not provide them
        //     int loopLength = posArray.Length - 1;
        //     float averageSpeed = 0f;
        //
        //     for (int i = 0; i < loopLength; i++)
        //     {
        //         Vector3 velocityBetweenPositions = posArray[i + 1] - posArray[i];
        //         averageSpeed += velocityBetweenPositions.magnitude;
        //     }
        //
        //     averageSpeed /= loopLength;
        //     // averageSpeed /= AionConstants.TEMP_REPEAT_RATE;
        //
        //     return averageSpeed;
        // }
#endregion
    }
}