namespace Server.Dtos;

public class LoginResponse
{
    public required string Token { get; set; }
    
    public required string Role { get; set; }
}