using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace PersonalApp.AZF
{
    public static class WakeUpFunction
    {
        
        [FunctionName(nameof(WakeUpFunction))]
        public static async Task<IActionResult> WakeUp(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wakeup")] HttpRequest req, ILogger log)
        {
            //, FunctionContext context
            //var log = context.GetLogger(nameof(WakeUpFunction));
            log.LogInformation($"{nameof(WakeUp)} Started");

            string version = GetRunningVersion();

            log.LogInformation($"{nameof(WakeUp)} Ended");
            return new OkObjectResult(version);
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