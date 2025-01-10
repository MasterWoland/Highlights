using simpleDI.Injection;
using UnityEngine;

namespace Aion.Highlights.Events
{
    [CreateAssetMenu(fileName = "IntEvent", menuName = "Events/IntEvent")]
    public class IntEvent : ScriptableObject, IInjectable
    {
        public delegate void EventHandler(int value);
        public EventHandler Handler;

        public void Dispatch(int value)
        {
            Handler?.Invoke(value);
        }
    }
}