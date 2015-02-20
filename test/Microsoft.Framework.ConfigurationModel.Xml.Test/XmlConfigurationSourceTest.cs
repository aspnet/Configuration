// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Xunit;

using Resources = Microsoft.Framework.ConfigurationModel.Xml.Resources;

namespace Microsoft.Framework.ConfigurationModel
{
    public class XmlConfigurationSourceTest
    {
        private static readonly string ArbitraryFilePath = "Unit tests do not touch file system";

        [Fact]
        public void LoadKeyValuePairsFromValidXml()
        {
            var xml = @"
                <settings>
                    <Data.Setting>
                        <DefaultConnection>
                            <Connection.String>Test.Connection.String</Connection.String>
                            <Provider>SqlClient</Provider>
                        </DefaultConnection>
                        <Inventory>
                            <ConnectionString>AnotherTestConnectionString</ConnectionString>
                            <Provider>MySql</Provider>
                        </Inventory>
                    </Data.Setting>
                </settings>";
            var xmlConfigSrc = new XmlConfigurationSource(ArbitraryFilePath);

            xmlConfigSrc.Load(StringToStream(xml));

            Assert.Equal(4, xmlConfigSrc.Data.Count);
            Assert.Equal("Test.Connection.String", xmlConfigSrc.Data["DATA.SETTING:DEFAULTCONNECTION:CONNECTION.STRING"]);
            Assert.Equal("SqlClient", xmlConfigSrc.Data["DATA.SETTING:DefaultConnection:Provider"]);
            Assert.Equal("AnotherTestConnectionString", xmlConfigSrc.Data["data.setting:inventory:connectionstring"]);
            Assert.Equal("MySql", xmlConfigSrc.Data["Data.setting:Inventory:Provider"]);
        }

