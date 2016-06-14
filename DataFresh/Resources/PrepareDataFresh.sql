IF EXISTS (SELECT * FROM [DBO].SYSOBJECTS WHERE ID = Object_ID(N'[DBO].[df_ChangedTableDataRefresh]') AND OBJECTPROPERTY(ID, N'IsProcedure') = 1)
     DROP PROCEDURE [dbo].[df_ChangedTableDataRefresh]
GO 

IF EXISTS (SELECT * FROM [DBO].SYSOBJECTS WHERE ID = Object_ID(N'[DBO].[df_ChangeTrackingTriggerCreate]') AND OBJECTPROPERTY(ID, N'IsProcedure') = 1)
     DROP PROCEDURE [dbo].[df_ChangeTrackingTriggerCreate]
GO

IF EXISTS (SELECT * FROM [DBO].SYSOBJECTS WHERE ID = Object_ID(N'[DBO].[df_ChangeTrackingTriggerRemove]') AND OBJECTPROPERTY(ID, N'IsProcedure') = 1)
     DROP PROCEDURE [dbo].[df_ChangeTrackingTriggerRemove]
GO

IF EXISTS (SELECT * FROM [DBO].SYSOBJECTS WHERE ID = Object_ID(N'[DBO].[df_TableDataExtract]') AND OBJECTPROPERTY(ID, N'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[df_TableDataExtract]
GO 

IF EXISTS (SELECT * FROM [DBO].SYSOBJECTS WHERE ID = Object_ID(N'[DBO].[df_TableDataImport]') AND OBJECTPROPERTY(ID, N'IsProcedure') = 1)
     DROP PROCEDURE [dbo].[df_TableDataImport]
GO 

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[df_ChangeTracking]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[df_ChangeTracking]
GO

CREATE TABLE [dbo].[df_ChangeTracking]
	(
		[TABLENAME] sysname,
		[TABLESCHEMA] sysname
	)
GO
	
CREATE PROCEDURE dbo.[df_ChangedTableDataRefresh]
(
	@BasePath NVARCHAR(512)
)
AS

	DECLARE @sql NVARCHAR(4000)
	DECLARE @TableName VARCHAR(255)
	DECLARE @TableSchema VARCHAR(255)

	SELECT DISTINCT TableName, TableSchema INTO #ChangedTables FROM df_ChangeTracking

	TRUNCATE TABLE df_ChangeTracking

	DECLARE Table_Cursor INSENSITIVE SCROLL CURSOR FOR
		SELECT [tablename], [tableschema] from #ChangedTables
		UNION
		SELECT DISTINCT 
				OBJECT_NAME(fkeyid) AS Referenced_Table_Name,
				OBJECT_SCHEMA_NAME(fkeyid) AS Referenced_Table_Schema
		FROM 
			sysreferences sr
			INNER JOIN #ChangedTables ct ON sr.rkeyid = OBJECT_ID(ct.tablename)

	OPEN Table_Cursor

	-- Deactivate Constrains for tables referencing changed tables
	FETCH NEXT FROM Table_Cursor INTO @TableName, @TableSchema

	WHILE (@@Fetch_Status = 0)
	BEGIN
			SET @sql = N'Alter Table [' + @TableSchema + '].[' + @TableName + '] NOCHECK CONSTRAINT ALL'
			EXEC sp_executesql @sql

			FETCH NEXT FROM Table_Cursor INTO @TableName, @TableSchema
	END

	-- Delete All data from Changed Tables and Refill
	DECLARE ChangedTable_Cursor CURSOR FOR
		SELECT [tablename], [tableschema] FROM #ChangedTables WHERE tablename not in('df_ChangeTracking', 'dr_DeltaVersion')

	OPEN ChangedTable_Cursor
	FETCH NEXT FROM ChangedTable_Cursor INTO @TableName, @TableSchema
	WHILE (@@Fetch_Status = 0)
	BEGIN
			PRINT @TableName
			SET @sql = N'DELETE [' + @TableSchema + '].[' + @TableName + ']; DELETE FROM df_ChangeTracking WHERE TableName=''' + @TableName + ''' and TableSchema=''' + @TableSchema + ''''
			EXEC sp_executesql @sql
			
			SET @sql = N'IF(SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_schema = ''' + @TableSchema + ''' AND table_name = ''' + @TableName + ''' AND IDENT_SEED(TABLE_NAME) IS NOT NULL) > 0
			BEGIN				
				DBCC CHECKIDENT ([' + @TableSchema + '.' + @TableName + '], RESEED, 0)
			END'

			EXEC sp_executesql @sql
	

			SET @sql = N'BULK INSERT [' + @TableSchema + '].[' + @TableName + ']
				FROM ''' + @BasePath + @TableSchema + '.' + @TableName + '.df''
   				WITH 
					(
						KEEPIDENTITY,
						KEEPNULLS,
						DATAFILETYPE=''native''
					)'
			EXEC sp_executesql @sql

			FETCH NEXT FROM ChangedTable_Cursor INTO @TableName, @TableSchema
	END
	CLOSE ChangedTable_Cursor
	DEALLOCATE ChangedTable_Cursor

	-- ReEnable Constrants for All Tables
	FETCH FIRST FROM Table_Cursor INTO @TableName, @TableSchema
	WHILE (@@Fetch_Status = 0)
	BEGIN
			SET @sql = N'Alter Table [' + @TableSchema + '].[' + @TableName + '] CHECK CONSTRAINT ALL'
			EXEC sp_executesql @sql

			FETCH NEXT FROM Table_Cursor INTO @TableName, @TableSchema
	END
	CLOSE Table_Cursor
	DEALLOCATE Table_Cursor
GO


CREATE PROCEDURE dbo.[df_ChangeTrackingTriggerCreate]
AS

	IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[df_ChangeTracking]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [df_ChangeTracking]
	(
		[TABLENAME] sysname,
		[TABLESCHEMA] sysname
	)

	DECLARE @sql NVARCHAR(4000)
	DECLARE @TableName VARCHAR(255)
	DECLARE @TableSchema VARCHAR(255)

	DECLARE Table_Cursor CURSOR FOR
		SELECT [table_name], [table_schema] FROM information_schema.tables WHERE table_type = 'BASE TABLE' 

	OPEN Table_Cursor
	FETCH NEXT FROM Table_Cursor INTO @TableName, @TableSchema

	WHILE (@@Fetch_Status = 0)
	BEGIN
			SET @sql = N'IF EXISTS (SELECT * FROM dbo.SYSOBJECTS WHERE ID = Object_ID(N''[' + @TableSchema + '].[trig_df_ChangeTracking_' + @TableName + ']'') AND OBJECTPROPERTY(ID, N''IsTrigger'') = 1) 
				DROP TRIGGER [' + @TableSchema + '].[trig_df_ChangeTracking_' + @TableName + ']'
			EXEC sp_executesql @sql

			SET @sql = N'CREATE TRIGGER [' + @TableSchema + '].[trig_df_ChangeTracking_' + @TableName + '] on [' + @TableSchema + '].[' + @TableName + '] for insert, update, delete
			as
			SET NOCOUNT ON
			INSERT INTO df_ChangeTracking (tablename, tableschema) VALUES (''' + @TableName + ''', ''' + @TableSchema + ''')
			SET NOCOUNT OFF' 
			
			EXEC sp_executesql @sql

			FETCH NEXT FROM Table_Cursor INTO @TableName, @TableSchema

	END
	CLOSE Table_Cursor
	DEALLOCATE Table_Cursor

GO

CREATE PROCEDURE dbo.[df_TableDataExtract]
(
	@BasePath NVARCHAR(512)
)
AS

	DECLARE @MkDirCmd NVARCHAR(4000)
	
	SET @MkDirCmd = N'MKDIR "' + @BASEPATH + '"'
		EXEC master.dbo.xp_cmdshell @MkDirCmd, no_output
	
	DECLARE @CMD NVARCHAR(4000)
	
	DECLARE Table_Cursor CURSOR FOR
		SELECT N'bcp "' + DB_NAME() + '.[' + Table_Schema + '].[' + Table_Name + ']" out "' + @BasePath + Table_Schema + '.' + Table_Name + '.df" -n -k -E -C 1252 -S ' + @@ServerName + ' -T' FROM Information_Schema.tables WHERE table_type = 'BASE TABLE'

	OPEN Table_Cursor
	FETCH NEXT FROM Table_Cursor INTO @CMD

	WHILE (@@Fetch_Status = 0)
	BEGIN
		EXEC master.dbo.xp_cmdshell @CMD, no_output
		FETCH NEXT FROM Table_Cursor INTO @CMD
	END

	CLOSE Table_Cursor
	Deallocate Table_Cursor
	
GO

CREATE PROCEDURE dbo.[df_TableDataImport]
(
	@BasePath NVARCHAR(512)
)
AS

	DECLARE @sql NVARCHAR(4000)
	DECLARE @TableName VARCHAR(255)
	DECLARE @TableSchema VARCHAR(255)

	SELECT Table_Name as TableName, Table_Schema as TableSchema INTO #UserTables FROM Information_Schema.tables WHERE table_type = 'BASE TABLE'

	DECLARE Table_Cursor INSENSITIVE SCROLL CURSOR FOR
		SELECT [tablename], [tableschema] FROM #UserTables

	OPEN Table_Cursor

	-- Deactivate Constrains for tables referencing changed tables
	FETCH NEXT FROM Table_Cursor INTO @TableName, @TableSchema

	WHILE (@@Fetch_Status = 0)
	BEGIN
			SET @sql = N'Alter Table [' + @TableSchema + '].[' + @TableName + '] NOCHECK CONSTRAINT ALL'
			EXEC sp_executesql @sql

			FETCH NEXT FROM Table_Cursor INTO @TableName, @TableSchema
	END

	-- Delete All data from Changed Tables and Refill
	DECLARE UserTable_Cursor CURSOR FOR
		SELECT [tablename], [tableschema] FROM #UserTables WHERE tablename not in ('df_ChangeTracking', 'dr_DeltaVersion') and tableschema <> 'dbo'

	OPEN UserTable_Cursor

	FETCH NEXT FROM UserTable_Cursor INTO @TableName, @TableSchema
	WHILE (@@Fetch_Status = 0)
	BEGIN
			PRINT @TableSchema + '.' + @TableName
			SET @sql = N'DELETE [' + @TableSchema + '].[' + @TableName + ']'
			EXEC sp_executesql @sql

			SET @sql = N'BULK INSERT [' + @TableSchema + '].[' + @TableName + ']
				FROM ''' + @BasePath + @TableName + '.df''
   				WITH 
					(
						KEEPIDENTITY,
						KEEPNULLS,
						DATAFILETYPE=''native''
					)'
			EXEC sp_executesql @sql

			FETCH NEXT FROM UserTable_Cursor INTO @TableName, @TableSchema

	END
	CLOSE UserTable_Cursor
	DEALLOCATE UserTable_Cursor

	-- ReEnable Constrants for All Tables
	FETCH FIRST FROM Table_Cursor INTO @TableName, @TableSchema
	WHILE (@@Fetch_Status = 0)
	BEGIN
			SET @sql = N'Alter Table [' + @TableSchema + '].[' + @TableName + '] CHECK CONSTRAINT ALL'
			EXEC sp_executesql @sql
			
			FETCH NEXT FROM Table_Cursor INTO @TableName, @TableSchema
	END

	CLOSE Table_Cursor
	DEALLOCATE Table_Cursor

GO

CREATE PROCEDURE dbo.[df_ChangeTrackingTriggerRemove]
AS
	DECLARE @sql NVARCHAR(4000)
	DECLARE @TableName VARCHAR(255)
	DECLARE @TableSchema VARCHAR(255)

	DECLARE Table_Cursor CURSOR FOR
		SELECT [table_name], [table_schema] FROM information_schema.tables WHERE table_type = 'BASE TABLE'

	OPEN Table_Cursor
	FETCH NEXT FROM Table_Cursor INTO @TableName, @TableSchema

	WHILE (@@Fetch_Status = 0)
	BEGIN
			SET @sql = N'IF EXISTS (SELECT * FROM DBO.SYSOBJECTS WHERE ID = Object_ID(N''[' + @TableSchema + '].[trig_df_ChangeTracking_' + @TableName + ']'') AND OBJECTPROPERTY(ID, N''IsTrigger'') = 1) 
				DROP TRIGGER [' + @TableSchema + '].[trig_df_ChangeTracking_' + @TableName + ']' 
			
			EXEC sp_executesql @sql

			FETCH NEXT FROM Table_Cursor INTO @TableName, @TableSchema

	END
	CLOSE Table_Cursor
	DEALLOCATE Table_Cursor
	
	IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[df_ChangeTracking]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
		DROP TABLE [dbo].[df_ChangeTracking]

GO