namespace UrlShortener.API.Models
{
    public class Url
    {
        // Primary Key - Auto-incrementing ID
        public int Id { get; set; }

        // The original long URL
        public string OriginalUrl { get; set; } = string.Empty;

        // The unique short code (e.g., "abc123")
        public string ShortCode { get; set; } = string.Empty;

        // When this URL was created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // How many times this short URL was clicked
        public int ClickCount { get; set; } = 0;
    }
}