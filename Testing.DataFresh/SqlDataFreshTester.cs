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
		[Test, ExpectedException(typeof (InvalidOperationException), "The ConnectionString property has not been initialized.")]
		public void PrepDatabaseforDataFresh_NoConnectionString()
		{
			SqlDataFresh dataFresh = new SqlDataFresh(null);
			dataFresh.PrepareDatabaseforDataFresh();
		}

		[Test]
		public void Encrypt()
		{
//			string key = "pass@word1";
//			
//			string enc = ResourceManagement.Encrypt("test", key);
//			Assert.AreEqual("test", ResourceManagement.Decrypt(enc, key));
			
//			ResourceManagement.Encrypt(@"C:\development\dataFresh\DataFresh\Resources\PrepareDataFresh.sql",
//			                           @"C:\development\dataFresh\DataFresh\Resources\PrepareDataFresh.sql.enc", key);
			
//			byte[] dec = ResourceManagement.GetDecryptedResourceBytes("DataFresh.Resources.PrepareDataFresh.sql.enc");
//			string decStr = System.Text.Encoding.ASCII.GetString(dec);
//			Console.Out.WriteLine("decStr = {0}", decStr);
			
//			string str = ResourceManagement.GetDecryptedResource("DataFresh.Resources.PrepareDataFresh.sql");
//			Console.Out.WriteLine("str = {0}", str);
		}
		
		[Test]
		public void RemoveDataFresh()
		{
			SqlDataFresh dataFresh = new SqlDataFresh(connectionString);

			dataFresh.RemoveDataFreshFromDatabase();
			Assert.IsFalse(dataFresh.TableExists(dataFresh.ChangeTrackingTableName));
			Assert.IsFalse(dataFresh.ProcedureExists(dataFresh.ExtractProcedureName));
			Assert.IsFalse(dataFresh.ProcedureExists(dataFresh.ImportProcedureName));
			Assert.IsFalse(dataFresh.ProcedureExists(dataFresh.PrepareProcedureName));
			Assert.IsFalse(dataFresh.ProcedureExists(dataFresh.RefreshProcedureName));
		}
		
		[Test]
		public void PrepDatabaseforDataFresh()
		{
			SqlDataFresh dataFresh = new SqlDataFresh(connectionString);

			dataFresh.RemoveDataFreshFromDatabase();
			Assert.IsFalse(dataFresh.TableExists(dataFresh.ChangeTrackingTableName));
			Assert.IsFalse(dataFresh.ProcedureExists(dataFresh.ExtractProcedureName));
			Assert.IsFalse(dataFresh.ProcedureExists(dataFresh.ImportProcedureName));
			Assert.IsFalse(dataFresh.ProcedureExists(dataFresh.PrepareProcedureName));
			Assert.IsFalse(dataFresh.ProcedureExists(dataFresh.RefreshProcedureName));

			dataFresh.PrepareDatabaseforDataFresh();
			Assert.IsTrue(dataFresh.TableExists(dataFresh.ChangeTrackingTableName));
			Assert.IsTrue(dataFresh.ProcedureExists(dataFresh.ExtractProcedureName));
			Assert.IsTrue(dataFresh.ProcedureExists(dataFresh.ImportProcedureName));
			Assert.IsTrue(dataFresh.ProcedureExists(dataFresh.PrepareProcedureName));
			Assert.IsTrue(dataFresh.ProcedureExists(dataFresh.RefreshProcedureName));
		}

		[Test]
		public void SnapshopPath_ManualOverride()
		{
			SqlDataFresh dataFresh = new SqlDataFresh(connectionString);
			DirectoryInfo tempPath = new DirectoryInfo(Path.GetTempPath());
			dataFresh.SnapshotPath = tempPath;
			Console.Out.WriteLine("dataFresh.SnapshotPath.FullName = {0}", dataFresh.SnapshotPath.FullName);
			Assert.AreEqual(tempPath.FullName, dataFresh.SnapshotPath.FullName);
		}

		[Test]
		public void SnapshopPath_ManualOverrideTrailingBackslash()
		{
			SqlDataFresh dataFresh = new SqlDataFresh(connectionString);
			string tempPath = @"c:\temp\folder";
			dataFresh.SnapshotPath = new DirectoryInfo(tempPath);
			Assert.AreEqual(@"c:\temp\folder\", dataFresh.SnapshotPath.FullName);
		}

		[Test]
		public void SnapshopPath_Resetting()
		{
			SqlDataFresh dataFresh = new SqlDataFresh(connectionString);
			dataFresh.SnapshotPath = null;
			Assert.IsTrue(dataFresh.SnapshotPath.FullName.IndexOf("Snapshot_DataFreshSample") > -1);
		}

		[Test]
		public void SnapshopPath()
		{
			SqlDataFresh dataFresh = new SqlDataFresh(connectionString);
			Assert.IsTrue(dataFresh.SnapshotPath.FullName.IndexOf("Snapshot_DataFreshSample") > -1);
		}

		[Test]
		public void IdentityReseedDuringRefresh()
		{
			InitializeTheDatabase();
			SqlDataFresh dataFresh = new SqlDataFresh(connectionString);
			dataFresh.PrepareDatabaseforDataFresh(true);
			int id = Convert.ToInt32(ExecuteScalar("insert into author (lastname, firstname) values ('brockey', 'mike'); select @@identity;"));
			Assert.AreEqual(6, id);
			dataFresh.RefreshTheDatabase();
			int id2 = Convert.ToInt32(ExecuteScalar("insert into author (lastname, firstname) values ('brockey', 'mike'); select @@identity;"));
			Assert.AreEqual(6, id2);
		}

		[Test]
		public void IdentityReseedDuringRefresh_TableWithNoRows()
		{
			InitializeTheDatabase();
			SqlDataFresh dataFresh = new SqlDataFresh(connectionString);
			dataFresh.PrepareDatabaseforDataFresh(true);
			int id = Convert.ToInt32(ExecuteScalar("insert into movie (title) values ('mike brockey takes over the world'); select @@identity;"));
			Assert.AreEqual(1, id);
			dataFresh.RefreshTheDatabase();
			int id2 = Convert.ToInt32(ExecuteScalar("insert into movie (title) values ('mike brockey takes over the world'); select @@identity;"));
			Assert.AreEqual(1, id2);
			int id3 = Convert.ToInt32(ExecuteScalar("insert into movie (title) values ('mike brockey takes over the world 2'); select @@identity;"));
			Assert.AreEqual(2, id3);
		}

		[Test]
		public void IdentityReseedDuringRefresh_TableWithNoRows2()
		{
			InitializeTheDatabase();
			SqlDataFresh dataFresh = new SqlDataFresh(connectionString);
			dataFresh.RemoveDataFreshFromDatabase();
			dataFresh.PrepareDatabaseforDataFresh(true);
			ExecuteNonQuery("insert into movie2 (movieid, title) values (1, 'mike brockey takes over the world');");
			dataFresh.RefreshTheDatabase();
		}
		
		[Test]
		public void IdentityReseedWorksWithTablesThatUseReservedWords()
		{
			InitializeTheDatabase();
			SqlDataFresh dataFresh = new SqlDataFresh(connectionString);
			dataFresh.RemoveDataFreshFromDatabase();
			dataFresh.PrepareDatabaseforDataFresh(true);
			ExecuteNonQuery("insert into [check] (title) values ('mike brockey takes over the world');");
			dataFresh.RefreshTheDatabase();
		}
		
		[Test]
		public void RefreshTheDatabaseSpeedTests()
		{
			SqlDataFresh dataFresh = new SqlDataFresh(connectionString, true);
			dataFresh.RemoveDataFreshFromDatabase();
			dataFresh.PrepareDatabaseforDataFresh(true);
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
			dataFresh.RefreshTheDatabase();
		}

		[Test]
		public void ShouldNotRefreshDeltaRunnerTable()
		{
			InitializeTheDatabase();
			SqlDataFresh dataFresh = new SqlDataFresh(connectionString);
			dataFresh.RemoveDataFreshFromDatabase();
			dataFresh.PrepareDatabaseforDataFresh(true);
			ExecuteNonQuery("insert into [dr_deltaversion] ([latestdelta], [hash], [filename]) values (99, 'blah-hash', 'whatever file');");
			Assert.AreEqual(1, ExecuteScalar("select count(*) from [dr_deltaversion] where [filename] = 'whatever file'"));
			dataFresh.RefreshTheDatabase();
			Assert.AreEqual(1, ExecuteScalar("select count(*) from [dr_deltaversion] where [filename] = 'whatever file'"));
		}

		private void InitializeTheDatabase()
		{
			SqlDeltaRunner deltaRunner = new SqlDeltaRunner(connectionString, new DirectoryInfo(deltaPath).FullName, true);
			deltaRunner.PrepareForDeltaRunner();
			deltaRunner.AddSqlFile(new FileInfo(Path.Combine(databaseFilesPath, "Database.sql")),SqlFileExecutionOption.ExecuteBeforeDeltas);
			deltaRunner.AddSqlFile(new FileInfo(Path.Combine(databaseFilesPath, "Setup.sql")),SqlFileExecutionOption.ExecuteAfterDeltas);
			deltaRunner.ApplyDeltas();
		}
	}
}