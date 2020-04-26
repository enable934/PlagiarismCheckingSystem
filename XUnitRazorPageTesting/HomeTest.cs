using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using AngleSharp;
using XUnitRazorPageTesting.Helpers;
using AngleSharp.Html.Dom;
using System.Collections.Generic;

namespace XUnitRazorPageTesting
{
    public class HomeTest : IClassFixture<PlagiarismCheckingSystemFactory<PlagiarismCheckingSystem.Startup>>
    {
        private readonly PlagiarismCheckingSystemFactory<PlagiarismCheckingSystem.Startup> _factory;

        public HomeTest(PlagiarismCheckingSystemFactory<PlagiarismCheckingSystem.Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task IndexPageTest()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }



        [Fact]
        public async Task RegisterTest()
        {
            // Arrange
            var client = _factory.CreateClient();
            var defaultPage = await client.GetAsync("/Account/Register");
            var content = await HtmlHelpers.GetDocumentAsync(defaultPage);
            List<KeyValuePair<string, string>> formValues = new List<KeyValuePair<string, string>>();
            formValues.Add(new KeyValuePair<string, string>("Email", "example@example.com"));
            formValues.Add(new KeyValuePair<string, string>("Password", "123"));
            formValues.Add(new KeyValuePair<string, string>("ConfirmPassword", "123"));

            // Act
            var response = await client.SendAsync((IHtmlFormElement)content.QuerySelector("form"), formValues);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
