namespace XboxAuthNetConsole.Cache
{
    public class CacheCommand : ICommand
    {
        private readonly SessionCacheManager _cacheManager;
        private readonly CacheOptions _options;

        public CacheCommand(
            SessionCacheManager cacheManager, 
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
                printCache();
            }
        }

        private void printCache()
        {
            
        }

        private async Task clearCache()
        {
            await _cacheManager.SaveCache(null);
            Console.WriteLine("Cache cleared");

            printCache();
        }
    }
}