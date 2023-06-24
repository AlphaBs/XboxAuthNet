using XboxAuthNet.OAuth;

namespace XboxAuthNetConsole
{
    public class OAuthObjectPrinter : IObjectPrinter
    {
        public bool CanPrint(object? obj)
        {
            return obj is MicrosoftOAuthResponse;
        }

        public void Print(TextWriter writeTo, object? obj)
        {
            var oauthResponse = obj as MicrosoftOAuthResponse;
            if (oauthResponse == null)
                throw new PrinterException(obj);
            printResponse(writeTo, oauthResponse);
        }

        private void printResponse(TextWriter writeTo, MicrosoftOAuthResponse response)
        {
            writeTo.WriteLine("Microsoft OAuth Login Success");
            writeTo.WriteLine($"AccessToken: {response.AccessToken}");
            writeTo.WriteLine($"RefreshToken: {response.RefreshToken}");
            writeTo.WriteLine($"ExpiresOn: {response.ExpiresOn}");
            writeTo.WriteLine($"IdToken: {response.IdToken}");
            writeTo.WriteLine($"Scope: {response.Scope}");
            writeTo.WriteLine($"TokenType: {response.TokenType}");
        }
    }
}