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

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[df_ChangeTracking]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [dbo].[df_ChangeTracking]
	(
		[TABLENAME] sysname
	)
GO
	
CREATE PROCEDURE dbo.[df_ChangedTableDataRefresh]
(
	@BasePath NVARCHAR(512)
)
AS

	DECLARE @sql NVARCHAR(4000)
	DECLARE @TableName VARCHAR(255)

	SELECT DISTINCT TableName INTO #ChangedTables FROM df_ChangeTracking

	TRUNCATE TABLE df_ChangeTracking

	DECLARE Table_Cursor INSENSITIVE SCROLL CURSOR FOR
		SELECT [tablename] from #ChangedTables
		UNION
		SELECT DISTINCT 
			OBJECT_NAME(fkeyid) AS Referenced_Table
		FROM 
			sysreferences sr
			INNER JOIN #ChangedTables ct ON sr.rkeyid = OBJECT_ID(ct.tablename)

	OPEN Table_Cursor

	-- Deactivate Constrains for tables referencing changed tables
	FETCH NEXT FROM Table_Cursor INTO @TableName

	WHILE (@@Fetch_Status = 0)
	BEGIN
			SET @sql = N'Alter Table [' + @TableName + '] NOCHECK CONSTRAINT ALL'
			EXEC sp_executesql @sql

			FETCH NEXT FROM Table_Cursor INTO @TableName
	END

	-- Delete All data from Changed Tables and Refill
	DECLARE ChangedTable_Cursor CURSOR FOR
		SELECT [tablename] FROM #ChangedTables WHERE tablename not in('df_ChangeTracking', 'dr_DeltaVersion')

	OPEN ChangedTable_Cursor
	FETCH NEXT FROM ChangedTable_Cursor INTO @TableName
	WHILE (@@Fetch_Status = 0)
	BEGIN
			PRINT @TableName
			SET @sql = N'DELETE [' + @TableName + ']; DELETE df_ChangeTracking WHERE TableName=''' + @TableName + ''''
			EXEC sp_executesql @sql
			
			SET @sql = N'IF(SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_name = ''' + @TableName + ''' AND IDENT_SEED(TABLE_NAME) IS NOT NULL) > 0
			BEGIN				
				DBCC CHECKIDENT ([' + @TableName + '], RESEED, 0)
			END'

			EXEC sp_executesql @sql
	

			SET @sql = N'BULK INSERT [' + @TableName + ']
				FROM ''' + @BasePath + @TableName + '.df''
   				WITH 
					(
						KEEPIDENTITY,
						KEEPNULLS,
						DATAFILETYPE=''native''
					)'
			EXEC sp_executesql @sql

			FETCH NEXT FROM ChangedTable_Cursor INTO @TableName
	END
	CLOSE ChangedTable_Cursor
	DEALLOCATE ChangedTable_Cursor

	-- ReEnable Constrants for All Tables
	FETCH FIRST FROM Table_Cursor INTO @TableName
	WHILE (@@Fetch_Status = 0)
	BEGIN
			SET @sql = N'Alter Table [' + @TableName + '] CHECK CONSTRAINT ALL'
			EXEC sp_executesql @sql

			FETCH NEXT FROM Table_Cursor INTO @TableName
	END
	CLOSE Table_Cursor
	DEALLOCATE Table_Cursor
GO


