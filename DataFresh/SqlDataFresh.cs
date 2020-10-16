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
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
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
		readonly string connectionString;
		public const string ChangeTrackingTableName = "df_ChangeTracking";
		const string SnapshotTableSuffix = "__backup";

		sealed class TableMetadata
		{
			public string Schema { get; set; }
			public string Name { get; set; }
		}

		readonly bool verbose;
		readonly string databaseName;

		public SqlDataFresh(string connectionString)
		{
			this.connectionString = connectionString;
			var connectionBuilder = new SqlConnectionStringBuilder(connectionString);
			databaseName = connectionBuilder.InitialCatalog;
		}

		public SqlDataFresh(string connectionString, bool verbose)
			: this(connectionString)
		{
			this.verbose = verbose;
		}

		public void PrepareDatabaseForDataFresh()
		{
			PrepareDatabaseForDataFresh(true);
		}

		public void PrepareDatabaseForDataFresh(bool createSnapshot)
		{
			var mode = createSnapshot ? "(with snapshot creation)" : string.Empty;
			Execute($"Prepare database{mode}", () =>
			{
				PrepareDataFresh();

				if (createSnapshot)
					CreateSnapshot();
			});
		}

		public void CreateSnapshot()
		{
			GuardDatabaseIsPrepared();
			Execute("Create snapshot", () =>
			{
				var allTables = GetAllTables();
				PerformBulkBackup(allTables);
			});
		}

		void PrepareDataFresh()
		{
			Execute("Prepare DataFresh", () =>
			{
				var allTables = GetAllTables();

				ExecuteNonQuery(@"
					IF OBJECT_ID(N'[dbo].[df_ChangeTracking]', N'U') IS NOT NULL
					BEGIN
						DROP TABLE [dbo].[df_ChangeTracking]
					END;

					CREATE TABLE [dbo].[df_ChangeTracking] ([TableSchema] SYSNAME, [TableName] SYSNAME)"
				);

				ExecuteForEach(allTables, t => $@"
					IF OBJECT_ID(N'[{t.Schema}].[trig_df_ChangeTracking_{t.Name}]') IS NOT NULL
					BEGIN
						DROP TRIGGER [{t.Schema}].[trig_df_ChangeTracking_{t.Name}]
					END;
					
					EXEC (N'CREATE TRIGGER [{t.Schema}].[trig_df_ChangeTracking_{t.Name}] ON [{t.Schema}].[{t.Name}]
					FOR INSERT, UPDATE, DELETE
					AS
					SET NOCOUNT ON
					INSERT INTO df_ChangeTracking (TableSchema, TableName) VALUES (''{t.Schema}'', ''{t.Name}'')
					SET NOCOUNT OFF');"
				);
			});
		}

		public void RemoveDataFreshFromDatabase()
		{
			Execute("Remove DataFresh", () =>
			{
				var allTables = GetAllTables();

				ExecuteForEach(allTables, t => $@"
					IF (OBJECT_ID(N'[{t.Schema}].[trig_df_ChangeTracking_{t.Name}]') IS NOT NULL)
					BEGIN
						DROP TRIGGER [{t.Schema}].[trig_df_ChangeTracking_{t.Name}]
					END;

					IF OBJECT_ID (N'[{t.Schema}].[{t.Name}{SnapshotTableSuffix}]', N'U') IS NOT NULL
					BEGIN
						DROP TABLE [{t.Schema}].[{t.Name}{SnapshotTableSuffix}];
					END;"
				);

				ExecuteNonQuery(@"
					IF OBJECT_ID (N'[dbo].[df_ChangeTracking]', N'U') IS NOT NULL
						DROP TABLE [dbo].[df_ChangeTracking]"
				);
			});
		}

		public void RefreshTheDatabase()
		{
			GuardDatabaseIsPrepared();
			Execute("Refresh", () =>
			{
				var changedAndReferencedTables = GetChangedAndReferencedTables();
				var changedTables = GetChangedTables();

				ExecuteNonQuery($"TRUNCATE TABLE [dbo].[{ChangeTrackingTableName}]");

				ExecuteForEach(changedAndReferencedTables, t =>
					$"ALTER TABLE [{t.Schema}].[{t.Name}] NOCHECK CONSTRAINT ALL;"
				);

				ExecuteForEach(changedTables, t => $@"
					DELETE [{t.Schema}].[{t.Name}];
					DELETE FROM df_ChangeTracking WHERE TableName='{t.Name}' and TableSchema='{t.Schema}';

					IF (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES
						WHERE table_schema = '{t.Schema}' AND table_name = '{t.Name}'
						AND IDENT_SEED(TABLE_NAME) IS NOT NULL) > 0
					BEGIN
						DBCC CHECKIDENT([{t.Schema}.{t.Name}], RESEED, 0)
					END"
				);

				PerformBulkRestore(changedTables);

				ExecuteForEach(changedAndReferencedTables, t =>
					$"ALTER TABLE [{t.Schema}].[{t.Name}] CHECK CONSTRAINT ALL;"
				);
			});
		}

		public void RefreshTheEntireDatabase()
		{
			GuardDatabaseIsPrepared();
			Execute("Refresh (entire)", () =>
			{
				var allTables = GetAllTables();
				var changedTables = GetChangedTables();
				ExecuteNonQuery($"TRUNCATE TABLE [dbo].[{ChangeTrackingTableName}]");

				ExecuteForEach(allTables, t =>
					$"ALTER TABLE [{t.Schema}].[{t.Name}] NOCHECK CONSTRAINT ALL;"
				);

				ExecuteForEach(changedTables, t =>
					$"DELETE FROM [{t.Schema}].[{t.Name}]"
				);

				PerformBulkRestore(changedTables);

				ExecuteForEach(allTables, t =>
					$"ALTER TABLE [{t.Schema}].[{t.Name}] CHECK CONSTRAINT ALL;"
				);
			});
		}

		public bool HasDatabaseBeenModified()
		{
			GuardDatabaseIsPrepared();
			var sql =
				$@"SELECT COUNT(*) FROM {ChangeTrackingTableName}
				WHERE TableName <> 'ChangeTrackingTableName' 
				AND TableName NOT LIKE '%{SnapshotTableSuffix}'";
			var ret = Convert.ToInt32(ExecuteScalar(sql));
			return ret > 0;
		}

		public void RunStatements(IEnumerable<string> statements)
		{
			var cb = new StringBuilder();
			const int maxLength = 6000;
			var currentLength = 0;
			foreach (var statement in statements)
			{
				if (currentLength + statement.Length > maxLength)
					Flush();

				currentLength += statement.Length;
				cb.Append(statement);
			}

			if (currentLength > 0)
				Flush();

			void Flush()
			{
				ExecuteNonQuery(cb.ToString());
				cb.Clear();
				currentLength = 0;
			}
		}

		void PerformBulkBackup(IReadOnlyCollection<TableMetadata> tables)
		{
			PerformBulkOperation(tables, backup: true);
		}

		void PerformBulkRestore(IReadOnlyCollection<TableMetadata> tables)
		{
			PerformBulkOperation(tables, backup: false);
		}

		void PerformBulkOperation(IReadOnlyCollection<TableMetadata> tables, bool backup)
		{
			var destinationSuffix = backup ? SnapshotTableSuffix : string.Empty;
			var sourceSuffix = backup ? string.Empty : SnapshotTableSuffix;

			if (backup)
			{
				ExecuteForEach(tables, t =>
				{
					var sourceTable = $"[{t.Schema}].[{t.Name}{sourceSuffix}]";
					var destinationTable = $"[{t.Schema}].[{t.Name}{destinationSuffix}]";

					return $@"
						IF OBJECT_ID (N'{destinationTable}', N'U') IS NULL BEGIN
							SELECT * INTO {destinationTable} FROM {sourceTable} WHERE 1=2 END 
						ELSE BEGIN
							TRUNCATE TABLE {destinationTable}
						END;";
				});
			}

			foreach (var table in tables)
			{
				var sourceTable = $"[{table.Schema}].[{table.Name}{sourceSuffix}]";
				var destinationTable = $"[{table.Schema}].[{table.Name}{destinationSuffix}]";

				CopyTable(connectionString, sourceTable, destinationTable);
			}
		}

		static void CopyTable(string dbConnectionString, string sourceTable, string destinationTable)
		{
			using (var sourceConnection = new SqlConnection(dbConnectionString))
			{
				sourceConnection.Open();
				var commandSourceData = new SqlCommand($"SELECT * FROM {sourceTable};", sourceConnection);
				var reader = commandSourceData.ExecuteReader();

				using (var destinationConnection = new SqlConnection(dbConnectionString))
				{
					destinationConnection.Open();

					using (var bulkCopy = new SqlBulkCopy(destinationConnection))
					{
						bulkCopy.DestinationTableName = destinationTable;
						try
						{
							bulkCopy.WriteToServer(reader);
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
						finally
						{
							reader.Close();
						}
					}
				}
			}
		}

		object ExecuteScalar(string sql)
		{
			using (var conn = new SqlConnection(connectionString))
			{
				sql += " --dataProfilerIgnore";
				var cmd = new SqlCommand(sql, conn) { CommandTimeout = 1200 };
				conn.Open();
				return cmd.ExecuteScalar();
			}
		}

		void ExecuteNonQuery(string sql)
		{
			using (var conn = new SqlConnection(connectionString))
			{
				conn.Open();
				sql += " --dataProfilerIgnore";
				using (var cmd = new SqlCommand(sql, conn) { CommandTimeout = 1200 })
					cmd.ExecuteNonQuery();
			}
		}

		void ConsoleWrite(string message)
		{
			if (verbose)
				Console.Out.WriteLine(message);
		}

		void Execute(string actionName, Action action)
		{
			var stopWatch = new Stopwatch();
			ConsoleWrite($"{actionName} for {databaseName} started");
			stopWatch.Start();
			try
			{
				action();
			}
			finally
			{
				stopWatch.Stop();
				ConsoleWrite($"{actionName} for {databaseName} complete: {stopWatch.Elapsed}");
			}
		}

		void ExecuteForEach(IEnumerable<TableMetadata> tables, Func<TableMetadata, string> func)
		{
			RunStatements(tables.Select(func));
		}

		IReadOnlyCollection<TableMetadata> SelectTables(string sql)
		{
			var tables = new List<TableMetadata>();
			using (var conn = new SqlConnection(connectionString))
			{
				conn.Open();
				using (var cmd = new SqlCommand(sql, conn))
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						tables.Add(new TableMetadata
						{
							Schema = reader.GetString(0),
							Name = reader.GetString(1)
						});
					}
				}
			}
			return tables;
		}

		void GuardDatabaseIsPrepared()
		{
			if (!TableExists(ChangeTrackingTableName))
				throw new SqlDataFreshException(
					$"DataFresh table ({ChangeTrackingTableName}) not found. Please prepare the database.");
		}

		IReadOnlyCollection<TableMetadata> GetAllTables()
		{
			var sql =
				$@"SELECT table_schema, table_name
				FROM Information_Schema.tables
				WHERE table_type = 'BASE TABLE'
					AND table_name NOT IN ('df_ChangeTracking', 'dr_DeltaVersion')
					AND table_name NOT LIKE '%{SnapshotTableSuffix}'";

			var tables = SelectTables(sql);
			return tables;
		}

		IReadOnlyCollection<TableMetadata> GetChangedTables()
		{
			var sql =
				$@"SELECT DISTINCT TableSchema, TableName
				FROM df_ChangeTracking
				WHERE TableName NOT IN ('df_ChangeTracking', 'dr_DeltaVersion')
				AND TableName NOT LIKE '%{SnapshotTableSuffix}'";

			var tables = SelectTables(sql);
			return tables;
		}

		IReadOnlyCollection<TableMetadata> GetChangedAndReferencedTables()
		{
			var sql = $@"
				SELECT DISTINCT x.TableSchema, x.TableName
				FROM (
					SELECT DISTINCT TableSchema, TableName
					FROM df_ChangeTracking 
					UNION
					SELECT DISTINCT
						OBJECT_SCHEMA_NAME(fkeyid) AS TableSchema,
						OBJECT_NAME(fkeyid) AS TableName
					FROM sysreferences sr 
					INNER JOIN df_ChangeTracking ct ON sr.rkeyid = OBJECT_ID(ct.TableName)
				) x
				WHERE x.TableName NOT IN ('df_ChangeTracking', 'dr_DeltaVersion')
					AND x.TableName NOT LIKE '%{SnapshotTableSuffix}'
			";

			var tables = SelectTables(sql);
			return tables;
		}

		public bool TableExists(string tableName)
		{
			var tableCount = int.Parse(ExecuteScalar(
				$"SELECT COUNT(TABLE_NAME) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='{tableName}'").ToString());
			return (tableCount > 0);
		}
	}
}