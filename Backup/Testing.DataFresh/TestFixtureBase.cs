using System.Data.SqlClient;
using System.IO;
using EntropyZero.deltaRunner;
using NUnit.Framework;

namespace Testing.DataFresh
{
	public class TestFixtureBase
	{
		public static string connectionString = "user id=test;password=test;Initial Catalog=DataFreshSample;Data Source=(local);pooling=false;";
		public static string connectionStringMaster = "user id=test;password=test;Initial Catalog=Master;Data Source=(local);pooling=false;";
		public static string databaseFilesPath = @"..\..\Database Files\";
		public static string deltaPath = @"..\..\Database Files\Deltas";

		[SetUp]
		public void SetUp()
		{
			SqlDeltaRunner.CreateDatabase("DataFreshSample", connectionStringMaster, true);
			DeltaRunnerInstance.ApplyDeltas();
		}
		
		public SqlDeltaRunner DeltaRunnerInstance
		{
			get
			{
				SqlDeltaRunner deltaRunner = new SqlDeltaRunner(connectionString, deltaPath, true);
				deltaRunner.PrepareForDeltaRunner();
				deltaRunner.AddSqlFile(new FileInfo(Path.Combine(databaseFilesPath, "database.sql")),SqlFileExecutionOption.ExecuteBeforeDeltas);
				deltaRunner.AddSqlFile(new FileInfo(Path.Combine(databaseFilesPath, "setup.sql")),SqlFileExecutionOption.ExecuteAfterDeltas);
				return deltaRunner;
			}
		}
		
		public void ExecuteNonQuery(string sql)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(sql, conn);
				cmd.CommandTimeout = 1200;
				conn.Open();
				cmd.ExecuteNonQuery();
			}
		}

		public object ExecuteScalar(string sql)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(sql, conn);
				cmd.CommandTimeout = 1200;
				conn.Open();
				return cmd.ExecuteScalar();
			}
		}
	}
}