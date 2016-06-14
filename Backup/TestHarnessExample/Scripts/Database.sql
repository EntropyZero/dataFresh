if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Book]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Book]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Author]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Author]
GO

CREATE TABLE [Author]
(
	[AuthorId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY
	, [Lastname] VARCHAR(55) NOT NULL
	, [Firstname] VARCHAR(25) NOT NULL
)
GO

CREATE TABLE [Book]
(
	[BookId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY
	, [AuthorId] INT NOT NULL CONSTRAINT fk_Book_Author_AuthorId FOREIGN KEY ([AuthorId]) REFERENCES [Author]([AuthorId])
	, [Title] VARCHAR(55) NOT NULL
)
GO

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
	SELECT 5, 'Johnson', 'Todd' UNION
	SELECT 6, 'Brockey', 'Michael' UNION
	SELECT 7, 'Buxton', 'Stevem' UNION
	SELECT 8, 'Jones', 'Happy' UNION
	SELECT 9, 'Smith', 'john' UNION
	SELECT 10, 'Johnson', 'Todd' UNION
	SELECT 11, 'Brockey', 'Michael' UNION
	SELECT 12, 'Buxton', 'Stevem' UNION
	SELECT 13, 'Jones', 'Happy' UNION
	SELECT 14, 'Smith', 'john' UNION
	SELECT 15, 'Johnson', 'Todd' UNION
	SELECT 16, 'Brockey', 'Michael' UNION
	SELECT 17, 'Buxton', 'Stevem' UNION
	SELECT 18, 'Jones', 'Happy' UNION
	SELECT 19, 'Smith', 'john' UNION
	SELECT 20, 'Johnson', 'Todd' UNION
	SELECT 21, 'Brockey', 'Michael' UNION
	SELECT 22, 'Buxton', 'Stevem' UNION
	SELECT 23, 'Jones', 'Happy' UNION
	SELECT 24, 'Smith', 'john' UNION
	SELECT 25, 'Johnson', 'Todd' UNION
	SELECT 26, 'Brockey', 'Michael' UNION
	SELECT 27, 'Buxton', 'Stevem' UNION
	SELECT 28, 'Jones', 'Happy' UNION
	SELECT 29, 'Smith', 'john' UNION
	SELECT 30, 'Johnson', 'Todd' UNION
	SELECT 31, 'Brockey', 'Michael' UNION
	SELECT 32, 'Buxton', 'Stevem' UNION
	SELECT 33, 'Jones', 'Happy' UNION
	SELECT 34, 'Smith', 'john' UNION
	SELECT 35, 'Johnson', 'Todd' UNION
	SELECT 36, 'Brockey', 'Michael' UNION
	SELECT 37, 'Buxton', 'Stevem' UNION
	SELECT 38, 'Jones', 'Happy' UNION
	SELECT 39, 'Smith', 'john' UNION
	SELECT 40, 'Johnson', 'Todd' UNION
	SELECT 41, 'Brockey', 'Michael' UNION
	SELECT 42, 'Buxton', 'Stevem' UNION
	SELECT 43, 'Jones', 'Happy' UNION
	SELECT 44, 'Smith', 'john' UNION
	SELECT 45, 'Johnson', 'Todd' UNION
	SELECT 46, 'Brockey', 'Michael' UNION
	SELECT 47, 'Buxton', 'Stevem' UNION
	SELECT 48, 'Jones', 'Happy' UNION
	SELECT 49, 'Smith', 'john' UNION
	SELECT 50, 'Johnson', 'Todd'
SET IDENTITY_INSERT [dbo].[Author] OFF
GO

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
	SELECT 7, 5, 'My First Book' UNION
	SELECT 8, 2, 'My Second Book' UNION
	SELECT 9, 3, 'My First Book' UNION
	SELECT 10, 4, 'My First Book' UNION
	SELECT 11, 1, 'My First Book' UNION
	SELECT 12, 1, 'My Second Book' UNION
	SELECT 13, 2, 'My First Book' UNION
	SELECT 14, 2, 'My Second Book' UNION
	SELECT 15, 3, 'My First Book' UNION
	SELECT 16, 4, 'My First Book' UNION
	SELECT 17, 5, 'My First Book' UNION
	SELECT 18, 2, 'My Second Book' UNION
	SELECT 19, 3, 'My First Book' UNION
	SELECT 20, 4, 'My First Book' UNION
	SELECT 21, 1, 'My First Book' UNION
	SELECT 22, 1, 'My Second Book' UNION
	SELECT 23, 2, 'My First Book' UNION
	SELECT 24, 2, 'My Second Book' UNION
	SELECT 25, 3, 'My First Book' UNION
	SELECT 26, 4, 'My First Book' UNION
	SELECT 27, 5, 'My First Book' UNION
	SELECT 28, 2, 'My Second Book' UNION
	SELECT 29, 3, 'My First Book' UNION
	SELECT 30, 4, 'My First Book' UNION
	SELECT 31, 1, 'My First Book' UNION
	SELECT 32, 1, 'My Second Book' UNION
	SELECT 33, 2, 'My First Book' UNION
	SELECT 34, 2, 'My Second Book' UNION
	SELECT 35, 3, 'My First Book' UNION
	SELECT 36, 4, 'My First Book' UNION
	SELECT 37, 5, 'My First Book' UNION
	SELECT 38, 2, 'My Second Book' UNION
	SELECT 39, 3, 'My First Book' UNION
	SELECT 40, 4, 'My First Book' UNION
	SELECT 41, 1, 'My First Book' UNION
	SELECT 42, 1, 'My Second Book' UNION
	SELECT 43, 2, 'My First Book' UNION
	SELECT 44, 2, 'My Second Book' UNION
	SELECT 45, 3, 'My First Book' UNION
	SELECT 46, 4, 'My First Book' UNION
	SELECT 47, 5, 'My First Book' UNION
	SELECT 48, 2, 'My Second Book' UNION
	SELECT 49, 3, 'My First Book' UNION
	SELECT 50, 4, 'My First Book' UNION
	SELECT 51, 1, 'My First Book' UNION
	SELECT 52, 1, 'My Second Book' UNION
	SELECT 53, 2, 'My First Book' UNION
	SELECT 54, 2, 'My Second Book' UNION
	SELECT 55, 3, 'My First Book' UNION
	SELECT 56, 4, 'My First Book' UNION
	SELECT 57, 5, 'My First Book' UNION
	SELECT 58, 2, 'My Second Book' UNION
	SELECT 59, 3, 'My First Book' UNION
	SELECT 60, 4, 'My First Book' UNION
	SELECT 61, 1, 'My First Book' UNION
	SELECT 62, 1, 'My Second Book' UNION
	SELECT 63, 2, 'My First Book' UNION
	SELECT 64, 2, 'My Second Book' UNION
	SELECT 65, 3, 'My First Book' UNION
	SELECT 66, 4, 'My First Book' UNION
	SELECT 67, 5, 'My First Book' UNION
	SELECT 68, 2, 'My Second Book' UNION
	SELECT 69, 3, 'My First Book' UNION
	SELECT 70, 4, 'My First Book' UNION
	SELECT 71, 1, 'My First Book' UNION
	SELECT 72, 1, 'My Second Book' UNION
	SELECT 73, 2, 'My First Book' UNION
	SELECT 74, 2, 'My Second Book' UNION
	SELECT 75, 3, 'My First Book' UNION
	SELECT 76, 4, 'My First Book' UNION
	SELECT 77, 5, 'My First Book' UNION
	SELECT 78, 2, 'My Second Book' UNION
	SELECT 79, 3, 'My First Book' UNION
	SELECT 80, 4, 'My First Book' UNION
	SELECT 81, 1, 'My First Book' UNION
	SELECT 82, 1, 'My Second Book' UNION
	SELECT 83, 2, 'My First Book' UNION
	SELECT 84, 2, 'My Second Book' UNION
	SELECT 85, 3, 'My First Book' UNION
	SELECT 86, 4, 'My First Book' UNION
	SELECT 87, 5, 'My First Book' UNION
	SELECT 88, 2, 'My Second Book' UNION
	SELECT 89, 3, 'My First Book' UNION
	SELECT 90, 4, 'My First Book' UNION
	SELECT 91, 1, 'My First Book' UNION
	SELECT 92, 1, 'My Second Book' UNION
	SELECT 93, 2, 'My First Book' UNION
	SELECT 94, 2, 'My Second Book' UNION
	SELECT 95, 3, 'My First Book' UNION
	SELECT 96, 4, 'My First Book' UNION
	SELECT 97, 5, 'My First Book' UNION
	SELECT 98, 2, 'My Second Book' UNION
	SELECT 99, 3, 'My First Book'
	
SET IDENTITY_INSERT [dbo].[Book] OFF
GO

