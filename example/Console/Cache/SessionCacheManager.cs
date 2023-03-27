using System.Text.Json;

namespace XboxAuthNetConsole.Cache
{
    public class SessionCacheManager
    {
        private readonly string _filePath;

        public SessionCacheManager(string filePath)
        {
            this._filePath = filePath;
        }

        private SessionCache? _sessionCache;
        public SessionCache SessionCache => _sessionCache 
            ?? throw new InvalidOperationException("Load cache first");

        public async Task<SessionCache?> LoadCache()
        {
            await using var fs = File.OpenRead(_filePath);
            return await JsonSerializer.DeserializeAsync<SessionCache>(fs);
        }

        public async Task SaveCache(SessionCache? sessionCache)
        {
            await using var fs = File.Create(_filePath);
            await JsonSerializer.SerializeAsync(fs, sessionCache);
        }
    }
}