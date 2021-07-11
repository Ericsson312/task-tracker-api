using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Data
{
    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<Card> TasksToDo { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<CardTag> TaskToDoTags { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<CardTag>().Ignore(xx => xx.Card).HasKey(x => new { x.CardId, x.TagName });
        }
    }
}
