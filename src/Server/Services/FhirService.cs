using System.Text.Json.Nodes;
using Server.Dtos;
using Server.Repositories;

namespace Server.Services;

public class FhirService : IFhirService
{
    private readonly IFhirMessageRepository _fhirRepo;
    private readonly IPatientService _patientService;

    public FhirService(
        IFhirMessageRepository fhirRepo,
        IPatientService patientService)
    {
        _fhirRepo = fhirRepo;
        _patientService = patientService;
    }

    public async Task<object> CreatePatientFromFhirAsync(HttpRequest request, CancellationToken token)
    {
        request.EnableBuffering();
        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body);
        var rawJson = await reader.ReadToEndAsync(token);

        _fhirRepo.AddMessage(rawJson, DateTime.UtcNow);

        JsonNode? root;
        try
        {
            root = JsonNode.Parse(rawJson);
        }
        catch (Exception ex)
        {
            return BuildOperationOutcome(
                severity: "error",
                code: "invalid",
                diagnostics: $"Invalid JSON: {ex.Message}"
            );
        }

        if (root?["resourceType"]?.ToString() != "Patient")
        {
            return BuildOperationOutcome(
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
            return BuildOperationOutcome(
                severity: "error",
                code: "invalid",
                diagnostics: "birthDate is missing or invalid"
            );
        }

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

        return BuildOperationOutcome(
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
            return BuildOperationOutcome(
                severity: "error",
                code: "not-found",
                diagnostics: $"Patient {id} not found"
            );
        }

        return BuildOperationOutcome(
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

        return bundle;
    }

    public IEnumerable<FhirStoredMessageDto> GetMessages(CancellationToken token)
    {
        return _fhirRepo.GetMessages(token);
    }

    private object BuildOperationOutcome(string severity, string code, string diagnostics)
    {
        return new
        {
            resourceType = "OperationOutcome",
            issue = new[]
            {
                new {
                    severity = severity,
                    code = code,
                    diagnostics = diagnostics
                }
            }
        };
    }
}