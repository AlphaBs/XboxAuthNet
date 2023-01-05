using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace XboxAuthNet.OAuth.Models
{
    public class WebUIOptions
    {
        public object? ParentObject { get; set; }
        public SynchronizationContext? SynchronizationContext { get; set; }
    }
}
