using simpleDI.Injection;
using UnityEngine;

namespace Aion.Highlights.Events
{
    [CreateAssetMenu(fileName = "FloatEvent", menuName = "Events/FloatEvent")]
    public class FloatEvent : ScriptableObject, IInjectable
    {
        public delegate void EventHandler(float value);
        public EventHandler Handler;

        public void Dispatch(float value)
        {
            Handler?.Invoke(value);
        }
    }
}