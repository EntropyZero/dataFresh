using System;
using System.Data.SqlClient;
using System.Xml;
using DataFresh;
using NUnit.Framework;
using System.IO;

namespace TestHarnessExample
{
	public class TestFixtureBase
	{
		public static SqlDataFresh dataFresh = null;
        public readonly string connectionString;

		public TestFixtureBase()
		{
            var doc = new XmlDocument();
            doc.Load(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), @"..\..\..\TestConnectionStrings.xml"));
            connectionString = 
                String.Format(doc.SelectSingleNode("/connectionStrings/sqlDataFreshSampleConnectionStringTemplate").InnerText,
                    doc.SelectSingleNode("/connectionStrings/properties/userId").InnerText,
                    doc.SelectSingleNode("/connectionStrings/properties/password").InnerText,
                    doc.SelectSingleNode("/connectionStrings/properties/server").InnerText,
                    doc.SelectSingleNode("/connectionStrings/properties/pooling").InnerText);

            if (dataFresh == null)
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

		public void ExecuteNonQuery(string sql)
		{
			using (var conn = new SqlConnection(connectionString))
			{
				var cmd = new SqlCommand(sql, conn);
				conn.Open();
				cmd.ExecuteNonQuery();
			}
		}

		public object ExecuteScalar(string sql)
		{
			using (var conn = new SqlConnection(connectionString))
			{
				var cmd = new SqlCommand(sql, conn);
				conn.Open();
				return cmd.ExecuteScalar();
			}
		}

		#endregion
	}
}