using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TaskTrackerApi.Domain
{
    public class Member
    {
        [Key]
        public string Email { get; set; }
    }
}