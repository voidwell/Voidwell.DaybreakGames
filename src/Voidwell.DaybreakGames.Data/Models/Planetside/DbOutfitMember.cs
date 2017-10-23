using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("OutfitMember")]
    public class DbOutfitMember : IDbModel<DbOutfitMember>
    {
        [Key]
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public string OutfitId { get; set; }

        public DateTime MemberSinceDate { get; set; }
        public string Rank { get; set; }
        public int RankOrdinal { get; set; }

        [ForeignKey("CharacterId")]
        public DbCharacter Character { get; set; }
        [ForeignKey("OutfitId")]
        public DbOutfit Outfit { get; set; }

        public Expression<Func<DbOutfitMember, bool>> Predicate { get => (a => a.CharacterId == CharacterId && a.OutfitId == OutfitId); }
    }
}
