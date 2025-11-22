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
        public DbSet<DiaryEntry> DiaryEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Unikalność nazwy taga w obrębie użytkownika
            builder.Entity<Tag>()
                .HasIndex(t => new {t.UserId, t.CustomName })
                .IsUnique();

            // Relacja: User → Tag
            builder.Entity<Tag>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacja: User → DiaryEntry
            builder.Entity<DiaryEntry>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<DiaryEntry>()
                .HasIndex(d => new { d.UserId, d.EntryDate })
                .IsUnique();
            // DiaryEntry ↔ Tag (many-to-many)
            builder.Entity<DiaryEntry>()
                .HasMany(d => d.Tags)
                .WithMany(t => t.Entries)
                .UsingEntity<Dictionary<string, object>>(
                    "DiaryEntryTags",
                    j => j.HasOne<Tag>()
                          .WithMany()
                          .HasForeignKey("TagsId")
                          .OnDelete(DeleteBehavior.Restrict), 
                    j => j.HasOne<DiaryEntry>()
                          .WithMany()
                          .HasForeignKey("EntriesId")
                          .OnDelete(DeleteBehavior.Cascade) 
                );
            // Dane początkowe (systemowe tagi)
            builder.Entity<Tag>().HasData(
                new Tag { Id = 1, ResourceKey = "Tag.Work", IsGlobal = true },
                new Tag { Id = 2, ResourceKey = "Tag.Family", IsGlobal = true },
                new Tag { Id = 3, ResourceKey = "Tag.Health", IsGlobal = true }
            );
        }
    }
}
