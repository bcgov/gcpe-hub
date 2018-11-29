using System;
using System.Net.Http;
using System.Text;
using Gcpe.Hub.Website.Models;
using Newtonsoft.Json;
using Xunit;

namespace Gcpe.Hub.WebApp.Tests
{
    public class MediaContactsIntegrationTests : ApiIntegrationTestBase
    {

        string baseURI = "api/mediacontacts";

        /// <summary>
        /// Test GET outlets
        /// </summary>
        [Fact]
        public async void OutletsGetTest()
        {
            string uri = baseURI + "/outlets";
            var response = await _client.GetAsync(uri);
            string contentString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Test GET titles
        /// </summary>
        [Fact]
        public async void TitlesGetTest()
        {
            string uri = baseURI + "/titles";
            var response = await _client.GetAsync(uri);
            string contentString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            // parse the content string, ensure that it contains the expected content.
            string[] result = JsonConvert.DeserializeObject<string[]>(contentString);
            Assert.NotNull(result);
        }

        /// <summary>
        /// Test contact CRU operations
        /// There is no delete at this time
        /// </summary>

        [Fact]
        public async void MediaContactCreateRetrieveUpdate()
        {
            // Create a Media Contact
            string uri = baseURI;
            string uniqueHash = DateTime.Now.GetHashCode().ToString();
            string originalEmail = "test" + uniqueHash + "@test.com";
            MediaContactDto mcdto = new MediaContactDto();
            mcdto.CellPhone = "123-123-1234";
            mcdto.Email = originalEmail;
            mcdto.FirstName = "test" + uniqueHash;
            mcdto.LastName = "user";

            // Attach to a new Outlet.
            OutletDto odto = new OutletDto();
            odto.Name = "TestOutlet";
            odto.IsMajor = false;

            MediaJobDto mediaJobDto = new MediaJobDto();
            mediaJobDto.Outlet = odto;
            mediaJobDto.Title = "Reporter";
            mcdto.Job = mediaJobDto;

            HttpContent payload = new StringContent(JsonConvert.SerializeObject(mcdto, Formatting.Indented), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(uri, payload);
            string contentString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            Assert.True(contentString.Length > 0);
            MediaContactDto retrieveRecord = JsonConvert.DeserializeObject<MediaContactDto>(contentString);

            Assert.Equal(mcdto.FirstName, retrieveRecord.FirstName);
            Assert.Equal(mcdto.CellPhone, retrieveRecord.CellPhone);

            // Update
            uri = baseURI + "/" + retrieveRecord.Id;
            string testString = "test2" + uniqueHash + "@test.com";
            retrieveRecord.Email = testString;
            payload = new StringContent(JsonConvert.SerializeObject(retrieveRecord, Formatting.Indented), Encoding.UTF8, "application/json");
            response = await _client.PutAsync(uri, payload);
            response.EnsureSuccessStatusCode();

            // Verify that the database was updated.
            response = await _client.GetAsync(uri);
            contentString = await response.Content.ReadAsStringAsync();
            MediaContactDto updatedRecord = JsonConvert.DeserializeObject<MediaContactDto>(contentString);
            Assert.Equal(updatedRecord.Email, testString);

            // Revert change.
            retrieveRecord.Email = originalEmail;
            payload = new StringContent(JsonConvert.SerializeObject(retrieveRecord, Formatting.Indented), Encoding.UTF8, "application/json");
            response = await _client.PutAsync(uri, payload);
            response.EnsureSuccessStatusCode();
        }

    }
}
