using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("PS2UpdaterScheduler")]
    public class DbPS2UpdaterScheduler : IDbModel<DbPS2UpdaterScheduler>
    {
        [Key]
        [Required]
        public string ServiceName { get; set; }
        [Required]
        public DateTime LastUpdateDate { get; set; }

        public Expression<Func<DbPS2UpdaterScheduler, bool>> Predicate => (a => a.ServiceName == ServiceName);
    }
}
