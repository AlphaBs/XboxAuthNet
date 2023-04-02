using XboxAuthNet.XboxLive.Responses;

namespace XboxAuthNetConsole
{
    public class XboxAuthObjectPrinter : IObjectPrinter
    {
        public bool CanPrint(object? obj)
        {
            return (obj is XboxAuthResponse) || (obj is XboxSisuResponse);
        }

        public void Print(TextWriter writeTo, object? obj)
        {
            switch (obj)
            {
                case XboxAuthResponse r: 
                    printResponse(r);
                    break;
                case XboxSisuResponse r:
                    printSisuResponse(r);
                    break;
                default:
                    throw new PrinterException(obj);
            }
        }

        private void printResponse(XboxAuthResponse? response)
        {
            if (response == null)
            {
                Console.WriteLine("null");
                return;
            }

            Console.WriteLine($"Token: {response.Token}");
            Console.WriteLine($"ExpireOn: {response.ExpireOn}");
            Console.WriteLine($"XuiClaims.AgeGroup: {response.XuiClaims?.AgeGroup}");
            Console.WriteLine($"XuiClaims.Gamertag: {response.XuiClaims?.Gamertag}");
            Console.WriteLine($"XuiClaims.Privileges: {response.XuiClaims?.Privileges}");
            Console.WriteLine($"XuiClaims.UserHash: {response.XuiClaims?.UserHash}");
            Console.WriteLine($"XuiClaims.UserSettingsRestrictions: {response.XuiClaims?.UserSettingsRestrictions}");
            Console.WriteLine($"XuiClaims.UserTitleRestrictions: {response.XuiClaims?.UserTitleRestrictions}");
            Console.WriteLine($"XuiClaims.XboxUserId: {response.XuiClaims?.XboxUserId}");
        }

        private void printSisuResponse(XboxSisuResponse response)
        {
            Console.WriteLine($"Sandbox: {response.Sandbox}");
            Console.WriteLine($"UseModernGamertag: {response.UseModernGamertag}");
            Console.WriteLine($"WebPage: {response.WebPage}");
            Console.WriteLine("UserToken:");
            printResponse(response.UserToken);
            Console.WriteLine("DeviceToken:");
            Console.WriteLine(response.DeviceToken);
            Console.WriteLine("TitleToken: ");
            printResponse(response.TitleToken);
            Console.WriteLine("XstsToken: ");
            printResponse(response.AuthorizationToken);
        }
    }
}