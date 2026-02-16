using Lumo.DTOs.DiaryEntry;
using Lumo.Models;
using Microsoft.Extensions.Localization;

namespace Lumo.Helpers
{
    public class DiaryMapper
    {
        private readonly IStringLocalizer _localizer;

        public DiaryMapper(IStringLocalizerFactory factory)
        {
            // Dokładnie ta sama logika zasobów co u Ciebie
            var assemblyName = typeof(Program).Assembly.GetName().Name!;
            _localizer = factory.Create("Tags", assemblyName);
        }

        public DiaryEntryReadDto MapToReadDto(DiaryEntry entry)
        {
            return new DiaryEntryReadDto
            {
                Id = entry.Id,
                Title = entry.Title,
                Content = entry.Content,
                EntryDate = entry.EntryDate,
                MoodRating = entry.MoodRating,
                IsFavorite = entry.IsFavorite,
                // TWOJA LOGIKA TAGÓW:
                Tags = entry.Tags.Select(t =>
                {
                    return t.CustomName
                           ?? (t.IsGlobal ? _localizer[t.ResourceKey!].Value : t.ResourceKey);
                }).ToList()
            };
        }
    }
}