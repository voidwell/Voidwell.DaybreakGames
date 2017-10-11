using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("Character")]
    public class DbCharacter
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

        public DbTitle Title { get; set; }
        public DbWorld World { get; set; }
        public DbFaction Faction { get; set; }
        public DbCharacterTime Time { get; set; }
        public DbOutfitMember OutfitMembership { get; set; }
        public DbCharacterStat Stats { get; set; }
        public DbCharacterStatByFaction StatsByFaction { get; set; }
        public DbCharacterWeaponStat WeaponStats { get; set; }
    }
}
