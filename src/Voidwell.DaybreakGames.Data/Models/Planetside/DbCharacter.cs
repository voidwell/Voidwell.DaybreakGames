using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("Character")]
    public class DbCharacter : IDbModel<DbCharacter>
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }

        public string FactionId { get; set; }
        public string TitleId { get; set; }
        public string WorldId { get; set; }
        public int BattleRank { get; set; }
        public int BattleRankPercentToNext { get; set; }
        public int CertsEarned { get; set; }

        [ForeignKey("TitleId")]
        public DbTitle Title { get; set; }
        [ForeignKey("WorldId")]
        public DbWorld World { get; set; }
        [ForeignKey("FactionId")]
        public DbFaction Faction { get; set; }
        [ForeignKey("Id")]
        public DbCharacterTime Time { get; set; }
        [ForeignKey("Id")]
        public DbOutfitMember OutfitMembership { get; set; }
        [ForeignKey("Id")]
        public IEnumerable<DbCharacterStat> Stats { get; set; }
        [ForeignKey("Id")]
        public IEnumerable<DbCharacterStatByFaction> StatsByFaction { get; set; }
        [ForeignKey("Id")]
        public IEnumerable<DbCharacterWeaponStat> WeaponStats { get; set; }
        [ForeignKey("Id")]
        public IEnumerable<DbCharacterWeaponStatByFaction> WeaponStatsByFaction { get; set; }

        public Expression<Func<DbCharacter, bool>> Predicate { get => (a => a.Id == Id); }
    }
}
