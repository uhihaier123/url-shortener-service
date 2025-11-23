using Microsoft.AspNetCore.Mvc;
using UrlShortener.API.Services;

namespace UrlShortener.API.Controllers
{
    [ApiController]
    public class RedirectController : ControllerBase
    {
        private readonly IUrlService _urlService;

        public RedirectController(IUrlService urlService)
        {
            _urlService = urlService;
        }

        // GET: /{shortCode}
        // Redirects to the original URL
        [HttpGet("/{shortCode}")]
        public async Task<IActionResult> RedirectToUrl(string shortCode)
        {
            // Find the URL in the database
            var url = await _urlService.GetUrlByShortCodeAsync(shortCode);

            if (url == null)
            {
                // Short code not found
                return NotFound(new { error = "Short URL not found" });
            }

            // Increment the click counter
            await _urlService.IncrementClickCountAsync(shortCode);

            // Redirect to the original URL (HTTP 302 redirect)
            return Redirect(url.OriginalUrl);
        }
    }
}