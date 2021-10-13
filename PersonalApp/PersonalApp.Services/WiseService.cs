using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace PersonalApp.Services
{
    public interface IWiseService
    {
        Task<JArray> GetProfiles(string apiToken);
        Task<JArray> GetRates(string apiToken, string source, string target);
        Task<JArray> GetRates(string apiToken, string source, string target, DateTime? time, DateTime? from, DateTime? to, string group);
    }

    public class WiseService : IWiseService
    {
        private const string APIVersion = "v1";
        private HttpClient HttpClient { get; }
        private ILogger Logger { get; }
        public WiseService(HttpClient httpClient, ILogger logger)
        {
            this.HttpClient = httpClient;
            this.Logger = logger;
        }

        /// <summary>
        /// Method to set the APIToken in case the user has created a Wise Account
        /// </summary>
        /// <param name="apiToken">Token on Wise</param>
        private void SetAPITokenToHttpClient(string apiToken)
        {
            try
            {
                Logger.LogDebug($"{ nameof(SetAPITokenToHttpClient)} Started");

                if (string.IsNullOrEmpty(apiToken))
                    throw new ArgumentNullException(nameof(apiToken));

                this.HttpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", apiToken);
    
                Logger.LogDebug($"{ nameof(SetAPITokenToHttpClient)} Ended");
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"{ nameof(SetAPITokenToHttpClient)} Exception", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get profile information associated with the Token on Wise
        /// </summary>
        /// <returns>Profiles defined in the Wise API</returns>
        public async Task<JArray> GetProfiles(string apiToken)
        {
            JArray result = null;
            try
            { 
                Logger.LogDebug($"{ nameof(GetProfiles)} Started");

                SetAPITokenToHttpClient(apiToken);
                HttpResponseMessage httpResponse = await HttpClient.GetAsync(APIVersion + "/profiles");
                httpResponse.EnsureSuccessStatusCode();

                Logger.LogDebug($"{ nameof(GetProfiles)} Successfully HttpClient Request");
                string content = await httpResponse.Content.ReadAsStringAsync();
                if(!string.IsNullOrEmpty(content))
                    result = JArray.Parse(content);
                else
                    result = new JArray();

                Logger.LogDebug($"{ nameof(GetProfiles)} Ended");
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, $"{ nameof(GetProfiles)} Exception");
                throw;
            }
            return result;
        }

        /// <summary>
        /// Adding Method to read online desired Rates 
        /// </summary>
        /// <param name="apiToken">Token on Wise</param>
        /// <param name="source">3 letters Currency Name</param>
        /// <param name="target">3 letters Currency Name</param>
        public async Task<JArray> GetRates(string apiToken, string source, string target)
        {
            return await GetRates(apiToken, source, target, null, null, null, string.Empty);
        }

        /// <summary>
        /// Adding Method to read online desired Rates 
        /// </summary>
        /// <param name="apiToken">Token on Wise</param>
        /// <param name="source">3 letters Currency Name</param>
        /// <param name="target">3 letters Currency Name</param>
        /// <param name="time">Datetime to get a specific time</param>
        /// <param name="from">Start Datetime to get a range of time</param>
        /// <param name="to">End Datetime to get a range of time</param>
        /// <param name="group">Fixed string: day | hour | minute</param>
        /// <returns>Range of Rates</returns>
        public async Task<JArray> GetRates(string apiToken, string source, string target, DateTime? time, DateTime? from, DateTime? to, string group)
        {
            JArray result = null;
            try
            {
                Logger.LogDebug($"{ nameof(GetRates)} Started");

                if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
                    throw new ArgumentException("Source and Target are mandatory, they cannot be Null or Empty");
                
                SetAPITokenToHttpClient(apiToken);

                var parameters = HttpUtility.ParseQueryString(string.Empty);
                parameters[nameof(source)] = source;
                parameters[nameof(target)] = target;

                if (time.HasValue)
                    parameters[nameof(time)] = Utils.Utils.ConvertDateTimeToTimestamp(time.Value).ToString();
                if (from.HasValue)
                    parameters[nameof(from)] = Utils.Utils.ConvertDateTimeToTimestamp(from.Value).ToString();
                if (to.HasValue)
                    parameters[nameof(to)] = Utils.Utils.ConvertDateTimeToTimestamp(to.Value).ToString();
                if(string.IsNullOrEmpty(group))
                    parameters[nameof(group)] = group;

                string requestUrl = APIVersion + "/rates?" + parameters.ToString();
                HttpResponseMessage httpResponse = await HttpClient.GetAsync(requestUrl);
                httpResponse.EnsureSuccessStatusCode();
                string content = await httpResponse.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(content))
                    result = JArray.Parse(content);
                else
                    result = new JArray();

                Logger.LogDebug($"{ nameof(GetRates)} Ended");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{ nameof(GetRates)} Exception");
                throw;
            }
            return result;
        }
    }
}
