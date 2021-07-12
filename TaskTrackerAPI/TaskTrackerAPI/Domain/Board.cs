using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TaskTrackerApi.Domain
{
    public class Board
    {
        [Key]
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        public string UserId { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
        
        public virtual List<Card> Cards { get; set; }
    }
}