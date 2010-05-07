namespace HS.Migration
{
    public class Migrator : IMigrator
    {
        private readonly IVersionedDb database;
        private readonly IMigrationStorage migrationStorage;

        public Migrator(IVersionedDb database, IMigrationStorage migrationStorage)
        {
            this.database = database;
            this.migrationStorage = migrationStorage;
        }

        #region IMigrator Members

        public void MigrateToNewestVersion(TransactionPolicy transactionPolicy)
        {
            database.MigrateToVersion(migrationStorage.LastVersionIndex(), migrationStorage, transactionPolicy);
        }

        public void MigrateToVersion(decimal targetVersion, TransactionPolicy transactionPolicy)
        {
            database.MigrateToVersion(targetVersion, migrationStorage, transactionPolicy);
        }

        #endregion
    }
}