// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.Extensions.Configuration.CommandLine;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Extension methods for registering <see cref="CommandLineConfigurationProvider"/> with <see cref="IConfigurationBuilder"/>.
    /// </summary>
    public static class CommandLineConfigurationExtensions
    {
        /// <summary>
        /// Adds a <see cref="CommandLineConfigurationProvider"/> <see cref="IConfigurationProvider"/> 
        /// that reads configuration values from the command line.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="args">The command line args.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        /// <remarks>
        /// <para>
        /// The values passed on the command line, in the <c>args</c> string array, should be a set
        /// of keys prefixed with two dashes ("--") and then values, separate by either the 
        /// equals sign ("=") or a space (" ").
        /// </para>
        /// <para>
        /// A forward slash ("/") can be used as an alternative prefix, with either equals or space, and when using
        /// an equals sign the prefix can be left out altogether. 
        /// </para>
        /// <para>
        /// There are five basic alternative formats for arguments: 
        /// <c>key1=value1 --key2=value2 /key3=value3 --key4 value4 /key5 value5</c>.
        /// </para>
        /// </remarks>
        /// <example>
        /// A simple console application that has five values. 
        /// <code>
        /// // dotnet run key1=value1 --key2=value2 /key3=value3 --key4 value4 /key5 value5
        /// using Microsoft.Extensions.Configuration;
        /// using System;
        /// 
        /// namespace CommandLineSample
        /// {
        ///    public class Program
        ///    {
        ///        public static void Main(string[] args)
        ///        {
        ///            var builder = new ConfigurationBuilder();
        ///            builder.AddCommandLine(args);
        ///
        ///            var config = builder.Build();
        ///
        ///            Console.WriteLine($"Key1: '{config["Key1"]}'");
        ///            Console.WriteLine($"Key2: '{config["Key2"]}'");
        ///            Console.WriteLine($"Key3: '{config["Key3"]}'");
        ///            Console.WriteLine($"Key4: '{config["Key4"]}'");
        ///            Console.WriteLine($"Key5: '{config["Key5"]}'");
        ///        }
        ///    }
        /// }
        /// </code>
        /// </example>
        public static IConfigurationBuilder AddCommandLine(this IConfigurationBuilder configurationBuilder, string[] args)
        {
            return configurationBuilder.AddCommandLine(args, switchMappings: null);
        }

        /// <summary>
        /// Adds a <see cref="CommandLineConfigurationProvider"/> <see cref="IConfigurationProvider"/> that reads configuration values from the command line using the specified switch mappings.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="args">The command line args.</param>
        /// <param name="switchMappings">The switch mappings.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddCommandLine(
            this IConfigurationBuilder configurationBuilder,
            string[] args,
            IDictionary<string, string> switchMappings)
        {
            configurationBuilder.Add(new CommandLineConfigurationSource { Args = args, SwitchMappings = switchMappings });
            return configurationBuilder;
        }
    }
}
