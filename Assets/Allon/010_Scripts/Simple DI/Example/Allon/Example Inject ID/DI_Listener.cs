using simpleDI.Injection.Allon;
using UnityEngine;

namespace SimpleDI.Example.Allon
{
    public class DI_Listener : MonoBehaviour
    {
        [InjectID(Id = "Hello")] public DI_StringEvent HelloListener;
        [InjectID(Id = "Mark")] public DI_StringEvent MarkListener;
        [InjectID(Id = "Allon")] public DI_StringEvent AllonListener;

        private void Awake()
        {
            // Test_DIBinder.Injector.Inject(this);
            Test_DIBinder.Injector.InjectID(this);
        }

        private void OnEnable()
        {
            HelloListener.Handler += OnHello;
            MarkListener.Handler += OnMark;
            AllonListener.Handler += OnAllon;
        }

        private void OnDisable()
        {
            HelloListener.Handler -= OnHello;
            MarkListener.Handler -= OnHello;
            AllonListener.Handler -= OnAllon;
        }

        private void OnHello(string value)
        {
            Debug.Log("[Listener] " + value);
        }

        private void OnMark(string value)
        {
            Debug.Log("[Listener] 02. value: " + value);
        }

        private void OnAllon(string value)
        {
            Debug.Log("[Listener] 03. value: " + value);
        }
    }
}