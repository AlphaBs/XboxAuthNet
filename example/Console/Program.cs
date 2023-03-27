using XboxAuthNetConsole.OAuth;
using XboxAuthNetConsole.Options;
using CommandLine;

namespace XboxAuthNetConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var program = new Program();

            Parser.Default.ParseArguments<
                MicrosoftOAuthOptions,
                XboxLiveAuthOptions>(args)
                .MapResult(
                    (MicrosoftOAuthOptions opts) => program.RunMicrosoftOAuth(opts),
                    (XboxLiveAuthOptions opts) => program.RunXboxAuth(opts),
                    errs => 1
                );
        }

        private MicrosoftOAuthClient? oauthClient;

        private int RunMicrosoftOAuth(MicrosoftOAuthOptions opts)
        {
            var command = new MicrosoftOAuthCommand(oauthClient, opts);
            return 0;
        }

        private int RunXboxAuth(XboxLiveAuthOptions opts)
        {
            return 0;
        }
    }
}