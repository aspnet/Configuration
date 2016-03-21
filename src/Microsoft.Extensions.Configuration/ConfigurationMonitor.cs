// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Configuration
{
    public class ConfigurationMonitor : IConfigurationMonitor
    {
        private bool _disposed;
        private readonly IConfigurationRoot _root;
        private IDisposable _event;
        private ConfigurationReloadToken _reloadToken = new ConfigurationReloadToken();
        private ConfigurationReloadToken _previousToken = new ConfigurationReloadToken();
        private readonly List<Action<IConfigurationRoot>> _listeners = new List<Action<IConfigurationRoot>>();

        public ConfigurationMonitor(IConfigurationRoot root) {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }
            _root = root;

            _event = ChangeToken.OnChange(
                () => _previousToken = Interlocked.Exchange(ref _reloadToken, new ConfigurationReloadToken()),
                () => NotifyListeners());
        }

        private void NotifyListeners()
        {
            foreach (var listener in _listeners)
            {
                listener(_root);
            }
        }

        /// <summary>
        /// Explicitly trigger OnChanged.
        /// </summary>
        public void RaiseChanged()
        {
            if (!_disposed)
            {
                _previousToken.OnReload();
            }
        }

        /// <summary>
        /// Used to monitor changes to an <see cref="IConfigurationRoot"/>.
        /// </summary>
        public void RegisterOnChanged(Action<IConfigurationRoot> listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }

            _listeners.Add(listener);
        }

        public void Dispose()
        {
            _disposed = true;
            if (_event != null)
            {
                _event.Dispose();
                _event = null;
                _previousToken = null;
                _reloadToken = null;
            }
        }
    }
}
