using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace CustomOptionClasses
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder();

            var builder = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();
        }
    }
}
