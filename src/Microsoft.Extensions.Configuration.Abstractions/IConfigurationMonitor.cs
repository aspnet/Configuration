// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Used to monitor changes to an <see cref="IConfigurationRoot"/>.
    /// </summary>
    public interface IConfigurationMonitor : IDisposable
    {
        /// <summary>
        /// Explicitly trigger the listeners for the changed event.
        /// </summary>
        void RaiseChanged();

        /// <summary>
        /// Used to monitor changes to an <see cref="IConfigurationRoot"/>.
        /// </summary>
        void RegisterOnChanged(Action<IConfigurationRoot> listener);
    }
}
