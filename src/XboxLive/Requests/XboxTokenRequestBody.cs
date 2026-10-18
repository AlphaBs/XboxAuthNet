namespace XboxAuthNet.XboxLive.Requests;

internal sealed class XboxTokenRequestBody<T>
{
    public T? Properties { get; set; }

    public string? RelyingParty { get; set; }

    public string TokenType { get; set; } = "JWT";
}
