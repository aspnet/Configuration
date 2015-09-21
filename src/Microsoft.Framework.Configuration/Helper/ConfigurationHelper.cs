// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;

namespace Microsoft.Framework.Configuration.Helper
{
    public static class ConfigurationHelper
    {
        public static string ResolveConfigurationFilePath(IConfigurationBuilder configuration, string path)
        {
            string basePath = configuration.Properties["BasePath"].ToString();

            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(basePath, path);
            }

            return path;
        }
    }
}