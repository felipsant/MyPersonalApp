using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using PersonalApp.Services;
using System;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace PersonalApp
{
    public class Program
    {
        private static ILoggerFactory _loggerFactory;

        public static void Main()
        {
            #if DEBUG
            Debugger.Launch();
            #endif

            _loggerFactory = new LoggerFactory();

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WiseAPIUrl")))
                throw new ArgumentNullException("WiseAPIUrl");

            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(services => {
                    services.AddLogging();

                    services.AddHttpClient("WiseAPI", c => c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("WiseAPIUrl")));

                    services.AddTransient<IWiseService>(ctx =>
                    {
                        var clientFactory = ctx.GetRequiredService<IHttpClientFactory>();
                        var httpClient = clientFactory.CreateClient("WiseAPI");
                        var wiseLogger = _loggerFactory.CreateLogger(nameof(WiseService));
                        return new WiseService(httpClient, wiseLogger);
                    });
                })
                .Build();

            host.Run();
        }
    }
}