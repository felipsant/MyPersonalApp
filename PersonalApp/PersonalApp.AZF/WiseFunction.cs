using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
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

        [Function(nameof(WiseFunction))]
        public async Task<HttpResponseData> GetProfile(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wise/profile/{wiseToken}")] HttpRequestData req, FunctionContext context, string wiseToken)
        {
            var log = context.GetLogger(nameof(WiseFunction));
            log.LogInformation($"{nameof(WiseFunction)} Started");

            JArray result = await this.WiseService.GetProfiles(wiseToken);

            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await response.WriteStringAsync(result.ToString());

            log.LogInformation($"{nameof(WiseFunction)} Ended");
            return response;
        }
    }
}
