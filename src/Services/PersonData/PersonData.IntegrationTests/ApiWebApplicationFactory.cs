using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace PersonData.IntegrationTests;

public class ApiWebApplicationFactory : WebApplicationFactory<AWC.PersonData.API.Program>
{
    public IConfiguration? Configuration { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("integrationsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            config.AddConfiguration(Configuration);
        });
    }
}
