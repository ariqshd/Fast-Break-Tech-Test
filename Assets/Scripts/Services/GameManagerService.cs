using System;
using UnityEngine;

namespace Services
{
    public class GameManager: IGameManagerService
    {
        public event Action OnGameStateChanged;

        public virtual string GetServiceName()
        {
            return nameof(GameManager); 
        }

        public virtual void Initialize()
        {
        }

        public virtual void PostInitialize()
        {
        }

        public virtual void Shutdown()
        {
        }
        
        public virtual void StartGame()
        {
        }

    }
}