using HS.Migration.Exceptions;

namespace HS.Migration
{
    public interface IMigrationStorage
    {
        /// <param name="versionIndex"></param>
        /// <returns>SQL text required to update database to <paramref name="versionIndex"/></returns>
        /// <exception cref="MigrationMissingException"></exception>
        string SQL(decimal versionIndex);

        /// <returns>Next version index available after <paramref name="versionIndex"/>
        /// or -1 if there is no such index.</returns>
        decimal NextVersionIndex(decimal versionIndex);

        /// <returns>Last version index available, or -1 if there are no migrations available.</returns>
        decimal LastVersionIndex();
    }
}