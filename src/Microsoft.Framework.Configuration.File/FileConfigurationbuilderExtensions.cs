// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Framework.Configuration
{
    public static class FileConfigurationBuilderExtensions
    {
        /// <summary>
        /// Sets the base path in <see cref="IConfigurationBuilder"/> for file based providers.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="basePath">Absolute path of file based providers.
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder SetBasePath(this IConfigurationBuilder builder, string basePath)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (basePath == null)
            {
                throw new ArgumentNullException(nameof(basePath));
            }

            if (builder.Properties.ContainsKey("BasePath"))
            {
                builder.Properties["BasePath"] = basePath;
            }
            else
            {
                builder.Properties.Add("BasePath", basePath);
            }

            return builder;
        }
    }
}
