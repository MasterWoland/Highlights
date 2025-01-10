using System;
using UnityEngine;
using UnityEngine.UI;

namespace Aion.Highlights.Managers.Simulation
{
	public class UICameraManager : MonoBehaviour
	{
		[SerializeField] private GameObject[] _cameras;
		[SerializeField] private Slider _camSlider;
		private int _camIndex = 0;

		private void Awake()
		{
			_camSlider.wholeNumbers = true;
		}

		private void Start()
		{
			SetActiveCamera();
		}

		private void SetActiveCamera()
		{
			for (int i = 0; i < _cameras.Length; i++)
			{
				_cameras[i].SetActive(i == _camIndex);
			}
		}

#region EVENTS
		private void OnEnable()
		{
			_camSlider.onValueChanged.AddListener(OnSliderValueChanged);
		}

		private void OnDisable()
		{
			_camSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
		}

		private void OnSliderValueChanged(float newIndex)
		{
			_camIndex = (int)newIndex;

			SetActiveCamera();
		}
#endregion
	}
}