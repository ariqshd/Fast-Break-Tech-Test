using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Services
{
    public interface ITeamService : IService
    {
        public void AddPlayerToTeam(GameObject pawn, Team team);
        public List<GameObject> GetPlayerTeam(Team team);
        public bool IsPlayerInTeam(GameObject pawn);
        public bool TryGetPlayerTeam(GameObject pawn, out Team team);
    }
}