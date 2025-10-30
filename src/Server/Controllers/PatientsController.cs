using Microsoft.AspNetCore.Mvc;
using Server.Dtos;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("patients")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _service;

    public PatientsController(IPatientService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PatientDto>> GetAll(CancellationToken token)
    {
        var all = _service.GetAllPatients(token);
        return Ok(all);
    }

    [HttpGet("{id:long}")]
    public ActionResult<PatientDto> GetById(long id, CancellationToken token)
    {
        var patient = _service.GetPatient(id, token);
        if (patient == null)
            return NotFound();

        return Ok(patient);
    }

    [HttpPost]
    public ActionResult<PatientDto> Create([FromBody] PatientDto dto, CancellationToken token)
    {
        var created = _service.CreatePatient(dto, token);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken token)
    {
        var ok = await _service.DeletePatient(id, token);
        if (!ok)
            return NotFound();

        return NoContent();
    }
}