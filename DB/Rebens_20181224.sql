USE [Rebens]
GO
/****** Object:  Table [dbo].[Address]    Script Date: 24/12/2018 5:53:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Address](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NULL,
	[Street] [nvarchar](400) NULL,
	[Number] [nvarchar](50) NULL,
	[Complement] [nvarchar](50) NULL,
	[Neighborhood] [nvarchar](200) NULL,
	[City] [nvarchar](200) NULL,
	[State] [nvarchar](200) NULL,
	[Country] [nvarchar](200) NULL,
	[Zipcode] [nvarchar](50) NULL,
	[Latitude] [nvarchar](50) NULL,
	[Longitude] [nvarchar](50) NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
 CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AdminUser]    Script Date: 24/12/2018 5:53:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdminUser](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](300) NOT NULL,
	[Email] [nvarchar](300) NOT NULL,
	[LastLogin] [datetime] NULL,
	[EncryptedPassword] [nvarchar](300) NOT NULL,
	[PasswordSalt] [nvarchar](300) NOT NULL,
	[IdProfile] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Banner]    Script Date: 24/12/2018 5:53:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Banner](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Image] [nvarchar](500) NOT NULL,
	[Order] [int] NOT NULL,
	[Link] [nvarchar](500) NOT NULL,
	[Type] [int] NOT NULL,
	[BackgroundColor] [nvarchar](50) NULL,
	[IdBenefit] [int] NULL,
	[Active] [bit] NOT NULL,
	[Start] [datetime] NULL,
	[End] [datetime] NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
 CONSTRAINT [PK_Banner] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BannerOperation]    Script Date: 24/12/2018 5:53:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BannerOperation](
	[IdBanner] [int] NOT NULL,
	[IdOperation] [int] NOT NULL,
 CONSTRAINT [PK_BannerOperation] PRIMARY KEY CLUSTERED 
(
	[IdBanner] ASC,
	[IdOperation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Benefit]    Script Date: 24/12/2018 5:53:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Benefit](
	[Id] [int] NOT NULL,
	[Title] [nvarchar](400) NOT NULL,
	[IdAdminUser] [int] NOT NULL,
	[Image] [nvarchar](500) NULL,
	[DueDate] [datetime] NULL,
	[WebSite] [nvarchar](500) NULL,
	[MaxDiscountPercentageOnline] [money] NULL,
	[CPVPercentageOnline] [money] NULL,
	[MaxDiscountPercentageOffline] [money] NULL,
	[CPVPercentageOffline] [money] NULL,
	[Start] [datetime] NULL,
	[End] [datetime] NULL,
	[IdBenefitType] [int] NOT NULL,
	[Exclusive] [bit] NOT NULL,
	[Active] [bit] NOT NULL,
	[IdIntegrationType] [int] NOT NULL,
	[IdPartner] [int] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
 CONSTRAINT [PK_Benefit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BenefitAddress]    Script Date: 24/12/2018 5:53:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BenefitAddress](
	[IdBenefit] [int] NOT NULL,
	[IdAddress] [int] NOT NULL,
 CONSTRAINT [PK_BenefitAddress] PRIMARY KEY CLUSTERED 
(
	[IdBenefit] ASC,
	[IdAddress] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BenefitCategory]    Script Date: 24/12/2018 5:53:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BenefitCategory](
	[IdBenefit] [int] NOT NULL,
	[IdCategory] [int] NOT NULL,
 CONSTRAINT [PK_BenefitCategory] PRIMARY KEY CLUSTERED 
(
	[IdBenefit] ASC,
	[IdCategory] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BenefitOperation]    Script Date: 24/12/2018 5:53:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BenefitOperation](
	[IdBenefit] [int] NOT NULL,
	[IdOperation] [int] NOT NULL,
	[IdPosition] [int] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
 CONSTRAINT [PK_BenefitOperation] PRIMARY KEY CLUSTERED 
(
	[IdBenefit] ASC,
	[IdOperation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BenefitOperationPosition]    Script Date: 24/12/2018 5:53:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BenefitOperationPosition](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Active] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
 CONSTRAINT [PK_BenefitOperationPosition] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BenefitType]    Script Date: 24/12/2018 5:53:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BenefitType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Active] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
 CONSTRAINT [PK_BenefitType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Category]    Script Date: 24/12/2018 5:53:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Order] [int] NOT NULL,
	[IdParent] [int] NULL,
	[Icon] [nvarchar](500) NULL,
	[Active] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Contact]    Script Date: 24/12/2018 5:53:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contact](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](300) NOT NULL,
	[Email] [nvarchar](400) NULL,
	[JobTitle] [nvarchar](200) NULL,
	[Phone] [nvarchar](50) NULL,
	[CellPhone] [nvarchar](50) NULL,
	[IdAddress] [int] NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
 CONSTRAINT [PK_Contact] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Faq]    Script Date: 24/12/2018 5:53:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Faq](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdOperation] [int] NOT NULL,
	[Question] [nvarchar](1000) NOT NULL,
	[Answer] [nvarchar](1000) NOT NULL,
	[Order] [int] NOT NULL,
	[Active] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
 CONSTRAINT [PK_Faq] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IntegrationType]    Script Date: 24/12/2018 5:53:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IntegrationType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Active] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
 CONSTRAINT [PK_IntegrationType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LogError]    Script Date: 24/12/2018 5:53:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogError](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Reference] [nvarchar](500) NOT NULL,
	[Complement] [nvarchar](500) NULL,
	[Message] [text] NOT NULL,
	[StackTrace] [text] NULL,
	[Created] [datetime] NOT NULL,
 CONSTRAINT [PK_LogError] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Operation]    Script Date: 24/12/2018 5:53:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Operation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](300) NOT NULL,
	[CompanyName] [nvarchar](300) NOT NULL,
	[CompanyDoc] [nvarchar](50) NULL,
	[Image] [nvarchar](500) NULL,
	[Domain] [nvarchar](200) NOT NULL,
	[IdContact] [int] NULL,
	[IdOperationType] [int] NOT NULL,
	[CashbackPercentage] [money] NULL,
	[Active] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
 CONSTRAINT [PK_Operation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OperationType]    Script Date: 24/12/2018 5:53:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OperationType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Active] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
 CONSTRAINT [PK_OperationType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Partner]    Script Date: 24/12/2018 5:53:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Partner](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [nvarchar](200) NOT NULL,
	[IdContact] [int] NOT NULL,
	[Active] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
 CONSTRAINT [PK_Partner] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PartnerAddress]    Script Date: 24/12/2018 5:53:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PartnerAddress](
	[IdPartner] [int] NOT NULL,
	[IdAddress] [int] NOT NULL,
 CONSTRAINT [PK_PartnerAddress] PRIMARY KEY CLUSTERED 
(
	[IdPartner] ASC,
	[IdAddress] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Permission]    Script Date: 24/12/2018 5:53:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permission](
	[Id] [bigint] NOT NULL,
	[Name] [nvarchar](300) NOT NULL,
	[IdParent] [bigint] NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
 CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Profile]    Script Date: 24/12/2018 5:53:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Profile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
	[Active] [bit] NOT NULL,
	[Permissions] [bigint] NOT NULL,
 CONSTRAINT [PK_Profile] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StaticText]    Script Date: 24/12/2018 5:53:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StaticText](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[Url] [nvarchar](200) NULL,
	[Html] [text] NOT NULL,
	[Style] [nvarchar](4000) NULL,
	[Order] [int] NOT NULL,
	[IdStaticTextType] [int] NOT NULL,
	[IdOperation] [int] NOT NULL,
	[Active] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
 CONSTRAINT [PK_StaticText] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StaticTextType]    Script Date: 24/12/2018 5:53:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StaticTextType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[Active] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
 CONSTRAINT [PK_StaticTextType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[BenefitType] ON 

INSERT [dbo].[BenefitType] ([Id], [Name], [Active], [Created], [Modified]) VALUES (1, N'On-line', 1, CAST(N'2018-12-24T11:02:51.657' AS DateTime), CAST(N'2018-12-24T11:02:51.657' AS DateTime))
INSERT [dbo].[BenefitType] ([Id], [Name], [Active], [Created], [Modified]) VALUES (2, N'Off-line', 1, CAST(N'2018-12-24T11:02:51.660' AS DateTime), CAST(N'2018-12-24T11:02:51.660' AS DateTime))
INSERT [dbo].[BenefitType] ([Id], [Name], [Active], [Created], [Modified]) VALUES (3, N'Cashback', 1, CAST(N'2018-12-24T11:02:51.663' AS DateTime), CAST(N'2018-12-24T11:02:51.663' AS DateTime))
INSERT [dbo].[BenefitType] ([Id], [Name], [Active], [Created], [Modified]) VALUES (4, N'Todos', 1, CAST(N'2018-12-24T11:02:51.663' AS DateTime), CAST(N'2018-12-24T11:02:51.663' AS DateTime))
SET IDENTITY_INSERT [dbo].[BenefitType] OFF
SET IDENTITY_INSERT [dbo].[Category] ON 

INSERT [dbo].[Category] ([Id], [Name], [Order], [IdParent], [Icon], [Active], [Created], [Modified]) VALUES (1, N'Categoria 1', 1, NULL, NULL, 1, CAST(N'2018-12-23T12:54:52.947' AS DateTime), CAST(N'2018-12-23T12:54:52.947' AS DateTime))
INSERT [dbo].[Category] ([Id], [Name], [Order], [IdParent], [Icon], [Active], [Created], [Modified]) VALUES (2, N'Categoria 2.1', 2, NULL, NULL, 1, CAST(N'2018-12-23T12:55:54.897' AS DateTime), CAST(N'2018-12-23T12:58:03.283' AS DateTime))
INSERT [dbo].[Category] ([Id], [Name], [Order], [IdParent], [Icon], [Active], [Created], [Modified]) VALUES (3, N'Categoria 3', 3, NULL, NULL, 1, CAST(N'2018-12-23T12:56:10.727' AS DateTime), CAST(N'2018-12-23T12:56:10.727' AS DateTime))
INSERT [dbo].[Category] ([Id], [Name], [Order], [IdParent], [Icon], [Active], [Created], [Modified]) VALUES (4, N'Categoria 4', 4, NULL, NULL, 1, CAST(N'2018-12-23T12:56:16.890' AS DateTime), CAST(N'2018-12-23T12:56:16.890' AS DateTime))
INSERT [dbo].[Category] ([Id], [Name], [Order], [IdParent], [Icon], [Active], [Created], [Modified]) VALUES (5, N'Categoria 5', 5, NULL, NULL, 1, CAST(N'2018-12-23T12:56:23.450' AS DateTime), CAST(N'2018-12-23T12:56:23.450' AS DateTime))
INSERT [dbo].[Category] ([Id], [Name], [Order], [IdParent], [Icon], [Active], [Created], [Modified]) VALUES (6, N'Categoria 1.2', 1, 1, NULL, 1, CAST(N'2018-12-24T13:50:30.307' AS DateTime), CAST(N'2018-12-24T16:52:50.433' AS DateTime))
INSERT [dbo].[Category] ([Id], [Name], [Order], [IdParent], [Icon], [Active], [Created], [Modified]) VALUES (7, N'Categoria 8', 8, NULL, NULL, 1, CAST(N'2018-12-24T16:52:34.383' AS DateTime), CAST(N'2018-12-24T16:52:34.383' AS DateTime))
SET IDENTITY_INSERT [dbo].[Category] OFF
SET IDENTITY_INSERT [dbo].[Faq] ON 

INSERT [dbo].[Faq] ([Id], [IdOperation], [Question], [Answer], [Order], [Active], [Created], [Modified]) VALUES (1, 1, N'como faço?', N'basta clicar', 1, 1, CAST(N'2018-12-24T19:30:53.133' AS DateTime), CAST(N'2018-12-24T19:30:53.133' AS DateTime))
INSERT [dbo].[Faq] ([Id], [IdOperation], [Question], [Answer], [Order], [Active], [Created], [Modified]) VALUES (2, 1, N'como faço?', N'basta clicar', 1, 1, CAST(N'2018-12-24T19:32:09.310' AS DateTime), CAST(N'2018-12-24T19:33:27.787' AS DateTime))
SET IDENTITY_INSERT [dbo].[Faq] OFF
SET IDENTITY_INSERT [dbo].[IntegrationType] ON 

INSERT [dbo].[IntegrationType] ([Id], [Name], [Active], [Created], [Modified]) VALUES (1, N'Rebens', 1, CAST(N'2018-12-24T11:06:34.517' AS DateTime), CAST(N'2018-12-24T11:06:34.517' AS DateTime))
INSERT [dbo].[IntegrationType] ([Id], [Name], [Active], [Created], [Modified]) VALUES (2, N'Zanox', 1, CAST(N'2018-12-24T11:06:34.553' AS DateTime), CAST(N'2018-12-24T11:06:34.553' AS DateTime))
SET IDENTITY_INSERT [dbo].[IntegrationType] OFF
SET IDENTITY_INSERT [dbo].[Operation] ON 

INSERT [dbo].[Operation] ([Id], [Title], [CompanyName], [CompanyDoc], [Image], [Domain], [IdContact], [IdOperationType], [CashbackPercentage], [Active], [Created], [Modified]) VALUES (1, N'Teste 1', N'ias', N'98798798798', NULL, N'iasdigitalgroup.com', NULL, 1, NULL, 1, CAST(N'2018-12-24T15:26:10.983' AS DateTime), CAST(N'2018-12-24T15:26:10.983' AS DateTime))
SET IDENTITY_INSERT [dbo].[Operation] OFF
SET IDENTITY_INSERT [dbo].[OperationType] ON 

INSERT [dbo].[OperationType] ([Id], [Name], [Active], [Created], [Modified]) VALUES (1, N'IES', 1, CAST(N'2018-12-24T11:03:54.860' AS DateTime), CAST(N'2018-12-24T11:03:54.860' AS DateTime))
INSERT [dbo].[OperationType] ([Id], [Name], [Active], [Created], [Modified]) VALUES (2, N'Clube', 1, CAST(N'2018-12-24T11:03:54.860' AS DateTime), CAST(N'2018-12-24T11:03:54.860' AS DateTime))
INSERT [dbo].[OperationType] ([Id], [Name], [Active], [Created], [Modified]) VALUES (3, N'Franquias', 1, CAST(N'2018-12-24T11:03:54.863' AS DateTime), CAST(N'2018-12-24T11:03:54.863' AS DateTime))
INSERT [dbo].[OperationType] ([Id], [Name], [Active], [Created], [Modified]) VALUES (4, N'Aluguel', 1, CAST(N'2018-12-24T11:03:54.863' AS DateTime), CAST(N'2018-12-24T11:03:54.863' AS DateTime))
SET IDENTITY_INSERT [dbo].[OperationType] OFF
INSERT [dbo].[Permission] ([Id], [Name], [IdParent], [Created], [Modified]) VALUES (1, N'Segurança', NULL, CAST(N'2018-12-24T11:07:30.390' AS DateTime), CAST(N'2018-12-24T11:07:30.390' AS DateTime))
INSERT [dbo].[Permission] ([Id], [Name], [IdParent], [Created], [Modified]) VALUES (2, N'Gerenciamento de Perfil', 1, CAST(N'2018-12-24T11:08:18.073' AS DateTime), CAST(N'2018-12-24T11:08:18.073' AS DateTime))
INSERT [dbo].[Permission] ([Id], [Name], [IdParent], [Created], [Modified]) VALUES (3, N'Conteúdo', NULL, CAST(N'2018-12-24T11:09:31.140' AS DateTime), CAST(N'2018-12-24T11:09:31.140' AS DateTime))
INSERT [dbo].[Permission] ([Id], [Name], [IdParent], [Created], [Modified]) VALUES (4, N'Gerenciamento de Usuário', 1, CAST(N'2018-12-24T11:08:18.077' AS DateTime), CAST(N'2018-12-24T11:08:18.077' AS DateTime))
INSERT [dbo].[Permission] ([Id], [Name], [IdParent], [Created], [Modified]) VALUES (8, N'Gerenciamento de Banner', 3, CAST(N'2018-12-24T11:09:31.140' AS DateTime), CAST(N'2018-12-24T11:09:31.140' AS DateTime))
INSERT [dbo].[Permission] ([Id], [Name], [IdParent], [Created], [Modified]) VALUES (16, N'Gerenciamento do FAQ', 3, CAST(N'2018-12-24T11:09:31.143' AS DateTime), CAST(N'2018-12-24T11:09:31.143' AS DateTime))
INSERT [dbo].[Permission] ([Id], [Name], [IdParent], [Created], [Modified]) VALUES (32, N'Gerenciamento do Páginas estáticas', 3, CAST(N'2018-12-24T11:09:31.143' AS DateTime), CAST(N'2018-12-24T11:09:31.143' AS DateTime))
SET IDENTITY_INSERT [dbo].[StaticTextType] ON 

INSERT [dbo].[StaticTextType] ([Id], [Name], [Description], [Active], [Created], [Modified]) VALUES (1, N'Operação Sobre', N'Descrição sobre a opreção', 1, CAST(N'2018-12-24T11:06:00.107' AS DateTime), CAST(N'2018-12-24T11:06:00.107' AS DateTime))
INSERT [dbo].[StaticTextType] ([Id], [Name], [Description], [Active], [Created], [Modified]) VALUES (2, N'Operação como funciona', N'Descrição de como funciona a opreção', 1, CAST(N'2018-12-24T11:06:00.120' AS DateTime), CAST(N'2018-12-24T11:06:00.120' AS DateTime))
SET IDENTITY_INSERT [dbo].[StaticTextType] OFF
ALTER TABLE [dbo].[AdminUser]  WITH CHECK ADD  CONSTRAINT [FK_User_Profile] FOREIGN KEY([IdProfile])
REFERENCES [dbo].[Profile] ([Id])
GO
ALTER TABLE [dbo].[AdminUser] CHECK CONSTRAINT [FK_User_Profile]
GO
ALTER TABLE [dbo].[Banner]  WITH CHECK ADD  CONSTRAINT [FK_Banner_Benefit] FOREIGN KEY([IdBenefit])
REFERENCES [dbo].[Benefit] ([Id])
GO
ALTER TABLE [dbo].[Banner] CHECK CONSTRAINT [FK_Banner_Benefit]
GO
ALTER TABLE [dbo].[BannerOperation]  WITH CHECK ADD  CONSTRAINT [FK_BannerOperation_Banner] FOREIGN KEY([IdBanner])
REFERENCES [dbo].[Banner] ([Id])
GO
ALTER TABLE [dbo].[BannerOperation] CHECK CONSTRAINT [FK_BannerOperation_Banner]
GO
ALTER TABLE [dbo].[BannerOperation]  WITH CHECK ADD  CONSTRAINT [FK_BannerOperation_Operation] FOREIGN KEY([IdOperation])
REFERENCES [dbo].[Operation] ([Id])
GO
ALTER TABLE [dbo].[BannerOperation] CHECK CONSTRAINT [FK_BannerOperation_Operation]
GO
ALTER TABLE [dbo].[Benefit]  WITH CHECK ADD  CONSTRAINT [FK_Benefit_BenefitType] FOREIGN KEY([IdBenefitType])
REFERENCES [dbo].[BenefitType] ([Id])
GO
ALTER TABLE [dbo].[Benefit] CHECK CONSTRAINT [FK_Benefit_BenefitType]
GO
ALTER TABLE [dbo].[Benefit]  WITH CHECK ADD  CONSTRAINT [FK_Benefit_IntegrationType] FOREIGN KEY([IdIntegrationType])
REFERENCES [dbo].[IntegrationType] ([Id])
GO
ALTER TABLE [dbo].[Benefit] CHECK CONSTRAINT [FK_Benefit_IntegrationType]
GO
ALTER TABLE [dbo].[Benefit]  WITH CHECK ADD  CONSTRAINT [FK_Benefit_Partner] FOREIGN KEY([IdPartner])
REFERENCES [dbo].[Partner] ([Id])
GO
ALTER TABLE [dbo].[Benefit] CHECK CONSTRAINT [FK_Benefit_Partner]
GO
ALTER TABLE [dbo].[BenefitAddress]  WITH CHECK ADD  CONSTRAINT [FK_BenefitAddress_Address] FOREIGN KEY([IdAddress])
REFERENCES [dbo].[Address] ([Id])
GO
ALTER TABLE [dbo].[BenefitAddress] CHECK CONSTRAINT [FK_BenefitAddress_Address]
GO
ALTER TABLE [dbo].[BenefitAddress]  WITH CHECK ADD  CONSTRAINT [FK_BenefitAddress_Benefit] FOREIGN KEY([IdBenefit])
REFERENCES [dbo].[Benefit] ([Id])
GO
ALTER TABLE [dbo].[BenefitAddress] CHECK CONSTRAINT [FK_BenefitAddress_Benefit]
GO
ALTER TABLE [dbo].[BenefitCategory]  WITH CHECK ADD  CONSTRAINT [FK_BenefitCategory_Benefit] FOREIGN KEY([IdBenefit])
REFERENCES [dbo].[Benefit] ([Id])
GO
ALTER TABLE [dbo].[BenefitCategory] CHECK CONSTRAINT [FK_BenefitCategory_Benefit]
GO
ALTER TABLE [dbo].[BenefitCategory]  WITH CHECK ADD  CONSTRAINT [FK_BenefitCategory_Category] FOREIGN KEY([IdCategory])
REFERENCES [dbo].[Category] ([Id])
GO
ALTER TABLE [dbo].[BenefitCategory] CHECK CONSTRAINT [FK_BenefitCategory_Category]
GO
ALTER TABLE [dbo].[BenefitOperation]  WITH CHECK ADD  CONSTRAINT [FK_BenefitOperation_Benefit] FOREIGN KEY([IdBenefit])
REFERENCES [dbo].[Benefit] ([Id])
GO
ALTER TABLE [dbo].[BenefitOperation] CHECK CONSTRAINT [FK_BenefitOperation_Benefit]
GO
ALTER TABLE [dbo].[BenefitOperation]  WITH CHECK ADD  CONSTRAINT [FK_BenefitOperation_BenefitOperationPosition] FOREIGN KEY([IdPosition])
REFERENCES [dbo].[BenefitOperationPosition] ([Id])
GO
ALTER TABLE [dbo].[BenefitOperation] CHECK CONSTRAINT [FK_BenefitOperation_BenefitOperationPosition]
GO
ALTER TABLE [dbo].[BenefitOperation]  WITH CHECK ADD  CONSTRAINT [FK_BenefitOperation_Operation] FOREIGN KEY([IdOperation])
REFERENCES [dbo].[Operation] ([Id])
GO
ALTER TABLE [dbo].[BenefitOperation] CHECK CONSTRAINT [FK_BenefitOperation_Operation]
GO
ALTER TABLE [dbo].[Category]  WITH CHECK ADD  CONSTRAINT [FK_Category_Category] FOREIGN KEY([IdParent])
REFERENCES [dbo].[Category] ([Id])
GO
ALTER TABLE [dbo].[Category] CHECK CONSTRAINT [FK_Category_Category]
GO
ALTER TABLE [dbo].[Contact]  WITH CHECK ADD  CONSTRAINT [FK_Contact_Address] FOREIGN KEY([IdAddress])
REFERENCES [dbo].[Address] ([Id])
GO
ALTER TABLE [dbo].[Contact] CHECK CONSTRAINT [FK_Contact_Address]
GO
ALTER TABLE [dbo].[Faq]  WITH CHECK ADD  CONSTRAINT [FK_Faq_Operation] FOREIGN KEY([IdOperation])
REFERENCES [dbo].[Operation] ([Id])
GO
ALTER TABLE [dbo].[Faq] CHECK CONSTRAINT [FK_Faq_Operation]
GO
ALTER TABLE [dbo].[Operation]  WITH CHECK ADD  CONSTRAINT [FK_Operation_OperationType] FOREIGN KEY([IdOperationType])
REFERENCES [dbo].[OperationType] ([Id])
GO
ALTER TABLE [dbo].[Operation] CHECK CONSTRAINT [FK_Operation_OperationType]
GO
ALTER TABLE [dbo].[Partner]  WITH CHECK ADD  CONSTRAINT [FK_Partner_Contact] FOREIGN KEY([IdContact])
REFERENCES [dbo].[Contact] ([Id])
GO
ALTER TABLE [dbo].[Partner] CHECK CONSTRAINT [FK_Partner_Contact]
GO
ALTER TABLE [dbo].[PartnerAddress]  WITH CHECK ADD  CONSTRAINT [FK_PartnerAddress_Address] FOREIGN KEY([IdAddress])
REFERENCES [dbo].[Address] ([Id])
GO
ALTER TABLE [dbo].[PartnerAddress] CHECK CONSTRAINT [FK_PartnerAddress_Address]
GO
ALTER TABLE [dbo].[PartnerAddress]  WITH CHECK ADD  CONSTRAINT [FK_PartnerAddress_Partner] FOREIGN KEY([IdPartner])
REFERENCES [dbo].[Partner] ([Id])
GO
ALTER TABLE [dbo].[PartnerAddress] CHECK CONSTRAINT [FK_PartnerAddress_Partner]
GO
ALTER TABLE [dbo].[Permission]  WITH CHECK ADD  CONSTRAINT [FK_Permission_Permission] FOREIGN KEY([IdParent])
REFERENCES [dbo].[Permission] ([Id])
GO
ALTER TABLE [dbo].[Permission] CHECK CONSTRAINT [FK_Permission_Permission]
GO
ALTER TABLE [dbo].[StaticText]  WITH CHECK ADD  CONSTRAINT [FK_StaticText_Operation] FOREIGN KEY([IdOperation])
REFERENCES [dbo].[Operation] ([Id])
GO
ALTER TABLE [dbo].[StaticText] CHECK CONSTRAINT [FK_StaticText_Operation]
GO
ALTER TABLE [dbo].[StaticText]  WITH CHECK ADD  CONSTRAINT [FK_StaticText_StaticTextType] FOREIGN KEY([IdStaticTextType])
REFERENCES [dbo].[StaticTextType] ([Id])
GO
ALTER TABLE [dbo].[StaticText] CHECK CONSTRAINT [FK_StaticText_StaticTextType]
GO
