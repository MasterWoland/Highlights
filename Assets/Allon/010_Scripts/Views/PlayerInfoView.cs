using System;
using Aion.Highlights.Data.Stream;
using Aion.Highlights.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Aion.Highlights.Views
{
    public class PlayerInfoView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _shirtNumberTMP;
        [SerializeField] private TextMeshProUGUI _lastNameTMP;
        [SerializeField] private TextMeshProUGUI _pressureLevelTMP;
        [SerializeField] private Image _pressureImage;
        [SerializeField] private SpriteRenderer _circle;
        [SerializeField] private SpriteRenderer _cone;
        [SerializeField] private Color _lowColor;
        [SerializeField] private Color _medColor;
        [SerializeField] private Color _highColor;
        [SerializeField] private Color _mainActorColor;
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Transform _coneHolderTransform;
        private Vector3 _defaultPosInfoView;

        private void Awake()
        {
            _defaultPosInfoView = transform.position;
        }

#region PUBLIC
        public void SetInfo(HighlightObject highlightObject)
        {
            _shirtNumberTMP.text = highlightObject.ShirtNumber.ToString();
            _lastNameTMP.text = highlightObject.LastName;
            _pressureImage.gameObject.SetActive(false);
        }

        public void Hide()
        {
            _circle.gameObject.SetActive(false);
            _cone.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        public void PrepareForHighlightMoment(HighlightMomentPlayer momentPlayer)
        {
            RotateTowardsCamera();

            _circle.gameObject.SetActive(true);

            if (momentPlayer.IsMainActor)
            {
                SetMainActor(momentPlayer);
            }
            else
            {
                _cone.gameObject.SetActive(true);
                SetPressure(momentPlayer);
                SetCone(momentPlayer);

                Debug.Log("[ "+name+" ] pressure level: "+momentPlayer.PressureLevel.ToString());
            }
        }

        public void SetHeight(int multiplier)
        {
            transform.Translate(0, multiplier * 1.2f, 0); // MRA: magic number alert
        }

        public void ResetHeight()
        {
            transform.position = _defaultPosInfoView;
        }
#endregion

#region HELPER METHODS
        private void RotateTowardsCamera()
        {
            if (!Camera.main) Debug.LogError("[ PlayerInfoView ] No Main Camera found");

            transform.LookAt(Camera.main.transform.position);
        }

        private void SetMainActor(HighlightMomentPlayer momentPlayer)
        {
            _circle.color = _mainActorColor;
        }
        private void SetCone(HighlightMomentPlayer momentPlayer)
        {
            // MRA: note: pitch width and length kunnen we uit de data halen
            Vector3 mainActorPos = new Vector3(momentPlayer.MainActorPositionX - AionConstants.HALF_PITCH_LENGTH,
                0,
                momentPlayer.MainActorPositionZ - AionConstants.HALF_PITCH_WIDTH);
  
            float distanceV3 = Vector3.Distance(mainActorPos, _playerTransform.position);
            distanceV3 *= 0.8f; // Make it a little smaller

            var coneTransform = _cone.transform;
            var localScale = coneTransform.localScale;
            Vector3 scale = new Vector3(localScale.x, distanceV3, localScale.z);
            coneTransform.localScale = scale;

            _coneHolderTransform.LookAt(mainActorPos);
        }

        private void SetPressure(HighlightMomentPlayer momentPlayer)
        {
            _pressureImage.gameObject.SetActive(true);
            _pressureLevelTMP.text = momentPlayer.PressureLevel.ToString();
            
            switch (momentPlayer.PressureLevel)
            {
                case PressureLevel.Low:
                    _pressureImage.color = _circle.color = _cone.color = _lowColor;
                    break;
                case PressureLevel.Med:
                    _pressureImage.color = _circle.color = _cone.color = _medColor;
                    break;
                case PressureLevel.High:
                    _pressureImage.color = _circle.color = _cone.color = _highColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
#endregion
    }
}