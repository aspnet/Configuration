// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Represents a base class for file based <see cref="IConfigurationSource"/>.
    /// </summary>
    /// <typeparam name="TProvider">The concrete FileConfigurationProvider to create.</typeparam>
    public class FileConfigurationSource<TProvider> : FileConfigurationSource where TProvider : FileConfigurationProvider, new()
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            var provider = new TProvider();
            InitializeProvider(provider);
            return provider;
        }
    }

    /// <summary>
    /// Represents a base class for file based <see cref="IConfigurationSource"/>.
    /// </summary>
    public abstract class FileConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// Used to access the contents of the file.
        /// </summary>
        public IFileProvider FileProvider { get; set; }

        /// <summary>
        /// The path to the file.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Determines if loading the file is optional.
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        /// Determines whether the source will be loaded if the underlying file changes.
        /// </summary>
        public bool ReloadOnChange { get; set; }

        public virtual IFileInfo GetFileInfo()
        {
            return FileProvider?.GetFileInfo(Path);
        }

        /// <summary>
        /// Initializes the file based provider
        /// </summary>
        /// <param name="provider"></param>
        public virtual void InitializeProvider(FileConfigurationProvider provider)
        {
            provider.File = GetFileInfo();
            provider.Optional = Optional;

            // TODO: aggregate watch?
            if (ReloadOnChange && FileProvider != null)
            {
                ChangeToken.OnChange(() => FileProvider.Watch(Path), () => provider.Load());
            }
        }

        public abstract IConfigurationProvider Build(IConfigurationBuilder builder);
    }
}