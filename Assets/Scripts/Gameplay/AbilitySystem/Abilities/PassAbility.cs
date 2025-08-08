using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Data;
using Gameplay.AbilitySystem.Tasks;
using Services;
using UnityEngine;

namespace Gameplay.AbilitySystem.Abilities
{
    [CreateAssetMenu(fileName = nameof(PassAbility), menuName = "AbilitySystem/Abilities/" + nameof(PassAbility), order = 0)]
    public class PassAbility : GameplayAbility
    {
        [SerializeField] private float angle = 45f;
        [SerializeField] private float forceMultiplier = .78f;
        private ITeamService _teamService;
        private IGameplayEventService _gameplayEventService;
        private Transform _closestTeammate;

        public override GameplayAbility CreateInstanceCopy()
        {
            var instance = base.CreateInstanceCopy() as PassAbility;
            if (instance == null) return null;
            instance.angle = angle;
            instance.forceMultiplier = forceMultiplier;

            return instance;
        }

        protected override void Initialize()
        {
            base.Initialize();
            _teamService = ServiceLocator.Get<ITeamService>();
            _gameplayEventService = ServiceLocator.Get<IGameplayEventService>();
            
        }

        public override async Task ActivateAbility()
        {
            await base.ActivateAbility();

            List<GameObject> teamPawns = _teamService.GetPlayerTeam(OwnerActor);

            List<GameObject> foundTeamPawns = new List<GameObject>();
            foreach (var pawn in teamPawns)
            {
               if(pawn == OwnerActor) continue;
               foundTeamPawns.Add(pawn);
            }
            
            // If no other teammates, exit early
            if (foundTeamPawns.Count == 0)
            {
                Debug.Log("No teammates to pass to.");
                CancelAbility();
                return;
            }
            
            // Find the closest pawn
            GameObject closestPawn = null;
            float closestDistance = float.MaxValue;
            
            foreach (var pawn in foundTeamPawns)
            {
                float distance = Vector3.Distance(OwnerActor.transform.position, pawn.transform.position);
        
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPawn = pawn;
                }
            }

            // found the closest teammate
            if (closestPawn != null)
            {
                _closestTeammate= closestPawn.transform;
                OwnerActor.transform.LookAt(closestPawn.transform, Vector3.up);
                
                // Example: Log the pass target
                Debug.Log($"Pass ability activated: Passing to {closestPawn.name} at distance {closestDistance:F2}");
            }
            
            _gameplayEventService.Subscribe(GameplayTags.Event_PassBall, obj => _ = OnPassEvent(obj));
        }

        private async Task OnPassEvent(object o)
        {
            _gameplayEventService.Unsubscribe(GameplayTags.Event_PassBall, x => _ = OnPassEvent(x));

            var ball = o as IBallController;
            
            Vector3 startPos = OwnerActor.transform.position + OwnerActor.transform.up * 2f;
            // ball?.Move(startPos, _closestTeammate.position, 1);
            ball?.MovePhysics(startPos, _closestTeammate.position, angle, forceMultiplier);

            Debug.Log("Passing ball");
            await TaskCoroutine.WaitForSeconds(.5f);
            OwnerActor.GetComponent<IPawnController>().SetInteractionColliderTrigger(true);
            
            await EndAbility();
        }
        
        public override async Task EndAbility()
        {
            await base.EndAbility();
            _closestTeammate = null;
        }

        public override void CancelAbility()
        {
            base.CancelAbility();
        }
    }
}