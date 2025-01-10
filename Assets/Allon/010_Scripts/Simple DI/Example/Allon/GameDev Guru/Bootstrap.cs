using simpleDI.Injection;
using UnityEngine;

namespace simpleDI.demo
{
    public class Bootstrap : MonoBehaviour
    {
        public static Injector Injector;

        private void Awake()
        {
            Injector = new Injector();
            // Backend backend = new Backend();
            // Injector.Bind<IBackend>(backend);
            Injector.Bind<IBackend>(new Backend());
            Injector.Bind<ILogger>(new LoggerFile());
            Injector.PostBindings();
        }
    }
}