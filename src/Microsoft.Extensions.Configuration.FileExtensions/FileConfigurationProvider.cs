// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Base class for file based <see cref="ConfigurationProvider"/>.
    /// </summary>
    public abstract class FileConfigurationProvider : ConfigurationProvider
    {
        private readonly FileConfigurationSource _source;

        public FileConfigurationProvider(FileConfigurationSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            _source = source;

            // TODO: aggregate watch?
            if (_source.ReloadOnChange)
            {
                ChangeToken.OnChange(() => _source.FileProvider.Watch(_source.Path), () => Load());
            }
        }

        /// <summary>
        /// Loads the contents of the file at <see cref="Path"/>.
        /// </summary>
        /// <exception cref="FileNotFoundException">If Optional is <c>false</c> on the source and a
        /// file does not exist at specified Path.</exception>
        public override void Load()
        {
            var file = _source.FileProvider?.GetFileInfo(_source.Path);
            if (file == null || !file.Exists)
            {
                if (_source.Optional)
                {
                    Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                else
                {
                    throw new FileNotFoundException($"The configuration file '{_source.Path}' was not found and is not optional.");
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
