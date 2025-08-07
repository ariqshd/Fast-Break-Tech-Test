using System.Threading.Tasks;

namespace Gameplay.AbilitySystem.Abilities
{
    public interface IGameplayAbility
    {
        public Task ActivateAbility();
        public bool CanActivateAbility();
    }
}