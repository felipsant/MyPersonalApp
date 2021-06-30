using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace PersonalApp.AZF
{
    public static class WakeUpFunction
    {
        [Function(nameof(WakeUpFunction))]
        public static HttpResponseData WakeUp(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wakeup")] HttpRequestData req, FunctionContext context)
        {
            var log = context.GetLogger(nameof(WakeUpFunction));
            log.LogInformation($"{nameof(WakeUp)} Started");

            string version = GetRunningVersion();

            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString(version);

            log.LogInformation($"{nameof(WakeUp)} Ended");
            return response;
        }

        private static string GetRunningVersion()
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                return assembly.GetName().Version.ToString();
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}