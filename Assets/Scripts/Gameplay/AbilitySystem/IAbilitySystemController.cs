using Data;
using Gameplay.AbilitySystem.Effects;
using UnityEngine;

namespace Gameplay.AbilitySystem
{
    public interface IAbilitySystemController
    {
        public void UpdateTagMap(GameplayTag gameplayTag, int count);
        public bool HasTag(GameplayTag gameplayTag);
        public GameObject GetAbilitySystemOwner();
        public AbilitySystemController GetAbilitySystemController();
        public void ApplyGameplayEffectToSelf(GameplayEffect gameplayEffect);
        public void ApplyGameplayEffectToTarget(GameplayEffect gameplayEffect, GameObject target);
    }
}