        [Fact]
        public void LoadMethodCanHandleEmptyValue()
        {
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<?xml-stylesheet type=""text/xsl"" href=""style1.xsl""?>
<settings>
    <?xml-stylesheet type=""text/xsl"" href=""style2.xsl""?>
    <Key1></Key1>
    <Key2 Key3="""" />
</settings>";
            var xmlConfigSrc = new XmlConfigurationSource(ArbitraryFilePath);

            xmlConfigSrc.Load(StringToStream(xml));

            Assert.Equal(2, xmlConfigSrc.Data.Count);
            Assert.Equal(string.Empty, xmlConfigSrc.Data["Key1"]);
            Assert.Equal(string.Empty, xmlConfigSrc.Data["Key2:Key3"]);
        }

        [Fact]
        public void CommonAttributesContributeToKeyValuePairs()
        {
            var xml =
@"<settings Port=""8008"">
    <Data>
        <DefaultConnection
            ConnectionString=""TestConnectionString""
            Provider=""SqlClient""/>
        <Inventory
            ConnectionString=""AnotherTestConnectionString""
            Provider=""MySql""/>
    </Data>
</settings>";
            var xmlConfigSrc = new XmlConfigurationSource(ArbitraryFilePath);

            xmlConfigSrc.Load(StringToStream(xml));

            Assert.Equal(5, xmlConfigSrc.Data.Count);
            Assert.Equal("8008", xmlConfigSrc.Data["Port"]);
            Assert.Equal("TestConnectionString", xmlConfigSrc.Data["Data:DefaultConnection:ConnectionString"]);
            Assert.Equal("SqlClient", xmlConfigSrc.Data["Data:DefaultConnection:Provider"]);
            Assert.Equal("AnotherTestConnectionString", xmlConfigSrc.Data["Data:Inventory:ConnectionString"]);
            Assert.Equal("MySql", xmlConfigSrc.Data["Data:Inventory:Provider"]);
        }

        [Fact]
        public void SupportMixingChildElementsAndAttributes()
        {
            var xml =
                @"<settings Port='8008'>
                    <Data>
                        <DefaultConnection Provider='SqlClient'>
                            <ConnectionString>TestConnectionString</ConnectionString>
                        </DefaultConnection>
                        <Inventory ConnectionString='AnotherTestConnectionString'>
                            <Provider>MySql</Provider>
                        </Inventory>
                    </Data>
                </settings>";
            var xmlConfigSrc = new XmlConfigurationSource(ArbitraryFilePath);

            xmlConfigSrc.Load(StringToStream(xml));

            Assert.Equal(5, xmlConfigSrc.Data.Count);
            Assert.Equal("8008", xmlConfigSrc.Data["Port"]);
            Assert.Equal("TestConnectionString", xmlConfigSrc.Data["Data:DefaultConnection:ConnectionString"]);
            Assert.Equal("SqlClient", xmlConfigSrc.Data["Data:DefaultConnection:Provider"]);
            Assert.Equal("AnotherTestConnectionString", xmlConfigSrc.Data["Data:Inventory:ConnectionString"]);
            Assert.Equal("MySql", xmlConfigSrc.Data["Data:Inventory:Provider"]);
        }

        [Fact]
        public void NameAttributeContributesToPrefix()
        {
            var xml = 
                @"<settings>
                    <Data Name='DefaultConnection'>
                        <ConnectionString>TestConnectionString</ConnectionString>
                        <Provider>SqlClient</Provider>
                    </Data>
                    <Data Name='Inventory'>
                        <ConnectionString>AnotherTestConnectionString</ConnectionString>
                        <Provider>MySql</Provider>
                    </Data>
                </settings>";
            var xmlConfigSrc = new XmlConfigurationSource(ArbitraryFilePath);

            xmlConfigSrc.Load(StringToStream(xml));

            Assert.Equal(4, xmlConfigSrc.Data.Count);
            Assert.Equal("TestConnectionString", xmlConfigSrc.Data["Data:DefaultConnection:ConnectionString"]);
            Assert.Equal("SqlClient", xmlConfigSrc.Data["Data:DefaultConnection:Provider"]);
            Assert.Equal("AnotherTestConnectionString", xmlConfigSrc.Data["Data:Inventory:ConnectionString"]);
            Assert.Equal("MySql", xmlConfigSrc.Data["Data:Inventory:Provider"]);
        }

        [Fact]
        public void NameAttributeInRootElementContributesToPrefix()
        {
            var xml = 
                @"<settings Name='Data'>
                    <DefaultConnection>
                        <ConnectionString>TestConnectionString</ConnectionString>
                        <Provider>SqlClient</Provider>
                    </DefaultConnection>
                    <Inventory>
                        <ConnectionString>AnotherTestConnectionString</ConnectionString>
                        <Provider>MySql</Provider>
                    </Inventory>
                </settings>";
            var xmlConfigSrc = new XmlConfigurationSource(ArbitraryFilePath);

            xmlConfigSrc.Load(StringToStream(xml));

            Assert.Equal(4, xmlConfigSrc.Data.Count);
            Assert.Equal("TestConnectionString", xmlConfigSrc.Data["Data:DefaultConnection:ConnectionString"]);
            Assert.Equal("SqlClient", xmlConfigSrc.Data["Data:DefaultConnection:Provider"]);
            Assert.Equal("AnotherTestConnectionString", xmlConfigSrc.Data["Data:Inventory:ConnectionString"]);
            Assert.Equal("MySql", xmlConfigSrc.Data["Data:Inventory:Provider"]);
        }

        [Fact]
        public void SupportMixingNameAttributesAndCommonAttributes()
        {
            var xml = 
                @"<settings>
                    <Data Name='DefaultConnection'
                          ConnectionString='TestConnectionString'
                          Provider='SqlClient' />
                    <Data Name='Inventory' ConnectionString='AnotherTestConnectionString'>
                          <Provider>MySql</Provider>
                    </Data>
                </settings>";
            var xmlConfigSrc = new XmlConfigurationSource(ArbitraryFilePath);

            xmlConfigSrc.Load(StringToStream(xml));

            Assert.Equal(4, xmlConfigSrc.Data.Count);
            Assert.Equal("TestConnectionString", xmlConfigSrc.Data["Data:DefaultConnection:ConnectionString"]);
            Assert.Equal("SqlClient", xmlConfigSrc.Data["Data:DefaultConnection:Provider"]);
            Assert.Equal("AnotherTestConnectionString", xmlConfigSrc.Data["Data:Inventory:ConnectionString"]);
            Assert.Equal("MySql", xmlConfigSrc.Data["Data:Inventory:Provider"]);
        }

        [Fact]
        public void SupportCDATAAsTextNode()
        {
            var xml = 
                @"<settings>
                    <Data>
                        <Inventory>
                            <Provider><![CDATA[SpecialStringWith<>]]></Provider>
                        </Inventory>
                    </Data>
                </settings>";
            var xmlConfigSrc = new XmlConfigurationSource(ArbitraryFilePath);

            xmlConfigSrc.Load(StringToStream(xml));

            Assert.Equal(1, xmlConfigSrc.Data.Count);
            Assert.Equal("SpecialStringWith<>", xmlConfigSrc.Data["Data:Inventory:Provider"]);
        }

        [Fact]
        public void SupportAndIgnoreComments()
        {
            var xml = 
                @"<!-- Comments --> <settings>
                    <Data> <!-- Comments -->
                        <DefaultConnection>
                            <ConnectionString><!-- Comments -->TestConnectionString</ConnectionString>
                            <Provider>SqlClient</Provider>
                        </DefaultConnection>
                        <Inventory>
                            <ConnectionString>AnotherTestConnectionString</ConnectionString>
                            <Provider>MySql</Provider>
                        </Inventory>
                    </Data>
                </settings><!-- Comments -->";
            var xmlConfigSrc = new XmlConfigurationSource(ArbitraryFilePath);

            xmlConfigSrc.Load(StringToStream(xml));

            Assert.Equal(4, xmlConfigSrc.Data.Count);
            Assert.Equal("TestConnectionString", xmlConfigSrc.Data["Data:DefaultConnection:ConnectionString"]);
            Assert.Equal("SqlClient", xmlConfigSrc.Data["Data:DefaultConnection:Provider"]);
            Assert.Equal("AnotherTestConnectionString", xmlConfigSrc.Data["Data:Inventory:ConnectionString"]);
            Assert.Equal("MySql", xmlConfigSrc.Data["Data:Inventory:Provider"]);
        }

        [Fact]
        public void SupportAndIgnoreXMLDeclaration()
        {
            var xml =
                @"<?xml version='1.0' encoding='UTF-8'?>
                <settings>
                    <Data>
                        <DefaultConnection>
                            <ConnectionString>TestConnectionString</ConnectionString>
                            <Provider>SqlClient</Provider>
                        </DefaultConnection>
                        <Inventory>
                            <ConnectionString>AnotherTestConnectionString</ConnectionString>
                            <Provider>MySql</Provider>
                        </Inventory>
                    </Data>
                </settings>";
            var xmlConfigSrc = new XmlConfigurationSource(ArbitraryFilePath);

            xmlConfigSrc.Load(StringToStream(xml));

            Assert.Equal(4, xmlConfigSrc.Data.Count);
            Assert.Equal("TestConnectionString", xmlConfigSrc.Data["Data:DefaultConnection:ConnectionString"]);
            Assert.Equal("SqlClient", xmlConfigSrc.Data["Data:DefaultConnection:Provider"]);
            Assert.Equal("AnotherTestConnectionString", xmlConfigSrc.Data["Data:Inventory:ConnectionString"]);
            Assert.Equal("MySql", xmlConfigSrc.Data["Data:Inventory:Provider"]);
        }

        [Fact]
        public void SupportAndIgnoreProcessingInstructions()
        {
            var xml =
                @"<?xml version='1.0' encoding='UTF-8'?>
                <?xml-stylesheet type='text/xsl' href='style1.xsl'?>
                    <settings>
                        <?xml-stylesheet type='text/xsl' href='style2.xsl'?>
                        <Data>
                            <DefaultConnection>
                                <ConnectionString>TestConnectionString</ConnectionString>
                                <Provider>SqlClient</Provider>
                            </DefaultConnection>
                            <Inventory>
                                <ConnectionString>AnotherTestConnectionString</ConnectionString>
                                <Provider>MySql</Provider>
                            </Inventory>
                        </Data>
                    </settings>";
            var xmlConfigSrc = new XmlConfigurationSource(ArbitraryFilePath);

            xmlConfigSrc.Load(StringToStream(xml));

            Assert.Equal(4, xmlConfigSrc.Data.Count);
            Assert.Equal("TestConnectionString", xmlConfigSrc.Data["Data:DefaultConnection:ConnectionString"]);
            Assert.Equal("SqlClient", xmlConfigSrc.Data["Data:DefaultConnection:Provider"]);
            Assert.Equal("AnotherTestConnectionString", xmlConfigSrc.Data["Data:Inventory:ConnectionString"]);
            Assert.Equal("MySql", xmlConfigSrc.Data["Data:Inventory:Provider"]);
        }

        [Fact]
        public void ThrowExceptionWhenFindDTD()
        {
            var xml =
                @"<!DOCTYPE DefaultConnection[
                    <!ELEMENT DefaultConnection (ConnectionString,Provider)>
                    <!ELEMENT ConnectionString (#PCDATA)>
                    <!ELEMENT Provider (#PCDATA)>
                ]>
                <settings>
                    <Data>
                        <DefaultConnection>
                            <ConnectionString>TestConnectionString</ConnectionString>
                            <Provider>SqlClient</Provider>
                        </DefaultConnection>
                        <Inventory>
                            <ConnectionString>AnotherTestConnectionString</ConnectionString>
                            <Provider>MySql</Provider>
                        </Inventory>
                    </Data>
                </settings>";
            var xmlConfigSrc = new XmlConfigurationSource(ArbitraryFilePath);
            var expectedMsg = "For security reasons DTD is prohibited in this XML document. "
                + "To enable DTD processing set the DtdProcessing property on XmlReaderSettings "
                + "to Parse and pass the settings into XmlReader.Create method.";

            var exception = Assert.Throws<System.Xml.XmlException>(() => xmlConfigSrc.Load(StringToStream(xml)));

            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenFindNamespace()
        {
            var xml =
                @"<settings xmlns:MyNameSpace='http://microsoft.com/wwa/mynamespace'>
                    <MyNameSpace:Data>
                        <DefaultConnection>
                            <ConnectionString>TestConnectionString</ConnectionString>
                            <Provider>SqlClient</Provider>
                        </DefaultConnection>
                        <Inventory>
                            <ConnectionString>AnotherTestConnectionString</ConnectionString>
                            <Provider>MySql</Provider>
                        </Inventory>
                    </MyNameSpace:Data>
                </settings>";
            var xmlConfigSrc = new XmlConfigurationSource(ArbitraryFilePath);
            var expectedMsg = Resources.FormatError_NamespaceIsNotSupported(Resources.FormatMsg_LineInfo(1, 11));

            var exception = Assert.Throws<FormatException>(() => xmlConfigSrc.Load(StringToStream(xml)));

            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenPassingNullAsFilePath()
        {
            var expectedMsg = new ArgumentException(Resources.Error_InvalidFilePath, "path").Message;

            var exception = Assert.Throws<ArgumentException>(() => new XmlConfigurationSource(null));

            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenPassingEmptyStringAsFilePath()
        {
            var expectedMsg = new ArgumentException(Resources.Error_InvalidFilePath, "path").Message;

            var exception = Assert.Throws<ArgumentException>(() => new XmlConfigurationSource(string.Empty));

            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenKeyIsDuplicated()
        {
            var xml = 
                @"<settings>
                    <Data>
                        <DefaultConnection>
                            <ConnectionString>TestConnectionString</ConnectionString>
                            <Provider>SqlClient</Provider>
                        </DefaultConnection>
                    </Data>
                    <Data Name='DefaultConnection' ConnectionString='NewConnectionString'>
                        <Provider>NewProvider</Provider>
                    </Data>
                </settings>";
            var xmlConfigSrc = new XmlConfigurationSource(ArbitraryFilePath);
            var expectedMsg = Resources.FormatError_KeyIsDuplicated("Data:DefaultConnection:ConnectionString",
                Resources.FormatMsg_LineInfo(8, 52));

            var exception = Assert.Throws<FormatException>(() => xmlConfigSrc.Load(StringToStream(xml)));

            Assert.Equal(expectedMsg, exception.Message);
        }

        [Fact]
        public void CommitMethodPreservesCommmentsAndProcessingInstructionsAndWhiteSpaces()
        {
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                    <?xml-stylesheet type=""text/xsl"" href=""style1.xsl""?>
                    <settings>
                        <?xml-stylesheet type=""text/xsl"" href=""style2.xsl""?>
                        <Data>
                            <DefaultConnection>
                                <ConnectionString>TestConnectionString</ConnectionString>
                                <Provider>SqlClient</Provider>
                            </DefaultConnection>
                            <Inventory>
                                <ConnectionString>AnotherTestConnectionString</ConnectionString>
                                <Provider>MySql</Provider>
                            </Inventory>
                        </Data>
                    </settings>";
            var xmlConfigSrc = new XmlConfigurationSource(ArbitraryFilePath);
            var outputCacheStream = new MemoryStream();
            xmlConfigSrc.Load(StringToStream(xml));

            xmlConfigSrc.Commit(StringToStream(xml), outputCacheStream);

            var newContents = StreamToString(outputCacheStream);
            Assert.Equal(xml, newContents);
        }

        [Fact]
        public void CommitMethodUpdatesValues()
        {
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<?xml-stylesheet type=""text/xsl"" href=""style1.xsl""?>
<settings>
    <?xml-stylesheet type=""text/xsl"" href=""style2.xsl""?>
    <Data>
        <DefaultConnection>
            <ConnectionString>TestConnectionString</ConnectionString>
            <Provider>SqlClient</Provider>
        </DefaultConnection>
        <Inventory>
            <ConnectionString>AnotherTestConnectionString</ConnectionString>
            <Provider>MySql</Provider>
        </Inventory>
    </Data>
</settings>";
            var xmlConfigSrc = new XmlConfigurationSource(ArbitraryFilePath);
            var outputCacheStream = new MemoryStream();
            xmlConfigSrc.Load(StringToStream(xml));
            xmlConfigSrc.Set("Data:DefaultConnection:Provider", "NewSqlClient");
            xmlConfigSrc.Set("Data:Inventory:Provider", "NewMySql");

            xmlConfigSrc.Commit(StringToStream(xml), outputCacheStream);

            var newContents = StreamToString(outputCacheStream);
            Assert.Equal(xml.Replace("SqlClient", "NewSqlClient").Replace("MySql", "NewMySql"), newContents);
        }

        [Fact]
        public void CommitMethodCanHandleEmptyValue()
        {
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<?xml-stylesheet type=""text/xsl"" href=""style1.xsl""?>
<settings>
    <?xml-stylesheet type=""text/xsl"" href=""style2.xsl""?>
    <Key1></Key1>
    <Key2 Key3="""" />
</settings>";
            var expectedXml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<?xml-stylesheet type=""text/xsl"" href=""style1.xsl""?>
<settings>
    <?xml-stylesheet type=""text/xsl"" href=""style2.xsl""?>
    <Key1>Value1</Key1>
    <Key2 Key3=""Value2"" />
</settings>";
            var xmlConfigSrc = new XmlConfigurationSource(ArbitraryFilePath);
            var outputCacheStream = new MemoryStream();
            xmlConfigSrc.Load(StringToStream(xml));
            xmlConfigSrc.Set("Key1", "Value1");
            xmlConfigSrc.Set("Key2:Key3", "Value2");

            xmlConfigSrc.Commit(StringToStream(xml), outputCacheStream);

            var newContents = StreamToString(outputCacheStream);
            Assert.Equal(expectedXml, newContents);
        }

