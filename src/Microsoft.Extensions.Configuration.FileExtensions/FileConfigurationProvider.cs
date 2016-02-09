// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Configuration.FileProviders
{
    public abstract class FileConfigurationProvider : ConfigurationProvider
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FileConfigurationProvider"/>.
        /// </summary>
        /// <param name="path">absolute path of the configuration file.</param>
        public FileConfigurationProvider(string path)
            : this(path, optional: false)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of <see cref="FileConfigurationProvider"/>.
        /// </summary>
        /// <param name="path">absolute path of the configuration file.</param>
        /// <param name="optional">Determines if the configuration is optional.</param>
        public FileConfigurationProvider(string path, bool optional)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("File path must be a non-empty string.", nameof(path));
            }

            Optional = optional;
            Path = path;
        }

        /// <summary>
        /// Constructor used for tests.
        /// </summary>
        protected FileConfigurationProvider()
        { }

        private IFileProvider _reloadFileProvider;


        /// <summary>
        /// Gets a value that determines if this instance of <see cref="FileConfigurationProvider"/> is optional.
        /// </summary>
        public bool Optional { get; }

        /// <summary>
        /// The absolute path of the file backing this instance of <see cref="FileConfigurationProvider"/>.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Called when the IConfigurationRoot is being built.
        /// </summary>
        /// <param name="root"></param>
        public override void Initialize(IConfigurationRoot root)
        {
            if (_reloadFileProvider != null)
            {
                // Setup the change token tracking to reload if the file changes
                ChangeToken.OnChange(() => _reloadFileProvider.Watch(Path), () => Load());
            }
        }

        public virtual void ReloadWhenFileChanges(IFileProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            _reloadFileProvider = provider;
        }

        /// <summary>
        /// Loads the contents of the file at <see cref="Path"/>.
        /// </summary>
        /// <exception cref="FileNotFoundException">If <see cref="Optional"/> is <c>false</c> and a
        /// file does not exist at <see cref="Path"/>.</exception>
        public override void Load()
        {
            // REVIEW: We should really consider making this provider always use IFileProviders interface instead of File directly
            if (!File.Exists(Path))
            {
                if (Optional)
                {
                    Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                else
                {
                    throw new FileNotFoundException($"The configuration file '{Path}' was not found and is not optional.");
                }
            }
            else
            {
                using (var stream = new FileStream(Path, FileMode.Open, FileAccess.Read))
                {
                    Load(stream);
                }
            }
        }

        public abstract void Load(Stream stream);
    }
}
