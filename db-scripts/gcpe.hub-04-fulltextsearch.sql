USE [Gcpe.Hub]
GO

/****** Object:  FullTextCatalog [FTS_MediaRequest]    Script Date: 2018-12-14 11:13:23 AM ******/
CREATE FULLTEXT CATALOG [FTS_MediaRequest] WITH ACCENT_SENSITIVITY = ON
AUTHORIZATION [public]
GO

/****** Object:  FullTextIndex     Script Date: 2018-12-14 11:13:43 AM ******/
CREATE FULLTEXT INDEX ON [media].[Company](
[CompanyName] LANGUAGE 'English')
KEY INDEX [PK_Company]ON ([FTS_MediaRequest], FILEGROUP [PRIMARY])
WITH (CHANGE_TRACKING = AUTO, STOPLIST = SYSTEM)

GO
/****** Object:  FullTextIndex     Script Date: 2018-12-14 11:13:43 AM ******/
CREATE FULLTEXT INDEX ON [media].[Contact](
[FirstName] LANGUAGE 'English', 
[LastName] LANGUAGE 'English')
KEY INDEX [PK_Contact]ON ([FTS_MediaRequest], FILEGROUP [PRIMARY])
WITH (CHANGE_TRACKING = AUTO, STOPLIST = SYSTEM)

GO
/****** Object:  FullTextIndex     Script Date: 2018-12-14 11:13:43 AM ******/
CREATE FULLTEXT INDEX ON [media].[MediaRequest](
[RequestContent] LANGUAGE 'English', 
[RequestTopic] LANGUAGE 'English', 
[Response] LANGUAGE 'English')
KEY INDEX [PK_MediaRequest]ON ([FTS_MediaRequest], FILEGROUP [PRIMARY])
WITH (CHANGE_TRACKING = AUTO, STOPLIST = OFF)

GO
