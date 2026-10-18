using System.Text.Json.Serialization;

namespace XboxAuthNet.XboxLive.Crypto
{
    internal sealed class ProofKeyBody
    {
        [JsonPropertyName("kty")]
        public string KeyType { get; set; } = "EC";

        [JsonPropertyName("x")]
        public string? X { get; set; }

        [JsonPropertyName("y")]
        public string? Y { get; set; }

        [JsonPropertyName("crv")]
        public string Curve { get; set; } = "P-256";

        [JsonPropertyName("alg")]
        public string Algorithm { get; set; } = "ES256";

        [JsonPropertyName("use")]
        public string Use { get; set; } = "sig";
    }
}
