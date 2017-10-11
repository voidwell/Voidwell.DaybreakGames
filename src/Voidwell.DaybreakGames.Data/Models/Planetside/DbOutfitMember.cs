using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("OutfitMember")]
    public class DbOutfitMember
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public string OutfitId { get; set; }

        public DateTime MemberSinceDate { get; set; }
        public string Rank { get; set; }
        public int RankOrdinal { get; set; }

        public DbCharacter Character { get; set; }
        public DbOutfit Outfit { get; set; }
    }
}
