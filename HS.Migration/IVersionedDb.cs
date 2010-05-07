using System.Data;

namespace HS.Migration
{
    public enum TransactionPolicy
    {
        RollBackOnError,
        CommitOnError
    }

    public interface IVersionedDb
    {
        decimal Version(IDbTransaction transaction);

        void MigrateToVersion(decimal targetVersionIndex, IMigrationStorage migrationStorage,
                             TransactionPolicy transactionPolicy);
    }
}