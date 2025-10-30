using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Dtos;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("fhir")]
public class FhirController : ControllerBase
{
    private readonly IFhirService _fhirService;

    public FhirController(IFhirService fhirService)
    {
        _fhirService = fhirService;
    }
    
    [HttpPost("patient")]
    [Authorize(Policy = "ReceptionOnly")]
    public async Task<IActionResult> CreatePatient(CancellationToken token)
    {
        var result = await _fhirService.CreatePatientFromFhirAsync(Request, token);
        return Ok(result);
    }

    [HttpDelete("patient/{id:long}")]
    [Authorize(Policy = "ReceptionOnly")]
    public async Task<IActionResult> DeletePatient(long id, CancellationToken token)
    {
        var result = await _fhirService.DeletePatient(id, token);
        return Ok(result);
    }

    [HttpGet("patients")]
    [Authorize(Policy = "ChiefOnly")]
    public async Task<IActionResult> GetAllPatients(CancellationToken token)
    {
        var user = HttpContext.User;
        var id = user?.Identity;

        Console.WriteLine("=== /fhir/patients request ===");
        Console.WriteLine("IsAuthenticated: " + (id?.IsAuthenticated ?? false));
        Console.WriteLine("AuthType: " + (id?.AuthenticationType ?? "<null>"));
        Console.WriteLine("Claims:");
        foreach (var c in user.Claims)
        {
            Console.WriteLine($"  {c.Type} = {c.Value}");
        }
        Console.WriteLine("=============================");
        var bundle = await _fhirService.GetAllPatientsAsFhirBundle(token);
        return Ok(bundle);
    }
    
    [HttpGet("messages")]
    public ActionResult<IEnumerable<FhirStoredMessageDto>> GetMessages(CancellationToken token)
    {
        var messages = _fhirService.GetMessages(token);
        return Ok(messages);
    }
}