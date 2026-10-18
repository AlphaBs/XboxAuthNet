namespace XboxAuthNet.XboxLive.Requests.Bodies
{
    internal sealed class TitleProperties
    {
        public string AuthMethod { get; set; } = "RPS";

        public string? DeviceToken { get; set; }

        public string? RpsTicket { get; set; }

        public string SiteName { get; set; } = "user.auth.xboxlive.com";

        public object? ProofKey { get; set; }
    }
}