        [Fact]
        public void CommitOperationThrowsExceptionWhenFindInvalidModificationAfterLoadOperation()
        {
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                    <?xml-stylesheet type=""text/xsl"" href=""style1.xsl""?>
                    <settings>
                        <?xml-stylesheet type=""text/xsl"" href=""style2.xsl""?>
                        <Data>
                            <DefaultConnection>
                                <ConnectionString>TestConnectionString</ConnectionString>
                                <Provider>SqlClient</Provider>
                            </DefaultConnection>
                            <Inventory>
                                <ConnectionString>AnotherTestConnectionString</ConnectionString>
                                <Provider>MySql</Provider>
                            </Inventory>
                        </Data>
                    </settings>";
            var modifiedXml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                    <?xml-stylesheet type=""text/xsl"" href=""style1.xsl""?>
                    <settings xmlns:MyNameSpace=""http://microsoft.com/wwa/mynamespace"">
                        <?xml-stylesheet type=""text/xsl"" href=""style2.xsl""?>
                        <MyNameSpace:Data>
                            <DefaultConnection>
                                <ConnectionString>TestConnectionString</ConnectionString>
                                <Provider>SqlClient</Provider>
                            </DefaultConnection>
                            <Inventory>
                                <ConnectionString>AnotherTestConnectionString</ConnectionString>
                                <Provider>MySql</Provider>
                            </Inventory>
                        </MyNameSpace:Data>
                    </settings>";
            var xmlConfigSrc = new XmlConfigurationSource(ArbitraryFilePath);
            var outputCacheStream = new MemoryStream();
            xmlConfigSrc.Load(StringToStream(xml));

            var exception = Assert.Throws<FormatException>(
                () => xmlConfigSrc.Commit(StringToStream(modifiedXml), outputCacheStream));

            Assert.Equal(Resources.FormatError_NamespaceIsNotSupported(Resources.FormatMsg_LineInfo(3, 31)),
                exception.Message);
        }

