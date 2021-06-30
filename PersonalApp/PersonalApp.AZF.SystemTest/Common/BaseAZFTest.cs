using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace PersonalApp.AZF.SystemTest.Common
{
    public class BaseAZFTest
    {
        internal HttpClient _client;

        public BaseAZFTest()
        {
            _client = new HttpClient()
            {
                BaseAddress = new System.Uri("http://localhost:7071/api/")
            };
            this._client.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
