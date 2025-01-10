using System;
using simpleDI.Injection;
using UnityEngine;

namespace simpleDI.demo
{
    public class Player : MonoBehaviour
    {
        private IPlayerProfile _playerProfile = new PlayerProfile();
        [Inject] private IBackend _backend;

        private void Start()
        {
            Bootstrap.Injector.Inject(this);
            Bootstrap.Injector.Inject(_playerProfile);

            // MRA: Injector is going to look for the fields inside of the _playerProfile with the [Inject] attribute.
            // We may want to put this somewhere else, in a special script for example
        }

        private void Update()
        {
            double d = Math.Floor(Time.timeSinceLevelLoad);
            int i = (int)d;

            _playerProfile.Execute(i);

            _backend.Test();
        }
    }
}