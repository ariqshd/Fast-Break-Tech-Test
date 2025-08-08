using System.Collections.Generic;
using Core;
using Data;
using UnityEngine;

namespace Services
{
    public interface ITeamService : IService
    {
        public void AddPlayerToTeam(GameObject pawn, Team team);
        public List<GameObject> GetPlayerTeam(Team team);
        public List<GameObject> GetPlayerTeam(GameObject pawn);
        public bool IsPlayerInTeam(GameObject pawn);
        public bool TryGetPlayerTeam(GameObject pawn, out Team team);
    }
}