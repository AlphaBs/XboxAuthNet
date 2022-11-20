using System.Text.Json.Serialization;

namespace XboxAuthNet.XboxLive.Entity
{
    // https://github.com/OpenXbox/xbox-webapi-csharp/blob/master/XboxWebApi/Authentication/Model/XboxUserInformation.cs
    public class XboxAuthXuiClaims
    {
        [JsonPropertyName("gtg")]
        public string? Gamertag { get; set; }
        [JsonPropertyName("mgt")]
        public string? ModernGamertag { get; set; }
        [JsonPropertyName("umg")]
        public string? UniqueModernGamertag { get; set; }
        [JsonPropertyName("mgs")]
        public string? ModernGamertagSuffix { get; set; }

        [JsonPropertyName("xid")]
        public string? XboxUserId { get; set; }

        [JsonPropertyName("uhs")]
        public string? UserHash { get; set; }

        [JsonPropertyName("agg")]
        public string? AgeGroup { get; set; }

        [JsonPropertyName("usr")]
        public string? UserSettingsRestrictions { get; set; }

        [JsonPropertyName("utr")]
        public string? UserTitleRestrictions { get; set; }

        [JsonPropertyName("prv")]
        public string? Privileges { get; set; }
    }
}
