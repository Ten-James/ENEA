namespace Api.BussinesLogic.Exceptions;

public class EntityNotFoundException(
    Guid id,
    string message) : Exception(message)
{
    public Guid Id => id;
    public override string Message => $"Entity with ID {id} not found. {base.Message}";
}