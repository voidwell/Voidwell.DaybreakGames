using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("Outfit")]
    public class DbOutfit
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LeaderCharacterId { get; set; }
        public int MemberCount { get; set; }
        public string FactionId { get; set; }
        public string WorldId { get; set; }

        [ForeignKey("FactionId")]
        public DbFaction Faction { get; set; }
        [ForeignKey("WorldId")]
        public DbWorld World { get; set; }
        [ForeignKey("LeaderCharacterId")]
        public DbCharacter Leader { get; set; }
    }
}
