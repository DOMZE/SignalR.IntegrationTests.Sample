using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ChatHub.IntegrationTests.Common
{
    public class WebApiFactory : WebApplicationFactory<TestStartup>
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder()
                .UseStartup<TestStartup>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseContentRoot(".");
            base.ConfigureWebHost(builder);
        }
    }
}