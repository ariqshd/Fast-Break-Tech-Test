using System;
using System.Collections.Generic;
using Data;
using Gameplay.AbilitySystem.Abilities;
using Gameplay.AbilitySystem.Effects;
using UnityEngine;

namespace Gameplay.AbilitySystem
{
    public class AbilitySystemController : MonoBehaviour
    {
        protected List<IGameplayAbility> RegisteredAbilities;
        protected Dictionary<Guid, IGameplayAbility> RegisteredAbilitiesByInputId;
        

        /// <summary>
        /// Store owned tags with stack count
        /// </summary>
        protected Dictionary<GameplayTag, int> TagMap;
        protected GameObject OwnerActor;
        protected List<AbilityInputBindingGuid> AbilityInputBindingData;
        
        private void Awake()
        {

        }

        public void Setup(AbilitySystemControllerSetup data)
        {
            SetOwnerActor(data.OwnerActor);
            AbilityInputBindingData = data.InputBindingData;
        }

        public virtual void Initialize()
        {
            RegisteredAbilities = new List<IGameplayAbility>();
            RegisteredAbilitiesByInputId = new Dictionary<Guid, IGameplayAbility>();
            TagMap = new Dictionary<GameplayTag, int>();
            
            foreach (var data in AbilityInputBindingData)
            {
                RegisterAbility(data.InputActionId, data.Ability);
            }
        }
        
        public virtual bool TryActivateAbility(GameplayAbility gameplayAbility)
        {
            return true;
        }

        public virtual bool TryActivateAbilityOnInput(Guid inputActionId)
        {
            foreach (var data in RegisteredAbilitiesByInputId)
            {
                if (data.Key != inputActionId) continue;
                if (!data.Value.CanActivateAbility()) return false;
                data.Value.ActivateAbility();
                return true;
            }
            
            return false;
        }

        public virtual void RegisterAbility(GameplayAbility ability)
        {
            var instance = ability.CreateInstanceCopy();
            if (instance != null)
            {
                RegisteredAbilities.Add(instance);
            }
            else
            {
                Debug.LogError($"Failed to create instance of {instance.name}");
            }
        }
        
        public virtual void RegisterAbility(Guid inputActionId, GameplayAbility ability)
        {
            var instance = ability.CreateInstanceCopy();
            
            instance.SetOwner(OwnerActor);
            if (instance != null)
            {
                RegisteredAbilitiesByInputId.Add(inputActionId, instance);
            }
            else
            {
                Debug.LogError($"Failed to create instance of {instance.name}");
            }
        }

        public virtual void RemoveAbility(GameplayAbility ability)
        {
            RegisteredAbilities.Remove(ability);
            
            if (!RegisteredAbilitiesByInputId.ContainsValue(ability)) return;
            
            Guid keyToRemove = Guid.Empty;
            bool found = false;
    
            foreach (var kvp in RegisteredAbilitiesByInputId)
            {
                if (kvp.Value != ability) continue; // Reference equality check
                keyToRemove = kvp.Key;
                found = true;
                break;
            }
    
            // Remove if found
            if (found)
            {
                RegisteredAbilitiesByInputId.Remove(keyToRemove);
            }
        }

        public virtual void ApplyGameplayEffectToSelf(GameplayEffect gameplayEffect)
        { 
            gameplayEffect.SetTarget(OwnerActor.GetComponent<IAbilitySystemController>());
            gameplayEffect.ApplyEffect();
            Debug.Log("Apply gameplay effect to self");
        }

        public virtual void ApplyGameplayEffectToTarget(GameplayEffect gameplayEffect, GameObject target)
        {
            
        }
        
        public void UpdateTagMap(GameplayTag gameplayTag, int count)
        {
            if (!TagMap.TryAdd(gameplayTag, count))
            {
                TagMap[gameplayTag] += count;
            }

            if (TagMap[gameplayTag] <= 0)
            {
                TagMap.Remove(gameplayTag);
            }
            
            Debug.Log($"Updated tag map for {gameplayTag.TagName} with count {count}");
        }

        public bool HasTag(GameplayTag gameplayTag)
        {
            return TagMap.ContainsKey(gameplayTag);
        }

        public void SetOwnerActor(GameObject actor)
        {
            OwnerActor = actor;
        }

        public GameObject GetActor()
        {
            return OwnerActor;
        }
    }
}