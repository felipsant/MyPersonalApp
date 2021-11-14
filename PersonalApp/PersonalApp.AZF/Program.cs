using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using PersonalApp.Services;
using System;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace PersonalApp
{
    //TODO: Azure Functions V4 - Dependency injection doesn't work.
    //https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection
    //This code doesn't run - Is just a Draft once microsoft fixes the issues
    public class Program
    {
        private static ILoggerFactory _loggerFactory;

        public static void Main()
        {
            _loggerFactory = new LoggerFactory();

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WiseAPIUrl")))
                throw new ArgumentNullException("WiseAPIUrl");

            var host = new HostBuilder()
                //.ConfigureFunctionsWorkerDefaults()
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