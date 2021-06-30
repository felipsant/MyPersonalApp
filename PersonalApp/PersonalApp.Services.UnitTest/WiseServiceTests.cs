using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PersonalApp.Services.UnitTest
{
    [TestClass]
    public class WiseServiceTests
    {
        private string apiToken = "FAKETOKEN";
        [TestMethod]
        public async Task GetProfiles_ReturnsData()
        {
            //Arrange
            string json;
            using (StreamReader r = new StreamReader(@".\WiseJsons\Profile.json"))
            {
                json = r.ReadToEnd();
            }
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                }); 
            var logger = new Mock<ILogger>();
            var client = new HttpClient(mockHttpMessageHandler.Object);
            client.BaseAddress = new System.Uri("http://localhost:80");
            var wiseService = new WiseService(client, logger.Object);

            //Act
            var result = await wiseService.GetProfiles(apiToken);

            //Assert
            Assert.AreEqual(result.Count, 2);
        }

        [TestMethod]
        public async Task GetProfiles_DidntFoundData()
        {
            //Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });
            var logger = new Mock<ILogger>();
            var client = new HttpClient(mockHttpMessageHandler.Object);
            client.BaseAddress = new System.Uri("http://localhost:80");
            var wiseService = new WiseService(client, logger.Object);

            //Act
            var result = await wiseService.GetProfiles(apiToken);

            //Assert
            Assert.AreEqual(result.Count, 0);
        }
    
        [TestMethod]
        public async Task GetProfiles_UnauthorizedRequest()
        {
            //Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized
                });

            var logger = new Mock<ILogger>();
            var client = new HttpClient(mockHttpMessageHandler.Object);
            client.BaseAddress = new System.Uri("http://localhost:80");
            var wiseService = new WiseService(client, logger.Object);

            //Act
            try
            {
                var result = await wiseService.GetProfiles(apiToken);
            }
            //Assert
            catch (HttpRequestException)
            {
                Assert.AreEqual(LogLevel.Error, logger.Invocations.Last().Arguments[0]);
                return;
            }

        }

        [TestMethod]
        public async Task GetProfiles_NoAPITokenException()
        {
            //Arrange
            var logger = new Mock<ILogger>();
            var client = new HttpClient();
            client.BaseAddress = new System.Uri("http://localhost:80");
            var wiseService = new WiseService(client, logger.Object);

            //Act
            try
            {
                var result = await wiseService.GetProfiles(string.Empty);
            }
            //Assert
            catch (System.ArgumentNullException)
            {
                Assert.AreEqual(LogLevel.Error, logger.Invocations.Last().Arguments[0]);
                return;
            }
            Assert.Fail("No exception was throw");
        }

        [TestMethod]
        public async Task GetProfiles_ExceptionsAreLogged()
        {
            //Arrange
            var logger = new Mock<ILogger>();
            var wiseService = new WiseService(null, logger.Object);
            
            //Act
            try
            {
                var result = await wiseService.GetProfiles(apiToken);
            }
            //Assert
            catch (System.NullReferenceException)
            {
                Assert.AreEqual(LogLevel.Error, logger.Invocations.Last().Arguments[0]);
                return;
            }
            Assert.Fail("No exception was throw");
        }
    }
}