using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using HS.Migration.Exceptions;

namespace HS.Migration
{
    public class AssemblyMigrationStorage : IMigrationStorage
    {
        private const string VersionIndexMatchExpression = @"(?<number>[0-9][0-9\.]+)-.*\.sql$";

        private readonly SortedDictionary<decimal, string> filenameDictionary =
            new SortedDictionary<decimal, string>();

        private decimal lastVersionIndex = -1;

        public AssemblyMigrationStorage(Assembly assembly)
        {
            StorageAssembly = assembly;

            ReadFilenames();
        }

        public Assembly StorageAssembly { get; set; }

        #region IMigrationStorage Members

        string IMigrationStorage.SQL(decimal versionIndex)
        {
            if (filenameDictionary.ContainsKey(versionIndex))
            {
                return ReadEntireTextResourceStream(filenameDictionary[versionIndex]);
            }

            throw new MigrationMissingException(versionIndex);
        }

        decimal IMigrationStorage.NextVersionIndex(decimal version)
        {
            // O(n) but it's doing something trivial so I'll leave it for now.

            foreach (decimal currentVersion in filenameDictionary.Keys)
            {
                if (currentVersion > version)
                    return currentVersion;
            }

            return -1;
        }

        decimal IMigrationStorage.LastVersionIndex()
        {
            return lastVersionIndex;
        }

        #endregion

        private void ReadFilenames()
        {
            decimal highestVersionIndex = -1;

            filenameDictionary.Clear();

            foreach (string resourceName in StorageAssembly.GetManifestResourceNames())
            {
                decimal versionIndex = -1;

                if (ExtractVersionIndex(resourceName, ref versionIndex))
                {
                    filenameDictionary.Add(versionIndex, resourceName);

                    highestVersionIndex = Math.Max(versionIndex, highestVersionIndex);
                }
            }

            lastVersionIndex = highestVersionIndex;
        }

        private string ReadEntireTextResourceStream(string resourceName)
        {
            var migrationStream = StorageAssembly.GetManifestResourceStream(resourceName);

            if (migrationStream == null)
            {
                throw new ApplicationException("Expected to be able to read resource with name '" + resourceName + "'");
            }

            using (var reader = new StreamReader(migrationStream))
            {
                return reader.ReadToEnd();
            }
        }

        private static bool ExtractVersionIndex(string name, ref decimal number)
        {
            var match = Regex.Match(name, VersionIndexMatchExpression, RegexOptions.IgnoreCase);

            if (match != Match.Empty)
            {
                number = decimal.Parse(match.Groups["number"].Value);
                return true;
            }

            return false;
        }
    }
}