// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Contains information about a load exception.
    /// </summary>
    public class LoadExceptionContext
    {
        /// <summary>
        /// The <see cref="IConfigurationProvider"/> that caused the exception.
        /// </summary>
        public IConfigurationProvider Provider { get; set; }

        /// <summary>
        /// The exception that occured in Load.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// If true, the exception will not be rethrown.
        /// </summary>
        public bool Ignore { get; set; }
    }
}