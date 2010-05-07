using System.Data;

namespace HS.Migration
{
    public abstract class VersionedDb : IVersionedDb
    {
        #region Delegates

        public delegate void BeginningMigration(decimal targetVersionIndex);

        public delegate void MigrationComplete(decimal newVersionIndex, bool successful, string message);

        #endregion

        public BeginningMigration beginningMigration;
        public MigrationComplete migrationComplete;

        #region IVersionedDb Members

        public abstract decimal Version();

        public abstract decimal Version(IDbTransaction transaction);

        public abstract void MigrateToVersion(decimal targetVersionIndex, IMigrationStorage migrationStorage,
                                             TransactionPolicy transactionPolicy);

        #endregion
    }
}