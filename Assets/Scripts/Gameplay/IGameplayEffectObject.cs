using Gameplay.AbilitySystem.Effects;
using UnityEngine;

namespace Gameplay
{
    public interface IGameplayEffectObject
    {
        public void ApplyEffectToTarget(GameObject target, GameplayEffect effect);
    }
}