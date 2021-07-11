using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaskTrackerApi.Domain
{
    public class CardTag
    {
        [ForeignKey(nameof(TagName))]
        public Tag Tag { get; set; }
        public string TagName { get; set; }
        public virtual Card Card { get; set; }
        public Guid CardId { get; set; }
    }
}
