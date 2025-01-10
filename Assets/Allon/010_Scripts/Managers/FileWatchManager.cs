using System.IO;
using Aion.Highlights.Components;
using Aion.Highlights.Data;
using Aion.Highlights.Events;
using simpleDI.Injection.Allon;
using UnityEngine;

namespace Aion.Highlights.Managers
{
    /// <summary>
    /// Listens to new json files in a specified folder.
    /// </summary>
    public class FileWatchManager : MonoBehaviour
    {
        // MRA: we could unsubscribe to the OnCreated event when we are busy processing a new file
        // MRA: and re-subscribe once we have finished processing (and capturing) that file.

        [InjectID(Id = ID.E_DATA_FILE_INFO)] private DataFileInfoEvent _dataFileInfoEventDispatcher;
        private string _path = @"C:\HighLights\new-data\"; // build
        private string _localPath = @"F:\Unity\AionSports\Captures\Input\"; 
        private FileSystemWatcher _watcher;
        private bool _isNewFile = false;
        private string _newFilePath;
        private string _newFileName;

        private void Awake()
        {
            DIBinder.Injector.Inject(this);
            DIBinder.Injector.InjectID(this);

#if UNITY_EDITOR
            _path = _localPath;
#endif
        }

        /// <summary>
        /// We cannot dispatch the event from OnCreated, because that uses a different thread
        /// </summary>
        private void Update()
        {
            if (!_isNewFile) return;

            // Check if the file has been completely processed
            bool isLocked = CheckIfFileIsLocked();
            // Debug.Log("[ FWM] ___ file is locked? " + isLocked);

            if (isLocked) return;

            DataFileInfo info = new DataFileInfo(_newFilePath, _newFileName);
            _dataFileInfoEventDispatcher?.Dispatch(info);
            Debug.Log("[ FWM ] New File: "+info.FileName);

            _isNewFile = false;
            _newFilePath = string.Empty;
            _newFileName = string.Empty;
        }


#region EVENTS
        private void OnEnable()
        {
            _watcher = new FileSystemWatcher(_path);

            // MRA: we may not need all those filters
            _watcher.NotifyFilter = NotifyFilters.Attributes
                                    | NotifyFilters.CreationTime
                                    | NotifyFilters.DirectoryName
                                    | NotifyFilters.FileName
                                    | NotifyFilters.LastAccess
                                    | NotifyFilters.LastWrite
                                    | NotifyFilters.Security
                                    | NotifyFilters.Size;

            _watcher.Created += OnCreated;
            // _watcher.Changed += OnChanged;
            // _watcher.Deleted += OnDeleted;
            // _watcher.Renamed += OnRenamed;
            // _watcher.Error += OnError;

            _watcher.Filter = "*.json";
            _watcher.IncludeSubdirectories = true;
            _watcher.EnableRaisingEvents = true;
        }

        private void OnDisable()
        {
            _watcher.Created -= OnCreated;
            _watcher.Dispose();
        }

        /// <summary>
        /// Note: this event handler is in a different thread. We should not dispatch events from here.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            // INFO: e.Name = fileName
            // INFO: e.FullPath = path including fileName and extension
            _isNewFile = true;
            _newFilePath = e.FullPath;
            _newFileName = e.Name;
        }
#endregion

#region HELPER METHODS
        private bool CheckIfFileIsLocked()
        {
            FileInfo fileInfo = new FileInfo(_newFilePath);
            FileStream stream = null;

            try
            {
                stream = fileInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                stream?.Close();
            }

            //file is not locked
            return false;
        }
#endregion
    }
}