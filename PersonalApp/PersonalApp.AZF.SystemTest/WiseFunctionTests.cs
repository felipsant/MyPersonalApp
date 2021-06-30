using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PersonalApp.AZF.SystemTest.Common;
using System.Threading.Tasks;

namespace PersonalApp.AZF.SystemTest
{
    [TestClass]
    public class WiseFunctionTests : BaseAZFTest
    {
        private string WiseToken = "";

        [TestMethod]
        public async Task GetProfiles_ReturnsData()
        {
            //Arrange
            //Act
            var response = await _client.GetAsync($"wise/profile/{WiseToken}");

            //Assert
            Assert.IsTrue(response.IsSuccessStatusCode);
            var content = await response.Content.ReadAsStringAsync();

            JArray result = JArray.Parse(content);
            Assert.IsTrue(result.Count > 0);
        }
    }
}
