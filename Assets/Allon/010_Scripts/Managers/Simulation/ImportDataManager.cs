using Aion.Highlights.Components;
using Aion.Highlights.Data;
using Aion.Highlights.Events;
using Crosstales.FB;
using simpleDI.Injection.Allon;
using UnityEngine;
using UnityEngine.UI;

namespace Aion.Highlights.Managers.Simulation.Old
{
    public class ImportDataManager : MonoBehaviour
    {
        [SerializeField] private Button _dataButton;

        // [InjectID(Id = ID.E_NEW_DATA_PATH)] private StringEvent _newDataPathEventDispatcher;
        
        private void Awake()
        {
            DIBinder.Injector.InjectID(this);
        }
        
        private void OnEnable()
        {
            _dataButton.onClick.AddListener(OnDataButtonPressed);
        }
        private void OnDisable()
        {
            _dataButton.onClick.RemoveListener(OnDataButtonPressed);
        }

        private void OnDataButtonPressed()
        {
            // Debug.Log("[ ] Hello");
            
            string[] extensions = { "dat", "csv" };
            string path = FileBrowser.Instance.OpenSingleFile("Open file", "", "", extensions);
            // Debug.Log("[ ] path: " + path);

            // TextAsset asset = new TextAsset(File.ReadAllText(path));
            // Debug.Log("[ ] asset: " + asset);
            
            // _newDataPathEventDispatcher?.Dispatch(path);
            
            // MatchMomentData matchMomentData = JsonUtility.FromJson<MatchMomentData>(asset.text);
            // matchMomentData.MatchPlayerData = GetMatchPlayerData(matchMomentData);
            // The multiplier may be different from different sources, therefore we check if one is provided

            // Debug.Log("[ ImportM ] ball pos = "+matchMomentData.BallStartPosition);

            // if (matchMomentData.Multiplier != 0)
            // {
            //     _multiplier = matchMomentData.Multiplier;
            //     // Debug.Log("[ Import M ] multiplier = " + _multiplier);
            // }

            // _matchMomentDataEvent_D.Dispatch(matchMomentData);
        }
    }
}