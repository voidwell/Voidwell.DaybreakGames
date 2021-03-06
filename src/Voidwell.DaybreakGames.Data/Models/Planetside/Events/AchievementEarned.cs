﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside.Events
{
    public class AchievementEarned
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public int WorldId { get; set; }
        public int ZoneId { get; set; }
        public int AchievementId { get; set; }
    }
}
