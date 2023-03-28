using XboxAuthNetConsole.Cache;
using XboxAuthNetConsole.OAuth;
using XboxAuthNetConsole.Options;
using XboxAuthNet.XboxLive;
using CommandLine;

namespace XboxAuthNetConsole
{
    public class Program
    {
        public static Program Instance = new Program();

        public static async Task<int> Main(string[] args)
        {
            var parseResult = Parser.Default.ParseArguments<
                MicrosoftOAuthOptions,
                XboxAuthOptions,
                CacheOptions
            >(args);

            await Instance.initialize();

            var result = await parseResult.MapResult(
                    (MicrosoftOAuthOptions opts) => Instance.runMicrosoftOAuthCommand(opts),
                    (XboxAuthOptions opts) => Instance.runXboxAuthCommand(opts),
                    (CacheOptions opts) => Instance.runCacheCommand(opts),
                    errs => Task.FromResult(-1)
                );
            return result;
        }

        public IObjectPrinter Printer;

        private CancellationToken cancellationToken;
        private MicrosoftOAuthClient? oauthClient;
        private XboxAuthClient? xboxAuthClient;
        private SessionCacheManager? cacheManager;
        private SessionCache? sessionCache;

        private async Task initialize()
        {
            
        }

        private async Task<int> runMicrosoftOAuthCommand(MicrosoftOAuthOptions opts)
        {
            Console.WriteLine("Microsoft OAuth");
            var command = new MicrosoftOAuthCommand(oauthClient, opts, sessionCache);
            return await runCommand(command);
        }

        private async Task<int> runXboxAuthCommand(XboxAuthOptions opts)
        {
            Console.WriteLine("XboxLive Auth");
            var command = new XboxAuthCommand(xboxAuthClient, opts, sessionCache);
            return await runCommand(command);
        }

        private async Task<int> runCacheCommand(CacheOptions opts)
        {
            Console.WriteLine("Cache Manager");
            var command = new CacheCommand(cacheManager, opts);
            return await runCommand(command);
        }

        private async Task<int> runCommand(ICommand command)
        {
            try
            {
                await command.Execute(cancellationToken);
                return 0;
            }
            catch (Exception ex)
            {   
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }
    }
}