using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace PersonalApp.AZF.SystemTest.Common
{
    public class BaseAZFTest
    {
        public static HttpClient _client;
        public static TestContext TestContext { get; set; }
        public static string AzureFunctionKey { get; set; }
        public static string AzureFunctionURL { get; set; }
        public static string WiseToken { get; set; }

        public static void InitializeBase(TestContext context)
        {
            TestContext = context;
            AzureFunctionURL = TestContext.Properties["AzureFunctionURL"].ToString();
            AzureFunctionKey = TestContext.Properties["AzureFunctionKey"].ToString();
            WiseToken = TestContext.Properties["WiseToken"].ToString();

            _client = new HttpClient()
            {
                BaseAddress = new System.Uri(AzureFunctionURL)
            };
            _client.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
