﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PersonalApp.Services
{
    public interface IWiseService
    {
        Task<JArray> GetProfiles(string apiToken);
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
    }
}