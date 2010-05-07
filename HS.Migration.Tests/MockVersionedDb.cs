using System.Data;

namespace HS.Migration.Tests
{
    internal class MockVersionedDb : VersionedDb
    {
        public const decimal InitialVersionIndex = 0.00001m;
        private decimal version = InitialVersionIndex;

        #region IVersionedDb Members

        public override decimal Version()
        {
            return Version(null);
        }

        public override decimal Version(IDbTransaction transaction)
        {
            return version;
        }

        public override void MigrateToVersion(decimal targetVersionIndex, IMigrationStorage migrationStorage, TransactionPolicy transactionPolicy)
        {
            while (version != targetVersionIndex)
            {
                decimal nextVersionIndex = migrationStorage.NextVersionIndex(version);

                OnBeginningMigration(nextVersionIndex);

                // Pretend to use SQL.
                migrationStorage.SQL(nextVersionIndex);
                version = nextVersionIndex;

                OnMigrationCompleted(nextVersionIndex, true, "");
            }
        }

        public void OnBeginningMigration(decimal targetVersionIndex)
        {
            var del = beginningMigration;

            if (del != null)
            {
                del.Invoke(targetVersionIndex);
            }
        }

        public void OnMigrationCompleted(decimal newVersionIndex, bool successful, string message)
        {
            var del = migrationComplete;

            if (del != null)
            {
                del.Invoke(newVersionIndex, successful, message);
            }
        }

        #endregion
    }
}