namespace XboxAuthNet.XboxLive.Requests.Bodies
{
    internal sealed class RpsProperties
    {
        public string AuthMethod { get; set; } = "RPS";

        public string SiteName { get; set; } = "user.auth.xboxlive.com";

        public string? RpsTicket { get; set; }
    }
}
