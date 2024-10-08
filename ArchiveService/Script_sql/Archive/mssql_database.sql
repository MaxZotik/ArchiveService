USE [MVK_Archive]
GO
/****** Object:  Table [dbo].[Archive]    Script Date: 10.03.2023 12:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Archive](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ID Parameters] [int] NULL,
	[ID Frequency] [int] NULL,
	[Time] [datetime] NULL,
	[MVK Value] [float] NULL,
	[Chanel] [int] NULL,
 CONSTRAINT [PK_Archive] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Frequency]    Script Date: 10.03.2023 12:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Frequency](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](30) NULL,
 CONSTRAINT [PK_Frequency] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Parameters]    Script Date: 10.03.2023 12:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Parameters](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](30) NULL,
 CONSTRAINT [PK_Parameters] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[Frequency] ON 

INSERT [dbo].[Frequency] ([ID], [Name]) VALUES (1, N'10-2000гц')
INSERT [dbo].[Frequency] ([ID], [Name]) VALUES (2, N'10-1000гц')
INSERT [dbo].[Frequency] ([ID], [Name]) VALUES (3, N'2-1000гц')
INSERT [dbo].[Frequency] ([ID], [Name]) VALUES (4, N'x-25гц')
INSERT [dbo].[Frequency] ([ID], [Name]) VALUES (5, N'10-3000гц')
INSERT [dbo].[Frequency] ([ID], [Name]) VALUES (6, N'0.8-300гц')
INSERT [dbo].[Frequency] ([ID], [Name]) VALUES (7, N'0.8-150гц')
INSERT [dbo].[Frequency] ([ID], [Name]) VALUES (8, N'Фильтр 1')
INSERT [dbo].[Frequency] ([ID], [Name]) VALUES (9, N'Фильтр 2')
SET IDENTITY_INSERT [dbo].[Frequency] OFF
SET IDENTITY_INSERT [dbo].[Parameters] ON 

INSERT [dbo].[Parameters] ([ID], [Name]) VALUES (1, N'Пик-Фактор')
INSERT [dbo].[Parameters] ([ID], [Name]) VALUES (2, N'СКЗ виброускорение')
INSERT [dbo].[Parameters] ([ID], [Name]) VALUES (3, N'СКЗ виброскорость')
INSERT [dbo].[Parameters] ([ID], [Name]) VALUES (4, N'СКЗ виброперемещение')
SET IDENTITY_INSERT [dbo].[Parameters] OFF
ALTER TABLE [dbo].[Archive]  WITH CHECK ADD  CONSTRAINT [FK_Archive_Frequency] FOREIGN KEY([ID Frequency])
REFERENCES [dbo].[Frequency] ([ID])
GO
ALTER TABLE [dbo].[Archive] CHECK CONSTRAINT [FK_Archive_Frequency]
GO
ALTER TABLE [dbo].[Archive]  WITH CHECK ADD  CONSTRAINT [FK_Archive_Parametrs] FOREIGN KEY([ID Parameters])
REFERENCES [dbo].[Parameters] ([ID])
GO
ALTER TABLE [dbo].[Archive] CHECK CONSTRAINT [FK_Archive_Parametrs]
GO
