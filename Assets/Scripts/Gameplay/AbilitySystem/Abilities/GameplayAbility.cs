using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Gameplay.AbilitySystem.Effects;
using Gameplay.AbilitySystem.Tasks;
using UnityEngine;

namespace Gameplay.AbilitySystem.Abilities
{
    /// <summary>
    /// Defines what an in-game ability does, what (if anything) it costs to use, when or under what conditions it can be used, and so on
    /// This serves as a base class for implementing specific abilities with behaviors like activation, cancellation, and ending.
    /// </summary>
    public class GameplayAbility : ScriptableObject, IGameplayAbility
    {
        /// <summary>
        /// Tags to grant to the activating actor while this ability is active.
        /// </summary>
        [SerializeField] protected List<GameplayTag> grantedTagsWhileActive;
        
        /// <summary>
        /// The owner must own tags to activate this ability
        /// </summary>
        [SerializeField] protected List<GameplayTag> ownerRequiredTags;

        /// <summary>
        /// Cost to activate this ability
        /// </summary>
        [SerializeField] protected GameplayEffect abilityCost;

        protected bool IsInstanced;
        protected IAbilitySystemController OwnerAbilitySystemController;
        protected GameObject OwnerActor;
        
        public virtual GameplayAbility CreateInstanceCopy()
        {
            var instance = CreateInstance(GetType()) as GameplayAbility;
            if (instance == null) return null;
        
            // Copy the data
            instance.grantedTagsWhileActive = new List<GameplayTag>(grantedTagsWhileActive);
            instance.ownerRequiredTags = new List<GameplayTag>(ownerRequiredTags);
            instance.abilityCost = abilityCost?.CreateInstanceCopy();
            instance.IsInstanced = true;
            instance.Initialize();
            return instance;
        }

        public void SetOwner(GameObject owner)
        {
            OwnerActor = owner;
            OwnerAbilitySystemController = owner.GetComponent<IAbilitySystemController>();
        }

        protected virtual void Initialize()
        {
            
        }
        
        public virtual async Task ActivateAbility()
        {
            foreach (var tag in grantedTagsWhileActive)
            {
                OwnerAbilitySystemController.UpdateTagMap(tag, 1); 
            }
            
            OwnerAbilitySystemController.ApplyGameplayEffectToSelf(abilityCost);
        }


        

        public virtual bool CanActivateAbility()
        {
            foreach (var tag in ownerRequiredTags)
            {
                if (!OwnerAbilitySystemController.HasTag(tag))
                {
                    Debug.Log($"The owner of {OwnerActor.name} does not have the required tag(s) to activate this ability");
                    return false;
                };
            }
            
            return true;
        }

        public virtual void CancelAbility()
        {
        }
        
        public virtual async Task EndAbility()
        {
            foreach (var tag in grantedTagsWhileActive)
            {
                OwnerAbilitySystemController.UpdateTagMap(tag, -1); 
            }
        }

        protected IEnumerator Wait(float seconds)
        {
            yield return new WaitForSeconds(seconds);
        }
    }
}