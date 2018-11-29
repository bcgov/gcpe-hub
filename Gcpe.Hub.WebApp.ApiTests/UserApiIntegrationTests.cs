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
    public class UserApiIntegrationTests : ApiIntegrationTestBase
    {

        string baseURI = "api/users";

        /// <summary>
        /// Test GET 
        /// </summary>
        [Fact]
        public async void GetTest()
        {
            string uri = baseURI;
            var response = await _client.GetAsync(uri);
            String contentString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            // Confirm we got a list of users
            List<UserDto> userDtos = JsonConvert.DeserializeObject<List<UserDto>>(contentString);
            Assert.NotNull(userDtos);
        }

        /// <summary>
        /// Test GET current user
        /// </summary>
        [Fact]
        public async void MeGetTest()
        {
            string uri = baseURI + "/me";
            var response = await _client.GetAsync(uri);
            String contentString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            // Confirm we got a user
            UserDto userDto = JsonConvert.DeserializeObject<UserDto>(contentString);
            Assert.NotNull(userDto);
        }
    }
}
