using DevEpos.CF.Demo.ServiceTests.Web;

namespace DevEpos.CF.Demo.IntegrationTests;

[Collection("Sequential")]
public class HelloWorldControllerTest : IClassFixture<WebTestFixture> {

    public HttpClient Client { get; }
    public HelloWorldControllerTest(WebTestFixture factory) {
        Client = factory.CreateClient();
    }

    [Fact]
    public async Task PrintHelloWorld() {
        var response = await Client.GetAsync("/HelloWorld");
        response.EnsureSuccessStatusCode();
        Assert.Equal("Hello World", await response.Content.ReadAsStringAsync());
    }
}
