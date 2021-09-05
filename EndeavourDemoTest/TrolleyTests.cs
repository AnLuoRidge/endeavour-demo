using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace EndeavourDemoTest
{
    public class TrolleyTests : IClassFixture<WebApplicationFactory<EndeavourDemo.Startup>>
    {
        private readonly HttpClient _client;
        public TrolleyTests(WebApplicationFactory<EndeavourDemo.Startup> fixture)
        {
            _client = fixture.CreateClient();
        }

        [Fact]
        public async Task ClearTrolley()
        {
            var response = await _client.DeleteAsync("/trolley/all");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
