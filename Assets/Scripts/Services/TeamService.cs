using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Services
{
    /// <summary>
    /// Provides team management functionality including adding players to teams,
    /// retrieving team rosters, and checking team membership.
    /// Designed to manage game objects grouped into predefined teams.
    /// </summary>
    public class TeamService : ITeamService
    {
        private Dictionary<GameObject, Team> _playerTeamDictionary;
        private Dictionary<Team, List<GameObject>> _teamCache;
        private Dictionary<Team, HashSet<GameObject>> _teamLookup;
        
        public void Initialize()
        {
            _playerTeamDictionary = new Dictionary<GameObject, Team>();
            _teamCache = new Dictionary<Team, List<GameObject>>
            {
                { Team.A, new List<GameObject>() },
                { Team.B, new List<GameObject>() }
            };
            _teamLookup = new Dictionary<Team, HashSet<GameObject>>
            {
                { Team.A, new HashSet<GameObject>() },
                { Team.B, new HashSet<GameObject>() }
            };
        }

        public void PostInitialize()
        {
        }

        public void Shutdown()
        {
            _playerTeamDictionary.Clear();
            _teamCache[Team.A].Clear();
            _teamCache[Team.B].Clear();
            _teamLookup[Team.A].Clear();
            _teamLookup[Team.B].Clear();
        }

        public string GetServiceName()
        {
            return nameof(TeamService);
        }

        public void AddPlayerToTeam(GameObject pawn, Team team)
        {
            if (pawn == null)
                throw new ArgumentNullException(nameof(pawn));

            if (_playerTeamDictionary.ContainsKey(pawn))
                throw new InvalidOperationException($"Player {pawn.name} already assigned to a team");

            _playerTeamDictionary.Add(pawn, team);
        
            // Update caches
            _teamCache[team].Add(pawn);
            _teamLookup[team].Add(pawn);
        }

        public List<GameObject> GetPlayerTeam(Team team)
        {
            return _teamCache[team];
        }

        public List<GameObject> GetPlayerTeam(GameObject pawn)
        {
            return _playerTeamDictionary.TryGetValue(pawn, out var team) ? GetPlayerTeam(team) : null;
        }

        public bool IsPlayerInTeam(GameObject pawn)
        {
            return pawn != null && _playerTeamDictionary.ContainsKey(pawn);
        }

        public bool TryGetPlayerTeam(GameObject pawn, out Team team)
        {
            return _playerTeamDictionary.TryGetValue(pawn, out team);
        }
    }
}