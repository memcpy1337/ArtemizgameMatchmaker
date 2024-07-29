
namespace Domain.Entities;

public class User
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public int Elo { get; set; } = 600;
}