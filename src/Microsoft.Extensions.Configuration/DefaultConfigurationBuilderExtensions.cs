// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration.Memory;

namespace Microsoft.Extensions.Configuration
{
    public static class CoreConfigurationBuilderExtensions
    {
        /// <summary>
        /// Includes an existing IConfiguration as a configuration provider to <paramref name="configuraton"/>.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="config">The <see cref="IConfiguration"/> to include.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder Include(this IConfigurationBuilder configurationBuilder, IConfiguration configuration)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }
            configurationBuilder.Add(new IncludeConfigurationProvider(configuration));
            return configurationBuilder;
        }

        /// <summary>
        /// Includes an existing IConfiguration as a configuration provider to <paramref name="configuraton"/>.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="sectionKey">Includes the configuration starting from the child section found with this key.</param>
        /// <param name="config">The <see cref="IConfiguration"/> to include.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder Include(this IConfigurationBuilder configurationBuilder, string sectionKey, IConfiguration configuration)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }
            configurationBuilder.Add(new IncludeConfigurationProvider(configuration.GetSection(sectionKey)));
            return configurationBuilder;
        }

        /// <summary>
        /// Adds the memory configuration provider to <paramref name="configuraton"/>.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddInMemoryCollection(this IConfigurationBuilder configurationBuilder)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }

            configurationBuilder.Add(new MemoryConfigurationProvider());
            return configurationBuilder;
        }

        /// <summary>
        /// Adds the memory configuration provider to <paramref name="configuraton"/>.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="initialData">The data to add to memory configuration provider.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddInMemoryCollection(
            this IConfigurationBuilder configurationBuilder,
            IEnumerable<KeyValuePair<string, string>> initialData)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }

            configurationBuilder.Add(new MemoryConfigurationProvider(initialData));
            return configurationBuilder;
        }
    }
}
