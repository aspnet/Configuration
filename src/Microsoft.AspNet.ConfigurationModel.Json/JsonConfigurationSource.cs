﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Microsoft.AspNet.ConfigurationModel.Sources
{
    public class JsonConfigurationSource : BaseConfigurationSource
    {
        public JsonConfigurationSource(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                // TODO: exception message localization
                throw new ArgumentException("File path must be a non-empty string", "path");
            }

            Path = path;
        }

        public string Path { get; private set; }

        public override void Load()
        {
            using (var stream = new FileStream(Path, FileMode.Open))
            {
                Load(stream);
            }
        }

        internal void Load(Stream stream)
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            using (var reader = new JsonTextReader(new StreamReader(stream)))
            {
                var startObjectCount = 0;

                // Dates are parsed as strings
                reader.DateParseHandling = DateParseHandling.None;

                // Move to the first token
                reader.Read();

                SkipComments(reader);

                if (reader.TokenType != JsonToken.StartObject)
                {
                    var lineInfo = reader as IJsonLineInfo;
                    // TODO: exception message localization
                    throw new FormatException("Only an object can be the root. Path '" +
                                    reader.Path + "', line " + lineInfo.LineNumber +
                                    " position " + lineInfo.LinePosition);
                }

                do
                {
                    SkipComments(reader);

                    switch (reader.TokenType)
                    {
                        case JsonToken.StartObject:
                            startObjectCount++;
                            break;

                        case JsonToken.EndObject:
                            startObjectCount--;
                            break;

                        // Keys in key-value pairs
                        case JsonToken.PropertyName:
                            break;

                        // Values in key-value pairs
                        case JsonToken.Integer:
                        case JsonToken.Float:
                        case JsonToken.String:
                        case JsonToken.Boolean:
                        case JsonToken.Bytes:
                        case JsonToken.Raw:
                        case JsonToken.Null:
                            data[reader.Path.Replace(".", Constants.KeyDelimiter)] = reader.Value.ToString();
                            break;

                        // End of file
                        case JsonToken.None:
                            {
                                // TODO: exception message localization
                                throw new FormatException("Unexpected end when parsing JSON. Path '" +
                                    reader.Path + "', line " + reader.LineNumber +
                                    " position " + reader.LinePosition);
                            }

                        default:
                            {
                                // TODO: exception message localization
                                // Unsupported elements: Array, Constructor, Undefined
                                throw new FormatException("Unsupported JSON token: " + reader.TokenType +
                                    ". Path '" + reader.Path + "', line " + reader.LineNumber +
                                    " position " + reader.LinePosition);
                            }
                    }

                    reader.Read();

                } while (startObjectCount > 0);
            }

            ReplaceData(data);
        }

        private void SkipComments(JsonReader reader)
        {
            while (reader.TokenType == JsonToken.Comment)
            {
                reader.Read();
            }
        }
    }
}
