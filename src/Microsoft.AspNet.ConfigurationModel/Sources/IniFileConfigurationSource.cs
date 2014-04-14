﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.AspNet.ConfigurationModel.Sources
{
    public class IniFileConfigurationSource : BaseConfigurationSource
    {
        // http://en.wikipedia.org/wiki/INI_file
        /// <summary>
        /// Files are simple line structures
        /// [Section:Header]
        /// key1=value1
        /// key2 = " value2 "
        /// ; comment
        /// # comment
        /// / comment
        /// </summary>
        /// <param name="path">The path and file name to load.</param>
        public IniFileConfigurationSource(string path)
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
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            string sectionPrefix = string.Empty;
            foreach (string rawLine in File.ReadLines(Path))
            {
                string line = rawLine.Trim();
                // Ignore blank lines
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                // Ignore comments
                if (line[0] == ';' || line[0] == '#' || line[0] == '/')
                {
                    continue;
                }
                // [Section:header] 
                if (line[0] == '[' && line[line.Length - 1] == ']')
                {
                    // remove the brackets
                    sectionPrefix = line.Substring(1, line.Length - 2) + ":";
                    continue;
                }

                // key = value OR "value"
                int seperator = line.IndexOf('=');
                if (seperator < 0)
                {
                    // TODO: exception message localization
                    throw new FormatException(string.Format("Unrecognized line format: '{0}'.", rawLine));
                }

                string key = sectionPrefix + line.Substring(0, seperator).Trim();
                string value = line.Substring(seperator + 1).Trim();

                // Remove quotes
                if (value.Length > 1 && value[0] == '"' && value[value.Length - 1] == '"')
                {
                    value = value.Substring(1, value.Length - 2);
                }

                if (data.ContainsKey(key))
                {
                    throw new FormatException(string.Format("Key '{0}' is duplicated.", key));
                }

                data[key] = value;
            }

            ReplaceData(data);
        }
    }
}
