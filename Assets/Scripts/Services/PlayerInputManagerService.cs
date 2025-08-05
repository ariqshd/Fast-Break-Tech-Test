using System;
using UnityEngine.InputSystem;

namespace Services
{
    /// <summary>
    /// Manages player input functionalities using Unity's PlayerInputManager.
    /// This service offers player joining operations, maximum player count management,
    /// and event subscription capabilities for player join events.
    /// Implements the <see cref="IPlayerInputManagerService"/> interface.
    /// </summary>
    public class PlayerInputManagerService : IPlayerInputManagerService
    {
        public void Initialize()
        {
        }

        public void PostInitialize()
        {
        }

        public void Shutdown()
        {
        }

        public string GetServiceName()
        {
            return nameof(PlayerInputManagerService);
        }

        public PlayerInput JoinPlayer(int playerIndex = -1, int splitScreenIndex = -1, string controlScheme = null,
            InputDevice pairWithDevice = null)
        {
            if (PlayerInputManager.instance == null)
            {
                throw new ArgumentNullException(nameof(PlayerInputManager.instance));
            }
            
            return PlayerInputManager.instance.JoinPlayer(playerIndex, splitScreenIndex, controlScheme, pairWithDevice);
        }
    }
}