﻿using System;
using System.IO;
using Xunit;

namespace Microsoft.AspNet.ConfigurationModel.Sources
{
    public class IniFileConfigurationSourceTest
    {
        private static readonly string ArbitraryFilePath = "Unit tests do not touch file system";

        [Fact]
        public void LoadKeyValuePairsFromValidIniFile()
        {
            var ini = @"
            [DefaultConnection]
            ConnectionString=TestConnectionString
            Provider=SqlClient
            [Data:Inventory]
            ConnectionString=AnotherTestConnectionString
            SubHeader:Provider=MySql
            ";
            var iniConfigSrc = new IniFileConfigurationSource(ArbitraryFilePath);

            iniConfigSrc.Load(StringToStream(ini));

            Assert.Equal(4, iniConfigSrc.Data.Count);
            Assert.Equal("TestConnectionString", iniConfigSrc.Data["DefaultConnection:ConnectionString"]);
            Assert.Equal("SqlClient", iniConfigSrc.Data["DefaultConnection:Provider"]);
            Assert.Equal("AnotherTestConnectionString", iniConfigSrc.Data["Data:Inventory:ConnectionString"]);
            Assert.Equal("MySql", iniConfigSrc.Data["Data:Inventory:SubHeader:Provider"]);
        }

        [Fact]
        public void LoadKeyValuePairsFromValidIniFileWithQuotedValues()
        {
            var ini = "[DefaultConnection]\n" + 
                      "ConnectionString=\"TestConnectionString\"\n" +
                      "Provider=\"SqlClient\"\n" +
                      "[Data:Inventory]\n" +
                      "ConnectionString=\"AnotherTestConnectionString\"\n" +
                      "Provider=\"MySql\"";
            var iniConfigSrc = new IniFileConfigurationSource(ArbitraryFilePath);

            iniConfigSrc.Load(StringToStream(ini));

            Assert.Equal(4, iniConfigSrc.Data.Count);
            Assert.Equal("TestConnectionString", iniConfigSrc.Data["DefaultConnection:ConnectionString"]);
            Assert.Equal("SqlClient", iniConfigSrc.Data["DefaultConnection:Provider"]);
            Assert.Equal("AnotherTestConnectionString", iniConfigSrc.Data["Data:Inventory:ConnectionString"]);
            Assert.Equal("MySql", iniConfigSrc.Data["Data:Inventory:Provider"]);
        }

        [Fact]
        public void DoubleQuoteIsPartOfValueIfNotPaired()
        {
            var ini = "[ConnectionString]\n" +
                      "DefaultConnection=\"TestConnectionString\n" +
                      "Provider=SqlClient\"";
            var iniConfigSrc = new IniFileConfigurationSource(ArbitraryFilePath);

            iniConfigSrc.Load(StringToStream(ini));

            Assert.Equal(2, iniConfigSrc.Data.Count);
            Assert.Equal("\"TestConnectionString", iniConfigSrc.Data["ConnectionString:DefaultConnection"]);
            Assert.Equal("SqlClient\"", iniConfigSrc.Data["ConnectionString:Provider"]);
        }

        [Fact]
        public void DoubleQuoteIsPartOfValueIfAppearInTheMiddleOfValue()
        {
            var ini = "[ConnectionString]\n" +
                      "DefaultConnection=Test\"Connection\"String\n" +
                      "Provider=Sql\"Client";
            var iniConfigSrc = new IniFileConfigurationSource(ArbitraryFilePath);

            iniConfigSrc.Load(StringToStream(ini));

            Assert.Equal(2, iniConfigSrc.Data.Count);
            Assert.Equal("Test\"Connection\"String", iniConfigSrc.Data["ConnectionString:DefaultConnection"]);
            Assert.Equal("Sql\"Client", iniConfigSrc.Data["ConnectionString:Provider"]);
        }

