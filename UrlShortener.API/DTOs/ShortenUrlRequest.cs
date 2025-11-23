using System.ComponentModel.DataAnnotations;

namespace UrlShortener.API.DTOs
{
    public class ShortenUrlRequest
    {
        [Required(ErrorMessage = "URL is required")]
        [Url(ErrorMessage = "Invalid URL format")]
        [StringLength(2048, MinimumLength = 10, ErrorMessage = "URL must be between 10 and 2048 characters")]
        public string Url { get; set; } = string.Empty;
    }
}