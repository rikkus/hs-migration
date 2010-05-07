using HS.Migration.Exceptions;

namespace HS.Migration.Tests
{
    public class MockStorage : IMigrationStorage
    {
        public readonly string[] Sql = {
                                           "first",
                                           "second",
                                           "last",
                                       };

        public readonly decimal[] VersionIndices = {
                                                       0.01m,
                                                       1.2567m,
                                                       1.2568m
                                                   };

        #region IMigrationStorage Members

        public string SQL(decimal versionIndex)
        {
            if (versionIndex == VersionIndices[0])
                return Sql[0];
            if (versionIndex == VersionIndices[1])
                return Sql[1];
            if (versionIndex == VersionIndices[2])
                return Sql[2];

            throw new MigrationMissingException(versionIndex);
        }

        public decimal NextVersionIndex(decimal versionIndex)
        {
            if (versionIndex < VersionIndices[0])
                return VersionIndices[0];
            if (versionIndex == VersionIndices[0])
                return VersionIndices[1];
            if (versionIndex == VersionIndices[1])
                return VersionIndices[2];

            return -1;
        }

        public decimal LastVersionIndex()
        {
            return VersionIndices[VersionIndices.Length - 1];
        }

        #endregion
    }
}