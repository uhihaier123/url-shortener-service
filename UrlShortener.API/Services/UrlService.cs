using Microsoft.EntityFrameworkCore;
using UrlShortener.API.Data;
using UrlShortener.API.Models;

namespace UrlShortener.API.Services
{
    public class UrlService : IUrlService
    {
        private readonly ApplicationDbContext _context;
        private const string Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int ShortCodeLength = 6;

        public UrlService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Shorten a URL
        public async Task<Url> ShortenUrlAsync(string originalUrl)
        {
            // Generate a unique short code
            string shortCode;
            bool isUnique;

            do
            {
                shortCode = GenerateShortCode();
                // Check if this code already exists
                isUnique = !await _context.Urls.AnyAsync(u => u.ShortCode == shortCode);
            }
            while (!isUnique); // Keep generating until we get a unique code

            // Create new URL entry
            var url = new Url
            {
                OriginalUrl = originalUrl,
                ShortCode = shortCode,
                CreatedAt = DateTime.UtcNow,
                ClickCount = 0
            };

            // Save to database
            _context.Urls.Add(url);
            await _context.SaveChangesAsync();

            return url;
        }

        // Get URL by short code
        public async Task<Url?> GetUrlByShortCodeAsync(string shortCode)
        {
            return await _context.Urls
                .FirstOrDefaultAsync(u => u.ShortCode == shortCode);
        }

        // Increment click count
        public async Task IncrementClickCountAsync(string shortCode)
        {
            var url = await _context.Urls
                .FirstOrDefaultAsync(u => u.ShortCode == shortCode);

            if (url != null)
            {
                url.ClickCount++;
                await _context.SaveChangesAsync();
            }
        }

        // Generate a random short code
        private string GenerateShortCode()
        {
            var random = new Random();
            var code = new char[ShortCodeLength];

            for (int i = 0; i < ShortCodeLength; i++)
            {
                code[i] = Characters[random.Next(Characters.Length)];
            }

            return new string(code);
        }
    }
}