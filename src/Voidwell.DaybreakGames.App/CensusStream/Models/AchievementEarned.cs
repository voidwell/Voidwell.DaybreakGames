﻿namespace Voidwell.DaybreakGames.CensusStream.Models
{
    public class AchievementEarned : PayloadBase
    {
        public string CharacterId { get; set; }
        public int AchievementId { get; set; }
    }
}
