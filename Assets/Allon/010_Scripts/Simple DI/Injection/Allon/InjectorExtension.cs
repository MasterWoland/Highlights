using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace simpleDI.Injection.Allon
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    [MeansImplicitUse]
    public sealed class InjectID : System.Attribute
    {
        public string Id = string.Empty;
    }
    
    public static class InjectorExtension
    {
        private static readonly Dictionary<string, object> _objectsWithId = new Dictionary<string, object>(); // string: InjectID.Id

        public static void BindWithId<T>(this Injector injector, T instance, string Id) where T : IInjectable
        {
            _objectsWithId[Id] = instance;
        }
        
        public static void InjectID(this Injector injector, object obj)
        {
            var fields = ReflectorID.Reflect(obj.GetType());

            string id = string.Empty;

            // Check all fields
            foreach (FieldInfo field in fields)
            {
                foreach (var customAttributeData in field.CustomAttributes)
                {
                    // null check
                    if (customAttributeData.NamedArguments == null) continue;

                    // here we find the ObjectID within the Inject attribute 
                    id = customAttributeData.NamedArguments[0].TypedValue.Value.ToString();
                }

                // MRA: we may want to check the ObjectID for string.Empty

                var value = GetWithId(injector, id);
                field.SetValue(obj, value); // here we set the field to the desired instance or Scriptable Object
            }
        }

        private static object GetWithId(this Injector injector, string id)
        {
            // MRA: because of a recursive loop, this part differs from Get() in Injector.cs
            while (true)
            {
                if (_objectsWithId.TryGetValue(id, out var instance)) return instance;

                if (injector.ParentInjector == null) throw new InjectorException("Could not get " + id + " from injector");
                
                injector = injector.ParentInjector;
            }
        }

        private static class ReflectorID
        {
            private static readonly System.Type _injectAttributeType = typeof(InjectID);
            private static readonly Dictionary<System.Type, FieldInfo[]> cachedFieldInfos =
                new Dictionary<System.Type, FieldInfo[]>();
            private static readonly List<FieldInfo> _reusableList = new List<FieldInfo>(1024);

            public static FieldInfo[] Reflect(System.Type type)
            {
                Assert.AreEqual(0, _reusableList.Count, "Reusable list in Reflector was not empty!");

                FieldInfo[] cachedResult;
                if (cachedFieldInfos.TryGetValue(type, out cachedResult))
                {
                    return cachedResult;
                }

                var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                            BindingFlags.FlattenHierarchy);
                for (var fieldIndex = 0; fieldIndex < fields.Length; fieldIndex++)
                {
                    var field = fields[fieldIndex];
                    var hasInjectAttribute = field.IsDefined(_injectAttributeType, inherit: false);
                    if (hasInjectAttribute)
                    {
                        _reusableList.Add(field);
                    }
                }

                var resultAsArray = _reusableList.ToArray();
                _reusableList.Clear();
                cachedFieldInfos[type] = resultAsArray;
                return resultAsArray;
            }
        }
    }
}