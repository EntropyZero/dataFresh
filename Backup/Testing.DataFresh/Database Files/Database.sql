if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Book]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Book]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Author]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Author]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Movie]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Movie]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Movie2]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Movie2]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Check]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Check]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Check Table With Spaces]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Check Table With Spaces]
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

CREATE TABLE [Movie]
(
	[MovieId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY
	, [Title] VARCHAR(55) NOT NULL
)
GO

CREATE TABLE [Movie2]
(
	[MovieId] INT NOT NULL PRIMARY KEY
	, [Title] VARCHAR(55) NOT NULL
)
GO

CREATE TABLE [Check]
(
	[MovieId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY
	, [Title] VARCHAR(55) NOT NULL
)
GO

CREATE TABLE [Check Table With Spaces]
(
	[MovieId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY
	, [Title] VARCHAR(55) NOT NULL
)
GO