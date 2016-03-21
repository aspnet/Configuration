// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;

// These would move to primitives
namespace Microsoft.Extensions.Primitives
{
    public class ChangeMonitor<TSource> : IChangeMonitor<TSource> where TSource : class
    {
        private bool _disposed;
        private readonly TSource _root;
        private IDisposable _event;
        private ConfigChangeToken _reloadToken = new ConfigChangeToken();
        private readonly List<Action<TSource>> _listeners = new List<Action<TSource>>();

        public ChangeMonitor(TSource root)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }
            _root = root;

            _event = ChangeToken.OnChange(
                () => ProduceToken(),
                () => NotifyListeners());
        }

        private IChangeToken ProduceToken()
        {
            Interlocked.Exchange(ref _reloadToken, new ConfigChangeToken());
            return _reloadToken;
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
                _reloadToken.OnReload();
            }
        }

        /// <summary>
        /// Used to monitor changes to a source..
        /// </summary>
        public void RegisterOnChanged(Action<TSource> listener)
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
                _reloadToken = null;
            }
        }

        private class ConfigChangeToken : IChangeToken
        {
            private CancellationTokenSource _cts = new CancellationTokenSource();

            public bool ActiveChangeCallbacks => true;

            public bool HasChanged => _cts.IsCancellationRequested;

            public IDisposable RegisterChangeCallback(Action<object> callback, object state) => _cts.Token.Register(callback, state);

            public void OnReload() => _cts.Cancel();
        }
    }

    /// <summary>
    /// Used to monitor changes. />.
    /// </summary>
    public interface IChangeMonitor<TSource> : IDisposable where TSource : class 
    {
        /// <summary>
        /// Explicitly trigger the listeners for the changed event.
        /// </summary>
        void RaiseChanged();

        /// <summary>
        /// Used to register for changes./>.
        /// </summary>
        void RegisterOnChanged(Action<TSource> listener);
    }
}
