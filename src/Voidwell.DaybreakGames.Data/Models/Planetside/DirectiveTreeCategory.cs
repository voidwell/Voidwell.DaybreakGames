using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class DirectiveTreeCategory
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<DirectiveTree> Trees { get; set; }
    }
}
