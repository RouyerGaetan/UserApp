namespace UserApp.Models
{
    public class OperationResult
    {
        public bool Succeeded { get; set; }
        public Dictionary<string, string>? Errors { get; set; }

        public static OperationResult Success() => new OperationResult { Succeeded = true };

        public static OperationResult Fail(Dictionary<string, string> errors) => new OperationResult { Succeeded = false, Errors = errors };

        public static OperationResult Fail(string key, string error) => Fail(new Dictionary<string, string> { { key, error } });
    }
}
