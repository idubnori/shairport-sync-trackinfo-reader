using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace TrackInfoReader.Tests.Logger
{
    internal static class TestOutputHelperExtensions
    {
        public static ILogger<T> CreateLogger<T>(this ITestOutputHelper testOutputHelper)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new XunitLoggerProvider(testOutputHelper));
            var logger = loggerFactory.CreateLogger<T>();
            return logger;
        }
    }
}
