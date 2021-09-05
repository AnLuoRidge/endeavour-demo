using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System.Text.Json;
using EndeavourDemo.Models;
using System.Linq;

namespace EndeavourDemoTest
{
    public class TrolleyTests : IClassFixture<WebApplicationFactory<EndeavourDemo.Startup>>
    {
        private readonly HttpClient _client;
        static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public TrolleyTests(WebApplicationFactory<EndeavourDemo.Startup> fixture)
        {
            _client = fixture.CreateClient();
        }

        [Fact]
        public async Task AddTwoSameProducts()
        {
            await _client.DeleteAsync("/trolley/all");
            var addResponse1 = await _client.PostAsync("/trolley/add?productId=1&qty=1", null);
            var addResponse2 = await _client.PostAsync("/trolley/add?productId=1&qty=1", null);
            addResponse1.StatusCode.Should().Be(HttpStatusCode.OK);
            addResponse2.StatusCode.Should().Be(HttpStatusCode.OK);
            var trolleyResponse = await _client.GetAsync("/trolley");
            var trolleyJson = await trolleyResponse.Content.ReadAsStringAsync();
            var trolley = JsonSerializer.Deserialize<TrolleyViewModel>(trolleyJson, jsonSerializerOptions);
            var trolleyItem = trolley.Items.Count(i => i.ProductId == 1 && i.Qty == 2).Should().Equals(1);
        }

        [Fact]
        public async Task AddItemNotExist()
        {
            var addResponse1 = await _client.PostAsync("/trolley/add?productId=-1&qty=1", null);
            addResponse1.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteOneItem()
        {
            await _client.DeleteAsync("/trolley/all");
            var addResponse = await _client.PostAsync("/trolley/add?productId=1&qty=1", null);
            var trolleyItemId = await addResponse.Content.ReadAsStringAsync();
            var removeResponse = await _client.DeleteAsync($"/trolley/remove/{trolleyItemId}");
            removeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteItemNotExist()
        {
            var removeResponse = await _client.DeleteAsync("/trolley/remove/-1");
            removeResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task PromotionComboTest1()
        {
            await _client.DeleteAsync("/trolley/all");
            var addResponse1 = await _client.PostAsync("/trolley/add?productId=1&qty=1", null);
            _ = await _client.PostAsync("/trolley/add?productId=2&qty=3", null);
            _ = await _client.PostAsync("/trolley/add?productId=3&qty=2", null);
            addResponse1.StatusCode.Should().Be(HttpStatusCode.OK);
            var trolleyResponse = await _client.GetAsync("/trolley");
            trolleyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var trolleyJson = await trolleyResponse.Content.ReadAsStringAsync();
            var trolley = JsonSerializer.Deserialize<TrolleyViewModel>(trolleyJson, jsonSerializerOptions);

            trolley.Items.FirstOrDefault(i => i.ProductId == 1).RealUnitPrice.Should().Be((decimal)19.49);
            trolley.Items.FirstOrDefault(i => i.ProductId == 1).RealSubtotal.Should().Be((decimal)19.49);
            trolley.Items.FirstOrDefault(i => i.ProductId == 2).RealSubtotal.Should().Be((decimal)40.98);
            trolley.Items.FirstOrDefault(i => i.ProductId == 3).RealSubtotal.Should().Be((decimal)39.98);
            trolley.RealTotal.Should().Be((decimal)95.45);
        }

        [Fact]
        public async Task PromotionComboTest2()
        {
            await _client.DeleteAsync("/trolley/all");
            var addResponse1 = await _client.PostAsync("/trolley/add?productId=1&qty=2", null);
            _ = await _client.PostAsync("/trolley/add?productId=2&qty=2", null);
            _ = await _client.PostAsync("/trolley/add?productId=3&qty=3", null);
            addResponse1.StatusCode.Should().Be(HttpStatusCode.OK);
            var trolleyResponse = await _client.GetAsync("/trolley");
            trolleyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var trolleyJson = await trolleyResponse.Content.ReadAsStringAsync();
            var trolley = JsonSerializer.Deserialize<TrolleyViewModel>(trolleyJson, jsonSerializerOptions);

            trolley.Items.FirstOrDefault(i => i.ProductId == 1).RealSubtotal.Should().Be((decimal)38.98);
            trolley.Items.FirstOrDefault(i => i.ProductId == 2).RealSubtotal.Should().Be((decimal)20.49);
            trolley.Items.FirstOrDefault(i => i.ProductId == 3).RealSubtotal.Should().Be((decimal)59.97);
            trolley.RealTotal.Should().Be((decimal)114.44);
        }

        [Fact]
        public async Task UpdateItem()
        {
            await _client.DeleteAsync("/trolley/all");
            var addResponse = await _client.PostAsync("/trolley/add?productId=3&qty=1", null);
            var trolleyItemId = await addResponse.Content.ReadAsStringAsync();
            var updateResponse = await _client.PutAsync($"/trolley/update?trolleyItemId={trolleyItemId}&qty=10", null);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var trolleyResponse = await _client.GetAsync("/trolley");
            var trolleyJson = await trolleyResponse.Content.ReadAsStringAsync();
            var trolley = JsonSerializer.Deserialize<TrolleyViewModel>(trolleyJson, jsonSerializerOptions);
            trolley.Items.FirstOrDefault(i => i.ProductId == 3).Qty.Should().Be(10);
        }

        [Fact]
        public async Task UpdateItemNotExist()
        {
            var updateResponse = await _client.PutAsync($"/trolley/update?trolleyItemId=-1&qty=10", null);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
