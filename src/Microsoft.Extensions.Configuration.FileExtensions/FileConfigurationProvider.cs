// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Base class for file based <see cref="ConfigurationProvider"/>.
    /// </summary>
    public abstract class FileConfigurationProvider : ConfigurationProvider
    {
        public FileConfigurationProvider(FileConfigurationSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            Source = source;

            // TODO: aggregate watch?
            if (Source.ReloadOnChange)
            {
                ChangeToken.OnChange(() => Source.FileProvider.Watch(Source.Path), () => Load());
            }
        }

        public FileConfigurationSource Source { get; }


        /// <summary>
        /// Loads the contents of the file at <see cref="Path"/>.
        /// </summary>
        /// <exception cref="FileNotFoundException">If Optional is <c>false</c> on the source and a
        /// file does not exist at specified Path.</exception>
        public override void Load()
        {
            var file = Source.FileProvider?.GetFileInfo(Source.Path);
            if (file == null || !file.Exists)
            {
                if (Source.Optional)
                {
                    Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                else
                {
                    throw new FileNotFoundException($"The configuration file '{Source.Path}' was not found and is not optional.");
                }
            }
            else
            {
                using (var stream = file.CreateReadStream())
                {
                    Load(stream);
                }
            }
        }

        public abstract void Load(Stream stream);
    }
}
