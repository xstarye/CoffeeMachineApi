using CoffeeMachineApi.Controllers;
using CoffeeMachineApi.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffieMachineApiTest
{
    //API test using MVC.testing, start point from API
    public class ApiTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly IConfiguration _config;
        public ApiTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _config = _factory.Services.GetService<IConfiguration>();
        }

        [Fact]
        public async Task Returns_503_5th_Call()
        {
            // Arrange the 5th time, which will return error 503, this method need to be run seperate
            var client = _factory.CreateClient();
            int result = 0;
            //arrage for 4 times

            for (int i = 0; i<4; i++)
            {
                result = ((int)client.GetAsync("/brew-coffee").Result.StatusCode);
            }
            //5th time
            result = ((int)client.GetAsync("/brew-coffee").Result.StatusCode);
            Assert.Equal(503, result);
        }

        [Fact]
        public async Task Returns_200_And_Valid()
        {
            // Arrange the normal situation
            var client = _factory.CreateClient();

            int result = ((int)client.GetAsync("/brew-coffee").Result.StatusCode);
            Assert.Equal(200, result);
        }

        [Fact]
        public async Task Response_Format_ISO8601()
        {
            // Arrange to test the format of the time to be in ISO 8601, as set hour span to 9
            var client = _factory.CreateClient();

            string result = await client.GetAsync("/brew-coffee").Result.Content.ReadAsStringAsync();

            Assert.Matches(@"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\+\d{2}:\d{2}", result);
        }

        [Fact]
        public async Task Returns_200_And_Refreshing()
        {
            //_config["Weather:ApiUrl"] = "";
            // Arrange the >30 situation, we need to set the API env to Dev, and config the host address based on wiremock address
            var client = _factory.CreateClient();

            string result = await client.GetAsync("/brew-coffee").Result.Content.ReadAsStringAsync();
            Assert.Contains("Your refreshing iced coffee is ready", result);
        }
    }
}
