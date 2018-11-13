using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatHub.IntegrationTests.Common
{
    public class TestStartup
    {
        public TestStartup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public IContainer ApplicationContainer { get; private set; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSignalR(options => { options.EnableDetailedErrors = Environment.IsDevelopment(); });

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            ApplicationContainer = containerBuilder.Build();
            return new AutofacServiceProvider(ApplicationContainer);
        }

        public void Configure(IApplicationBuilder app,
                              IApplicationLifetime applicationLifetime)
        {

            app.UseAuthentication();
            app.UseWebSockets();

            app.UseSignalR(route =>
            {
                route.MapHub<ChatHub>("/hubs/chat");
            });

            app.UseMvc();

            applicationLifetime.ApplicationStopped.Register(() => ApplicationContainer.Dispose());
        }
    }
}