# XboxAuthNet
Xbox Live authentication

Ported from [xboxlive-auth](https://github.com/XboxReplay/xboxlive-auth)


### Usage
```csharp
    var x = new XboxAuth();
    var r = x.Authenticate("your email", "your password");
    Console.WriteLine("UserXUID {0}", r.UserXUID);
    Console.WriteLine("UserHash {0}", r.UserHash);
    Console.WriteLine("XSTSToken {0}", r.XSTSToken);
    Console.WriteLine("ExpireOn {0}", r.ExpireOn);
```
