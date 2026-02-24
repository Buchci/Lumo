using Lumo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lumo.Data
{
    public static class DbInitializer
    {
        public static async Task SeedData(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Upewnij się, że baza jest utworzona
            context.Database.EnsureCreated();

            // Tworzenie Użytkownika (jeśli nie istnieje)
            var userEmail = "user@example.com";
            var user = await userManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = "user@example.com",
                    Email = userEmail,
                    EmailConfirmed = true,
                    Nickname = "user" // Twój customowy property
                };

                var result = await userManager.CreateAsync(user, "zaq1@WSX");
                if (!result.Succeeded)
                {
                    throw new Exception("Nie udało się utworzyć użytkownika testowego: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            // Sprawdzamy, czy użytkownik ma już jakieś wpisy.
            if (context.DiaryEntries.Any(e => e.UserId == user.Id))
            {
                return;
            }

            // Pobieramy globalne tagi z bazy
            var allTags = context.Tags.Where(t => t.IsGlobal).ToList();

            // customowe tagi usera
            var customTags = new List<Tag>
            {
                new Tag { CustomName = "Natura 🌿", UserId = user.Id, IsGlobal = false },
                new Tag { CustomName = "Kodowanie 💻", UserId = user.Id, IsGlobal = false },
                new Tag { CustomName = "Siłownia 🏋️", UserId = user.Id, IsGlobal = false },
                new Tag { CustomName = "Gaming 🎮", UserId = user.Id, IsGlobal = false },
                new Tag { CustomName = "Stres 😫", UserId = user.Id, IsGlobal = false },
                new Tag { CustomName = "Impreza 🍻", UserId = user.Id, IsGlobal = false },
                new Tag { CustomName = "Książka 📖", UserId = user.Id, IsGlobal = false }
            };

            context.Tags.AddRange(customTags);
            await context.SaveChangesAsync(); 

            allTags.AddRange(customTags);

            // Generowanie wpisów 
            var entries = new List<DiaryEntry>();
            var random = new Random();
            var startDate = DateTime.Today.AddMonths(-6);
            var today = DateTime.Today;

            // Pętla dzień po dniu
            for (var date = startDate; date <= today; date = date.AddDays(1))
            {
                if (random.NextDouble() > 0.85) continue;

                // Weekendy mają bonus do nastroju
                int baseMood = (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) ? 4 : 3;
                int moodRating = Math.Clamp(baseMood + random.Next(-2, 3), 1, 5);

                // Losowanie tagów
                var entryTags = allTags.OrderBy(x => random.Next()).Take(random.Next(1, 4)).ToList();

                var entry = new DiaryEntry
                {
                    Title = GetRandomTitle(moodRating, random),
                    Content = GetRandomContent(moodRating, random),
                    EntryDate = date, 
                    CreatedAt = date.AddHours(random.Next(18, 23)), 
                    MoodRating = moodRating,
                    IsFavorite = (moodRating == 5 && random.NextDouble() > 0.7),
                    UserId = user.Id,
                    Tags = entryTags
                };

                entries.Add(entry);
            }

            context.DiaryEntries.AddRange(entries);
            await context.SaveChangesAsync();
        }

        // Pomocnicze metody do generowania tekstu
        private static string GetRandomTitle(int mood, Random r)
        {
            var goodTitles = new[] { "Świetny dzień!", "Mały sukces", "W końcu weekend", "Spotkanie z przyjaciółmi", "Produktywność level hard", "Spacerek", "Dobra energia" };
            var neutralTitles = new[] { "Zwyczajny wtorek", "Praca, praca...", "Nic specjalnego", "Dzień jak co dzień", "Rutyna", "Spokojny wieczór" };
            var badTitles = new[] { "Ciężki poranek", "Stresująca sytuacja", "Chcę już spać", "Wszystko idzie nie tak", "Ból głowy", "Trudna rozmowa" };

            return mood switch
            {
                >= 4 => goodTitles[r.Next(goodTitles.Length)],
                3 => neutralTitles[r.Next(neutralTitles.Length)],
                _ => badTitles[r.Next(badTitles.Length)]
            };
        }

        private static string GetRandomContent(int mood, Random r)
        {
            var intros = new[] { "Dzisiaj obudziłem się wcześnie.", "Dzień zaczął się powoli.", "To był szalony dzień.", "Nie mam zbyt wiele do napisania, ale..." };
            var middles = new[]
            {
                "Udało mi się zrobić większość rzeczy z listy zadań. Cieszy mnie postęp w projekcie Lumo.",
                "Poszedłem na długi spacer, żeby przewietrzyć głowę. Pogoda była znośna.",
                "W pracy totalny chaos, ale jakoś to ogarnąłem. Kawa uratowała mi życie.",
                "Spotkałem się ze znajomymi, dawno się tak nie uśmiałem.",
                "Czasami zastanawiam się, dokąd to wszystko zmierza. Ale jest okej."
            };
            var outros = new[] { "Zobaczymy co przyniesie jutro.", "Idę spać, dobranoc.", "Jutro muszę wstać wcześniej.", "Oby weekend przyszedł szybciej." };

            return $"{intros[r.Next(intros.Length)]} {middles[r.Next(middles.Length)]} {outros[r.Next(outros.Length)]}";
        }
    }
}