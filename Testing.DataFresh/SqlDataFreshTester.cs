using System;
using System.IO;
using DataFresh;
using EntropyZero.deltaRunner;
using NUnit.Framework;

namespace Testing.DataFresh
{
	[TestFixture]
	public class SqlDataFreshTester : TestFixtureBase
	{
		[Test]
		public void PrepDatabaseForDataFresh_NoConnectionString()
		{
			var dataFresh = new SqlDataFresh(null);
			Assert.Throws<InvalidOperationException>(() => dataFresh.PrepareDatabaseForDataFresh(), "The ConnectionString property has not been initialized.");
		}

		[Test]
		public void RemoveDataFresh()
		{
			var dataFresh = new SqlDataFresh(connectionString);

			dataFresh.RemoveDataFreshFromDatabase();
			Assert.IsFalse(dataFresh.TableExists(SqlDataFresh.ChangeTrackingTableName));
		}

		[Test]
		public void PrepDatabaseForDataFresh()
		{
			var dataFresh = new SqlDataFresh(connectionString);

			dataFresh.RemoveDataFreshFromDatabase();
			Assert.IsFalse(dataFresh.TableExists(SqlDataFresh.ChangeTrackingTableName));

			dataFresh.PrepareDatabaseForDataFresh();
			Assert.IsTrue(dataFresh.TableExists(SqlDataFresh.ChangeTrackingTableName));
		}

		[Test]
		public void IdentityReseedDuringRefresh()
		{
			InitializeTheDatabase();
			var dataFresh = new SqlDataFresh(connectionString);
			dataFresh.PrepareDatabaseForDataFresh();
			var id = Convert.ToInt32(ExecuteScalar("insert into author (lastname, firstname) values ('brockey', 'mike'); select @@identity;"));
			Assert.AreEqual(6, id);
			dataFresh.RefreshTheDatabase();
			var id2 = Convert.ToInt32(ExecuteScalar("insert into author (lastname, firstname) values ('brockey', 'mike'); select @@identity;"));
			Assert.AreEqual(6, id2);
		}

		[Test]
		public void IdentityReseedDuringRefresh_TableWithNoRows()
		{
			InitializeTheDatabase();
			var dataFresh = new SqlDataFresh(connectionString);
			dataFresh.PrepareDatabaseForDataFresh();
			var id = Convert.ToInt32(ExecuteScalar("insert into movie (title) values ('mike brockey takes over the world'); select @@identity;"));
			Assert.AreEqual(1, id);
			dataFresh.RefreshTheDatabase();
			var id2 = Convert.ToInt32(ExecuteScalar("insert into movie (title) values ('mike brockey takes over the world'); select @@identity;"));
			Assert.AreEqual(1, id2);
			var id3 = Convert.ToInt32(ExecuteScalar("insert into movie (title) values ('mike brockey takes over the world 2'); select @@identity;"));
			Assert.AreEqual(2, id3);
		}

		[Test]
		public void IdentityReseedDuringRefresh_TableWithNoRows2()
		{
			InitializeTheDatabase();
			var dataFresh = new SqlDataFresh(connectionString);
			dataFresh.RemoveDataFreshFromDatabase();
			dataFresh.PrepareDatabaseForDataFresh();
			ExecuteNonQuery("insert into movie2 (movieid, title) values (1, 'mike brockey takes over the world');");
			Assert.DoesNotThrow(() => dataFresh.RefreshTheDatabase());
		}

		[Test]
		public void IdentityReseedWorksWithTablesThatUseReservedWords()
		{
			InitializeTheDatabase();
			var dataFresh = new SqlDataFresh(connectionString);
			dataFresh.RemoveDataFreshFromDatabase();
			dataFresh.PrepareDatabaseForDataFresh();
			ExecuteNonQuery("insert into [check] (title) values ('mike brockey takes over the world');");
			Assert.DoesNotThrow(() => dataFresh.RefreshTheDatabase());
		}

		[Test]
		public void RefreshTheDatabaseSpeedTests()
		{
			var dataFresh = new SqlDataFresh(connectionString, true);
			dataFresh.RemoveDataFreshFromDatabase();
			dataFresh.PrepareDatabaseForDataFresh();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			dataFresh.RefreshTheDatabase();
			ExecuteNonQuery("insert into author (lastname, firstname) values ('brockey', 'mike');");
			Assert.DoesNotThrow(() => dataFresh.RefreshTheDatabase());
		}

		[Test]
		public void ShouldNotRefreshDeltaRunnerTable()
		{
			InitializeTheDatabase();
			var dataFresh = new SqlDataFresh(connectionString);
			dataFresh.RemoveDataFreshFromDatabase();
			dataFresh.PrepareDatabaseForDataFresh();
			ExecuteNonQuery("insert into [dr_deltaversion] ([latestdelta], [hash], [filename]) values (99, 'blah-hash', 'whatever file');");
			Assert.AreEqual(1, ExecuteScalar("select count(*) from [dr_deltaversion] where [filename] = 'whatever file'"));
			dataFresh.RefreshTheDatabase();
			Assert.AreEqual(1, ExecuteScalar("select count(*) from [dr_deltaversion] where [filename] = 'whatever file'"));
		}

		void InitializeTheDatabase()
		{
			var binDir = Path.GetDirectoryName(GetType().Assembly.Location) ?? string.Empty;
			var deltaRunner = new SqlDeltaRunner(connectionString, Path.Combine(binDir, deltaPath), true);
			deltaRunner.PrepareForDeltaRunner();
			deltaRunner.AddSqlFile(new FileInfo(Path.Combine(binDir, databaseFilesPath, "Database.sql")), SqlFileExecutionOption.ExecuteBeforeDeltas);
			deltaRunner.AddSqlFile(new FileInfo(Path.Combine(binDir, databaseFilesPath, "Setup.sql")), SqlFileExecutionOption.ExecuteAfterDeltas);
			deltaRunner.ApplyDeltas();
		}
	}
}