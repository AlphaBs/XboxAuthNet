using System.Text.Json.Serialization;

namespace XboxAuthNet.XboxLive.Responses
{
    // https://github.com/OpenXbox/xbox-webapi-csharp/blob/master/XboxWebApi/Authentication/Model/XboxUserInformation.cs
    public class XboxAuthXuiClaims
    {
        [JsonPropertyName(XboxAuthXuiClaimNames.Gamertag)]
        public string? Gamertag { get; set; }

        [JsonPropertyName(XboxAuthXuiClaimNames.XboxUserId)]
        public string? XboxUserId { get; set; }

        [JsonPropertyName(XboxAuthXuiClaimNames.UserHash)]
        public string? UserHash { get; set; }

        [JsonPropertyName(XboxAuthXuiClaimNames.AgeGroup)]
        public string? AgeGroup { get; set; }

        [JsonPropertyName(XboxAuthXuiClaimNames.UserSettingsRestrictions)]
        public string? UserSettingsRestrictions { get; set; }

        [JsonPropertyName(XboxAuthXuiClaimNames.UserTitleRestrictions)]
        public string? UserTitleRestrictions { get; set; }

        [JsonPropertyName(XboxAuthXuiClaimNames.Privileges)]
        public string? Privileges { get; set; }
    }
}
