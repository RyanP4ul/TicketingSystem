namespace LanBasedHelpDeskTickingSystem.Entities.Responses;

public class ApiResultResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public object Errors { get; set; }
    
    public static ApiResultResponse Error(string message) => new() { Success = false, Message = message };
    public static ApiResultResponse Error(object errors) => new() { Success = false, Errors = errors };
    public static ApiResultResponse Ok(string message) => new() { Success = true, Message = message };
}