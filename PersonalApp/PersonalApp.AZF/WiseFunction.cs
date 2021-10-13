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

        public WiseFunction(IWiseService wiseService)
        {
            this.WiseService = wiseService;
        }
        
        [FunctionName(nameof(GetProfile))]
        public async Task<IActionResult> GetProfile(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wise/profile/{wiseToken}")] HttpRequest req, ILogger log, string wiseToken)
        {
            log.LogInformation($"{nameof(GetProfile)} Started");

            JArray result = await this.WiseService.GetProfiles(wiseToken);

            log.LogInformation($"{nameof(GetProfile)} Ended");
            return new OkObjectResult(result.ToString());
        }

        [FunctionName(nameof(GetRates))]
        public async Task<IActionResult> GetRates(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wise/rate/{wiseToken}/{source}/{target}")] HttpRequest req, ILogger log, string wiseToken, string source, string target)
        {
            log.LogInformation($"{nameof(GetProfile)} Started");

            JArray result = await this.WiseService.GetRates(wiseToken, source, target);

            log.LogInformation($"{nameof(GetProfile)} Ended");
            return new OkObjectResult(result.ToString());
        }
    }
}
