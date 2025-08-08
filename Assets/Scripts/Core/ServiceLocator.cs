using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// ServiceLocator provides a global registry for services in the game.
    /// Services can be registered, retrieved, and managed by type.
    /// Supports dependency ordering for initialization and shutdown.
    /// </summary>
    public static class ServiceLocator
    {
        private static Dictionary<Type, object> services = new Dictionary<Type, object>();
        private static Dictionary<Type, List<Type>> dependencyMap = new Dictionary<Type, List<Type>>();

        /// <summary>
        /// Register a service with the locator.
        /// </summary>
        /// <example>
        /// ServiceLocator.Register<IMyService>(new MyService()); or
        /// ServiceLocator.Register(service); if using IService
        /// </example>
        /// <typeparam name="T">The interface type of the service</typeparam>
        /// <param name="service">The concrete service implementation</param>
        public static void Register<T>(T service) where T : class
        {
            Type type = typeof(T);

            if (services.ContainsKey(type))
            {
                Debug.LogWarning($"Replacing existing service of type {type.Name}");
                services[type] = service;
            }
            else
            {
                services.Add(type, service);
                Debug.Log($"Registered service: {type.Name}, {service}");
            }
        }

        /// <summary>
        /// Get a service by using the locator
        /// </summary>
        /// <typeparam name="T">The interface type of the service</typeparam>
        /// <returns>The registered service implementation or null if not found</returns>
        public static T Get<T>() where T : class
        {
            Type type = typeof(T);

            if (services.TryGetValue(type, out var service))
            {
                return (T)service;
            }

            Debug.LogWarning($"Service not found: {type.Name}");
            return null;
        }

        /// <summary>
        /// Check if a service is registered with the locator
        /// </summary>
        /// <typeparam name="T">The interface type of the service</typeparam>
        /// <returns>True if the service is registered, false otherwise</returns>
        public static bool IsRegistered<T>() where T : class
        {
            return services.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Unregister a service from the locator.
        /// </summary>
        /// <typeparam name="T">The interface type of the service</typeparam>
        public static void Unregister<T>() where T : class
        {
            Type type = typeof(T);

            if (services.ContainsKey(type))
            {
                services.Remove(type);
                Debug.Log($"Unregistered service: {type.Name}");
            }
        }

        /// <summary>
        /// Register a dependency between services for initialization ordering.
        /// </summary>
        /// <typeparam name="TDependent">The service that depends on another</typeparam>
        /// <typeparam name="TDependency">The service that is depended upon</typeparam>
        public static void RegisterDependency<TDependent, TDependency>()
            where TDependent : class
            where TDependency : class
        {
            Type dependentType = typeof(TDependent);
            Type dependencyType = typeof(TDependency);

            if (!dependencyMap.ContainsKey(dependentType))
            {
                dependencyMap[dependentType] = new List<Type>();
            }

            if (!dependencyMap[dependentType].Contains(dependencyType))
            {
                dependencyMap[dependentType].Add(dependencyType);
            }
        }

        /// <summary>
        /// Get all registered services in dependency order.
        /// </summary>
        /// <returns>Services ordered so that dependencies are before dependents</returns>
        public static List<object> GetServicesInDependencyOrder()
        {
            List<Type> orderedTypes = new List<Type>();
            HashSet<Type> visited = new HashSet<Type>();

            foreach (Type type in services.Keys)
            {
                AddTypeInDependencyOrder(type, orderedTypes, visited, new HashSet<Type>());
            }

            return orderedTypes.Select(t => services[t]).ToList();
        }

        /// <summary>
        /// Clear all registered services.
        /// </summary>
        public static void Clear()
        {
            services.Clear();
            dependencyMap.Clear();
        }

        /// <summary>
        /// Helper method for topological sort of dependencies
        /// </summary>
        /// <param name="type">current type of dependency</param>
        /// <param name="orderedTypes">list of type</param>
        /// <param name="visited">hashset of types visited</param>
        /// <param name="visiting">hashset of types currently visiting to prevent circular dependency</param>
        private static void AddTypeInDependencyOrder(Type type, List<Type> orderedTypes,
            HashSet<Type> visited, HashSet<Type> visiting)
        {
            if (visited.Contains(type))
                return;

            if (visiting.Contains(type))
            {
                Debug.LogError($"Circular dependency detected involving {type.Name}");
                return;
            }

            visiting.Add(type);

            if (dependencyMap.TryGetValue(type, out var dependencies))
            {
                foreach (Type dependency in dependencies)
                {
                    if (services.ContainsKey(dependency))
                    {
                        AddTypeInDependencyOrder(dependency, orderedTypes, visited, visiting);
                    }
                }
            }

            visiting.Remove(type);
            visited.Add(type);
            orderedTypes.Add(type);
        }
    }
}
