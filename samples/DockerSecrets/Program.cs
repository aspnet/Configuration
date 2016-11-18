using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DockerSecrets
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                    .AddDockerSecrets()
                    .Build();

            foreach(var item in config.AsEnumerable())
            {
                Console.WriteLine($"SecretKey: {item.Key}, SecretValue: {item.Value}");
            }
        }
    }
}
