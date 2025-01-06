using XboxAuthNetConsole.Cache;
using XboxAuthNetConsole.OAuth;
using XboxAuthNetConsole.Options;
using XboxAuthNetConsole.Serializer;
using CommandLine;

namespace XboxAuthNetConsole
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var parseResult = Parser.Default.ParseArguments<
                MicrosoftOAuthOptions,
                XboxAuthOptions,
                CacheOptions
            >(args);

            var program = new Program();
            var result = await parseResult.MapResult(
                    (MicrosoftOAuthOptions opts) => program.runMicrosoftOAuthCommand(opts),
                    (XboxAuthOptions opts) => program.runXboxAuthCommand(opts),
                    (CacheOptions opts) => program.runCacheCommand(opts),
                    errs => Task.FromResult(-1)
                );
            return result;
        }

        private readonly string _settingFilePath = Path.Combine(Environment.CurrentDirectory, "settings.json");
        private readonly string _sessionCacheFilePath = Path.Combine(Environment.CurrentDirectory, "session.json");

        private ISerializer<SessionCache>? _sessionCacheSerializer;
        private SessionCache? _sessionCache;
        private CancellationToken cancellationToken;

        private async Task<int> runMicrosoftOAuthCommand(MicrosoftOAuthOptions opts)
        {
            Console.WriteLine("Microsoft OAuth");
            Console.WriteLine("Currently Microsoft OAuth only works on Windows");

            var settings = await getSettings();
            opts.ClientId ??= settings?.ClientId;
            opts.Scopes ??= settings?.Scopes;

            var sessionCache = await getSessionCache();
            var command = new MicrosoftOAuthCommand(opts, sessionCache);
            return await runCommand(command);
        }

        private async Task<int> runXboxAuthCommand(XboxAuthOptions opts)
        {
            if (string.IsNullOrEmpty(opts.ClientId))
            {
                var settings = await getSettings();
                opts.ClientId = settings.ClientId;
            }

            Console.WriteLine("XboxLive Auth");
            var sessionCache = await getSessionCache();
            var command = new XboxAuthCommand(opts, sessionCache);
            return await runCommand(command);
        }

        private async Task<int> runCacheCommand(CacheOptions opts)
        {
            Console.WriteLine("Cache Manager");
            var sessionCacheSerializer = initializeSessionCacheSerializer();
            var command = new CacheCommand(sessionCacheSerializer, opts);
            return await runCommand(command);
        }

        private async Task<Settings> getSettings()
        {
            var settingSerializer = new JsonFileSerializer<Settings>(_settingFilePath);
            var settings = await settingSerializer.Load();
            if (settings == null)
                settings = new Settings();
            return settings;
        }

        private async Task<SessionCache> getSessionCache()
        {
            if (_sessionCacheSerializer == null)
            {
                _sessionCacheSerializer = initializeSessionCacheSerializer();
                _sessionCache = await _sessionCacheSerializer.Load();
            }

            if (_sessionCache == null)
                _sessionCache = new SessionCache();
            
            return _sessionCache;
        }

        private ISerializer<SessionCache> initializeSessionCacheSerializer()
        {
            return new JsonFileSerializer<SessionCache>(_sessionCacheFilePath);
        }

        private async Task<int> runCommand(ICommand command)
        {
            try
            {
                await command.Execute(cancellationToken);
                await finalize();
                return 0;
            }
            catch (Exception ex)
            {   
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }

        private async Task finalize()
        {
            if (_sessionCacheSerializer != null)
            {
                await _sessionCacheSerializer.Save(_sessionCache);
            }
        }
    }
}