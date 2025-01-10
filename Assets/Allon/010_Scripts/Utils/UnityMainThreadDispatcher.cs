using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aion.Highlights.Utils
{
    public class UnityMainThreadDispatcher : MonoBehaviour
    {
        private static readonly Queue<Action> EXECUTION_QUEUE = new Queue<Action>();

        private static UnityMainThreadDispatcher _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != null)
            {
                Destroy(this);
            }
        }

        public void Update()
        {
            lock (EXECUTION_QUEUE)
            {
                while (EXECUTION_QUEUE.Count > 0)
                {
                    EXECUTION_QUEUE.Dequeue()();
                }
            }
        }

        public void Enqueue(IEnumerator action)
        {
            lock (EXECUTION_QUEUE)
            {
                EXECUTION_QUEUE.Enqueue(delegate
                {
                    StartCoroutine(action);
                });
            }
        }

        public void Enqueue(Action action)
        {
            Enqueue(ActionWrapper(action));
        }

        private IEnumerator ActionWrapper(Action action)
        {
            action();
            yield return null;
            /*Error: Unable to find new state assignment for yield return*/
        }

        public static bool Exists()
        {
            return _instance != null;
        }

        public static UnityMainThreadDispatcher Instance()
        {
            if (!Exists())
            {
                throw new Exception("UnityMainThreadDispatcher could not find the UnityMainThreadDispatcher object. Please ensure you have added the MainThreadExecutor Prefab to your scene.");
            }
            return _instance;
        }

        private void OnDestroy()
        {
            _instance = null;
        }
    }
}
