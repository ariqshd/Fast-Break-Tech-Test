namespace Data
{
    /// <summary>
    /// Centralized tag definitions
    /// </summary>
    public static class GameplayTags
    {
        public static readonly GameplayTag State = new GameplayTag("State");
        public static readonly GameplayTag State_Player = new GameplayTag("State_Player");
        public static readonly GameplayTag State_Player_PossessBall = new GameplayTag("State_Player_PossessBall");
        
        public static readonly GameplayTag Team = new GameplayTag("Team");
        public static readonly GameplayTag Team_TeamA = new GameplayTag("TeamA");
        public static readonly GameplayTag Team_TeamB = new GameplayTag("TeamB");
        
        public static readonly GameplayTag Event = new GameplayTag("Event");
        public static readonly GameplayTag Event_PassBall = new GameplayTag("Event_PassBall");
        public static readonly GameplayTag Event_ShootBall = new GameplayTag("Event_ShootBall");
        public static readonly GameplayTag Event_Score = new GameplayTag("Event_Score");
    }
}