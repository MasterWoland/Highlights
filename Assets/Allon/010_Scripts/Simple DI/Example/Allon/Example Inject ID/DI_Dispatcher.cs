using simpleDI.Injection.Allon;
using UnityEngine;

namespace SimpleDI.Example.Allon
{
    public class DI_Dispatcher : MonoBehaviour
    {
        [InjectID(Id = "Hello")] public DI_StringEvent HelloDispatcher;
        [InjectID(Id = "Mark")] public DI_StringEvent MarkDispatcher;
        [InjectID(Id = "Allon")] public DI_StringEvent AllonDispatcher;

        private void Awake()
        {
            Test_DIBinder.Injector.InjectID(this);
        }

        private void Start()
        {
            HelloDispatcher?.Dispatch(HelloDispatcher.TestValue);

            MarkDispatcher?.Dispatch(MarkDispatcher.TestValue);

            AllonDispatcher?.Dispatch(AllonDispatcher.TestValue);
        }
    }
}