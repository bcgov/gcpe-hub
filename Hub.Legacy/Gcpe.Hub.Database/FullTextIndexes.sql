CREATE FULLTEXT INDEX ON [media].[MediaRequest]
    ([RequestTopic] LANGUAGE 1033, [RequestContent] LANGUAGE 1033, [Response] LANGUAGE 1033)
    KEY INDEX [PK_MediaRequest]
    ON [FTS_MediaRequest]
    WITH STOPLIST OFF;


GO
CREATE FULLTEXT INDEX ON [media].[Contact]
    ([FirstName] LANGUAGE 1033, [LastName] LANGUAGE 1033)
    KEY INDEX [PK_Contact]
    ON [FTS_MediaRequest];


GO
CREATE FULLTEXT INDEX ON [media].[Company]
    ([CompanyName] LANGUAGE 1033)
    KEY INDEX [PK_Company]
    ON [FTS_MediaRequest];

