
namespace Domain.Entities;

public class User
{
    public required string Id { get; set; }
    public int Elo { get; set; } = 600;
}