using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Services
{
    public interface IGameManagerService : IService
    {
        public void StartGame();
        event Action OnGameStateChanged;
    }
}