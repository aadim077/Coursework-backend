using Microsoft.EntityFrameworkCore;
using VehicleParts.Infrastructure;
using VehicleParts.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add Infrastructure services (EF Core, Identity, JWT, Authorization)
builder.Services.AddInfrastructure(builder.Configuration);

// Swagger/OpenAPI configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevPolicy", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});

var app = builder.Build();

// Database migration and role seeding
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        var roleManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole>>();
        await RoleSeeder.SeedRolesAsync(roleManager);
    }
    catch (Npgsql.NpgsqlException ex) when (ex.SqlState == "28P01")
    {
        Console.WriteLine("⚠️ PostgreSQL authentication failed. Please verify your database password in appsettings.json");
        Console.WriteLine($"Error: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Database migration failed: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("DevPolicy");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();



