using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Services
{
    public interface IPlayerInputManagerService : IService
    {
        public PlayerInput JoinPlayer(int playerIndex = -1, int splitScreenIndex = -1, string controlScheme = null, InputDevice pairWithDevice = null);
    }
}