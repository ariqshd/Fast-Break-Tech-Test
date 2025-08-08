using Services;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// ServiceInitializer is responsible for registering and initializing all core game services
    /// at the start of the game, and shutting them down cleanly when the object is destroyed.
    /// Attach this script to a GameObject in your scene.
    /// </summary>
    public class ServiceInitializer : MonoBehaviour
    {
        [SerializeField] private bool _isNetworked = false;

        private void Awake()
        {
            InitializeServices(_isNetworked);
        }

        private void Start()
        {
            var orderedServices = ServiceLocator.GetServicesInDependencyOrder();
            foreach (var service in orderedServices)
            {
                if (service is IService gs)
                    gs.PostInitialize();
            }
        }

        /// <summary>
        /// Registers all required services and initializes them in dependency order.
        /// Add your service registration logic here.
        /// </summary>
        private void InitializeServices(bool isNetworked)
        {
            // Register dependencies first if needed
            ServiceLocator.RegisterDependency<LocalMultiplayerGameManagerService, IPlayerInputManagerService>();
            ServiceLocator.RegisterDependency<LocalMultiplayerGameManagerService, ITeamService>();
            ServiceLocator.RegisterDependency<LocalMultiplayerGameManagerService, IEntitySpawnerService>();
            ServiceLocator.RegisterDependency<LocalMultiplayerGameManagerService, IEntityPositionService>();

            // Create and register services (with null checks)
            ServiceLocator.Register<IGameplayEventService>(new GameplayEventService());
            ServiceLocator.Register<IPlayerInputManagerService>(new PlayerInputManagerService());
            ServiceLocator.Register<ITeamService>(new TeamService());
            RegisterService<IEntitySpawnerService>();
            RegisterService<IEntityPositionService>();
            ServiceLocator.Register<IGameManagerService>(new LocalMultiplayerGameManagerService());

            // Add networked services if needed
            if (isNetworked)
            {
                //ServiceLocator.Register<INetworkService>(new NetworkService());
            }

            // Initialize services in dependency order
            var orderedServices = ServiceLocator.GetServicesInDependencyOrder();
            foreach (var service in orderedServices)
            {
                if (service is IService gs)
                    gs.Initialize();
            }
        }

        /// <summary>
        /// Helper method to register a MonoBehaviour service attached to this GameObject.
        /// </summary>
        private void RegisterService<T>() where T : class
        {
            var service = gameObject.GetComponent<T>();
            if (service != null)
                ServiceLocator.Register(service);
            else
                Debug.LogWarning($"Service of type {typeof(T).Name} not found on GameObject.");
        }

        /// <summary>
        /// Shuts down all registered services in reverse dependency order when the game ends or this object is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            // Shutdown services in dependency order (BUG: should be reverse order)
            var orderedServices = ServiceLocator.GetServicesInDependencyOrder();
            orderedServices.Reverse(); // Shutdown dependents before dependencies
            foreach (var service in orderedServices)
            {
                if (service is IService gs)
                    gs.Shutdown();
            }
        }
    }
}