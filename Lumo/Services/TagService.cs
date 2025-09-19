using Lumo.Data;
using Lumo.Models;
using Microsoft.EntityFrameworkCore;

namespace Lumo.Services
{
    public class TagService
    {
        private readonly ApplicationDbContext _db;

        public TagService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Tag>> GetUserTagsAsync(string userId)
        {
            return await _db.Tags
                .Where(t => t.IsGlobal || t.UserId == userId)
                .ToListAsync();
        }

        public async Task<Tag> CreateTagAsync(string userId, string? resourceKey, string? customName, bool isGlobal = false)
        {
            var tag = new Tag
            {
                ResourceKey = resourceKey,
                CustomName = customName,
                IsGlobal = isGlobal,
                UserId = isGlobal ? null : userId
            };

            _db.Tags.Add(tag);
            await _db.SaveChangesAsync();
            return tag;
        }
        public async Task<Tag?> UpdateTagAsync(int id, string userId, string? customName)
        {
            var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (tag == null) return null;

            if (!string.IsNullOrEmpty(customName))
                tag.CustomName = customName;

            await _db.SaveChangesAsync();
            return tag;
        }

        public async Task<bool> DeleteTagAsync(int id, string userId)
        {
            var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (tag == null) return false;

            _db.Tags.Remove(tag);
            await _db.SaveChangesAsync();
            return true;
        }
    }

}
