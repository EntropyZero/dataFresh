using System;
using System.Data.SqlClient;
using DataFresh;
using NUnit.Framework;

namespace TestHarnessExample
{
	public class TestFixtureBase
	{
		public static SqlDataFresh dataFresh = null;
		public static string connectionString = "user id=test;password=test;Initial Catalog=DataFreshSample;Data Source=(local);";

		public TestFixtureBase()
		{
			if(dataFresh == null)
			{
				dataFresh = new SqlDataFresh(connectionString);
			}
		}

		[SetUp]
		public void SetUp()
		{
			Console.Out.WriteLine("Setup");
			if (dataFresh.HasDatabaseBeenModified())
			{
				dataFresh.RefreshTheDatabase();
			}
		}

		[TearDown]
		public void TearDown()
		{
			Console.Out.WriteLine("TearDown");
			if (dataFresh.HasDatabaseBeenModified())
			{
				Console.Out.WriteLine("Database Write");
			}
		}

		#region Data Access Helpers

		public static void ExecuteNonQuery(string sql)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(sql, conn);
				conn.Open();
				cmd.ExecuteNonQuery();
			}
		}

		public static object ExecuteScalar(string sql)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(sql, conn);
				conn.Open();
				return cmd.ExecuteScalar();
			}
		}

		#endregion
	}
}