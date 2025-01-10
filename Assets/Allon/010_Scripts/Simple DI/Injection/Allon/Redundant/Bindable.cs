using System;

namespace simpleDI.Injection.Allon
{
    /// <summary>
    /// Redundant.
    /// </summary>
    [Serializable]
    public class Bindable
    {
        private System.Type _type;
        private object _instance;
        // public Type Type => _type;
        public object Instance => _instance;

        public Bindable(System.Type type, object instance)
        {
            _type = type; // = Class
            _instance = instance; // = instance of class or Scriptable Object  
        }
    }
}