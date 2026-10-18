using System.Text.Json.Serialization;

namespace XboxAuthNet.XboxLive.Requests;

internal sealed class XboxUserRequestBody<T>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RelyingParty { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string TokenType { get; set; } = "JWT";

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Properties { get; set; }
}
