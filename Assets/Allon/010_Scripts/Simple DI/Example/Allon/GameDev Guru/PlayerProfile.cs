using simpleDI.Injection;
using UnityEngine;

namespace simpleDI.demo
{
    public class PlayerProfile : IPlayerProfile
    {
        [Inject] private IBackend _backend = null;

        public void Execute(int transaction)
        {
            Debug.Log("[PP] transaction = " + transaction);
            _backend.UpdatePlayerProfile(this);
        }
    }
}