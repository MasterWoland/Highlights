using simpleDI.Injection;
using UnityEngine;

namespace simpleDI.demo
{
    public class Backend : IBackend
    {
        [Inject] private ILogger _logger;

        public void UpdatePlayerProfile(IPlayerProfile profile)
        {
            // Debug.Log("[Backend] UpdatePlayerProfile() _____");
            _logger.Log("[Backend] UpdatePlayerProfile() _____");
        }

        public void Test()
        {
            Debug.Log("[Backend] Player says hello ___");
        }
    }
}