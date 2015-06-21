// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNet.Testing;
using Xunit;

namespace Microsoft.Framework.Configuration.Binder.Test
{
    public class ConfigurationBinderTests
    {
        public class ComplexOptions
        {
            public ComplexOptions()
            {
                Nested = new NestedOptions();
                Virtual = "complex";
            }
            public NestedOptions Nested { get; set; }
            public int Integer { get; set; }
            public bool Boolean { get; set; }
            public virtual string Virtual { get; set; }

            public string PrivateSetter { get; private set; }
            public string ProtectedSetter { get; protected set; }
            public string InternalSetter { get; internal set; }
            public static string StaticProperty { get; set; }
            public string ReadOnly
            {
                get { return null; }
            }            
            public TimeSpan TimeSpan { get; set; }
            public DateTimeOffset DateTimeOffset { get; set; }
        }

        public class NestedOptions
        {
            public int Integer { get; set; }
        }

        public class DerivedOptions : ComplexOptions
        {
            public override string Virtual
            {
                get
                {
                    return base.Virtual;
                }
                set
                {
                    base.Virtual = "Derived:" + value;
                }
            }
        }

        public class NullableOptions
        {
            public bool? MyNullableBool { get; set; }
            public int? MyNullableInt { get; set; }
            public DateTime? MyNullableDateTime { get; set; }
        }

        public class EnumOptions
        {
            public UriKind UriKind { get; set; }
        }

        [Fact]
        public void CanReadComplexProperties()
        {
            var dic = new Dictionary<string, string>
            {
                {"Integer", "-2"},
                {"Boolean", "TRUe"},
                {"Nested:Integer", "11"}
            };
            var builder = new ConfigurationBuilder(new MemoryConfigurationSource(dic));
            var config = builder.Build();
            var options = ConfigurationBinder.Bind<ComplexOptions>(config);
            Assert.True(options.Boolean);
            Assert.Equal(-2, options.Integer);
            Assert.Equal(11, options.Nested.Integer);
        }

        [Fact]
        public void CanReadInheritedProperties()
        {
            var dic = new Dictionary<string, string>
            {
                {"Integer", "-2"},
                {"Boolean", "TRUe"},
                {"Nested:Integer", "11"},
                {"Virtual","Sup"}
            };
            var builder = new ConfigurationBuilder(new MemoryConfigurationSource(dic));
            var config = builder.Build();
            var options = ConfigurationBinder.Bind<DerivedOptions>(config);
            Assert.True(options.Boolean);
            Assert.Equal(-2, options.Integer);
            Assert.Equal(11, options.Nested.Integer);
            Assert.Equal("Derived:Sup", options.Virtual);
        }

        [Fact]
        public void CanReadStaticProperty()
        {
            var dic = new Dictionary<string, string>
            {
                {"StaticProperty", "stuff"},
            };
            var builder = new ConfigurationBuilder(new MemoryConfigurationSource(dic));
            var config = builder.Build();
            var options = ConfigurationBinder.Bind<ComplexOptions>(config);
            Assert.Equal("stuff", ComplexOptions.StaticProperty);
        }

        [Theory]
        [InlineData("ReadOnly")]
        [InlineData("PrivateSetter")]
        [InlineData("ProtectedSetter")]
        [InlineData("InternalSetter")]
        public void ShouldBeIgnoredTests(string property)
        {
            var dic = new Dictionary<string, string>
            {
                {property, "stuff"},
            };
            var builder = new ConfigurationBuilder(new MemoryConfigurationSource(dic));
            var config = builder.Build();
            var options = ConfigurationBinder.Bind<ComplexOptions>(config);
            Assert.Null(options.GetType().GetTypeInfo().GetDeclaredProperty(property).GetValue(options));
        }

        [Theory]
        [InlineData("en-GB")]
        [InlineData("de-DE")]
        [InlineData("zh-CN")]
        public void CanReadTimeSpanProperties(string culture)
        {
            var values = new string[]
            {
                "00:00:00",
                "-14.00:00:00",
                "01:02:03",
                "00:00:00.2500000",
                "99.23:59:59.9990000"
            };

            using (new CultureReplacer(culture, culture))
            {
                foreach (var value in values)
                {
                    var dic = new Dictionary<string, string>
                    {
                        {"TimeSpan", value}
                    };
                    var builder = new ConfigurationBuilder(new MemoryConfigurationSource(dic));
                    var config = builder.Build();
                    var options = ConfigurationBinder.Bind<ComplexOptions>(config);
                    Assert.Equal(value, options.TimeSpan.ToString());
                }
            }
        }

        [Theory]
        [InlineData("en-GB")]
        [InlineData("de-DE")]
        [InlineData("zh-CN")]
        public void CanReadDateTimeOffsetProperties(string culture)
        {
            var values = new string[]
            {
                "05/22/2015 22:55:22 +02:00",
                "03-12-07",
                "09/15/14 08:45:00 +1:00",
                "Thu May 01, 2008 1:00:00 +1:00"
            };

            using (new CultureReplacer(culture, culture))
            {
                foreach (var value in values)
                {
                    var dic = new Dictionary<string, string>
                    {
                        {"DateTimeOffset", value}
                    };
                    var builder = new ConfigurationBuilder(new MemoryConfigurationSource(dic));
                    var config = builder.Build();
                    var options = ConfigurationBinder.Bind<ComplexOptions>(config);

                    // Also verifies the binder always parses with invariant culture only
                    Assert.Equal(DateTimeOffset.Parse(value, CultureInfo.InvariantCulture), options.DateTimeOffset);
                }
            }
        }