        [Fact]
        public void LoadKeyValuePairsFromValidIniFileWithoutSectionHeader()
        {
            var ini = @"
            DefaultConnection:ConnectionString=TestConnectionString
            DefaultConnection:Provider=SqlClient
            Data:Inventory:ConnectionString=AnotherTestConnectionString
            Data:Inventory:Provider=MySql
            ";
            var iniConfigSrc = new IniFileConfigurationSource(ArbitraryFilePath);

            iniConfigSrc.Load(StringToStream(ini));

            Assert.Equal(4, iniConfigSrc.Data.Count);
            Assert.Equal("TestConnectionString", iniConfigSrc.Data["DefaultConnection:ConnectionString"]);
            Assert.Equal("SqlClient", iniConfigSrc.Data["DefaultConnection:Provider"]);
            Assert.Equal("AnotherTestConnectionString", iniConfigSrc.Data["Data:Inventory:ConnectionString"]);
            Assert.Equal("MySql", iniConfigSrc.Data["Data:Inventory:Provider"]);
        }

        [Fact]
        public void SupportAndIgnoreComments()
        {
            var ini = @"
            ; Comments
            [DefaultConnection]
            # Comments
            ConnectionString=TestConnectionString
            / Comments
            Provider=SqlClient
            [Data:Inventory]
            ConnectionString=AnotherTestConnectionString
            Provider=MySql
            ";
            var iniConfigSrc = new IniFileConfigurationSource(ArbitraryFilePath);

            iniConfigSrc.Load(StringToStream(ini));

            Assert.Equal(4, iniConfigSrc.Data.Count);
            Assert.Equal("TestConnectionString", iniConfigSrc.Data["DefaultConnection:ConnectionString"]);
            Assert.Equal("SqlClient", iniConfigSrc.Data["DefaultConnection:Provider"]);
            Assert.Equal("AnotherTestConnectionString", iniConfigSrc.Data["Data:Inventory:ConnectionString"]);
            Assert.Equal("MySql", iniConfigSrc.Data["Data:Inventory:Provider"]);
        }

        [Fact]
        public void ThrowExceptionWhenFoundInvalidLine()
        {
            var ini = @"
ConnectionString
            ";
            var iniConfigSrc = new IniFileConfigurationSource(ArbitraryFilePath);
            var expectedMsg = Resources.FormatError_UnrecognizedLineFormat("ConnectionString");

            var exception = Assert.Throws<FormatException>(() => iniConfigSrc.Load(StringToStream(ini)));

            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenFoundBrokenSectionHeader()
        {
            var ini = @"
[ConnectionString
DefaultConnection=TestConnectionString
            ";
            var iniConfigSrc = new IniFileConfigurationSource(ArbitraryFilePath);
            var expectedMsg = Resources.FormatError_UnrecognizedLineFormat("[ConnectionString");
            
            var exception = Assert.Throws<FormatException>(() => iniConfigSrc.Load(StringToStream(ini)));

            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenPassingNullAsFilePath()
        {
            var expectedMsg = new ArgumentException(Resources.Error_InvalidFilePath, "path").Message;

            var exception = Assert.Throws<ArgumentException>(() => new IniFileConfigurationSource(null));

            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenPassingEmptyStringAsFilePath()
        {
            var expectedMsg = new ArgumentException(Resources.Error_InvalidFilePath, "path").Message;

            var exception = Assert.Throws<ArgumentException>(() => new IniFileConfigurationSource(string.Empty));

            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenKeyIsDuplicated()
        {
            var ini = @"
            [Data:DefaultConnection]
            ConnectionString=TestConnectionString
            Provider=SqlClient
            [Data]
            DefaultConnection:ConnectionString=AnotherTestConnectionString
            Provider=MySql
            ";
            var iniConfigSrc = new IniFileConfigurationSource(ArbitraryFilePath);
            var expectedMsg = Resources.FormatError_KeyIsDuplicated("Data:DefaultConnection:ConnectionString");

            var exception = Assert.Throws<FormatException>(() => iniConfigSrc.Load(StringToStream(ini)));

            Assert.Equal(expectedMsg, exception.Message);
        }

        private static Stream StringToStream(string str)
        {
            var memStream = new MemoryStream();
            var textWriter = new StreamWriter(memStream);
            textWriter.Write(str);
            textWriter.Flush();
            memStream.Seek(0, SeekOrigin.Begin);

            return memStream;
        }
    }
}
