namespace XboxAuthNet.XboxLive.Requests;

internal sealed class XboxUserRequestBody<T>
{
    public string? RelyingParty { get; set; }

    public string TokenType { get; set; } = "JWT";

    public T? Properties { get; set; }
}