        [Fact]
        public void CommitOperationThrowsExceptionWhenFindNewlyAddedKeyInOriginalFile()
        {
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<?xml-stylesheet type=""text/xsl"" href=""style1.xsl""?>
<settings>
    <?xml-stylesheet type=""text/xsl"" href=""style2.xsl""?>
    <Data>
        <DefaultConnection>
            <ConnectionString>TestConnectionString</ConnectionString>
            <Provider>SqlClient</Provider>
        </DefaultConnection>
        <Inventory>
            <ConnectionString>AnotherTestConnectionString</ConnectionString>
            <Provider>MySql</Provider>
        </Inventory>
    </Data>
</settings>";
            var modifiedXml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<?xml-stylesheet type=""text/xsl"" href=""style1.xsl""?>
<settings>
    <?xml-stylesheet type=""text/xsl"" href=""style2.xsl""?>
    <Data>
        <DefaultConnection>
            <ConnectionString>TestConnectionString</ConnectionString>
            <Provider>SqlClient</Provider>
            <NewKey>NewValue</NewKey>
        </DefaultConnection>
        <Inventory>
            <ConnectionString>AnotherTestConnectionString</ConnectionString>
            <Provider>MySql</Provider>
        </Inventory>
    </Data>
</settings>";
            var xmlConfigSrc = new XmlConfigurationSource(ArbitraryFilePath);
            var outputCacheStream = new MemoryStream();
            xmlConfigSrc.Load(StringToStream(xml));

            var exception = Assert.Throws<InvalidOperationException>(
                () => xmlConfigSrc.Commit(StringToStream(modifiedXml), outputCacheStream));

            Assert.Equal(
                Resources.FormatError_CommitWhenNewKeyFound("Data:DefaultConnection:NewKey"), exception.Message);
        }

