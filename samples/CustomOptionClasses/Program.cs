using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace CustomOptionClasses
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
