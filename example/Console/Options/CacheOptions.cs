using CommandLine;

namespace XboxAuthNetConsole
{
    [Verb("cache")]
    public class CacheOptions
    {
        [Value(0)]
        public IEnumerable<string>? Name { get; set; }

        [Option("clear", Default=false)]
        public bool Clear { get; set; }
    }
}