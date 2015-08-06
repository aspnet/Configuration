// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if DNXCORE50
// These tests only run on Core CLR.

using System;
using Microsoft.Framework.Configuration.Tests;
using Xunit;

namespace Microsoft.Framework.Configuration.Xml.Test
{
    public partial class XmlConfigurationSourceTest
    {
        [Fact]
        public void LoadKeyValuePairsFromValidEncryptedXml_ThrowsPlatformNotSupported()
        {
            var xml = @"
                <settings>
                    <Data.Setting>
                        <DefaultConnection>
                            <Connection.String>Test.Connection.String</Connection.String>
                            <Provider>SqlClient</Provider>
                        </DefaultConnection>
                        <Inventory>
                            <EncryptedData Type=""http://www.w3.org/2001/04/xmlenc#Element"" xmlns=""http://www.w3.org/2001/04/xmlenc#"">
                            <EncryptionMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#aes256-cbc"" />
                            <KeyInfo xmlns=""http://www.w3.org/2000/09/xmldsig#"">
                                <EncryptedKey xmlns=""http://www.w3.org/2001/04/xmlenc#"">
                                <EncryptionMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#kw-aes256"" />
                                <KeyInfo xmlns=""http://www.w3.org/2000/09/xmldsig#"">
                                    <KeyName>myKey</KeyName>
                                </KeyInfo>
                                <CipherData>
                                    <CipherValue>b0dxJI/o00vZgTNOJ6wUt0/6wCKWlQANAYE8cBsEzok4LQma7ErEnA==</CipherValue>
                                </CipherData>
                                </EncryptedKey>
                            </KeyInfo>
                            <CipherData>
                                <CipherValue>iXzecb+Cha80LLrl4zON3o7HfpRc0NxlJsnBb6zbKFa1HqtNhy2VrTnrEsZUViBWRkRbl+MCix7TiaIs4NtLijNU5Ob8Ez3vcD4T/QcmPywBYJDJhj1OUUeJSKH+icjg</CipherValue>
                            </CipherData>
                            </EncryptedData>
                            <Provider>MySql</Provider>
                        </Inventory>
                    </Data.Setting>
                </settings>";

            // Arrange
            var xmlConfigSrc = new XmlConfigurationSource(ArbitraryFilePath);

            // Act & assert
            var ex = Assert.Throws<PlatformNotSupportedException>(() => xmlConfigSrc.Load(TestStreamHelpers.StringToStream(xml)));
            Assert.Equal(Resources.Error_EncryptedXmlNotSupported, ex.Message);
        }
    }
}
#endif
