namespace Domain.User;

public class UserReadDto: IIdentifier
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }
    
}