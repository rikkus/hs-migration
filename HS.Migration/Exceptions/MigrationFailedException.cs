using System;

namespace HS.Migration.Exceptions
{
    public class MigrationFailedException : Exception
    {
        private readonly string sqlServerMessage;

        public MigrationFailedException(decimal version, string sqlServerMessage)
            : base(String.Format("Migration to version {0} failed: {1}", version, sqlServerMessage))
        {
            this.sqlServerMessage = sqlServerMessage;
        }

        public string SqlServerMessage
        {
            get { return sqlServerMessage; }
        }
    }
}