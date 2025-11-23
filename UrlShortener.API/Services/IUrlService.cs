using UrlShortener.API.Models;

namespace UrlShortener.API.Services
{
    public interface IUrlService
    {
        // Create a shortened URL
        Task<Url> ShortenUrlAsync(string originalUrl);

        // Get original URL by short code
        Task<Url?> GetUrlByShortCodeAsync(string shortCode);

        // Increment click count when URL is accessed
        Task IncrementClickCountAsync(string shortCode);
    }
}