using System;
using Data;

namespace Services
{
    public interface IGameplayEventService : IService
    {
        public void Subscribe(GameplayTag eventTag, Action<object> listener);
        public void Subscribe(GameplayTag eventTag, Action listener);
        public void Unsubscribe(GameplayTag eventTag, Action<object> listener);
        public void Unsubscribe(GameplayTag eventTag, Action listener);
        public void Broadcast(GameplayTag eventTag, object data = null);
    }
}