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
## Native AOT 검증

- 기존 대상 프레임워크를 유지하고 `net10.0` 자산을 추가했습니다. 이 자산에는 AOT·트리밍 분석을 활성화합니다.
- 내장 요청·응답 모델은 System.Text.Json 소스 생성 메타데이터를 사용합니다.
- 실제 계정이나 외부 API 없이 mock HTTP 응답과 요청 본문으로 검증합니다. 실제 로그인 성공 여부를 검증하는 테스트는 아닙니다.
- .NET 10 SDK와 대상 플랫폼의 Native AOT 도구 체인이 필요합니다. 아래 예시는 Windows x64입니다.

```powershell
dotnet build src/XboxAuthNet.csproj -c Release
dotnet run --project smoke/NativeAotSmoke -c Release
dotnet publish smoke/NativeAotSmoke -c Release -r win-x64 -o artifacts/native-smoke
./artifacts/native-smoke/NativeAotSmoke.exe
```

### 사용자 정의 모델과 확장

- JIT에서 JSON 리플렉션이 활성화된 경우 기존 사용자 정의 모델의 처리는 유지됩니다. Native AOT는 내장 모델의 생성 메타데이터만 기본으로 사용합니다.
- 사용자 정의 HTTP 본문은 `HttpHelper.CreateJsonContent(value, jsonTypeInfo)`를, 응답은 `XboxAuthResponseHandler.HandleResponse(response, jsonTypeInfo)`를 사용할 수 있습니다.
- 사용자 정의 서명 요청은 `AbstractXboxSignedAuthRequest.SerializeBody`를 재정의하고 생성 `JsonTypeInfo`로 JSON 문자열을 반환할 수 있습니다. 서명과 전송은 동일한 문자열을 사용합니다.
- 사용자 정의 proof key는 `JsonElement`·`JsonObject`로 전달하거나 위 직렬화 확장 지점에서 자신의 생성 메타데이터를 사용합니다.
- OAuth 파생 매개변수는 AOT에서 `AddQueries`를 재정의하고 `base.AddQueries(query)` 호출 후 추가 항목을 넣습니다. 단순한 추가 항목은 `ExtraQueries`로 전달할 수 있으며, 마지막에 기본 항목을 덮어씁니다.
- 기능 guard의 IL4000 추론 진단 한 곳만 이유를 명시해 억제합니다. 실제 JSON·리플렉션 호출의 IL2026·IL3050 진단은 억제하지 않습니다. 리플렉션 호환 경로는 동적 코드 지원과 표준 JSON 기능 스위치가 모두 활성화된 경우에만 사용합니다.

JIT 호환 경로도 별도로 검증할 수 있습니다.

```powershell
dotnet run --project smoke/NativeAotSmoke -c Release -p:PublishAot=false -p:JsonSerializerIsReflectionEnabledByDefault=true
dotnet test test/XboxAuthNet.Test.csproj -c Release -p:TargetFramework=net10.0 -p:TargetFrameworks=net10.0
```
