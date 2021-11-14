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

namespace PersonalApp.Services.UnitTest.WiseServiceTests
{
    [TestClass]
    public class When_Get_Profile : WiseServiceBaseTests
    {
        [TestMethod]
        public async Task FoundData_Should_Contain_Results()
        {
            //Arrange           
            SetJsonResponseWiseService("Profile.json");

            //Act
            var result = await wiseService.GetProfiles(apiToken);

            //Assert
            Assert.AreEqual(result.Count, 2);
        }

        [TestMethod]
        public async Task DidntFoundData_Should_Work_And_Not_Contain_Results()
        {
            //Arrange
            SetHttpResponseWiseService(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

            //Act
            var result = await wiseService.GetProfiles(apiToken);

            //Assert
            Assert.AreEqual(result.Count, 0);
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
            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => wiseService.GetProfiles(apiToken));
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
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => wiseService.GetProfiles(string.Empty));
            Assert.AreEqual(LogLevel.Error, mockLogger.Invocations.Last().Arguments[0]);   
        }
    }
}
