﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside.Events
{
    public class Death
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public string AttackerCharacterId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public int WorldId { get; set; }
        public int ZoneId { get; set; }
        public int? CharacterLoadoutId { get; set; }
        public string CharacterOutfitId { get; set; }
        public int? AttackerFireModeId { get; set; }
        public int? AttackerLoadoutId { get; set; }
        public int? AttackerVehicleId { get; set; }
        public int? AttackerWeaponId { get; set; }
        public string AttackerOutfitId { get; set; }
        public bool IsHeadshot { get; set; }

        public Character Character { get; set; }
        public Character AttackerCharacter { get; set; }
        public Outfit CharacterOutfit { get; set; }
        public Outfit AttackerOutfit { get; set; }
        public Vehicle AttackerVehicle { get; set; }
        public Item AttackerWeapon { get; set; }
    }
}
