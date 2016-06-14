// EntropyZero dataFresh Copyright (C) 2007 EntropyZero Consulting, LLC.
// Please visit us on the web: http://blogs.ent0.com/
//
// This library is free software; you can redistribute it and/or modify 
// it under the terms of the GNU Lesser General Public License as 
// published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU 
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public 
// License along with this library; if not, write to:
// Free Software Foundation, Inc., 
// 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 

using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace DataFresh
{
	/// <summary>
	/// dataFresh by Entropy Zero Consulting is a library that will enable
	/// the test driven developer to build a test harness that will refresh
	/// the database to a known state between tests
	/// </summary>
	public class SqlDataFresh : IDataFresh
	{
		#region Member Variables

		private string connectionString = null;
		private DirectoryInfo snapshotPath = null;

		private string PrepareScriptResourceName = "DataFresh.Resources.PrepareDataFresh.sql";
		private string RemoveScriptResourceName = "DataFresh.Resources.RemoveDataFresh.sql";

		public string PrepareProcedureName = "df_ChangeTrackingTriggerCreate";
		public string RefreshProcedureName = "df_ChangedTableDataRefresh";
		public string ExtractProcedureName = "df_TableDataExtract";
		public string ImportProcedureName = "df_TableDataImport";
		public string ChangeTrackingTableName = "df_ChangeTracking";

		private bool verbose = false;

		#endregion

		#region Public Methods

		public SqlDataFresh(string connectionString)
		{
			this.connectionString = connectionString;
		}

		public SqlDataFresh(string connectionString, bool verbose)
		{
			this.connectionString = connectionString;
			this.verbose = verbose;
		}

		public bool TableExists(string tableName)
		{
			int tableCount = Int32.Parse(ExecuteScalar(string.Format("SELECT COUNT(TABLE_NAME) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='{0}'", tableName)).ToString());
			return (tableCount > 0);
		}

		public bool ProcedureExists(string procedureName)
		{
			int procedureCount = Int32.Parse(ExecuteScalar(string.Format("SELECT COUNT(ROUTINE_NAME) FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME='{0}'", procedureName)).ToString());
			return (procedureCount > 0);
		}

		#endregion

		#region IDataFresh Members

		/// <summary>
		/// prepare the database to use the dataFresh library
		/// </summary>
		public void PrepareDatabaseforDataFresh()
		{
			PrepareDatabaseforDataFresh(true);
		}

		public void PrepareDatabaseforDataFresh(bool createSnapshot)
		{
			DateTime before = DateTime.Now;
			ConsoleWrite("PrepareDatabaseforDataFresh Started");
			RunSqlScript(ResourceManagement.GetDecryptedResourceStream(PrepareScriptResourceName));

			ExecuteNonQuery("exec " + PrepareProcedureName);

			if(createSnapshot)
			{
				CreateSnapshot();
			}
			ConsoleWrite("PrepareDatabaseforDataFresh Complete : " + (DateTime.Now - before));
		}

		/// <summary>
		/// remove the dataFresh objects from a database
		/// </summary>
		public void RemoveDataFreshFromDatabase()
		{
			DateTime before = DateTime.Now;
			ConsoleWrite("RemoveDataFreshFromDatabase Started");
			RunSqlScript(ResourceManagement.GetDecryptedResourceStream(RemoveScriptResourceName));
			ConsoleWrite("RemoveDataFreshFromDatabase Complete : " + (DateTime.Now - before));
		}

		/// <summary>
		/// refresh the database to a known state
		/// </summary>
		public void RefreshTheDatabase()
		{
			DateTime before = DateTime.Now;
			ConsoleWrite("RefreshTheDatabase Started");
			if (!ProcedureExists(RefreshProcedureName))
			{
				throw new SqlDataFreshException("DataFresh procedure not found. Please prepare the database.");
			}
			ExecuteNonQuery(string.Format("exec {0} '{1}'", RefreshProcedureName, SnapshotPath.FullName));
			ConsoleWrite("RefreshTheDatabase Complete : " + (DateTime.Now - before));
		}

		/// <summary>
		/// refresh the database ignoring the dataFresh change tracking table.
		/// </summary>
		public void RefreshTheEntireDatabase()
		{
			DateTime before = DateTime.Now;
			ConsoleWrite("RefreshTheEntireDatabase Started");
			if (!ProcedureExists(ImportProcedureName))
			{
				throw new SqlDataFreshException("DataFresh procedure not found. Please prepare the database.");
			}
			ExecuteNonQuery(string.Format("exec {0} '{1}'", ImportProcedureName, SnapshotPath.FullName));
			ConsoleWrite("RefreshTheEntireDatabase Complete : " + (DateTime.Now - before));
		}

		/// <summary>
		/// create snapshot of database
		/// </summary>
		public void CreateSnapshot()
		{
			DateTime before = DateTime.Now;
			ConsoleWrite("CreateSnapshot Started");
			if (!ProcedureExists(ExtractProcedureName))
			{
				throw new SqlDataFreshException("DataFresh procedure not found. Please prepare the database.");
			}
			ExecuteNonQuery(string.Format("exec {0} '{1}'", ExtractProcedureName, SnapshotPath.FullName));
			ConsoleWrite("CreateSnapshot Complete : " + (DateTime.Now - before));
		}

		/// <summary>
		/// determine if the database has been modified
		/// </summary>
		/// <returns>true if modified.</returns>
		public bool HasDatabaseBeenModified()
		{
			if (!TableExists(ChangeTrackingTableName))
			{
				throw new SqlDataFreshException("DataFresh procedure not found. Please prepare the database.");
			}

			string sql = string.Format(@"SELECT COUNT(*) FROM {0} WHERE TableName <> '{0}'", ChangeTrackingTableName);
			int ret = Convert.ToInt32(ExecuteScalar(sql));
			return ret > 0;
		}

		/// <summary>
		/// location on the server where the snapshot files are located
		/// </summary>
		public DirectoryInfo SnapshotPath
		{
			get
			{
				if (snapshotPath == null)
				{
					return GetSnapshopPath();
				}
				return snapshotPath;
			}
			set
			{
				snapshotPath = CheckForIllegalCharsAndAppendTrailingSlash(value);
			}
		}

		#endregion

		#region Private Methods

		private static DirectoryInfo CheckForIllegalCharsAndAppendTrailingSlash(DirectoryInfo value)
		{
			if(value == null)
			{
				return null;
			}
			string path = value.FullName;
			path = path.Replace("\"", "");
			if(!path.EndsWith(@"\"))
			{
				path += @"\";
			}
			return new DirectoryInfo(path);
		}	
		
		private object ExecuteScalar(string sql)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				sql = sql + " --dataProfilerIgnore";
				SqlCommand cmd = new SqlCommand(sql, conn);
				cmd.CommandTimeout = 1200;
				conn.Open();
				return cmd.ExecuteScalar();
			}
		}

		private void ExecuteNonQuery(string sql)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				sql = sql + " --dataProfilerIgnore";
				SqlCommand cmd = new SqlCommand(sql, conn);
				cmd.CommandTimeout = 1200;
				conn.Open();
				cmd.ExecuteNonQuery();
			}
		}

		private DirectoryInfo GetSnapshopPath()
		{
			string dbName = (string) ExecuteScalar("SELECT DB_Name()");
			string mdfFilePath = Path.GetDirectoryName(ExecuteScalar("select filename from sysfiles where filename like '%.MDF%'").ToString().Trim());
			return new DirectoryInfo(string.Format(@"{0}\Snapshot_{1}\", mdfFilePath, dbName));
		}

		private void RunSqlScript(StreamReader reader)
		{
			string line = "";
			StringBuilder cmd = new StringBuilder();
			while ((line = reader.ReadLine()) != null)
			{
				if (line.Trim().ToLower().Equals("go"))
				{
					ExecuteNonQuery(cmd.ToString());
					cmd.Length = 0;
				}
				else
				{
					cmd.Append(line);
					cmd.Append(Environment.NewLine);
				}
			}
			if (cmd.ToString().Trim().Length > 0)
			{
				ExecuteNonQuery(cmd.ToString());
			}
		}

		private void ConsoleWrite(string message)
		{
			if(this.verbose)
			{
				Console.Out.WriteLine(message);
			}
		}

		#endregion
	}
}