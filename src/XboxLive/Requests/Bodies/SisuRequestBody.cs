namespace XboxAuthNet.XboxLive.Requests.Bodies
{
    internal sealed class SisuRequestBody
    {
        public string? AccessToken { get; set; }

        public string? AppId { get; set; }

        public string? DeviceToken { get; set; }

        public string Sandbox { get; set; } = "RETAIL";

        public bool UseModernGamertag { get; set; } = true;

        public string SiteName { get; set; } = "user.auth.xboxlive.com";

        public string? RelyingParty { get; set; }

        public object? ProofKey { get; set; }
    }
}
