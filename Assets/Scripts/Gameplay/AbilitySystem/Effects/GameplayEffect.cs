using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Gameplay.AbilitySystem.Effects
{
    [CreateAssetMenu(fileName = nameof(GameplayEffect), menuName = "AbilitySystem/Effects/" + nameof(GameplayEffect), order = 0)]
    public class GameplayEffect : ScriptableObject
    {
        /// <summary>
        /// These tags are applied to the actor when the effect is applied.
        /// </summary>
        [SerializeField] private List<GameplayTag> grantedTags = new List<GameplayTag>();

        /// <summary>
        /// These tags are removed from the actor when the effect is applied.
        /// </summary>
        [SerializeField] private List<GameplayTag> removedTags = new List<GameplayTag>();

        private IAbilitySystemController _target;
        private bool _isInstanced;

        // Create and return a new instance
        public virtual GameplayEffect CreateInstanceCopy()
        {
            var instance = CreateInstance(GetType()) as GameplayEffect;
            if (instance == null) return null;
        
            // Copy the data
            instance._target = _target;
            instance.grantedTags = new List<GameplayTag>(grantedTags);
            instance.removedTags = new List<GameplayTag>(removedTags);
            instance._isInstanced = true;
            return instance;
        }

        public bool IsInstance()
        {
            return _isInstanced;
        }

        public virtual void ApplyEffect()
        {
            ApplyGrantedTags();
            ApplyRemovedTags();
        }
        
        protected void ApplyGrantedTags()
        {
            foreach (var gameplayTag in grantedTags)
            {
                _target.UpdateTagMap(gameplayTag, 1);
            }
        }

        protected void ApplyRemovedTags()
        {
            foreach (var gameplayTag in removedTags)
            {
                _target.UpdateTagMap(gameplayTag, -1);
            }
        }

        public void SetTarget(IAbilitySystemController target)
        {
            _target = target;
        }
    }
}