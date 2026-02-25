using Lumo.Data;
using Lumo.DTOs.Tag;
using Lumo.Models;
using Microsoft.EntityFrameworkCore;

namespace Lumo.Services
{
    public interface ITagService
    {
        Task<List<Tag>> GetUserTagsAsync(string userId);
        Task<Tag> CreateTagAsync(string userId, CreateTagDto dto);
        Task<Tag?> UpdateTagAsync(int id, string userId, UpdateTagDto dto);
        Task<bool> DeleteTagAsync(int id, string userId);
    }

    public class TagService : ITagService
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

        public async Task<Tag> CreateTagAsync(string userId, CreateTagDto dto)
        {
            // Najpierw sprawdzamy warunek błędu (tzw. "fail-fast")

            var tag = new Tag
            {
                ResourceKey = null,
                CustomName = dto.CustomName,
                IsGlobal = false,
                UserId = userId
            };

            _db.Tags.Add(tag);
            await _db.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag?> UpdateTagAsync(int id, string userId, UpdateTagDto dto)
        {
            var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (tag == null) return null;

            // Używamy pola z przekazanego DTO
            if (!string.IsNullOrEmpty(dto.CustomName))
                tag.CustomName = dto.CustomName;

            await _db.SaveChangesAsync();
            return tag;
        }

        public async Task<bool> DeleteTagAsync(int id, string userId)
        {
            var tag = await _db.Tags
                .Include(t => t.Entries)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (tag == null)
                return false;

            tag.Entries.Clear();
            await _db.SaveChangesAsync();

            _db.Tags.Remove(tag);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}