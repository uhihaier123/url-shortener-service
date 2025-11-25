using Microsoft.EntityFrameworkCore;
using UrlShortener.API.Data;
using UrlShortener.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (builder.Environment.IsProduction())
    {
        // Use SQLite in Render
        options.UseSqlite("Data Source=urlshortener.db");
    }
    else
    {
        // Use SQL Server locally
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

// Register the URL service
builder.Services.AddScoped<IUrlService, UrlService>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Use CORS
app.UseCors("AllowAll");

// Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI();

// Only use HTTPS redirection in development
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

// **IMPORTANT: Add root endpoint BEFORE MapControllers**
app.MapGet("/", () => Results.Content(@"
<!DOCTYPE html>
<html>
<head>
    <title>URL Shortener API</title>
    <style>
        body { 
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; 
            max-width: 900px; 
            margin: 50px auto; 
            padding: 20px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
        }
        .container {
            background: white;
            border-radius: 10px;
            padding: 40px;
            box-shadow: 0 10px 40px rgba(0,0,0,0.2);
        }
        h1 { 
            color: #333; 
            margin-bottom: 10px;
        }
        .subtitle {
            color: #666;
            margin-bottom: 30px;
        }
        .endpoint { 
            background: #f8f9fa; 
            padding: 20px; 
            margin: 15px 0; 
            border-radius: 8px;
            border-left: 4px solid #667eea;
        }
        .method { 
            display: inline-block; 
            padding: 5px 12px; 
            border-radius: 4px; 
            font-weight: bold;
            font-size: 12px;
            margin-right: 10px;
        }
        .post { background: #49cc90; color: white; }
        .get { background: #61affe; color: white; }
        code { 
            background: #e9ecef; 
            padding: 3px 8px; 
            border-radius: 4px;
            font-family: 'Courier New', monospace;
            color: #d63384;
        }
        .description {
            margin-top: 10px;
            color: #555;
        }
        .links {
            margin-top: 30px;
            padding-top: 20px;
            border-top: 2px solid #e9ecef;
        }
        a {
            color: #667eea;
            text-decoration: none;
            font-weight: 600;
        }
        a:hover {
            text-decoration: underline;
        }
        .status {
            display: inline-block;
            background: #d4edda;
            color: #155724;
            padding: 8px 16px;
            border-radius: 20px;
            font-size: 14px;
            margin-bottom: 20px;
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='status'>✓ API is running</div>
        <h1>🔗 URL Shortener API</h1>
        <p class='subtitle'>A simple and efficient URL shortening service</p>
        
        <h2>Available Endpoints:</h2>
        
        <div class='endpoint'>
            <div>
                <span class='method post'>POST</span>
                <code>/api/urls/shorten</code>
            </div>
            <div class='description'>
                Create a shortened URL<br>
                <strong>Body:</strong> <code>{ ""url"": ""https://example.com"" }</code>
            </div>
        </div>
        
        <div class='endpoint'>
            <div>
                <span class='method get'>GET</span>
                <code>/api/urls/{shortCode}/stats</code>
            </div>
            <div class='description'>
                Get statistics for a shortened URL (click count, creation date, etc.)
            </div>
        </div>
        
        <div class='endpoint'>
            <div>
                <span class='method get'>GET</span>
                <code>/{shortCode}</code>
            </div>
            <div class='description'>
                Redirect to the original URL using the short code
            </div>
        </div>
        
        <div class='links'>
            <p>📖 <a href='/swagger'>View Interactive API Documentation (Swagger)</a></p>
            <p>💡 <strong>Example:</strong> After creating a short URL, you can access it directly at <code>https://url-shortener-e3rq.onrender.com/{shortCode}</code></p>
        </div>
    </div>
</body>
</html>
", "text/html"));

app.MapControllers();

// Apply migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();