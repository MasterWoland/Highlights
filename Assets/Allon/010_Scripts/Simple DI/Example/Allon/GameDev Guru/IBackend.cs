using simpleDI.Injection;

namespace simpleDI.demo
{
    public interface IBackend : IInjectable
    {
        void UpdatePlayerProfile(IPlayerProfile profile);

        void Test();
    }
}