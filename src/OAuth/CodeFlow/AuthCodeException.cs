namespace XboxAuthNet.OAuth.CodeFlow;

public class AuthCodeException : Exception
{
    public AuthCodeException(string? error, string? errorDescription) : base(error ?? errorDescription)
    {
        Error = error;
        ErrorDescription = errorDescription;
    }

    public string? Error { get; }
    public string? ErrorDescription { get; }
}
