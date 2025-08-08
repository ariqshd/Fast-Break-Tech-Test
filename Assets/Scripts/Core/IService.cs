namespace Core
{
    /// <summary>
    /// Interface for all services managed by the ServiceLocator.
    /// All services should implement this interface for consistent lifecycle management.
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Initializes the service and its dependencies.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Performs post-initialization operations for the service after the primary initialization has completed.
        /// </summary>
        void PostInitialize();

        /// <summary>
        /// Shuts down the service cleanly.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Pauses the service when the game is paused.
        /// Services should stop processing or use reduced processing during pause.
        /// </summary>
        //void Pause();

        /// <summary>
        /// Resumes the service when the game continues from pause.
        /// Services should resume normal processing.
        /// </summary>
        //void Resume();

        /// <summary>
        /// Gets the name of the service for debugging and logging.
        /// </summary>
        /// <returns>The service name</returns>
        string GetServiceName();
    }
}