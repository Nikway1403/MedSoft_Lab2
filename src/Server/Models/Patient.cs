namespace Server.Models;

public class Patient
{
    public long Id { get; set; }

    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    public string MiddleName { get; set; }

    public DateTime DateOfBirth { get; set; }
}