using System;

namespace HS.Migration.Exceptions
{
    public class MigrationMissingException : Exception
    {
        public MigrationMissingException(decimal index)
            : base(String.Format("Migration {0} missing", index))
        {
        }
    }
}