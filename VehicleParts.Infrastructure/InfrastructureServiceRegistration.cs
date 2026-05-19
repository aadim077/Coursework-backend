using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VehicleParts.Application.Interfaces;
using VehicleParts.Application.Services;
using VehicleParts.Domain.Entities;
using VehicleParts.Infrastructure.Data;
using VehicleParts.Infrastructure.Repositories;
using VehicleParts.Infrastructure.Services;
using VehicleParts.Infrastructure.Settings;

namespace VehicleParts.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // JWT Settings
        var jwtSettings = new JwtSettings();
        configuration.GetSection("JwtSettings").Bind(jwtSettings);
        services.AddSingleton(jwtSettings);

        // Email Settings
        var emailSettings = new EmailSettings();
        configuration.GetSection("EmailSettings").Bind(emailSettings);
        services.AddSingleton(emailSettings);

        // Database - Use SQLite for development if PostgreSQL is unavailable
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        });

        // Identity
        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // JWT Authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        // Authorization policies
        services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
            .AddPolicy("StaffOnly", policy => policy.RequireRole("Staff"))
            .AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"))
            .AddPolicy("StaffOrAdmin", policy => policy.RequireRole("Staff", "Admin"))
            .AddPolicy("AnyRole", policy => policy.RequireRole("Admin", "Staff", "Customer"));

        // Services
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPartService, PartService>();
        services.AddScoped<IPurchaseInvoiceService, PurchaseInvoiceService>();
        services.AddScoped<ISalesOrderService, SalesOrderService>();
        services.AddScoped<IStaffService, StaffService>();
        services.AddScoped<IVendorService, VendorService>();
        services.AddScoped<IFinancialReportService, FinancialReportService>();
        services.AddScoped<ICustomerProfileService, CustomerProfileService>();
        services.AddScoped<ICustomerVehicleService, CustomerVehicleService>();
        services.AddScoped<IPartRepository, PartRepository>();
        services.AddScoped<ISaleInvoiceRepository, SaleInvoiceRepository>();
        services.AddScoped<ISaleInvoiceService, SaleInvoiceService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IUnavailablePartRequestService, UnavailablePartRequestService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<ICustomerHistoryService, CustomerHistoryService>();
        services.AddScoped<IEmailService, EmailService>();
        return services;
    }
}
