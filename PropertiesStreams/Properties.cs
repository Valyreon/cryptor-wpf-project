using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PropertiesStreams
{
    /// <summary>
    /// Defines the <see cref="Properties" /> class that is used for writing properties files.
    /// </summary>
    public class Properties
    {
        private readonly Dictionary<string, string> keyValuePairs;

        /// <summary>
        /// Initializes a new instance of the <see cref="Properties"/> class.
        /// </summary>
        public Properties()
        {
            this.keyValuePairs = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Properties"/> class with the keys and values from parameter.
        /// </summary>
        /// <param name="dictionary">The dictionary containing properties keys and values.<see cref="Dictionary{string, string}"/></param>
        public Properties(Dictionary<string, string> dictionary)
        {
            this.keyValuePairs = dictionary;
        }

        public HashSet<string> PropertyNames { get => new HashSet<string>(this.keyValuePairs.Keys); }

        public string GetProperty(string key)
        {
            var trimmedKey = key.Trim();
            if (this.keyValuePairs.Keys.Contains(trimmedKey))
            {
                return this.keyValuePairs[trimmedKey];
            }
            else
            {
                throw new KeyNotFoundException($"There is no '{key}' key.");
            }
        }

        public string GetProperty(string key, string defaultValue)
        {
            var trimmedKey = key.Trim();
            if (this.keyValuePairs.Keys.Contains(trimmedKey))
            {
                return this.keyValuePairs[trimmedKey];
            }
            else
            {
                return defaultValue;
            }
        }

        public void Load(Stream inStream)
        {
            inStream.Position = 0;
            StreamReader reader = new StreamReader(inStream);
            this.keyValuePairs.Clear();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] split = line.Trim().Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length == 2)
                {
                    this.keyValuePairs.Add(split[0].Trim(), split[1].Trim());
                }
            }
        }

        public void SetProperty(string key, string value)
        {
            var trimmedKey = key.Trim();
            if (this.keyValuePairs.Keys.Contains(trimmedKey))
            {
                this.keyValuePairs[trimmedKey] = value.Trim();
            }
        }

        public void Store(Stream outStream)
        {
            StreamWriter writer = new StreamWriter(outStream);
            foreach (var i in this.keyValuePairs)
            {
                writer.WriteLine($"{i.Key}={i.Value}");
            }

            writer.Flush();
        }

        public void AddProperty(string key, string value)
        {
            this.keyValuePairs.Add(key.Trim(), value.Trim());
        }
    }
}