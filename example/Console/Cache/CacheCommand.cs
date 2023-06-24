using XboxAuthNetConsole.Serializer;
using XboxAuthNetConsole.Printer;

namespace XboxAuthNetConsole.Cache
{
    public class CacheCommand : ICommand
    {
        private readonly ISerializer<SessionCache> _cacheManager;
        private readonly CacheOptions _options;

        public CacheCommand(
            ISerializer<SessionCache> cacheManager, 
            CacheOptions options)
        {
            _cacheManager = cacheManager;
            _options = options;
        }

        public async Task Execute(CancellationToken cancellationToken)
        {
            if (_options.Clear)
            {
                await clearCache();
            }
            else
            {  
                await printCache();
            }
        }

        private async Task printCache()
        {
            Console.WriteLine("Cached session: ");

            var cache = await _cacheManager.Load();
            if (cache == null)
            {
                Console.WriteLine("Empty cache");
                return;
            }

            Console.WriteLine("Microsoft OAuth: ");
            ConsolePrinter.Print(cache.MicrosoftOAuth);
            Console.WriteLine("XboxUserToken: ");
            ConsolePrinter.Print(cache.XboxUserToken);
            Console.WriteLine("XboxDeviceToken: ");
            ConsolePrinter.Print(cache.XboxDeviceToken);
            Console.WriteLine("XboxTitleToken: ");
            ConsolePrinter.Print(cache.XboxTitleToken);
            Console.WriteLine("XboxXstsToken: ");
            ConsolePrinter.Print(cache.XboxXstsToken);

        }

        private async Task clearCache()
        {
            await _cacheManager.Save(null);
            Console.WriteLine("Cache cleared");

            await printCache();
        }
    }
}