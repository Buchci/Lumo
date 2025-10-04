using Lumo.Data;
using Lumo.DTOs.DiaryEntry;
using Lumo.Models;
using Microsoft.EntityFrameworkCore;

public class DiaryService
{
    private readonly ApplicationDbContext _db;

    public DiaryService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<DiaryEntry>> GetUserEntriesAsync(string userId)
    {
        return await _db.DiaryEntries
            .Include(d => d.Tags)
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.EntryDate)
            .ToListAsync();
    }

    public async Task<DiaryEntry> CreateEntryAsync(string userId, CreateDiaryEntryDto dto)
    {
        var entry = new DiaryEntry
        {
            Title = dto.Title,
            Content = dto.Content,
            EntryDate = dto.EntryDate,
            MoodRating = dto.MoodRating,
            IsFavorite = dto.IsFavorite,
            UserId = userId,
            Tags = await _db.Tags.Where(t => dto.TagIds.Contains(t.Id)).ToListAsync()
        };

        _db.DiaryEntries.Add(entry);
        await _db.SaveChangesAsync();
        return entry;
    }

    public async Task<DiaryEntry?> UpdateEntryAsync(int id, string userId, UpdateDiaryEntryDto dto)
    {
        var entry = await _db.DiaryEntries.Include(d => d.Tags)
            .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

        if (entry == null) return null;

        if (!string.IsNullOrEmpty(dto.Title)) entry.Title = dto.Title;
        if (!string.IsNullOrEmpty(dto.Content)) entry.Content = dto.Content;
        if (dto.EntryDate.HasValue) entry.EntryDate = dto.EntryDate.Value;
        if (dto.MoodRating.HasValue) entry.MoodRating = dto.MoodRating.Value;
        if (dto.IsFavorite.HasValue) entry.IsFavorite = dto.IsFavorite.Value;
        if (dto.TagIds != null)
            entry.Tags = await _db.Tags.Where(t => dto.TagIds.Contains(t.Id)).ToListAsync();

        entry.LastModifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return entry;
    }

    public async Task<bool> DeleteEntryAsync(int id, string userId)
    {
        var entry = await _db.DiaryEntries.FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);
        if (entry == null) return false;

        _db.DiaryEntries.Remove(entry);
        await _db.SaveChangesAsync();
        return true;
    }
    public async Task<DiaryEntry?> GetEntryByIdAsync(string userId, int id)
    {
        return await _db.DiaryEntries
            .Include(d => d.Tags)
            .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);
    }
}
