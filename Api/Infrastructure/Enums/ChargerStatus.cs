namespace Api.Infrastructure.Enums;

public enum ChargerStatus
{
    /// <summary>
    ///     Available for use
    /// </summary>
    Available,

    /// <summary>
    ///     Charging in progress
    /// </summary>
    Charging,

    /// <summary>
    ///     Out of order
    /// </summary>
    OutOfOrder
}