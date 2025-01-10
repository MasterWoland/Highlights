using simpleDI.Injection;
using simpleDI.Injection.Allon;
using UnityEngine;

namespace SimpleDI.Example.Allon
{
    public class Test_DIBinder : MonoBehaviour
    {
        public static Injector Injector;

        public DI_StringEvent HelloEvent;
        public DI_StringEvent MarkEvent;
        public DI_StringEvent AllonEvent;

        private void Awake()
        {
            DontDestroyOnLoad(this);

            Injector = new Injector();

            Bind();
        }

        private void Bind()
        {
            // MRA: note: order is important!
            // MRA: SceneLoader contains a _sceneLoadedCompleteEvent, so this must be bound first for some reason
            // Injector.BindWithId<BaseEvent>(_testSimpleEvent, "2");

            // Injector.Bind(HelloEvent);

            // Creating a Dictionary in Injector
            Injector.BindWithId(HelloEvent, "Hello");
            Injector.BindWithId(MarkEvent, "Mark");
            Injector.BindWithId(AllonEvent, "Allon");
            Injector.PostBindings();
        }
    }
}