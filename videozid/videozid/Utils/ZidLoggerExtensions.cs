using Microsoft.Extensions.Logging;
using System;

namespace videozid.Utils
{
    public static class ZidLoggerExtensions
    {
        public static ILoggerFactory AddZidLogger(this ILoggerFactory factory, IServiceProvider serviceProvider, Func<LogLevel, bool> filter = null)
        {
            factory.AddProvider(new ZidLoggerProvider(serviceProvider, filter));
            return factory;
        }
    }
}
