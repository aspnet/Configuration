// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Microsoft.Framework.OptionsModel;
using Xunit;

namespace Microsoft.Framework.OptionsModel.Tests
{
    public class OptionsTest
    {
        [Fact]
        public void SetupCallsSortedInOrder()
        {
            var services = new ServiceCollection();
            services.Add(OptionsServices.GetDefaultServices());
            services.SetupOptions<FakeOptions>(o => o.Message += "a", -100);
            services.AddSetup<FakeOptionsSetupC>();
            services.AddSetup(new FakeOptionsSetupB());
            services.AddSetup(typeof(FakeOptionsSetupA));
            services.SetupOptions<FakeOptions>(o => o.Message += "z", 10000);

            var service = services.BuildServiceProvider().GetService<IOptionsAccessor<FakeOptions>>();
            Assert.NotNull(service);
            var options = service.Options;
            Assert.NotNull(options);
            Assert.Equal("aABCz", options.Message);
        }

    }
}
