using System;

namespace Shop.source.Logging
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } // z.B. Info, Warning, Error
        public string Message { get; set; }
    }
}
