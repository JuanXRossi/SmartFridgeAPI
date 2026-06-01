namespace SmartFridgeAPI.Models
{
    public class UpdateUserResult
    {
        public bool Succeeded { get; set; }
        public string? ErrorMessage { get; set; }

        public static UpdateUserResult Success() => new() { Succeeded = true };
        public static UpdateUserResult Failure(string message) => new() { Succeeded = false, ErrorMessage = message };
    }
}