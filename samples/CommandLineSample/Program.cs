// dotnet run key1=value1 --key2=value2 /key3=value3 --key4 value4 /key5 value5
// dotnet run -k1=value1 -k2 value2 --alt3=value2 /alt4=value3 --alt5 value5 /alt6 value6

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace CommandLineSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var switchMappings = new Dictionary<string, string> ()
            {
                { "-k1", "key1" },
                { "-k2", "key2" },
                { "--alt3", "key3" },
                { "--alt4", "key4" },
                { "--alt5", "key5" },
                { "--alt6", "key6" },
            }; 
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args, switchMappings);

            var config = builder.Build();

            Console.WriteLine($"Key1: '{config["Key1"]}'");
            Console.WriteLine($"Key2: '{config["Key2"]}'");
            Console.WriteLine($"Key3: '{config["Key3"]}'");
            Console.WriteLine($"Key4: '{config["Key4"]}'");
            Console.WriteLine($"Key5: '{config["Key5"]}'");
            Console.WriteLine($"Key6: '{config["Key6"]}'");
        }
    }
}
