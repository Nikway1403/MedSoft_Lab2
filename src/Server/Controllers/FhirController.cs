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
    public async Task<IActionResult> CreatePatient(CancellationToken token)
    {
        var result = await _fhirService.CreatePatientFromFhirAsync(Request, token);
        return Ok(result);
    }

    [HttpDelete("patient/{id:long}")]
    public IActionResult DeletePatient(long id, CancellationToken token)
    {
        var result = _fhirService.DeletePatient(id, token);
        return Ok(result);
    }

    [HttpGet("patients")]
    public IActionResult GetAllPatients(CancellationToken token)
    {
        var bundle = _fhirService.GetAllPatientsAsFhirBundle(token);
        return Ok(bundle);
    }
    
    [HttpGet("messages")]
    public ActionResult<IEnumerable<FhirStoredMessageDto>> GetMessages(CancellationToken token)
    {
        var messages = _fhirService.GetMessages(token);
        return Ok(messages);
    }
}