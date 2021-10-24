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
        #region GetProfiles
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
        #endregion
        #region GetRates
        [TestMethod]
        public async Task GetRates_ReturnsData()
        {
            //Arrange
            string json;
            using(StreamReader r = new StreamReader(@".\WiseJsons\SimpleRate.json"))
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
            var result = await wiseService.GetRates(apiToken, "EUR", "BRL");

            //Assert
            Assert.AreEqual(result.Count, 1);
        }

        [TestMethod]
        public async Task GetRates_ExceptionsAreLogged()
        {
            //Arrange
            var logger = new Mock<ILogger>();
            var wiseService = new WiseService(null, logger.Object);

            //Act
            try
            {
                var result = await wiseService.GetRates(apiToken, "test", "test");
            }
            //Assert
            catch (System.NullReferenceException)
            {
                Assert.AreEqual(LogLevel.Error, logger.Invocations.Last().Arguments[0]);
                return;
            }
            Assert.Fail("No exception was throw");
        }

        [TestMethod]
        public async Task GetRates_ParametersAreMandatory()
        {
            //Arrange
            var logger = new Mock<ILogger>();
            var wiseService = new WiseService(null, logger.Object);

            //Act
            try
            {
                var result = await wiseService.GetRates(apiToken, "", "");
            }
            //Assert
            catch (System.ArgumentException)
            {
                Assert.AreEqual(LogLevel.Error, logger.Invocations.Last().Arguments[0]);
                return;
            }
            Assert.Fail("No exception was throw");
        }

        [TestMethod]
        public async Task GetRates_NoAPITokenException()
        {
            //Arrange
            var logger = new Mock<ILogger>();
            var client = new HttpClient();
            client.BaseAddress = new System.Uri("http://localhost:80");
            var wiseService = new WiseService(client, logger.Object);

            //Act
            try
            {
                var result = await wiseService.GetRates(string.Empty, "test", "test");
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
        public async Task GetRates_UnauthorizedRequest()
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
                var result = await wiseService.GetRates(apiToken, "TEST", "TEST");
            }
            //Assert
            catch (HttpRequestException)
            {
                Assert.AreEqual(LogLevel.Error, logger.Invocations.Last().Arguments[0]);
                return;
            }
        }

        [TestMethod]
        public async Task GetRatesSpecificDate_ReturnsData()
        {
            //Arrange
            string json;
            using (StreamReader r = new StreamReader(@".\WiseJsons\SimpleRate.json"))
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
            var result = await wiseService.GetRates(apiToken, "EUR", "BRL", DateTime.Now.AddDays(-1));

            //Assert
            Assert.AreEqual(result.Count, 1);
        }

        [TestMethod]
        public async Task GetRatesMultipleDays_ReturnsData()
        {
            //Arrange
            string json;
            using (StreamReader r = new StreamReader(@".\WiseJsons\MultipleDaysRate.json"))
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
            var result = await wiseService.GetRates(apiToken, "EUR", "BRL", DateTime.Now.AddDays(-5), DateTime.Now, "day");

            //Assert
            Assert.AreEqual(result.Count, 6);
        }
        #endregion
    }
}
