using System.Data.SqlClient;
using System.IO;
using EntropyZero.deltaRunner;
using NUnit.Framework;

namespace Testing.DataFresh
{
	public class TestFixtureBase
	{
		public readonly string connectionString;
		public readonly string connectionStringMaster;
		public static string databaseFilesPath = @"..\..\Database Files";
		public static string deltaPath = @"..\..\Database Files\Deltas";

		public TestFixtureBase()
		{
			var doc = new System.Xml.XmlDocument();
			doc.Load(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location) ?? string.Empty, @"..\..\..\TestConnectionStrings.xml"));
			connectionString = connectionString =
				 string.Format(doc.SelectSingleNode("/connectionStrings/sqlDataFreshSampleConnectionStringTemplate")?.InnerText ?? string.Empty,
					doc.SelectSingleNode("/connectionStrings/properties/userId")?.InnerText,
					doc.SelectSingleNode("/connectionStrings/properties/password")?.InnerText,
					doc.SelectSingleNode("/connectionStrings/properties/server")?.InnerText,
					doc.SelectSingleNode("/connectionStrings/properties/pooling")?.InnerText);
			connectionStringMaster =
				 string.Format(doc.SelectSingleNode("/connectionStrings/sqlMasterDatabaseConnectionStringTemplate")?.InnerText ?? string.Empty,
					doc.SelectSingleNode("/connectionStrings/properties/userId")?.InnerText,
					doc.SelectSingleNode("/connectionStrings/properties/password")?.InnerText,
					doc.SelectSingleNode("/connectionStrings/properties/server")?.InnerText,
					doc.SelectSingleNode("/connectionStrings/properties/pooling")?.InnerText);
		}

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
				var binDir = Path.GetDirectoryName(GetType().Assembly.Location) ?? string.Empty;
				var deltaRunner = new SqlDeltaRunner(connectionString, Path.Combine(binDir, deltaPath), true);
				deltaRunner.PrepareForDeltaRunner();
				deltaRunner.AddSqlFile(new FileInfo(Path.Combine(binDir, databaseFilesPath, "database.sql")), SqlFileExecutionOption.ExecuteBeforeDeltas);
				deltaRunner.AddSqlFile(new FileInfo(Path.Combine(binDir, databaseFilesPath, "setup.sql")), SqlFileExecutionOption.ExecuteAfterDeltas);
				return deltaRunner;
			}
		}

		public void ExecuteNonQuery(string sql)
		{
			using (var conn = new SqlConnection(connectionString))
			{
				var cmd = new SqlCommand(sql, conn) { CommandTimeout = 1200 };
				conn.Open();
				cmd.ExecuteNonQuery();
			}
		}

		public object ExecuteScalar(string sql)
		{
			using (var conn = new SqlConnection(connectionString))
			{
				var cmd = new SqlCommand(sql, conn) { CommandTimeout = 1200 };
				conn.Open();
				return cmd.ExecuteScalar();
			}
		}
	}
}