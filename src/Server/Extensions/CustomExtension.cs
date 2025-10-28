using Server.Repositories;
using Server.Services;

namespace Server.Extensions;

public static class CustomExtension
{
    public static IServiceCollection AddMyServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IPatientRepository, PatientRepository>();
        services.AddScoped<IPatientService, PatientService>();
        
        services.AddSingleton<IFhirMessageRepository, FhirMessageRepository>();
        services.AddScoped<IFhirService, FhirService>();
        
        return services;
    }
}