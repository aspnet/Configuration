// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Load error extension methods for <see cref="IConfigurationBuilder"/>.
    /// </summary>
    public static class LoadErrorExtensions
    {
        private static string LoadExceptionHandlerKey = "LoadExceptionHandler";

        /// <summary>
        /// Sets a default action to be invoked when an error occurs during load.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="handler">The Action to be invoked on a load exception.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder SetLoadExceptionHandler(this IConfigurationBuilder builder, Action<LoadExceptionContext> handler)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Properties[LoadExceptionHandlerKey] = handler;
            return builder;
        }

        /// <summary>
        /// Gets the default action to be invoked when an error occurs during load.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>The Action to be invoked on a load exception.</returns>
        public static Action<LoadExceptionContext> GetLoadExceptionHandler(this IConfigurationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            object handler;
            if (builder.Properties.TryGetValue(LoadExceptionHandlerKey, out handler))
            {
                return builder.Properties[LoadExceptionHandlerKey] as Action<LoadExceptionContext>;
            }
            return null;
        }
    }
}