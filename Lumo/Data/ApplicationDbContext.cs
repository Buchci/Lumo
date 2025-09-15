using Lumo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lumo.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Unikalność nazwy w obrębie użytkownika
            builder.Entity<Tag>()
                .HasIndex(t => new { t.ResourceKey, t.UserId, t.CustomName})
                .IsUnique();

            // Relacja: User → Tag
            builder.Entity<Tag>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Dane początkowe (systemowe tagi)
            builder.Entity<Tag>().HasData(
                new Tag { Id = 1, ResourceKey = "Tag.Work", IsGlobal = true },
                new Tag { Id = 2, ResourceKey = "Tag.Family", IsGlobal = true },
                new Tag { Id = 3, ResourceKey = "Tag.Health", IsGlobal = true }
            );
        }
    }
}