        [Fact]
        public void CommitOperationAppendsNewKeyValuePairsAtTheEndOfFile()
        {
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<?xml-stylesheet type=""text/xsl"" href=""style1.xsl""?>
<settings>
    <?xml-stylesheet type=""text/xsl"" href=""style2.xsl""?>
    <Data>
        <DefaultConnection>
            <ConnectionString>TestConnectionString</ConnectionString>
            <Provider>SqlClient</Provider>
        </DefaultConnection>
        <Inventory>
            <ConnectionString>AnotherTestConnectionString</ConnectionString>
            <Provider>MySql</Provider>
        </Inventory>
    </Data>
</settings>";
            var expectedResult = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<?xml-stylesheet type=""text/xsl"" href=""style1.xsl""?>
<settings>
    <?xml-stylesheet type=""text/xsl"" href=""style2.xsl""?>
    <Data>
        <DefaultConnection>
            <ConnectionString>TestConnectionString</ConnectionString>
            <Provider>SqlClient</Provider>
        </DefaultConnection>
        <Inventory>
            <ConnectionString>AnotherTestConnectionString</ConnectionString>
            <Provider>MySql</Provider>
        </Inventory>
    </Data>
<NewKey1>NewValue1</NewKey1>
<NewKey2 Name=""NewKey3"">NewValue2</NewKey2>
</settings>";
            var xmlConfigSrc = new XmlConfigurationSource(ArbitraryFilePath);
            var outputCacheStream = new MemoryStream();
            xmlConfigSrc.Load(StringToStream(xml));
            // Add new key-value pairs
            xmlConfigSrc.Set("NewKey1", "NewValue1");
            xmlConfigSrc.Set("NewKey2:NewKey3", "NewValue2");

            xmlConfigSrc.Commit(StringToStream(xml), outputCacheStream);

            var newContents = StreamToString(outputCacheStream);
            Assert.Equal(expectedResult, newContents);
        }

        [Fact]
        public void CanCreateNewConfig()
        {
            var targetXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<settings>
  <Key1 Name=""Key2:Key3"">Value1</Key1>
  <Key4>Value2</Key4>
</settings>";
            var xmlConfigSrc = new XmlConfigurationSource(ArbitraryFilePath);
            var outputCacheStream = new MemoryStream();
            xmlConfigSrc.Data["Key1:Key2:Key3"] = "Value1";
            xmlConfigSrc.Data["Key4"] = "Value2";

            xmlConfigSrc.GenerateNewConfig(outputCacheStream);

            var newContents = StreamToString(outputCacheStream);
            Assert.Equal(targetXml, newContents);
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

        private static string StreamToString(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}
