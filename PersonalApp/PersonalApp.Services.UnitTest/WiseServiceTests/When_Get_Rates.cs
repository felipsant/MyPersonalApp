using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    [TestClass]
    public class When_Get_Rates : WiseServiceBaseTests
    {
        [TestMethod]
        public async Task CurrentDayRate_Should_Contain_Results()
        {
            //Arrange
            SetJsonResponseWiseService("SingleDayRate.json");

            //Act
            var result = await wiseService.GetRates(apiToken, "EUR", "BRL");

            //Assert
            Assert.AreEqual(result.Count, 1);
        }

        [TestMethod]
        public async Task SpecificDayRate_Should_Contain_Results()
        {
            //Arrange
            SetJsonResponseWiseService("SingleDayRate.json");


            //Act
            var result = await wiseService.GetRates(apiToken, "EUR", "BRL", DateTime.Now.AddDays(-1));

            //Assert
            Assert.AreEqual(result.Count, 1);
        }

        [TestMethod]
        public async Task DateRangeRate_Should_Contain_Many_Results()
        {
            //Arrange
            SetJsonResponseWiseService("MultipleDaysRate.json");

            //Act
            var result = await wiseService.GetRates(apiToken, "EUR", "BRL", DateTime.Now.AddDays(-5), DateTime.Now, "day");

            //Assert
            Assert.AreEqual(result.Count, 6);
        }

        [TestMethod]
        public async Task MandatoryParameters_Not_Present_Should_Throw_Exception()
        {
            //Arrange
            SetHttpResponseWiseService(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

            //Act && Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => wiseService.GetRates(apiToken, "", ""));
            Assert.AreEqual(LogLevel.Error, mockLogger.Invocations.Last().Arguments[0]);
        }

        [TestMethod]
        public async Task NoAPIToken_Should_Throw_Exception()
        {
            //Arrange
            SetHttpResponseWiseService(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

            //Act && Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => wiseService.GetRates(string.Empty, "EUR", "BRL"));
            Assert.AreEqual(LogLevel.Error, mockLogger.Invocations.Last().Arguments[0]);
        }

        [TestMethod]
        public async Task UnauthorizedRequest_Should_Throw_Exception()
        {
            //Arrange
            SetHttpResponseWiseService(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized
            });

            //Act && Assert
            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => wiseService.GetRates(apiToken, "TEST", "TEST"));
            Assert.AreEqual(LogLevel.Error, mockLogger.Invocations.Last().Arguments[0]);
        }


        


    }
}
