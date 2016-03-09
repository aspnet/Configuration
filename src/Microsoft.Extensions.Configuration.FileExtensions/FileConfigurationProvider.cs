// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Base class for file based <see cref="ConfigurationProvider"/>.
    /// </summary>
    public abstract class FileConfigurationProvider : ConfigurationProvider
    {
        /// <summary>
        /// Gets a value that determines if this instance of <see cref="FileConfigurationProvider"/> is optional.
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        /// The <see cref="IFileInfo"/> representing the file.
        /// </summary>
        public IFileInfo File { get; set; }

        /// <summary>
        /// Loads the contents of the file at <see cref="Path"/>.
        /// </summary>
        /// <exception cref="FileNotFoundException">If <see cref="Optional"/> is <c>false</c> and a
        /// file does not exist at <see cref="Path"/>.</exception>
        public override void Load()
        {
            if (File == null || !File.Exists)
            {
                if (Optional)
                {
                    Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                else
                {
                    // REVIEW: should this be detected at build time instead?
                    throw new FileNotFoundException($"The configuration file '{File.Name}' was not found and is not optional.");
                }
            }
            else
            {
                using (var stream = File.CreateReadStream())
                {
                    Load(stream);
                }
            }
        }

        public abstract void Load(Stream stream);
    }
}
