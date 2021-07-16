using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaskTrackerApi.Domain
{
    public class Card
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
        public virtual List<CardTag> Tags { get; set; }
    }
}
