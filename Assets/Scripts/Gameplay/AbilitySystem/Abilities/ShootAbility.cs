using System.Threading.Tasks;
using Core;
using Data;
using Gameplay.AbilitySystem.Tasks;
using Services;
using UnityEngine;

namespace Gameplay.AbilitySystem.Abilities
{
    [CreateAssetMenu(fileName = nameof(ShootAbility), menuName = "AbilitySystem/Abilities/" + nameof(ShootAbility), order = 0)]
    public class ShootAbility : GameplayAbility
    {
        [SerializeField] private float angle = 60f;
        [SerializeField] private float forceMultiplier = .78f;
        private IEntityPositionService _entityPositionService;
        private IGameplayEventService _gameplayEventService;

        private Vector3 _basketPosition;

        public override GameplayAbility CreateInstanceCopy()
        {
            var instance = base.CreateInstanceCopy() as ShootAbility;
            if (instance == null) return null;
            instance.angle = angle;
            instance.forceMultiplier = forceMultiplier;
            return instance;
        }

        protected override void Initialize()
        {
            base.Initialize();
            _entityPositionService = ServiceLocator.Get<IEntityPositionService>();
            _gameplayEventService = ServiceLocator.Get<IGameplayEventService>();
        }

        public override async Task ActivateAbility()
        {
            await base.ActivateAbility();
            
            _basketPosition = _entityPositionService.GetBasketPosition();
            _gameplayEventService.Subscribe(GameplayTags.Event_ShootBall, obj => _ = OnShootEvent(obj));
        }

        private async Task OnShootEvent(object o)
        {
            _gameplayEventService.Unsubscribe(GameplayTags.Event_ShootBall, obj => _ = OnShootEvent(obj));
            var ball = o as IBallController;
            
            Vector3 startPos = OwnerActor.transform.position + OwnerActor.transform.up * 2f;
            ball?.MovePhysics(startPos, _basketPosition, angle, forceMultiplier);
            
            Debug.Log("Passing ball");
            await TaskCoroutine.WaitForSeconds(.5f);
            OwnerActor.GetComponent<IPawnController>().SetInteractionColliderTrigger(true);
            await EndAbility();
        }
        
        public override async Task EndAbility()
        {
            await base.EndAbility();
            _basketPosition = Vector3.zero;
        }

    }
}