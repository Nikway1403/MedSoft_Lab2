using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Server.Dtos;
using Server.Models;
using Server.Repositories;

namespace Server.Services;

public class FhirService : IFhirService
{
    private readonly IFhirMessageRepository _fhirRepo;
    private readonly IPatientService _patientService;
    private readonly ILogger<FhirService> _logger;

    public FhirService(
        ILogger<FhirService> logger,
        IFhirMessageRepository fhirRepo,
        IPatientService patientService)
    {
        _logger = logger;
        _fhirRepo = fhirRepo;
        _patientService = patientService;
    }

    public async Task<object> CreatePatientFromFhirAsync(HttpRequest request, CancellationToken token)
    {
        request.EnableBuffering();
        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body);
        var rawJson = await reader.ReadToEndAsync(token);
        
        var receivedAt = DateTime.UtcNow;
        
        _logger.LogInformation("FHIR message received at {ts}: {json}", receivedAt, rawJson);

        await _fhirRepo.AddMessage(rawJson, DateTime.UtcNow, token);

        JsonNode? root;
        try
        {
            root = JsonNode.Parse(rawJson);
        }
        catch (Exception ex)
        {
            return BuildOperationOutcome(400,
                severity: "error",
                code: "invalid",
                diagnostics: $"Invalid JSON: {ex.Message}"
            );
        }

        if (root?["resourceType"]?.ToString() != "Patient")
        {
            return BuildOperationOutcome(400,
                severity: "error",
                code: "invalid",
                diagnostics: "resourceType must be 'Patient'"
            );
        }

        var lastName  = root?["name"]?[0]?["family"]?.ToString() ?? string.Empty;
        var given0    = root?["name"]?[0]?["given"]?[0]?.ToString() ?? string.Empty;
        var given1    = root?["name"]?[0]?["given"]?[1]?.ToString() ?? string.Empty;
        var birthDate = root?["birthDate"]?.ToString() ?? string.Empty;

        if (!DateTime.TryParse(birthDate, out var birthDateParsed))
        {
            return BuildOperationOutcome(400,
                severity: "error",
                code: "invalid",
                diagnostics: "birthDate is missing or invalid"
            );
        }
        
        var count = (await _patientService.GetAllPatients(token)).Count();
        if (count >= 10)
            return BuildOperationOutcome(409, "error", "too-many", "Patient limit reached (10)");
        
        var created = await _patientService.CreatePatient(
            new PatientDto
            {
                FirstName   = given0,
                MiddleName  = given1,
                LastName    = lastName,
                DateOfBirth = DateTime.SpecifyKind(birthDateParsed, DateTimeKind.Utc)
            },
            token
        );

        return BuildOperationOutcome(201,
            severity: "information",
            code: "informational",
            diagnostics: $"Patient created with id {created.Id}"
        );
    }
    
    public async Task<object> DeletePatient(long id, CancellationToken token)
    {
        var ok = await _patientService.DeletePatient(id, token);

        if (!ok)
        {
            return BuildOperationOutcome(404,
                severity: "error",
                code: "not-found",
                diagnostics: $"Patient {id} not found"
            );
        }

        return BuildOperationOutcome(204,
            severity: "information",
            code: "informational",
            diagnostics: $"Patient {id} deleted"
        );
    }

    public async Task<object> GetAllPatientsAsFhirBundle(CancellationToken token)
    {
        var all = await _patientService.GetAllPatients(token);

        var bundle = new
        {
            resourceType = "Bundle",
            type = "collection",
            entry = all.Select(p => new
            {
                resource = new
                {
                    resourceType = "Patient",
                    id = p.Id.ToString(),
                    name = new[]
                    {
                        new {
                            family = p.LastName,
                            given = string.IsNullOrWhiteSpace(p.MiddleName)
                                ? new[] { p.FirstName }
                                : new[] { p.FirstName, p.MiddleName }
                        }
                    },
                    birthDate = p.DateOfBirth.ToString("yyyy-MM-dd")
                }
            })
        };

        return new JsonResult(bundle)
        {
            StatusCode = 200,
            ContentType = "application/fhir+json"
        };
    }

    public async Task<IEnumerable<FhirLog>> GetMessages(CancellationToken token)
    {
        return await _fhirRepo.GetMessages(token);
    }

    private object BuildOperationOutcome(int statusCode, string severity, string code, string diagnostics)
    {
        var outcome = new
        {
            resourceType = "OperationOutcome",
            issue = new[]
            {
                new {
                    severity,
                    code,
                    diagnostics
                }
            }
        };

        return new JsonResult(outcome)
        {
            StatusCode = statusCode,
            ContentType = "application/fhir+json"
        };
    }
}