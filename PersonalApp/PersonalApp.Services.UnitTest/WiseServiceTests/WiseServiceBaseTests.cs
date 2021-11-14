using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PersonalApp.Services.UnitTest.WiseServiceTests
{
    public class WiseServiceBaseTests
    {
        protected string apiToken = "FAKETOKEN";
        protected string wiseJsonsLocation = @".\WiseServiceTests\WiseJsons\";        
        protected Mock<HttpMessageHandler> mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        protected Mock<ILogger> mockLogger = new Mock<ILogger>();
        protected HttpClient wiseClient;
        protected WiseService wiseService;

        protected void SetJsonResponseWiseService(string fileName)
        {
            var response = NewHttpResponseMessage(wiseJsonsLocation + fileName);
            SetHttpResponseWiseService(response);
        }

        protected void SetHttpResponseWiseService(HttpResponseMessage response)
        {
            SetHttpMessageHandler(response);
            SetHttpClient();
            wiseService = new WiseService(wiseClient, mockLogger.Object);
        }


        private void SetHttpClient()
        {
            wiseClient = new HttpClient(mockHttpMessageHandler.Object);
            wiseClient.BaseAddress = new System.Uri("http://localhost:80");
        }

        private HttpResponseMessage NewHttpResponseMessage(string fileName)
        {
            string json = File.ReadAllText(fileName);

            return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json)
            };
        }

        private void SetHttpMessageHandler(HttpResponseMessage httpResponse)
        {
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);
        }
    }
}
