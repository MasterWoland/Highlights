using Aion.Highlights.Data.Stream;
using simpleDI.Injection;
using UnityEngine;

namespace Aion.Highlights.Events
{
    [CreateAssetMenu(fileName = "HighlightMomentEvent", menuName = "Events/HighlightMomentEvent")]
    public class HighlightMomentEvent : ScriptableObject, IInjectable
    {
        public delegate void EventHandler(HighlightMoment highlightMoment);
        public EventHandler Handler;

        public void Dispatch(HighlightMoment highlightMoment)
        {
            Handler?.Invoke(highlightMoment);
        }
    }
}