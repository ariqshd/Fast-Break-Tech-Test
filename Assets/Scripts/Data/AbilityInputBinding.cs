using System;
using Gameplay.AbilitySystem.Abilities;
using UnityEngine.InputSystem;

namespace Data
{
    [Serializable]
    public struct AbilityInputBinding
    {
        public GameplayAbility Ability;
        public InputActionReference ActionReference;
    }

    [Serializable]
    public struct AbilityInputBindingGuid
    {
        public GameplayAbility Ability;
        public Guid InputActionId;
    }
}