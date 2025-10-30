using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.BackgroundServices;
using Server.Database;
using Server.Options;
using Server.Repositories;
using Server.Services;

namespace Server.Extensions;

public static class CustomExtension
{
    public static IServiceCollection AddMyServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<UserCredentialsOptions>(configuration);
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        var dbOptions = new DatabaseOptions();
        configuration.GetSection("DatabaseOptions").Bind(dbOptions);
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(dbOptions.ConnectionString));
        
        var jwtSection = configuration.GetSection("Jwt");
        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSection["Key"]!)
        );
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSection["Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateLifetime = true
                };
            });
        
        services.AddAuthorization(options =>
        {
            options.AddPolicy("ReceptionOnly", policy =>
                policy.RequireRole("Reception"));

            options.AddPolicy("ChiefOnly", policy =>
                policy.RequireRole("Chief"));
        });
        
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IPatientService, PatientService>();
        
        services.AddSingleton<IFhirMessageRepository, FhirMessageRepository>();
        services.AddScoped<IFhirService, FhirService>();
        
        services.AddHostedService<MigrationBackgroundService>();
        
        return services;
    }
}