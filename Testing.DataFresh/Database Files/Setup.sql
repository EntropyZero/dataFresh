if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[df_Setup]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[df_Setup]
GO

CREATE PROCEDURE [dbo].[df_Setup]
AS
	DELETE BOOK
	DELETE AUTHOR

	PRINT CAST(CURRENT_TIMESTAMP AS VARCHAR(30)) + ': Inserting Author'
	SET IDENTITY_INSERT [dbo].[Author] ON
	INSERT INTO [Author]
		(
		[AuthorId]
		, [Lastname] 
		, [Firstname]
		)
		SELECT 1, 'Brockey', 'Michael' UNION
		SELECT 2, 'Buxton', 'Stevem' UNION
		SELECT 3, 'Jones', 'Happy' UNION
		SELECT 4, 'Smith', 'john' UNION
		SELECT 5, 'Johnson', 'Todd'
	SET IDENTITY_INSERT [dbo].[Author] OFF

	PRINT CAST(CURRENT_TIMESTAMP AS VARCHAR(30)) + ': Inserting Book'
	SET IDENTITY_INSERT [dbo].[Book] ON
	INSERT INTO [Book]
		(
		[BookId]
		, [AuthorId]
		, [Title]
		)
		SELECT 1, 1, 'My First Book' UNION
		SELECT 2, 1, 'My Second Book' UNION
		SELECT 3, 2, 'My First Book' UNION
		SELECT 4, 2, 'My Second Book' UNION
		SELECT 5, 3, 'My First Book' UNION
		SELECT 6, 4, 'My First Book' UNION
		SELECT 7, 5, 'My First Book' 
	SET IDENTITY_INSERT [dbo].[Book] OFF
GO

EXEC df_SETUP
go
