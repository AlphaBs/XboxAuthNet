using System.Threading;

namespace XboxAuthNet.OAuth.Models
{
    public class WebUIOptions
    {
        public object? ParentObject { get; set; }
        public string? Title { get; set; }
        public SynchronizationContext? SynchronizationContext { get; set; }
    }
}
