Making changes to a database schema or its data requires careful planning and execution, to avoid mistakes.

This library provides support for this task, allowing the user to write update scripts once, save them in a common location, with logical naming, and apply them easily.

Support for storing migration scripts in a .NET assembly and applying them to an SQL Server database is built in. The architecture caters for those wishing to add support for other forms of migration storage or database provider.

```
var db = new SqlVersionedDb(connection);

decimal version = db.Version();

if (version == Constants.Unversioned)
    log.WriteLine("Creating database.");
else
    log.WriteLine(string.Format("Initial database version: {0}", version);

db.beginningUpdate +=
    index => log.Write(string.Format("Updating to version: {0} ... ", index));

db.updateComplete +=
    (index, successful, message) =>
    {
        if (successful)
        {
            log.WriteLine(LogMessageType.Success, "OK");
        }
        else
        {
            log.WriteLine(LogMessageType.Failure, "FAILED");
            log.WriteLine(LogMessageType.Failure, message);
        }
    };

IVersionUpdateStorage storage =
    new AssemblyVersionUpdateStorage(typeof (Locator).Assembly));

var migrator = new VersionUpdater(db, storage);

log.WriteLine(string.Format("Target version: {0}", storage.LastVersionIndex()));

migrator.MigrateToNewestVersion(TransactionPolicy.RollBackOnError);

log.WriteLine(string.Format("Final database version: {0}", db.Version()));
```