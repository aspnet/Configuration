﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;

namespace Microsoft.Framework.ConfigurationModel.Helper
{
    public static class ConfigurationHelper
    {
        public static string ResolveConfigurationFilePath(IConfigurationSourceRoot configuration, string path)
        {
            if (!Path.IsPathRooted(path))
            {
                if (configuration.BasePath == null)
                {
                    // Unable to resolve path '{0}'; construct this {2} with a non-null {1}.
                    throw new InvalidOperationException(
                        string.Format(Resources.Error_MissingBasePath,
                            path,
                            typeof(IConfigurationSourceRoot).Name,
                            nameof(configuration.BasePath)));
                }
                else
                {
                    path = Path.Combine(configuration.BasePath, path);
                }
            }

            return path;
        }
    }
}