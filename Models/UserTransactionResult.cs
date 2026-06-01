namespace SmartFridgeAPI.Models
{
    public class UserTransactionResult
    {
        public bool Succeeded { get; set; }
        public bool IsServerError { get; set; }
        public string? ErrorMessage { get; set; }

        public static UserTransactionResult Success() => new() { Succeeded = true };
        public static UserTransactionResult Failure(string message, bool isServerError = false) => 
            new() { Succeeded = false, ErrorMessage = message, IsServerError = isServerError };
    }
}