        [Fact]
        public void ExceptionWhenTryingToBindToInterface()
        {
            var input = new Dictionary<string, string>
            {
                {"ISomeInterfaceProperty:Subkey", "x"}
            };

            var builder = new ConfigurationBuilder(new MemoryConfigurationSource(input));
            var config = builder.Build();

            var exception = Assert.Throws<InvalidOperationException>(
                () => ConfigurationBinder.Bind<TestOptions>(config));
            Assert.Equal(
                Resources.FormatError_CannotActivateAbstractOrInterface(typeof(ISomeInterface)),
                exception.Message);
        }

        [Fact]
        public void ExceptionWhenTryingToBindClassWithoutParameterlessConstructor()
        {
            var input = new Dictionary<string, string>
            {
                {"ClassWithoutPublicConstructorProperty:Subkey", "x"}
            };

            var builder = new ConfigurationBuilder(new MemoryConfigurationSource(input));
            var config = builder.Build();

            var exception = Assert.Throws<InvalidOperationException>(
                () => ConfigurationBinder.Bind<TestOptions>(config));
            Assert.Equal(
                Resources.FormatError_MissingParameterlessConstructor(typeof(ClassWithoutPublicConstructor)),
                exception.Message);
        }

        [Fact]
        public void ExceptionWhenTryingToBindToTypeThatCannotBeConverted()
        {
            const string IncorrectValue = "This is not an int";

            var input = new Dictionary<string, string>
            {
                {"IntProperty", IncorrectValue}
            };

            var builder = new ConfigurationBuilder(new MemoryConfigurationSource(input));
            var config = builder.Build();

            var exception = Assert.Throws<InvalidOperationException>(
                () => ConfigurationBinder.Bind<TestOptions>(config));
            Assert.NotNull(exception.InnerException);
            Assert.Equal(
                Resources.FormatError_FailedBinding(IncorrectValue, typeof(int)),
                exception.Message);
        }
        
        [Fact]
        public void ExceptionWhenTryingToBindTimeSpanFromBadInput()
        {
            const string IncorrectValue = "This is not a TimeSpan";

            var input = new Dictionary<string, string>
            {
                {"TimeSpan", IncorrectValue}
            };

            var builder = new ConfigurationBuilder(new MemoryConfigurationSource(input));
            var config = builder.Build();

            var exception = Assert.Throws<InvalidOperationException>(
                () => ConfigurationBinder.Bind<ComplexOptions>(config));
            Assert.NotNull(exception.InnerException);
            Assert.Equal(
                Resources.FormatError_FailedBinding(IncorrectValue, typeof(TimeSpan)),
                exception.Message);
        }

        [Fact]
        public void ExceptionWhenTryingToBindDateTimeOffsetFromBadInput()
        {
            const string IncorrectValue = "This is not a DateTimeOffset";

            var input = new Dictionary<string, string>
            {
                {"DateTimeOffset", IncorrectValue}
            };

            var builder = new ConfigurationBuilder(new MemoryConfigurationSource(input));
            var config = builder.Build();

            var exception = Assert.Throws<InvalidOperationException>(
                () => ConfigurationBinder.Bind<ComplexOptions>(config));
            Assert.NotNull(exception.InnerException);
            Assert.Equal(
                Resources.FormatError_FailedBinding(IncorrectValue, typeof(DateTimeOffset)),
                exception.Message);
        }

        [Fact]
        public void ExceptionWhenTryingToBindToTypeThrowsWhenActivated()
        {
            var input = new Dictionary<string, string>
            {
                {"ThrowsWhenActivatedProperty:subkey", "x"}
            };

            var builder = new ConfigurationBuilder(new MemoryConfigurationSource(input));
            var config = builder.Build();

            var exception = Assert.Throws<InvalidOperationException>(
                () => ConfigurationBinder.Bind<TestOptions>(config));
            Assert.NotNull(exception.InnerException);
            Assert.Equal(
                Resources.FormatError_FailedToActivate(typeof(ThrowsWhenActivated)),
                exception.Message);
        }

        [Fact]
        public void ExceptionIncludesKeyOfFailedBinding()
        {
            var input = new Dictionary<string, string>
            {
                {"NestedOptionsProperty:NestedOptions2Property:ISomeInterfaceProperty:subkey", "x"}
            };

            var builder = new ConfigurationBuilder(new MemoryConfigurationSource(input));
            var config = builder.Build();

            var exception = Assert.Throws<InvalidOperationException>(
                () => ConfigurationBinder.Bind<TestOptions>(config));
            Assert.Equal(
                Resources.FormatError_CannotActivateAbstractOrInterface(typeof(ISomeInterface)),
                exception.Message);
        }

        private interface ISomeInterface
        {
        }

        private class ClassWithoutPublicConstructor
        {
            private ClassWithoutPublicConstructor()
            {
            }
        }

        private class ThrowsWhenActivated
        {
            public ThrowsWhenActivated()
            {
                throw new Exception();
            }
        }

        private class NestedOptions1
        {
            public NestedOptions2 NestedOptions2Property { get; set; }
        }

        private class NestedOptions2
        {
            public ISomeInterface ISomeInterfaceProperty { get; set; }
        }

        private class TestOptions
        {
            public ISomeInterface ISomeInterfaceProperty { get; set; }

            public ClassWithoutPublicConstructor ClassWithoutPublicConstructorProperty { get; set; }

            public int IntProperty { get; set; }

            public ThrowsWhenActivated ThrowsWhenActivatedProperty { get; set; }

            public NestedOptions1 NestedOptionsProperty { get; set; }
        }
    }
}
