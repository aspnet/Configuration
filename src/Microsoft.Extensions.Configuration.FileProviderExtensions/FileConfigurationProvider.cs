// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Extensions.Configuration
{
    public abstract class FileConfigurationProvider : ConfigurationProvider
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FileConfigurationProvider"/>.
        /// </summary>
        /// <param name="path">Absolute path of the configuration file.</param>
        public FileConfigurationProvider(string path)
            : this(path, optional: false)
        {
        }


        /// <summary>
        /// Initializes a new instance of <see cref="IniConfigurationProvider"/>.
        /// </summary>
        /// <param name="path">Absolute path of the INI configuration file.</param>
        /// <param name="optional">Determines if the configuration is optional.</param>
        public FileConfigurationProvider(string path, bool optional)
            : this(path, optional: false, reloadOnFileChanged: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="IniConfigurationProvider"/>.
        /// </summary>
        /// <param name="path">Absolute path of the INI configuration file.</param>
        /// <param name="optional">Determines if the configuration is optional.</param>
        /// <param name="reloadOnFileChanged">Determines if the configuration provider should be reloaded if the file changes.</param>
        public FileConfigurationProvider(string path, bool optional, bool reloadOnFileChanged)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(Resources.Error_InvalidFilePath, nameof(path));
            }

            Optional = optional;
            Path = path;
            ReloadOnFileChanged = reloadOnFileChanged;
        }

        /// <summary>
        /// Gets a value that determines if this instance of <see cref="FileConfigurationProvider"/> is optional.
        /// </summary>
        public bool Optional { get; }

        /// <summary>
        /// The absolute path of the file backing this instance of <see cref="FileConfigurationProvider"/>.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// If true, the configuration will be reloaded if the file changes.
        /// </summary>
        public bool ReloadOnFileChanged { get; }

        /// <summary>
        /// Called when the IConfigurationRoot is being built.
        /// </summary>
        /// <param name="root"></param>
        public override void Initialize(IConfigurationRoot root)
        {
            if (ReloadOnFileChanged)
            {
                this.ReloadOnChanged(Path);
            }
        }

        /// <summary>
        /// Loads the contents of the file at <see cref="Path"/>.
        /// </summary>
        /// <exception cref="FileNotFoundException">If <see cref="Optional"/> is <c>false</c> and a
        /// file does not exist at <see cref="Path"/>.</exception>
        public override void Load()
        {
            if (!File.Exists(Path))
            {
                if (Optional)
                {
                    Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                else
                {
                    throw new FileNotFoundException(Resources.FormatError_FileNotFound(Path), Path);
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

        protected internal abstract void Load(Stream stream);
    }
}
