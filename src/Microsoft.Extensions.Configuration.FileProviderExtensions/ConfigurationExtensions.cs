// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Configuration
{
    public static class FileProviderExtensions
    {
        public static IConfiguration ReloadOnChanged(this IConfiguration config, string filename)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (filename == null)
            {
                throw new ArgumentNullException(nameof(filename));
            }
#if NET451
            var basePath = AppDomain.CurrentDomain.GetData("APP_CONTEXT_BASE_DIRECTORY") as string ??
                AppDomain.CurrentDomain.BaseDirectory ?? string.Empty;
#else
            var basePath = AppContext.BaseDirectory ?? string.Empty;
#endif
            return ReloadOnChanged(config, basePath, filename);
        }

        public static IConfiguration ReloadOnChanged(this IConfiguration config, string basePath, string filename)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (basePath == null)
            {
                throw new ArgumentNullException(nameof(basePath));
            }

            if (filename == null)
            {
                throw new ArgumentNullException(nameof(filename));
            }

            var fileProvider = new PhysicalFileProvider(basePath);
            return ReloadOnChanged(config, fileProvider, filename);
        }

        public static IConfiguration ReloadOnChanged(this IConfiguration config, IFileProvider fileProvider, string filename)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (fileProvider == null)
            {
                throw new ArgumentNullException(nameof(fileProvider));
            }

            if (filename == null)
            {
                throw new ArgumentNullException(nameof(filename));
            }

            ChangeToken.OnChange(() => fileProvider.Watch(filename), () => config.ReloadAll());
            return config;
        }
    }
}
