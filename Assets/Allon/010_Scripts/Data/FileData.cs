using System.IO;
using simpleDI.Injection;
using UnityEngine;

namespace Aion.Highlights.Data
{
    [CreateAssetMenu(fileName = "FileData", menuName = "Data/FileData")]
    public class FileData : ScriptableObject, IInjectable
    {
        // paths
        private string _path = string.Empty;

        public string Path { get => _path; }

        private string _fileName = string.Empty;

        public string FileName { get => _fileName; }

        // data
        [field: SerializeField]
        public string[] TrackingDataEntries { get; private set; } // all lines of the raw tracking data from the .txt file

        public void AssignTrackingDataEntries(string[] entries) { TrackingDataEntries = entries; }

        public void AssignPath(string path)
        {
            _path = path;
            // Debug.Log("[ FileData ] tracking data is null? " + (TrackingDataEntries == null));
        }

        public void AssignFileName(string fileName) { _fileName = fileName; }
    }
}