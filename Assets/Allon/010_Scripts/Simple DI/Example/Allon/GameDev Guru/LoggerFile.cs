using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace simpleDI.demo
{
    public class LoggerFile : ILogger
    {
        public void Log(string message)
        {
            // Debug.Log("[ILogger] logging from LoggerFile: "+message);
            // log to file
        }
    }
}