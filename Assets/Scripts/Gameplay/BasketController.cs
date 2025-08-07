using System;
using Data;
using Services;
using UnityEngine;

namespace Gameplay
{
    public class BasketController : MonoBehaviour
    {
        private IGameplayEventService _gameplayEventService;

        private void Start()
        {
            _gameplayEventService = ServiceLocator.Get<IGameplayEventService>();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<IBallController>(out var ball))
            {
                _gameplayEventService.Broadcast(GameplayTags.Event_Score);
            }
        }
    }
}
