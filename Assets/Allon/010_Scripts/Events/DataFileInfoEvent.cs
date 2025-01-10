using Aion.Highlights.Data;
using simpleDI.Injection;
using UnityEngine;

namespace Aion.Highlights.Events
{
    [CreateAssetMenu(fileName = "DataFileInfoEvent", menuName = "Events/DataFileInfoEvent")]
    public class DataFileInfoEvent : ScriptableObject, IInjectable
    {
        public delegate void EventHandler(DataFileInfo dataFileInfo);
        public EventHandler Handler;

        public void Dispatch(DataFileInfo dataFileInfo)
        {
            Handler?.Invoke(dataFileInfo);
        }
    }
}