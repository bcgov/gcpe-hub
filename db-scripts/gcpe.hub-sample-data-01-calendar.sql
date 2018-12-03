USE [Gcpe.Hub]
GO

INSERT [calendar].[Initiative] ([Name], [IsActive], [SortOrder], [ShortName]) VALUES (N'Sample Initiative', 1, 999, N'SampInit')

INSERT [calendar].[CommunicationMaterial] ([Name], [IsActive], [SortOrder]) VALUES (N'Sample Comm. Material', 1, 999)

INSERT [calendar].[NROrigin] ([Name], [IsActive], [SortOrder]) VALUES (N'Sample Origin', 1, 999)

INSERT [calendar].[NRDistribution] ([Name], [SortOrder], [IsActive]) VALUES (N'Sample Distribution', 1, 999)

INSERT [calendar].[GovernmentRepresentative] ([Name], [Description], [IsActive], [SortOrder]) VALUES (N'Sample Representative', N'Sample Representative', 1, 999)

INSERT [calendar].[EventPlanner] ([Name], [PhoneNumber], [JobTitle], [SortOrder], [IsActive]) VALUES ( N'Sample Planner', N'555 555-5555', N'Manager, Event Services', 999, 1)

INSERT [calendar].[Videographer] ([Name], [JobTitle], [SortOrder], [IsActive]) VALUES (N'Sample Videographer', N'Videographer', 999, 1)

INSERT [calendar].[Keyword] ([Name], [SortOrder], [IsActive], [LastUpdatedBy]) VALUES (N'Sample Keyword', 999, 1, 1)
GO