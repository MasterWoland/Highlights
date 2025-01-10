using simpleDI.Injection;

namespace simpleDI.demo
{
    public interface ILogger : IInjectable
    {
        void Log(string message);
    }
}