using simpleDI.Injection;
using UnityEngine;

namespace SimpleDI.Example.Allon
{
    [CreateAssetMenu(fileName = "DI_StringEvent", menuName = "Events/DI_StringEvent")]
    public class DI_StringEvent : ScriptableObject, IInjectable
    {
        public string TestValue;
        
        public delegate void EventHandler(string value);
        public EventHandler Handler;

        public void Dispatch(string value)
        {
            Handler?.Invoke(value);
        }
    }
}