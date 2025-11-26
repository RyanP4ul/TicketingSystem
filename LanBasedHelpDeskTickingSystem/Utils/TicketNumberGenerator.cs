namespace LanBasedHelpDeskTickingSystem.Utils;

public static class TicketNumberGenerator
{
    public static string GenerateTicketNumber()
    {
        return $"TKT-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }
}