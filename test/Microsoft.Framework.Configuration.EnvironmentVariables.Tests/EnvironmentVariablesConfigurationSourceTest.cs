// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using Microsoft.Framework.Configuration.Test;
using Xunit;

namespace Microsoft.Framework.Configuration.EnvironmentVariables.Tests
{
    public class EnvironmentVariablesConfigurationSourceTest
    {
        [Fact]
        public void LoadKeyValuePairsFromEnvironmentDictionary()
        {
            var dict = new Hashtable()
                {
                    {"DefaultConnection:ConnectionString", "TestConnectionString"},
                    {"DefaultConnection:Provider", "SqlClient"},
                    {"Inventory:ConnectionString", "AnotherTestConnectionString"},
                    {"Inventory:Provider", "MySql"}
                };
            var envConfigSrc = new EnvironmentVariablesConfigurationSource();

            envConfigSrc.Load(dict);

            Assert.Equal("TestConnectionString", envConfigSrc.Get("defaultconnection:ConnectionString"));
            Assert.Equal("SqlClient", envConfigSrc.Get("DEFAULTCONNECTION:PROVIDER"));
            Assert.Equal("AnotherTestConnectionString", envConfigSrc.Get("Inventory:CONNECTIONSTRING"));
            Assert.Equal("MySql", envConfigSrc.Get("Inventory:Provider"));
        }

        [Fact]
        public void LoadKeyValuePairsFromEnvironmentDictionaryWithPrefix()
        {
            var dict = new Hashtable()
                {
                    {"DefaultConnection:ConnectionString", "TestConnectionString"},
                    {"DefaultConnection:Provider", "SqlClient"},
                    {"Inventory:ConnectionString", "AnotherTestConnectionString"},
                    {"Inventory:Provider", "MySql"}
                };
            var envConfigSrc = new EnvironmentVariablesConfigurationSource("DefaultConnection:");

            envConfigSrc.Load(dict);

            Assert.Equal("TestConnectionString", envConfigSrc.Get("ConnectionString"));
            Assert.Equal("SqlClient", envConfigSrc.Get("Provider"));
        }

        [Fact]
        public void LoadKeyValuePairsFromAzureEnvironment()
        {
            var dict = new Hashtable()
                {
                    {"APPSETTING_AppName", "TestAppName"},
                    {"CUSTOMCONNSTR_db1", "CustomConnStr"},
                    {"SQLCONNSTR_db2", "SQLConnStr"},
                    {"MYSQLCONNSTR_db3", "MySQLConnStr"},
                    {"SQLAZURECONNSTR_db4", "SQLAzureConnStr"},
                    {"CommonEnv", "CommonEnvValue"},
                };
            var envConfigSrc = new EnvironmentVariablesConfigurationSource();

            envConfigSrc.Load(dict);

            string value;
            Assert.Equal("TestAppName", envConfigSrc.Get("APPSETTING_AppName"));
            Assert.False(envConfigSrc.TryGet("AppName", out value));
            Assert.Equal("CustomConnStr", envConfigSrc.Get("Data:db1:ConnectionString"));
            Assert.Equal("SQLConnStr", envConfigSrc.Get("Data:db2:ConnectionString"));
            Assert.Equal("System.Data.SqlClient", envConfigSrc.Get("Data:db2:ProviderName"));
            Assert.Equal("MySQLConnStr", envConfigSrc.Get("Data:db3:ConnectionString"));
            Assert.Equal("MySql.Data.MySqlClient", envConfigSrc.Get("Data:db3:ProviderName"));
            Assert.Equal("SQLAzureConnStr", envConfigSrc.Get("Data:db4:ConnectionString"));
            Assert.Equal("System.Data.SqlClient", envConfigSrc.Get("Data:db4:ProviderName"));
            Assert.Equal("CommonEnvValue", envConfigSrc.Get("CommonEnv"));
        }

        [Fact]
        public void LoadKeyValuePairsFromAzureEnvironmentWithPrefix()
        {
            var dict = new Hashtable()
                {
                    {"CUSTOMCONNSTR_db1", "CustomConnStr"},
                    {"SQLCONNSTR_db2", "SQLConnStr"},
                    {"MYSQLCONNSTR_db3", "MySQLConnStr"},
                    {"SQLAZURECONNSTR_db4", "SQLAzureConnStr"},
                    {"CommonEnv", "CommonEnvValue"},
                };
            var envConfigSrc = new EnvironmentVariablesConfigurationSource("Data:");

            envConfigSrc.Load(dict);

            Assert.Equal("CustomConnStr", envConfigSrc.Get("db1:ConnectionString"));
            Assert.Equal("SQLConnStr", envConfigSrc.Get("db2:ConnectionString"));
            Assert.Equal("System.Data.SqlClient", envConfigSrc.Get("db2:ProviderName"));
            Assert.Equal("MySQLConnStr", envConfigSrc.Get("db3:ConnectionString"));
            Assert.Equal("MySql.Data.MySqlClient", envConfigSrc.Get("db3:ProviderName"));
            Assert.Equal("SQLAzureConnStr", envConfigSrc.Get("db4:ConnectionString"));
            Assert.Equal("System.Data.SqlClient", envConfigSrc.Get("db4:ProviderName"));
        }

        [Fact]
        public void LastVariableAddedWhenKeyIsDuplicatedInAzureEnvironment()
        {
            var dict = new Hashtable()
                {
                    {"Data:db2:ConnectionString", "CommonEnvValue"},
                    {"SQLCONNSTR_db2", "SQLConnStr"},
                };
            var envConfigSrc = new EnvironmentVariablesConfigurationSource();

            envConfigSrc.Load(dict);

            Assert.True(!string.IsNullOrEmpty(envConfigSrc.Get("Data:db2:ConnectionString")));
            Assert.Equal("System.Data.SqlClient", envConfigSrc.Get("Data:db2:ProviderName"));
        }

        [Fact]
        public void LastVariableAddedWhenMultipleEnvironmentVariablesWithSameNameButDifferentCaseExist()
        {
            var dict = new Hashtable()
                {
                    {"CommonEnv", "CommonEnvValue1"},
                    {"commonenv", "commonenvValue2"},
                    {"cOMMonEnv", "commonenvValue3"},
                };
            var envConfigSrc = new EnvironmentVariablesConfigurationSource();

            envConfigSrc.Load(dict);

            Assert.True(!string.IsNullOrEmpty(envConfigSrc.Get("cOMMonEnv")));
            Assert.True(!string.IsNullOrEmpty(envConfigSrc.Get("CommonEnv")));
        }
    }
}
