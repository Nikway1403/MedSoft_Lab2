namespace Server.Dtos;

public class PatientDto
{
    public long Id { get; set; }

    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string MiddleName { get; set; }

    public DateTime DateOfBirth { get; set; }
}