using HS.Migration.Exceptions;
using NUnit.Framework;

namespace HS.Migration.Tests
{
    [TestFixture]
    public class DbMigratorFixture
    {
        [Test]
        public void InitVersionedDb()
        {
            var versionedDb = new MockVersionedDb();

            Assert.AreEqual(versionedDb.Version(null), MockVersionedDb.InitialVersionIndex);
        }

        [Test]
        [ExpectedException(typeof (MigrationMissingException))]
        public void MigrateFailsProperlyAskingForNegativeVersion()
        {
            var storage = new MockStorage();
            var versionedDb = new MockVersionedDb();
            var migrator = new Migrator(versionedDb, storage);

            migrator.MigrateToVersion(-1m, TransactionPolicy.RollBackOnError);
        }

        [Test]
        [ExpectedException(typeof (MigrationMissingException))]
        public void MigrateFailsProperlyAskingForNonExistentVersion()
        {
            var storage = new MockStorage();
            var versionedDb = new MockVersionedDb();
            var migrator = new Migrator(versionedDb, storage);

            migrator.MigrateToVersion(storage.LastVersionIndex() + 1m, TransactionPolicy.RollBackOnError);
        }

        [Test]
        public void MigrateOneVersion()
        {
            var storage = new MockStorage();
            var versionedDb = new MockVersionedDb();
            var migrator = new Migrator(versionedDb, storage);

            migrator.MigrateToVersion(storage.VersionIndices[1], TransactionPolicy.RollBackOnError);

            Assert.AreEqual(versionedDb.Version(null), storage.VersionIndices[1]);
        }

        [Test]
        public void MigrateOneVersionIdempotent()
        {
            var storage = new MockStorage();
            var versionedDb = new MockVersionedDb();
            var migrator = new Migrator(versionedDb, storage);

            migrator.MigrateToVersion(storage.VersionIndices[0], TransactionPolicy.RollBackOnError);

            Assert.AreEqual(versionedDb.Version(null), storage.VersionIndices[0]);
        }

        [Test]
        public void MigrateToNewestVersion()
        {
            var storage = new MockStorage();
            var versionedDb = new MockVersionedDb();
            var migrator = new Migrator(versionedDb, storage);

            migrator.MigrateToNewestVersion(TransactionPolicy.RollBackOnError);

            Assert.AreEqual(versionedDb.Version(null), storage.LastVersionIndex());
        }

        [Test]
        public void MigrateTwoVersions()
        {
            var storage = new MockStorage();
            var versionedDb = new MockVersionedDb();
            var migrator = new Migrator(versionedDb, storage);

            migrator.MigrateToVersion(storage.VersionIndices[2], TransactionPolicy.RollBackOnError);

            Assert.AreEqual(versionedDb.Version(null), storage.VersionIndices[2]);
        }
    }
}