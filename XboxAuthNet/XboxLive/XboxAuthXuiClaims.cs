using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace XboxAuthNet.XboxLive
{
    // https://github.com/OpenXbox/xbox-webapi-csharp/blob/master/XboxWebApi/Authentication/Model/XboxUserInformation.cs
    public class XboxAuthXuiClaims
    {
        [JsonPropertyName("gtg")]
        public string? Gamertag { get; set; }

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
