using System;
using Xunit;

namespace Gcpe.Hub.WebApp.Tests
{

    public class SearchWithAzure : ApiIntegrationTestBase
    {
        /// <summary>
        /// Setup the test
        /// </summary>
        public SearchWithAzure() : base(true)
        {
        }

        /// <summary>
        /// Test GET search
        /// </summary>
        [Fact]
        public async void SearchGetTest()
        {
            string uri = "api/mediarequests/search?query=test";
            var response = await _client.GetAsync(uri);
            String contentString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }
    }
}
