using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PersonalApp.Services;

namespace PersonalApp.AZF
{
    public class WiseFunction
    {
        private IWiseService WiseService;
        private HttpClient Client;

        public WiseFunction()
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("WiseAPIUrl"));
        }
        /*
        public WiseFunction(IWiseService wiseService)
        {
            this.WiseService = wiseService;
        }*/

        [FunctionName(nameof(GetProfile))]
        public async Task<IActionResult> GetProfile(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wise/profile/{wiseToken}")] HttpRequest req, ILogger log, string wiseToken)
        {
            log.LogInformation($"{nameof(GetProfile)} Started");

            WiseService = new WiseService(Client, log);
            JArray result = await this.WiseService.GetProfiles(wiseToken);

            log.LogInformation($"{nameof(GetProfile)} Ended");
            return new OkObjectResult(result);
        }

        [FunctionName(nameof(GetRates))]
        public async Task<IActionResult> GetRates(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wise/rate/{wiseToken}/{source}/{target}")] HttpRequest req, ILogger log, string wiseToken, string source, string target)
        {
            log.LogInformation($"{nameof(GetRates)} Started");

            WiseService = new WiseService(Client, log);
            JArray result = await this.WiseService.GetRates(wiseToken, source, target);

            log.LogInformation($"{nameof(GetRates)} Ended");
            return new OkObjectResult(result);
        }

        [FunctionName(nameof(GetRatesSpecificDay))]
        public async Task<IActionResult> GetRatesSpecificDay(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wise/rate/{wiseToken}/{source}/{target}/{time}")] HttpRequest req, ILogger log, string wiseToken, string source, string target, DateTime time)
        {
            log.LogInformation($"{nameof(GetRatesSpecificDay)} Started");

            WiseService = new WiseService(Client, log);
            JArray result = await this.WiseService.GetRates(wiseToken, source, target, time);

            log.LogInformation($"{nameof(GetRatesSpecificDay)} Ended");
            return new OkObjectResult(result);
        }

        [FunctionName(nameof(GetRatesMultipleDays))]
        public async Task<IActionResult> GetRatesMultipleDays(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wise/rate/{wiseToken}/{source}/{target}/{from}/{to}/{group}")] HttpRequest req, ILogger log, string wiseToken, string source, string target, DateTime from, DateTime to, string group)
        {
            log.LogInformation($"{nameof(GetRatesMultipleDays)} Started");

            WiseService = new WiseService(Client, log);
            JArray result = await this.WiseService.GetRates(wiseToken, source, target, from, to, group);

            log.LogInformation($"{nameof(GetRatesMultipleDays)} Ended");
            return new OkObjectResult(result);
        }
    }
}
