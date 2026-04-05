using Microsoft.EntityFrameworkCore;
using MarketingApp.API.Data;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------
// 1. Register services with the DI container
// ---------------------------------------------------------------

// Add API controllers (with JSON serialization)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Keep property names as PascalCase to match our DTOs
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Register DbContext with SQL Server connection string from appsettings.json
builder.Services.AddDbContext<MarketingDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure() // Retry on transient failures
    )
);

// Swagger / OpenAPI – makes it easy to test the API endpoints
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title   = "Marketing Personnel API",
        Version = "v1",
        Description = "REST API for managing marketing personnel and their sales records."
    });
});

// CORS – allow any origin so the frontend (served as static files or separately) can call the API.
// In production, restrict this to your actual domain.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// ---------------------------------------------------------------
// 2. Build the app and configure the HTTP pipeline
// ---------------------------------------------------------------
var app = builder.Build();

// In development, show Swagger UI at /swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Marketing Personnel API v1");
        c.RoutePrefix = "swagger";
    });
}

// Serve static files (our single-page HTML/JS/CSS from wwwroot)
app.UseDefaultFiles();  // Serves index.html at the root /
app.UseStaticFiles();   // Serves files from wwwroot/

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
