# XboxAuthNet
Xbox Live authentication

Ported from [xboxlive-auth](https://github.com/XboxReplay/xboxlive-auth)


### Install
Nuget Package: XboxAuthNet  
or download dlls from [release](https://github.com/AlphaBs/XboxAuthNet/releases)

### Sample program
[XboxAuthNetTest](https://github.com/AlphaBs/XboxAuthNet/tree/main/XboxAuthNetTest)

### Usage
**Microsoft OAuth: Getting new tokens**

```csharp
MicrosoftOAuth oauth = new MicrosoftOAuth("CLIENT_ID", "SCOPE");
string url = oauth.CreateUrl(); // show url into webbrowser

// execute below codes when the url of webbrowser is changed (ex: NavigationStarting event)
// redirectUrl: new url of webbrowser
if (oauth.CheckLoginSuccess(redirectUrl))
{
  MicrosoftOAuthResponse response;
  if (oauth.TryGetTokens(out response)
  {
    // success to get MicrosoftOAuth token
    // cache AccessToken, RefreshToken and ExpireIn to use next time
    
    // response.AccessToken
    // response.ExpireIn
    // response.RefreshToken
    // response.Scope
    // response.TokenType
    // response.UserId
  }
  else
  {
    // failed to login
  }
}
```

**Microsoft OAuth: Validating & Refreshing tokens**

```csharp
MicrosoftOAuth oauth = new MicrosoftOAuth("CLIENT_ID", "SCOPE");
MicrosoftOAuthResponse response = ~~~; // read from cache

if (oauth.TryGetTokens(out response, response.RefreshToken)
{
  // success
  // response.AccessToken
}
else
{
  // failed to refresh tokens
}
```

**XboxLive Login**

```csharp
XboxAuth xbox = new XboxAuth();
var rps = xbox.ExchangeRpsTicketForUserToken("Microsoft OAuth AccessToken");
var xsts = xbox.ExchangeTokensForXSTSIdentity(
  rps.Token, // userToken
  null, // deviceToken
  null, // titleToken
  "relyingParty", // relyingParty 
  null); // optionalDisplayClaims

if (xsts.IsSuccess)
{
  // success
  // xsts.Token: XSTS Token
  // xsts.UserHash: UserHash (uhs)
  // xsts.UserXUID
  // xsts.IssueInstant
  // xsts.ExpireOn
}
else
{
  // fail
  // xsts.Error
  // xsts.Message
}
```
