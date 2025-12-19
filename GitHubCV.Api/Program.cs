using GitHubCV.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// --- 1. רישום שירותים למערכת (DI Container) ---
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// טעינת ההגדרות (Token ו-Username) מתוך ה-Configuration/Secrets
builder.Services.Configure<GitHubSettings>(builder.Configuration.GetSection("GitHubSettings"));

// --- 2. הגדרות Caching ו-Decorator (לפי דרישות הפרויקט) ---
// הוספת הזיכרון המטמון למערכת
builder.Services.AddMemoryCache();

// רישום השירות הבסיסי
builder.Services.AddScoped<IGitHubService, GitHubService>();

// שימוש ב-Scrutor כדי לעטוף את השירות ב-Cache (Decorator Pattern)
builder.Services.Decorate<IGitHubService, CachedGitHubService>();

var app = builder.Build();

// --- 3. הגדרת צינור הבקשות (Middleware Pipeline) ---
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// הגשת קבצים סטטיים (Frontend) - חשוב: לפני MapControllers/MapGet כדי שיוגש index.html
app.UseDefaultFiles();
app.UseStaticFiles();

// חיבור הנתיבים של הקונטרולרים (כדי ש-api/Portfolio יעבוד)
app.MapControllers();

// Print listening URLs to console so user knows the local server addresses
try
{
    // If Kestrel already has bound URLs they appear in app.Urls
    var urls = app.Urls;
    if (urls != null && urls.Count > 0)
    {
        Console.WriteLine("Listening on:");
        foreach (var u in urls) Console.WriteLine(" " + u);
    }
    else
    {
        // Check environment variable
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
        if (!string.IsNullOrEmpty(env))
        {
            Console.WriteLine("Listening on (from ASPNETCORE_URLS): " + env);
        }
        else
        {
            // Fallback: try to read Properties/launchSettings.json
            var lsPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Properties", "launchSettings.json");
            if (File.Exists(lsPath))
            {
                var text = File.ReadAllText(lsPath);
                using var doc = JsonDocument.Parse(text);
                if (doc.RootElement.TryGetProperty("profiles", out var profiles))
                {
                    Console.WriteLine("Launch profiles URLs:");
                    foreach (var prop in profiles.EnumerateObject())
                    {
                        if (prop.Value.TryGetProperty("applicationUrl", out var appUrl))
                        {
                            Console.WriteLine($" {prop.Name}: {appUrl.GetString()}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No explicit server URL found. The app will use defaults (Kestrel). Look for runtime message 'Now listening on:' after start.");
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine("Could not determine server URLs: " + ex.Message);
}

app.Run();