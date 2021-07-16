using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskTrackerApi.Domain
{
    public class BoardMember
    {
        [ForeignKey(nameof(MemberEmail))]
        public virtual Member Member { get; set; }
        public string MemberEmail { get; set; }
        public virtual Board Board { get; set; }
        public Guid BoardId { get; set; }
    }
}