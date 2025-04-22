using Api.Infrastructure.Enums;

namespace Api.BussinesLogic.Exceptions;

public class ChargerInInvalidStateException(
    ChargerStatus expected,
    string message) : Exception(message)
{
    public override string Message =>
        $"Charger is in invalid state. Expected: {expected}. {message}";
}