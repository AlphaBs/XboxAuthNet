namespace XboxAuthNet.XboxLive.Requests.Bodies
{
    internal sealed class XstsProperties
    {
        public string[]? UserTokens { get; set; }

        public string? DeviceToken { get; set; }

        public string? TitleToken { get; set; }

        public string[]? OptionalDisplayClaims { get; set; }

        public string SandboxId { get; set; } = "RETAIL";
    }
}
