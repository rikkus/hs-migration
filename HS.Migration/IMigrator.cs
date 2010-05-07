using System.Data.SqlClient;
using HS.Migration.Exceptions;

namespace HS.Migration
{
    public interface IMigrator
    {
        /// <exception cref="SqlException">
        /// Thrown if migration storage cannot provide the necessary migrations.
        /// </exception>
        /// <exception cref="MigrationMissingException"></exception>
        void MigrateToNewestVersion(TransactionPolicy transactionPolicy);

        /// <exception cref="MigrationMissingException">
        /// Thrown if migration storage cannot provide the necessary migrations.
        /// </exception>
        /// <exception cref="SqlException"></exception>
        void MigrateToVersion(decimal versionIndex, TransactionPolicy transactionPolicy);
    }
}