# Offline JWT regression test

Run `dotnet run --project test/JwtRegression/JwtRegression.csproj` with the .NET 8 SDK or newer. This package-free test compiles the production decoder directly and verifies Base64URL symbols, padding lengths, Unicode deserialization, and malformed inputs without any real token or network request. It does not replace the full multi-target build or existing NUnit suite.
