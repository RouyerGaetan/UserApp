namespace UserApp.Models
{
    public class OperationResult
    {
        public bool Succeeded { get; set; }
        public Dictionary<string, string>? Errors { get; set; }

        // Indique s'il y a des erreurs
        public bool HasErrors => Errors != null && Errors.Count > 0;

        public static OperationResult Success() => new OperationResult { Succeeded = true };

        public static OperationResult Fail(Dictionary<string, string> errors) =>
            new OperationResult { Succeeded = false, Errors = errors };

        public static OperationResult Fail(string key, string error) =>
            Fail(new Dictionary<string, string> { { key, error } });

        // Ajout utile : méthode pour fusionner plusieurs erreurs
        public OperationResult AddError(string key, string error)
        {
            if (Errors == null)
                Errors = new Dictionary<string, string>();

            Errors[key] = error;
            Succeeded = false;
            return this;
        }
    }
}
