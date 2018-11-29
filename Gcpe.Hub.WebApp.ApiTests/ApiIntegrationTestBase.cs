using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Gcpe.Hub.WebApp.Tests
{
    public abstract class ApiIntegrationTestBase
    {
        protected readonly TestServer _server;
        protected readonly HttpClient _client;

        /// <summary>
        /// Setup the test
        /// </summary>
        public ApiIntegrationTestBase()
        {
            string contentRoot = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName + "\\Gcpe.Hub.WebApp";

            _server = new TestServer(new WebHostBuilder()
            .UseContentRoot(contentRoot)
            .UseEnvironment("")
            .UseStartup<Startup>());

            _client = _server.CreateClient();
        }
    }
}
