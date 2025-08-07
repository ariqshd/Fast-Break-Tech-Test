using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Gameplay.AbilitySystem
{
    [CreateAssetMenu(fileName = nameof(AbilityInputBindingData), menuName = "AbilitySystem/Data/" + nameof(AbilityInputBindingData), order = 0)]
    public class AbilityInputBindingData : ScriptableObject
    {
        [SerializeField]
        private List<AbilityInputBinding> abilityInputBindings;

        public List<AbilityInputBinding> Data => abilityInputBindings;

        public List<AbilityInputBindingGuid> ConvertToGuidBindings()
        {
            var guidBindings = new List<AbilityInputBindingGuid>();
        
            if (abilityInputBindings != null)
            {
                foreach (var binding in abilityInputBindings)
                {
                    var guidBinding = new AbilityInputBindingGuid
                    {
                        Ability = binding.Ability,
                        InputActionId = binding.ActionReference.action.id
                    };
                    guidBindings.Add(guidBinding);
                }
            }
        
            return guidBindings;
        }
    }
}