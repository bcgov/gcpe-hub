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
    public class MediaRequestsIntegrationTests : ApiIntegrationTestBase
    {

        string baseURI = "api/mediarequests";

        /// <summary>
        /// Test GET api/mediarequests
        /// </summary>
        [Fact]
        public async void MediaRequestsGetTest()
        {
            string uri = baseURI;
            var response = await _client.GetAsync(uri);
            string contentString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Test GET api/mediarequests/search
        /// </summary>
        [Fact]
        public async void MediaRequestsSearchGetTest()
        {
            string uri = baseURI + "/search?query=test";
            var response = await _client.GetAsync(uri);
            string contentString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }


        /// <summary>
        /// Get a valid Ministry DTO object for use when creating a new media request.
        /// </summary>
        /// <returns>First MinistryDto available.</returns>
        private async Task<MinistryDto> GetMinistryDto()
        {
            string uri = "api/ministries";
            MinistryDto result = null;

            var response = await _client.GetAsync(uri);
            string contentString = await response.Content.ReadAsStringAsync();
            List < MinistryDto > data = JsonConvert.DeserializeObject<List<MinistryDto>>(contentString);
            if (data.Count > 0)
            {
                result = data[0];
            }

            return result;
        }

        /// <summary>
        /// Get a MediaContact object for use when creating a new media request.
        /// </summary>
        /// <returns>MediaContactDto for the first contact with an "e" in the name.</returns>
        private async Task<MediaContactDto> GetMediaContact()
        {
            string uri = "api/mediacontacts?filter=e";
            MediaContactDto result = null;

            var response = await _client.GetAsync(uri);
            string contentString = await response.Content.ReadAsStringAsync();
            List<MediaContactDto> data = JsonConvert.DeserializeObject<List<MediaContactDto>>(contentString);
            if (data.Count > 0)
            {
                result = data[0];
            }

            return result;
        }

        /// <summary>
        /// Get current user for use when creating a new media request.
        /// </summary>
        /// <returns>UserDto of the current user</returns>
        private async Task<UserDto> GetCurrentUser()
        {
            string uri = "api/users/me";
            UserDto result = null;

            var response = await _client.GetAsync(uri);
            string contentString = await response.Content.ReadAsStringAsync();
            result = JsonConvert.DeserializeObject<UserDto>(contentString);
            
            return result;
        }

        /// <summary>
        /// Create a new MediaRequest.
        /// </summary>
        /// <returns></returns>
        private async Task<MediaRequestDto> GetNewMediaRequest()
        {
            // Create a Media Request
            MediaRequestDto mrdto = new MediaRequestDto();
            // get a ministry object.
            mrdto.LeadMinistry = await GetMinistryDto();
            List<MediaContactDto> mediaContacts = new List<MediaContactDto>();
            mediaContacts.Add(await GetMediaContact());
            mrdto.MediaContacts = mediaContacts;
            mrdto.ResponsibleUser = await GetCurrentUser();
            mrdto.RequestedAt = DateTimeOffset.Now;
            mrdto.RequestContent = "TEST";
            mrdto.RequestTopic = "TEST";
            return mrdto;
        }

        /// <summary>
        /// Test media request CRUD operations
        /// </summary>
        ///

        [Fact]
        public async void MediaRequestCreateRetrieveUpdateDelete()
        {
            string uri = baseURI;

            // Create a Media Request
            MediaRequestDto mrdto = await GetNewMediaRequest();

            string textData = JsonConvert.SerializeObject(mrdto);
            HttpContent payload = new StringContent(textData, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(uri, payload);
            string contentString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            string newRecordId = JsonConvert.DeserializeObject<string>(contentString);
            uri = baseURI + "/" + newRecordId;

            // Retrieve
            response = await _client.GetAsync(uri);
            contentString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            MediaRequestDto retrieveRecord = JsonConvert.DeserializeObject<MediaRequestDto>(contentString);

            // Update
            string testString = "testUpdate";
            retrieveRecord.RequestTopic = testString;
            textData = JsonConvert.SerializeObject(retrieveRecord);
            payload = new StringContent(textData, Encoding.UTF8, "application/json");
            response = await _client.PutAsync(uri, payload);
            contentString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            // Verify that the database was updated.
            response = await _client.GetAsync(uri);
            contentString = await response.Content.ReadAsStringAsync();
            MediaRequestDto updatedRecord = JsonConvert.DeserializeObject<MediaRequestDto>(contentString);

            Assert.Equal(updatedRecord.RequestTopic,testString);

            // Delete 
            response = await _client.DeleteAsync(uri);
            response.EnsureSuccessStatusCode();

            // should get a 410 now
            response = await _client.GetAsync(uri);
            Assert.Equal(410, (int)response.StatusCode);
        }

        /// <summary>
        /// Test POST to postendofdayupdates
        /// </summary>
        [Fact]
        public async void MediaRequestsPostPostEndOfDayUpdates()
        {
            string uri = baseURI + "/postendofdayupdates";
            // end of day updates expects a list of ReportUpdate records.

            List<ReportUpdate> data = new List<ReportUpdate>();

            string stringData = JsonConvert.SerializeObject(data, Formatting.Indented);

            HttpContent content = new StringContent(stringData, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(uri, content);
            string contentString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Test POST to postendofdaysummary
        /// </summary>
        [Fact]
        public async void MediaRequestsPostPostEndOfDaySummary()
        {
            string uri = baseURI + "/postendofdaysummary";
            HttpContent content = new StringContent("[]", Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(uri, content);
            string contentString = await response.Content.ReadAsStringAsync();
            // Result can be forbidden if the current user does not have the Advanced role.
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Forbidden);
        }
        
        /// <summary>
        /// Test GET children by ID (returns count of children)
        /// </summary>
        [Fact]
        public async void MediaRequestsGetChildrenById()
        {
            // create a parent
            // Create a Media Request
            string uri = baseURI;

            // Create a Media Request
            MediaRequestDto mrdto = await GetNewMediaRequest();

            string textData = JsonConvert.SerializeObject(mrdto);
            HttpContent payload = new StringContent(textData, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(uri, payload);
            string contentString = await response.Content.ReadAsStringAsync();
            string recordId = JsonConvert.DeserializeObject<string>(contentString);
            uri = baseURI + "/children/" + recordId;
            string deleteUri = baseURI + "/" + recordId;
            response = await _client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            contentString = await response.Content.ReadAsStringAsync();

            int count;
            Assert.True(int.TryParse(contentString, out count));

            // clean up.
            response = await _client.DeleteAsync(deleteUri);
            response.EnsureSuccessStatusCode();

        }

        /// <summary>
        /// Test GET openforme
        /// </summary>
        [Fact]
        public async void MediaRequestsGetOpenforme()
        {
            string uri = baseURI + "/openforme";
            var response = await _client.GetAsync(uri);
            string contentString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Test GET resolutions
        /// </summary>
        [Fact]
        public async void MediaRequestsGetResolutions()
        {
            string uri = baseURI + "/resolutions";
            var response = await _client.GetAsync(uri);
            string contentString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }

    }
}
