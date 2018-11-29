using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using Gcpe.Hub.Website.Models;
using Gcpe.Hub.WebApp.Models;

namespace Gcpe.Hub.WebApp.Tests
{
    public class MinistryApiIntegrationTests : ApiIntegrationTestBase
    {
        string baseURI = "api/ministries";

        /// <summary>
        /// Test GET eodsummary
        /// </summary>
        [Fact]
        public async void EodSummaryGetTest()
        {
            string uri = baseURI + "/eodsummary";
            var response = await _client.GetAsync(uri);
            string contentString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }
        
        /// <summary>
        /// Test GET and PUT
        /// </summary>

        [Fact]
        public async void GetPutTest()
        {
            string getUri = baseURI;

            // get the list of Ministries
            var response = await _client.GetAsync(getUri);
            string contentString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            List<MinistryDto> Ministries = JsonConvert.DeserializeObject<List<MinistryDto>>(contentString);

            Assert.True(Ministries.Count > 0);

            MinistryDto mdto = Ministries[0];

            string afterHoursPhone = mdto.AfterHoursPhone;
            string updateTest = "000-000-0000";
            mdto.AfterHoursPhone = updateTest;

            string updateUri = getUri + "/" + mdto.Id;
            string jsonData = JsonConvert.SerializeObject(mdto);
            // update the record
            HttpContent payload = new StringContent(jsonData, Encoding.UTF8,"application/json");
            response = await _client.PutAsync(updateUri, payload);
            response.EnsureSuccessStatusCode();

            // verify the update
            response = await _client.GetAsync(getUri);
            contentString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            List<MinistryDto> UpdatedMinistries = JsonConvert.DeserializeObject<List<MinistryDto>>(contentString);
            Assert.True(UpdatedMinistries.Count > 0);
            Assert.Equal(UpdatedMinistries[0].AfterHoursPhone, updateTest);

            // switch it back
            mdto.AfterHoursPhone = afterHoursPhone;
            payload = new StringContent(JsonConvert.SerializeObject(mdto, Formatting.Indented), Encoding.UTF8, "application/json");
            response = await _client.PutAsync(updateUri, payload);
            response.EnsureSuccessStatusCode();
        }

    }
}
