using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersonalApp.AZF.SystemTest.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalApp.AZF.SystemTest
{
    [TestClass]
    public class WakeUpFunctionTests : BaseAZFTest
    {
        [TestMethod]
        public async Task WakeUp_ReturnsAssemblyVersion()
        {
            //Arrange
            //Act
            var response = await _client.GetAsync("wakeup");

            //Assert
            Assert.IsTrue(response.IsSuccessStatusCode);
            var content = await response.Content.ReadAsStringAsync();

            Assert.IsFalse(string.IsNullOrEmpty(content));
        }
    }
}
