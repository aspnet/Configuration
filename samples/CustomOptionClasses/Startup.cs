using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CustomOptionClasses
{
    public class Startup 
    {
        private readonly IConfigurationRoot _configuration;

        public Startup()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            // Make IOptions<MyOptions> available for dependency injection.
            services.Configure<MyOptions>(_configuration.GetSection("MyOptions"));

            // Access MyOptions in ConfigureServices
            var myOptions = services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<MyOptions>>()
                .Value;

            Console.WriteLine($"MyOptions in ConfigureServices: {myOptions.StringOption} {myOptions.IntegerOption}"); 
            
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IOptions<MyOptions> myOptionsProvider)
        {
            // Access MyOptions in Configure
            var myOptions = myOptionsProvider.Value;
            Console.WriteLine($"MyOptions in Configure: {myOptions.StringOption} {myOptions.IntegerOption}"); 

            app.UseMvcWithDefaultRoute();
        }
    }
}