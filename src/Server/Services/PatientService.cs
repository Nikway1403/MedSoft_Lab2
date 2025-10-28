using Server.Dtos;
using Server.Models;
using Server.Repositories;

namespace Server.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;

    public PatientService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public PatientDto CreatePatient(PatientDto dto, CancellationToken token)
    {
        var model = new Patient
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            MiddleName = dto.MiddleName,
            DateOfBirth = dto.DateOfBirth,
        };

        var newId = _patientRepository.CreatePatient(model, token);

        return new PatientDto
        {
            Id = newId,
            FirstName = model.FirstName,
            LastName = model.LastName,
            MiddleName = model.MiddleName,
            DateOfBirth = model.DateOfBirth
        };
    }

    public PatientDto? GetPatient(long id, CancellationToken token)
    {
        var patient = _patientRepository.GetPatient(id, token);
        if (patient == null) return null;

        return new PatientDto
        {
            Id = patient.Id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            MiddleName = patient.MiddleName,
            DateOfBirth = patient.DateOfBirth
        };
    }

    public IEnumerable<PatientDto> GetAllPatients(CancellationToken token)
    {
        return _patientRepository
            .GetPatients(token)
            .Select(p => new PatientDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                MiddleName = p.MiddleName,
                DateOfBirth = p.DateOfBirth
            });
    }

    public bool DeletePatient(long id, CancellationToken token)
    {
        return _patientRepository.DeletePatient(id, token);
    }
}