namespace LanBasedHelpDeskTickingSystem.Entities.Responses;

public class ApiResponse<T>
{
    public bool Success { get; private set; }
    public string? Message { get; private set; }
    public Dictionary<string, string[]> Errors { get; private set; }
    public T? Data { get; private set; }

    public static ApiResponse<T> Error(string message) => new() { Success = false, Message = message };
    public static ApiResponse<T> Error(Dictionary<string, string[]> errors) => new() { Success = false, Errors = errors };
    public static ApiResponse<T> Ok(T data) => new() { Success = true, Data = data };
}