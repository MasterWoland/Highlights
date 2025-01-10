using simpleDI.Injection;
using UnityEngine;

namespace Aion.Highlights.Events
{
    [CreateAssetMenu(fileName = "BaseEvent", menuName = "Events/BaseEvent")]
    public class BaseEvent : ScriptableObject, IInjectable
    {
        public delegate void EventHandler();
        public EventHandler Handler;

        public void Dispatch()
        {
            Handler?.Invoke();
        }
    }
}