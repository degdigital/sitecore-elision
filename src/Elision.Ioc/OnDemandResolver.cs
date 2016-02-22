using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sitecore.Diagnostics;

namespace Elision.Ioc
{
    public class OnDemandResolver
    {
        protected readonly Dictionary<string, Type> InterfaceMap;
        protected Type[] KnownTypes;

        private static OnDemandResolver _current;
        public static OnDemandResolver Current
        {
            get { return _current ?? (_current = new OnDemandResolver()); }
            set { _current = value; }
        }

        public OnDemandResolver()
        {
            InterfaceMap = new Dictionary<string, Type>();
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof (T));
        }

        public object Resolve(Type type)
        {
            if (type.IsInterface || type.IsAbstract)
            {
                var fullName = type.AssemblyQualifiedName ?? "";
                if (string.IsNullOrWhiteSpace(fullName))
                    return null;

                if (InterfaceMap.ContainsKey(fullName))
                {
                    type = InterfaceMap[fullName];
                }
                else
                {
                    var impl = GetConcreteTypesFromInterface(type);
                    if (impl.Length != 1)
                    {
                        Log.SingleError(
                            string.Format(
                                "Unable to resolve implementation for type '{0}'. {1} eligible types were found ({2}). " +
                                "You can resolve this issue by manually registering the implementation type.",
                                fullName, impl.Length, string.Join(", ", impl.Select(x => x.AssemblyQualifiedName))),
                            this);
                        return null;
                    }

                    type = impl[0];
                    InterfaceMap.Add(fullName, type);
                }
            }

            var ctor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .OrderByDescending(x => x.GetParameters().Length)
                .FirstOrDefault();

            if (ctor == null)
                return Activator
                    .CreateInstance(type,
                                    BindingFlags.Instance | BindingFlags.Public |
                                    BindingFlags.NonPublic | BindingFlags.CreateInstance,
                                    null, null, null);

            var args = ctor.GetParameters().Select(x => Resolve(x.ParameterType)).ToArray();

            return ctor.Invoke(args);
        }

        public void RegisterType<TInterface, TImplementation>(bool replaceExistingRegistration = false)
        {
            var interfaceType = typeof (TInterface);
            var fullName = interfaceType.AssemblyQualifiedName ?? "";
            if (string.IsNullOrWhiteSpace(fullName))
                return;
            if (InterfaceMap.ContainsKey(fullName) && !replaceExistingRegistration)
                return;

            if (!interfaceType.IsAbstract && !interfaceType.IsInterface)
                throw new InvalidOperationException("Cannot register mapping because interface type is not an interface or abstract class.");

            var implementationType = typeof (TImplementation);
            if (!CanBeImplementationOfType(implementationType, interfaceType))
                throw new InvalidOperationException("Cannot register mapping for implementation type that does not inherit from the interface type.");

            InterfaceMap.Add(fullName, implementationType);
        }

        protected virtual Type[] GetConcreteTypesFromInterface(Type interfaceType)
        {
            if (KnownTypes == null)
            {
                var assemblies = AppDomain.CurrentDomain
                                          .GetAssemblies()
                                          .Where(x => !x.FullName.StartsWith("System.")
                                                      && !x.FullName.StartsWith("Microsoft.")
                                                      && !x.FullName.StartsWith("Sitecore."));
                var types = new List<Type>();
                foreach (var assembly in assemblies)
                {
                    try
                    {
                        types.AddRange(assembly.GetExportedTypes());
                    } catch { }
                }
                KnownTypes = types.ToArray();
            }
            return KnownTypes
                .Where(x => CanBeImplementationOfType(x, interfaceType))
                .ToArray();
        }

        protected virtual bool CanBeImplementationOfType(Type x, Type interfaceType)
        {
            if (x.IsInterface || x.IsAbstract)
                return false;

            if (x.BaseType != null && x.BaseType.AssemblyQualifiedName == interfaceType.AssemblyQualifiedName)
                return true;

            return x.GetInterfaces()
                    .Any(i => i.AssemblyQualifiedName == interfaceType.AssemblyQualifiedName);
        }
    }
}
