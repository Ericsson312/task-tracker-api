using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaskTrackerApi.Domain
{
    public class TaskToDoTag
    {
        [ForeignKey(nameof(TagName))]
        public Tag Tag { get; set; }
        public string TagName { get; set; }
        public virtual TaskToDo TaskToDo { get; set; }
        public Guid TaskToDoId { get; set; }
    }
}
