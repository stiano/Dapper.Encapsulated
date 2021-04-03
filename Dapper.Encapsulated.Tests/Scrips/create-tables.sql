USE [dapper.encapsulated.tests]
GO

/****** Object:  Table [dbo].[User]    Script Date: 03-Apr-21 01:53:37 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[User](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


INSERT INTO [User] ([name]) VALUES ('one')
INSERT INTO [User] ([name]) VALUES ('two')
INSERT INTO [User] ([name]) VALUES ('three')