CREATE PROCEDURE dbo.[df_ChangeTrackingTriggerCreate]
AS

	IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[df_ChangeTracking]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [df_ChangeTracking]
	(
		[TABLENAME] sysname
	)

	DECLARE @sql NVARCHAR(4000)
	DECLARE @TableName VARCHAR(255)

	DECLARE Table_Cursor CURSOR FOR
		SELECT [table_name] FROM information_schema.tables WHERE table_type = 'BASE TABLE' 

	OPEN Table_Cursor
	FETCH NEXT FROM Table_Cursor INTO @TableName

	WHILE (@@Fetch_Status = 0)
	BEGIN
			SET @sql = N'IF EXISTS (SELECT * FROM dbo.SYSOBJECTS WHERE ID = Object_ID(N''[dbo].[trig_df_ChangeTracking_' + @TableName + ']'') AND OBJECTPROPERTY(ID, N''IsTrigger'') = 1) 
				DROP TRIGGER [dbo].[trig_df_ChangeTracking_' + @TableName + ']'
			EXEC sp_executesql @sql

			SET @sql = N'CREATE TRIGGER [dbo].[trig_df_ChangeTracking_' + @TableName + '] on [' + @TableName + '] for insert, update, delete
			as
			SET NOCOUNT ON
			INSERT INTO df_ChangeTracking (tablename) VALUES (''' + @TableName + ''')
			SET NOCOUNT OFF' 
			
			EXEC sp_executesql @sql

			FETCH NEXT FROM Table_Cursor INTO @TableName

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
		SELECT N'bcp "' + DB_NAME() + '.dbo.[' + Table_Name + ']" out "' + @BasePath + Table_Name + '.df" -n -k -E -C 1252 -S ' + @@ServerName + ' -T' FROM Information_Schema.tables WHERE table_type = 'BASE TABLE'

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

	SELECT Table_Name as TableName	INTO #UserTables FROM Information_Schema.tables WHERE table_type = 'BASE TABLE'

	DECLARE Table_Cursor INSENSITIVE SCROLL CURSOR FOR
		SELECT [tablename] FROM #UserTables

	OPEN Table_Cursor

	-- Deactivate Constrains for tables referencing changed tables
	FETCH NEXT FROM Table_Cursor INTO @TableName

	WHILE (@@Fetch_Status = 0)
	BEGIN
			SET @sql = N'Alter Table [' + @TableName + '] NOCHECK CONSTRAINT ALL'
			EXEC sp_executesql @sql

			FETCH NEXT FROM Table_Cursor INTO @TableName
	END

	-- Delete All data from Changed Tables and Refill
	DECLARE UserTable_Cursor CURSOR FOR
		SELECT [tablename] FROM #UserTables WHERE tablename not in ('df_ChangeTracking', 'dr_DeltaVersion')

	OPEN UserTable_Cursor

	FETCH NEXT FROM UserTable_Cursor INTO @TableName
	WHILE (@@Fetch_Status = 0)
	BEGIN
			PRINT @TableName
			SET @sql = N'DELETE [' + @TableName + ']'
			EXEC sp_executesql @sql

			SET @sql = N'BULK INSERT [' + @TableName + ']
				FROM ''' + @BasePath + @TableName + '.df''
   				WITH 
					(
						KEEPIDENTITY,
						KEEPNULLS,
						DATAFILETYPE=''native''
					)'
			EXEC sp_executesql @sql

			FETCH NEXT FROM UserTable_Cursor INTO @TableName

	END
	CLOSE UserTable_Cursor
	DEALLOCATE UserTable_Cursor

	-- ReEnable Constrants for All Tables
	FETCH FIRST FROM Table_Cursor INTO @TableName
	WHILE (@@Fetch_Status = 0)
	BEGIN
			SET @sql = N'Alter Table [' + @TableName + '] CHECK CONSTRAINT ALL'
			EXEC sp_executesql @sql
			
			FETCH NEXT FROM Table_Cursor INTO @TableName
	END

	CLOSE Table_Cursor
	DEALLOCATE Table_Cursor

GO

CREATE PROCEDURE dbo.[df_ChangeTrackingTriggerRemove]
AS
	DECLARE @sql NVARCHAR(4000)
	DECLARE @TableName VARCHAR(255)

	DECLARE Table_Cursor CURSOR FOR
		SELECT [table_name] FROM information_schema.tables WHERE table_type = 'BASE TABLE'

	OPEN Table_Cursor
	FETCH NEXT FROM Table_Cursor INTO @TableName

	WHILE (@@Fetch_Status = 0)
	BEGIN
			SET @sql = N'IF EXISTS (SELECT * FROM DBO.SYSOBJECTS WHERE ID = Object_ID(N''[dbo].[trig_df_ChangeTracking_' + @TableName + ']'') AND OBJECTPROPERTY(ID, N''IsTrigger'') = 1) 
				DROP TRIGGER [dbo].[trig_df_ChangeTracking_' + @TableName + ']' 
			
			EXEC sp_executesql @sql

			FETCH NEXT FROM Table_Cursor INTO @TableName

	END
	CLOSE Table_Cursor
	DEALLOCATE Table_Cursor
	
	IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[df_ChangeTracking]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
		DROP TABLE [dbo].[df_ChangeTracking]

GO