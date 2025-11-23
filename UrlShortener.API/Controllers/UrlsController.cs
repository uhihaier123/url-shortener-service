using Microsoft.AspNetCore.Mvc;
using UrlShortener.API.DTOs;
using UrlShortener.API.Services;

namespace UrlShortener.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlsController : ControllerBase
    {
        private readonly IUrlService _urlService;
        private readonly IConfiguration _configuration;

        // Constructor - dependency injection
        public UrlsController(IUrlService urlService, IConfiguration configuration)
        {
            _urlService = urlService;
            _configuration = configuration;
        }

        // POST: api/urls/shorten
        // Creates a shortened URL
        [HttpPost("shorten")]
        public async Task<ActionResult<ShortenUrlResponse>> ShortenUrl([FromBody] ShortenUrlRequest request)
        {
            // Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Call the service to create shortened URL
                var url = await _urlService.ShortenUrlAsync(request.Url);

                // Build the short URL (e.g., https://localhost:7001/abc123)
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var shortUrl = $"{baseUrl}/{url.ShortCode}";

                // Create response
                var response = new ShortenUrlResponse
                {
                    Id = url.Id,
                    OriginalUrl = url.OriginalUrl,
                    ShortCode = url.ShortCode,
                    ShortUrl = shortUrl,
                    CreatedAt = url.CreatedAt,
                    ClickCount = url.ClickCount
                };

                // Return 201 Created status with the response
                return CreatedAtAction(nameof(GetUrlStats), new { shortCode = url.ShortCode }, response);
            }
            catch (Exception ex)
            {
                // Log the error (in production, use proper logging)
                return StatusCode(500, new { error = "An error occurred while shortening the URL", details = ex.Message });
            }
        }

        // GET: api/urls/{shortCode}/stats
        // Get statistics for a shortened URL
        [HttpGet("{shortCode}/stats")]
        public async Task<ActionResult<ShortenUrlResponse>> GetUrlStats(string shortCode)
        {
            var url = await _urlService.GetUrlByShortCodeAsync(shortCode);

            if (url == null)
            {
                return NotFound(new { error = "Short URL not found" });
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var shortUrl = $"{baseUrl}/{url.ShortCode}";

            var response = new ShortenUrlResponse
            {
                Id = url.Id,
                OriginalUrl = url.OriginalUrl,
                ShortCode = url.ShortCode,
                ShortUrl = shortUrl,
                CreatedAt = url.CreatedAt,
                ClickCount = url.ClickCount
            };

            return Ok(response);
        }
    }
}