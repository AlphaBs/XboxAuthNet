namespace XboxAuthNet.XboxLive.Requests.Bodies
{
    internal sealed class DeviceProperties
    {
        public string AuthMethod { get; set; } = "ProofOfPossession";

        public string? Id { get; set; }

        public string? DeviceType { get; set; }

        public string? SerialNumber { get; set; }

        public string? Version { get; set; }

        public object? ProofKey { get; set; }
    }
}
