// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Microsoft.Extensions.Configuration.Json
{
    /// <summary>
    /// A JSON file based <see cref="ConfigurationProvider"/>.
    /// </summary>
    public class JsonConfigurationProvider : FileConfigurationProvider
    {
        /// <summary>
        /// Initializes a new instance of <see cref="JsonConfigurationProvider"/>.
        /// </summary>
        /// <param name="path">Absolute path of the JSON configuration file.</param>
        public JsonConfigurationProvider(string path)
            : base(path)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonConfigurationProvider"/>.
        /// </summary>
        /// <param name="path">Absolute path of the JSON configuration file.</param>
        /// <param name="optional">Determines if the configuration is optional.</param>
        public JsonConfigurationProvider(string path, bool optional)
            : base(path, optional)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JsonConfigurationProvider"/>.
        /// </summary>
        /// <param name="path">Absolute path of the JSON configuration file.</param>
        /// <param name="optional">Determines if the configuration is optional.</param>
        /// <param name="reloadOnFileChanged">Determines if the Load will be called again if the file changes.</param>
        public JsonConfigurationProvider(string path, bool optional, bool reloadOnFileChanged) : base(path, optional, reloadOnFileChanged)
        {
        }

        public override void Load(Stream stream)
        {
            var parser = new JsonConfigurationFileParser();
            try
            {
                Data = parser.Parse(stream);
            }
            catch (JsonReaderException e)
            {
                var errorLine = string.Empty;
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    IEnumerable<string> fileContent;
                    using (var streamReader = new StreamReader(stream))
                    {
                        fileContent = ReadLines(streamReader);
                        errorLine = RetrieveErrorContext(e, fileContent);
                    }
                }

                throw new FormatException(Resources.FormatError_JSONParseError(e.LineNumber, errorLine), e);
            }
        }

        private static string RetrieveErrorContext(JsonReaderException e, IEnumerable<string> fileContent)
        {
            string errorLine;
            if (e.LineNumber >= 2)
            {
                var errorContext = fileContent.Skip(e.LineNumber - 2).Take(2).ToList();
                errorLine = errorContext[0].Trim() + Environment.NewLine + errorContext[1].Trim();
            }
            else
            {
                var possibleLineContent = fileContent.Skip(e.LineNumber - 1).FirstOrDefault();
                errorLine = possibleLineContent ?? string.Empty;
            }

            return errorLine;
        }

        private static IEnumerable<string> ReadLines(StreamReader streamReader)
        {
            string line;
            do
            {
                line = streamReader.ReadLine();
                yield return line;
            } while (line != null);
        }
    }
}
