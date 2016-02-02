// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.FileProviders;

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
        /// Initializes a new instance of <see cref="FileConfigurationProvider"/>.
        /// </summary>
        /// <param name="path">Absolute path of the INI configuration file.</param>
        /// <param name="optional">Determines if the configuration is optional.</param>
        public FileConfigurationProvider(string path, bool optional)
            : this(path, optional: false, reloadOnFileChanged: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FileConfigurationProvider"/>.
        /// </summary>
        /// <param name="path">Absolute path of the configuration file.</param>
        /// <param name="optional">Determines if the configuration is optional.</param>
        /// <param name="reloadOnFileChanged">Determines if the configuration provider should be reloaded if the file changes.</param>
        public FileConfigurationProvider(string path, bool optional, bool reloadOnFileChanged)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("File path must be a non - empty string.", nameof(path));
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
                ReloadOnFileChange();
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

        public IFileProvider FileProvider { get; set; }

        private void ReloadOnFileChange()
        {
            var fileProvider = FileProvider;
            if (fileProvider == null)
            {
#if NET451
                var basePath = AppDomain.CurrentDomain.GetData("APP_CONTEXT_BASE_DIRECTORY") as string ??
                    AppDomain.CurrentDomain.BaseDirectory ??
                    string.Empty;
#else
                var basePath = AppContext.BaseDirectory ?? string.Empty;
#endif
                fileProvider = new PhysicalFileProvider(basePath);
            }

            Action<object> callback = null;
            callback = _ =>
            {
                // The order here is important. We need to take the token and then apply our changes BEFORE
                // registering. This prevents us from possible having two change updates to process concurrently.
                //
                // If the file changes after we take the token, then we'll process the update immediately upon
                // registering the callback.
                var token = fileProvider.Watch(Path);
                Load();
                token.RegisterChangeCallback(callback, null);
            };

            fileProvider.Watch(Path).RegisterChangeCallback(callback, null);
        }
    }
}
