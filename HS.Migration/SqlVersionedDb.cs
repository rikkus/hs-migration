using System;
using System.Data;
using HS.Migration.Exceptions;

namespace HS.Migration
{
    public class SqlVersionedDb : VersionedDb
    {
        private const string DefaultSchemaInfoTableName = "SchemaInfo";

        public SqlVersionedDb(IDbConnection connection)
        {
            Connection = connection;
            SchemaInfoTableName = DefaultSchemaInfoTableName;
        }

        protected IDbConnection Connection { get; set; }
        private string SchemaInfoTableName { get; set; }

        public override decimal Version()
        {
            return Version(null /* No transaction */);
        }

        public override decimal Version(IDbTransaction transaction)
        {
            EnsureSchemaInfoTableExists(transaction);

            using (var command = Connection.CreateCommand())
            {
                command.Transaction = transaction;

                command.CommandText =
                    string.Format
                        (
                        @"
                    SELECT  [Value]
                    FROM    [{0}]
                    WHERE   [Key] = 'Version';",
                        SchemaInfoTableName
                        );

                var value = (string) command.ExecuteScalar();

                if (string.IsNullOrEmpty(value))
                    return -1;

                return decimal.Parse(value);
            }
        }

        public override void MigrateToVersion(decimal targetVersionIndex, IMigrationStorage migrationStorage,
                                             TransactionPolicy transactionPolicy)
        {
            using (var transaction = Connection.BeginTransaction())
            {
                try
                {
                    decimal currentVersionIndex = Version(transaction);

                    while (currentVersionIndex != targetVersionIndex)
                    {
                        UpdateOneStepToVersion
                            (
                            migrationStorage,
                            migrationStorage.NextVersionIndex(currentVersionIndex),
                            transaction
                            );

                        currentVersionIndex = Version(transaction);
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    if (transactionPolicy == TransactionPolicy.CommitOnError)
                    {
                        transaction.Commit();
                    }

                    throw;
                }
            }
        }

        private void OnBeginningMigration(decimal targetVersionIndex)
        {
            var del = beginningMigration;

            if (del != null)
            {
                del.Invoke(targetVersionIndex);
            }
        }

        private void OnMigrationCompleted(decimal newVersionIndex, bool successful, string message)
        {
            var del = migrationComplete;

            if (del != null)
            {
                del.Invoke(newVersionIndex, successful, message);
            }
        }

        private void CreateSchemaInfoTable(IDbTransaction transaction)
        {
            using (var command = Connection.CreateCommand())
            {
                command.Transaction = transaction;

                command.CommandText =
                    string.Format
                        (
                        @"
                    CREATE  TABLE [{0}]
                    (
                        [Key]     VARCHAR(50),
                        [Value]   VARCHAR(255),
                        PRIMARY KEY([Key])
                    );",
                        SchemaInfoTableName
                        );

                command.ExecuteNonQuery();
            }
        }

        private bool SchemaInfoTableExists(IDbTransaction transaction)
        {
            using (var command = Connection.CreateCommand())
            {
                command.Transaction = transaction;

                command.CommandText =
                    string.Format
                        (
                        @"
                    SELECT  COUNT(1)
                    FROM    INFORMATION_SCHEMA.TABLES
                    WHERE   TABLE_NAME = '{0}';
                    ",
                        SchemaInfoTableName
                        );

                return (int) command.ExecuteScalar() == 1;
            }
        }

        private void EnsureSchemaInfoTableExists(IDbTransaction transaction)
        {
            if (!SchemaInfoTableExists(transaction))
            {
                CreateSchemaInfoTable(transaction);
            }
        }

        private void UpdateOneStepToVersion
            (
            IMigrationStorage storage,
            decimal targetVersion,
            IDbTransaction transaction
            )
        {
            OnBeginningMigration(targetVersion);

            try
            {
                using (var command = Connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandTimeout = 0; // No timeout.

                    command.CommandText = storage.SQL(targetVersion);

                    command.ExecuteNonQuery();
                }

                UpdateVersionInSchemaInfoTable(targetVersion, transaction);

                OnMigrationCompleted(targetVersion, true, "");
            }
            catch (Exception ex)
            {
                OnMigrationCompleted(targetVersion, false, ex.Message);
                throw new MigrationFailedException(targetVersion, ex.Message);
            }
        }

        private void UpdateVersionInSchemaInfoTable(decimal targetVersion, IDbTransaction transaction)
        {
            DeleteAnyExistingVersionInSchemaInfoTable(transaction);
            AddVersionToSchemaInfoTable(targetVersion, transaction);
        }

        private void DeleteAnyExistingVersionInSchemaInfoTable(IDbTransaction transaction)
        {
            using (var command = Connection.CreateCommand())
            {
                command.Transaction = transaction;

                command.CommandText =
                    string.Format
                        (
                        @"
                    DELETE  FROM [{0}]
                    WHERE   [Key] = 'Version';
                    ",
                        SchemaInfoTableName
                        );

                command.ExecuteNonQuery();
            }
        }

        private void AddVersionToSchemaInfoTable(decimal targetVersion, IDbTransaction transaction)
        {
            using (var command = Connection.CreateCommand())
            {
                command.Transaction = transaction;

                command.CommandText =
                    string.Format
                        (
                        @"
                    INSERT INTO [{0}]   ([Key], [Value])
                    VALUES              ('Version', '{1}');
                    ",
                        SchemaInfoTableName,
                        targetVersion
                        );

                command.ExecuteNonQuery();
            }
        }
    }
}