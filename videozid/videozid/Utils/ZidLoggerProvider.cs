using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace videozid.Utils
{
    public class ZidLoggerProvider : ILoggerProvider
    {
        private IServiceProvider serviceProvider;
        private Func<LogLevel, bool> filter;
        public ZidLoggerProvider(IServiceProvider serviceProvider, Func<LogLevel, bool> filter)
        {
            this.filter = filter;
            this.serviceProvider = serviceProvider;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new ZidLogger(serviceProvider, filter);
        }

        public void Dispose()
        {

        }
    }
}
