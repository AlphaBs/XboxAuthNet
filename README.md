# XboxAuthNet

Microsoft OAuth 2.0 and Xbox Authentication

## Features

- [Microsoft OAuth Code Flow](https://learn.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow)
- Xbox Authentication
- Xbox Sisu Authentication with Proof-of-Possession

## Install

![Install nuget package XboxAuthNet](https://img.shields.io/nuget/v/XboxAuthNet?label=XboxAuthNet&style=flat-square)

[XboxAuthNet](https://www.nuget.org/packages/XboxAuthNet)

## Usage: Microsoft OAuth

Currently only [auth code flow](https://learn.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow) is supported. 

```csharp
// Initialize API client
var httpClient = new HttpClient();
var apiClient = new MicrosoftOAuthCodeApiClient("<CLIENT-ID>", "<SCOPES>", httpClient); // replace "SCOPES" to XboxAuth.XboxScope for Xbox Authentication

// Authenticate with auth code flow
var codeFlow = new MicrosoftOAuthCodeFlowBuilder(apiClient)
    .Build();
MicrosoftOAuthResponse result = await codeFlow.Authenticate();

// `result.AccessToken` can be used on Xbox Authentication
// store `result` variable to refresh token later.
// `MicrosoftOAuthResponse` can be serialized (like json)
Console.WriteLine(result.AccessToken);
Console.WriteLine(result.RefreshToken);
```

### Refresh Microsoft OAuth Token
```csharp
if (!result.Validate())
{
    var newResult = await apiClient.RefreshToken(result.RefreshToken, CancellationToken.None);
    Console.WriteLine(newResult.AccessToken);
    Console.WriteLine(newResult.RefreshToken);
}
```

## Usage: Xbox Authentication

There are three Xbox authentication methods. You can find a description of each method [here](https://github.com/AlphaBs/XboxAuthNet/wiki/).

### Xbox Basic Authentication

```csharp
var httpClient = new HttpClient();
var xboxAuthClient = new XboxAuthClient(httpClient);

var userToken = await xboxAuthClient.RequestUserToken("<microsoft_oauth2_access_token>");
var xsts = await xboxAuthClient.RequestXsts(userToken.Token, "<relying_party>");

Console.WriteLine(xsts.Token);
```

### Xbox Full Authentication
```csharp
var httpClient = new HttpClient();
var xboxAuthClient = new XboxAuthClient(httpClient);

var userToken = await xboxAuthClient.RequestSignedUserToken(new XboxSignedUserTokenRequest
{
    AccessToken = "<microsoft_oauth2_access_token>",
    TokenPrefix = AbstractXboxAuthRequest.XboxTokenPrefix
});
var deviceToken = await xboxAuthClient.RequestDeviceToken(new XboxDeviceTokenRequest
{
    DeviceType = XboxDeviceTypes.Nintendo,
    DeviceVersion = "0.0.0"
});
var titleToken = await xboxAuthClient.RequestTitleToken(new XboxTitleTokenRequest
{
    AccessToken = "<microsoft_oauth2_access_token>",
    DeviceToken = deviceToken.Token
});
var xsts = await xboxAuthClient.RequestXsts(new XboxXstsRequest
{
    UserToken = userToken.Token,
    DeviceToken = deviceToken.Token,
    TitleToken = titleToken.Token,
    RelyingParty = "<relying_party>"
});

Console.WriteLine(xsts.Token);
```

### Xbox Sisu Authentication
```csharp
var httpClient = new HttpClient();
var xboxAuthClient = new XboxAuthClient(httpClient);

var deviceToken = await xboxAuthClient.RequestDeviceToken(XboxDeviceTypes.Win32, "0.0.0");
var sisuResult = await xboxAuthClient.SisuAuth(new XboxSisuAuthRequest
{
    AccessToken = "<microsoft_oauth2_access_token>",
    ClientId = XboxGameTitles.MinecraftJava,
    DeviceToken = deviceToken.Token,
    RelyingParty = "<relying_party>"
});

Console.WriteLine(xsts.Token);
```

## Example

[Example project](/example/WinForm)

## References

These documents explain how Microsoft OAuth 2.0 works.

[Microsoft OAuth 2.0](https://learn.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow)

[Desktop application calling a web api](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/scenarios#desktop-application-calling-a-web-api-in-the-name-of-the-signed-in-user) (XboxAuthNet implements [interactive authentication](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Acquiring-tokens-interactively))

This project was made possible thanks to the contributions of various open-source projects. not used any document from [NDA developer program](https://learn.microsoft.com/en-us/gaming/gdk/_content/gc/getstarted/gc-getstarted-toc)

[MSAL.NET](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet)

[xbox-live-api](https://github.com/microsoft/xbox-live-api)

[xboxlive-auth](https://github.com/XboxReplay/xboxlive-auth)

[prismarine-auth](https://github.com/PrismarineJS/prismarine-auth)