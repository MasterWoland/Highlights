using simpleDI.Injection;
using UnityEngine;

namespace Aion.Highlights.Events
{
    [CreateAssetMenu(fileName = "StringEvent", menuName = "Events/StringEvent")]
    public class StringEvent : ScriptableObject, IInjectable
    {
        public delegate void EventHandler(string value);
        public EventHandler Handler;

        public void Dispatch(string value)
        {
            Handler?.Invoke(value);
        }
    }
}