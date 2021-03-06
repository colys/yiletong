USE [yiletong]
GO
/****** Object:  Table [dbo].[users]    Script Date: 08/26/2015 22:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[users](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userName] [varchar](20) NOT NULL,
	[password] [varchar](20) NULL,
	[secretKey] [varchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[transactionsum]    Script Date: 08/26/2015 22:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[transactionsum](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[terminal] [varchar](20) NOT NULL,
	[tradeMoney] [decimal](18, 2) NOT NULL,
	[tixianfeiMoney] [decimal](18, 2) NOT NULL,
	[discountMoney] [decimal](18, 2) NOT NULL,
	[finallyMoney] [decimal](18, 2) NOT NULL,
	[status] [int] NOT NULL,
	[results] [varchar](20) NULL,
	[createDate] [varchar](20) NULL,
	[uploadDate] [varchar](20) NULL,
	[reciveDate] [varchar](20) NULL,
	[batchAmount] [int] NULL,
	[bankName] [varchar](20) NULL,
	[bankName2] [varchar](64) NULL,
	[bankName3] [varchar](64) NULL,
	[province] [varchar](20) NULL,
	[city] [varchar](20) NULL,
	[tel] [varchar](20) NULL,
	[batchCurrnum] [varchar](20) NULL,
	[batchCount] [int] NULL,
	[faren] [varchar](50) NULL,
	[shanghuName] [varchar](64) NULL,
	[bankAccount] [varchar](32) NULL,
	[sourceAccount] [varchar](256) NULL,
	[sum2Id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[transactionlogs]    Script Date: 08/26/2015 22:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[transactionlogs](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[terminal] [varchar](20) NULL,
	[tradeName] [varchar](120) NULL,
	[results] [varchar](516) NULL,
	[shanghuName] [varchar](64) NULL,
	[faren] [varchar](32) NULL,
	[tradeMoney] [decimal](18, 2) NULL,
	[time] [varchar](20) NULL,
	[finallyMoney] [decimal](18, 2) NULL,
	[cards] [varchar](20) NULL,
	[jiesuantime] [varchar](20) NULL,
	[jiesuanstatus] [int] NULL,
	[record] [varchar](255) NULL,
	[timeStr] [varchar](20) NULL,
	[resultCode] [varchar](2) NULL,
	[discountMoney] [decimal](18, 2) NULL,
	[tixianfeiMoney] [decimal](18, 2) NULL,
	[status] [int] NULL,
	[sumid] [int] NULL,
	[isValid] [tinyint] NULL,
 CONSTRAINT [PK__transact__3213E83F47DBAE45] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[sourceaccounts]    Script Date: 08/26/2015 22:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[sourceaccounts](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[faren] [varchar](20) NOT NULL,
	[bankName] [varchar](64) NOT NULL,
	[bankAccount] [varchar](32) NOT NULL,
	[status] [tinyint] NOT NULL,
 CONSTRAINT [PK__sourceac__3213E83F1367E606] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[customers]    Script Date: 08/26/2015 22:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[customers](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[terminal] [varchar](20) NULL,
	[faren] [varchar](20) NULL,
	[shanghuName] [varchar](20) NULL,
	[tel] [varchar](20) NULL,
	[discount] [float] NULL,
	[fengding] [float] NULL,
	[tixianfeiEles] [float] NULL,
	[tixianfei] [float] NULL,
	[bankName] [varchar](64) NULL,
	[bankName2] [varchar](64) NULL,
	[bankAccount] [varchar](32) NULL,
	[status] [int] NULL,
	[province] [varchar](20) NULL,
	[city] [varchar](20) NULL,
	[bankName3] [varchar](64) NULL,
	[sourceAccount] [varchar](256) NULL,
	[lastQuery] [varchar](10) NULL,
	[IsFengDing] [bit] NULL,
	[frozen] [tinyint] NULL,
	[eachMin] [float] NULL,
	[eachMax] [float] NULL,
	[dayMin] [float] NULL,
	[dayMax] [float] NULL,
 CONSTRAINT [PK__customer__3213E83F7F60ED59] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Default [DF__customers__disco__014935CB]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__disco__014935CB]  DEFAULT (NULL) FOR [discount]
GO
/****** Object:  Default [DF__customers__fengd__023D5A04]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__fengd__023D5A04]  DEFAULT (NULL) FOR [fengding]
GO
/****** Object:  Default [DF__customers__tixia__03317E3D]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__tixia__03317E3D]  DEFAULT (NULL) FOR [tixianfeiEles]
GO
/****** Object:  Default [DF__customers__tixia__0425A276]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__tixia__0425A276]  DEFAULT (NULL) FOR [tixianfei]
GO
/****** Object:  Default [DF__customers__bankN__0519C6AF]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__bankN__0519C6AF]  DEFAULT (NULL) FOR [bankName2]
GO
/****** Object:  Default [DF__customers__statu__060DEAE8]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__statu__060DEAE8]  DEFAULT ('1') FOR [status]
GO
/****** Object:  Default [DF__customers__provi__07020F21]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__provi__07020F21]  DEFAULT (NULL) FOR [province]
GO
/****** Object:  Default [DF__customers__city__07F6335A]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__city__07F6335A]  DEFAULT (NULL) FOR [city]
GO
/****** Object:  Default [DF__customers__bankN__08EA5793]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__bankN__08EA5793]  DEFAULT (NULL) FOR [bankName3]
GO
/****** Object:  Default [DF__customers__sourc__09DE7BCC]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__sourc__09DE7BCC]  DEFAULT (NULL) FOR [sourceAccount]
GO
/****** Object:  Default [DF__customers__lastQ__0AD2A005]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__lastQ__0AD2A005]  DEFAULT (NULL) FOR [lastQuery]
GO
/****** Object:  Default [DF__customers__IsFen__0BC6C43E]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__IsFen__0BC6C43E]  DEFAULT (NULL) FOR [IsFengDing]
GO
/****** Object:  Default [DF__customers__froze__0CBAE877]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__froze__0CBAE877]  DEFAULT ('0') FOR [frozen]
GO
/****** Object:  Default [DF__customers__eachM__0DAF0CB0]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__eachM__0DAF0CB0]  DEFAULT ('0') FOR [eachMin]
GO
/****** Object:  Default [DF__customers__eachM__0EA330E9]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__eachM__0EA330E9]  DEFAULT ('0') FOR [eachMax]
GO
/****** Object:  Default [DF__customers__dayMi__0F975522]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__dayMi__0F975522]  DEFAULT ('0') FOR [dayMin]
GO
/****** Object:  Default [DF__customers__dayMa__108B795B]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF__customers__dayMa__108B795B]  DEFAULT ('0') FOR [dayMax]
GO
/****** Object:  Default [DF__sourceacc__statu__15502E78]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[sourceaccounts] ADD  CONSTRAINT [DF__sourceacc__statu__15502E78]  DEFAULT ('0') FOR [status]
GO
/****** Object:  Default [DF__transacti__jiesu__49C3F6B7]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[transactionlogs] ADD  CONSTRAINT [DF__transacti__jiesu__49C3F6B7]  DEFAULT ('1') FOR [jiesuanstatus]
GO
/****** Object:  Default [DF__transacti__isVal__4AB81AF0]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[transactionlogs] ADD  CONSTRAINT [DF__transacti__isVal__4AB81AF0]  DEFAULT ('0') FOR [isValid]
GO
/****** Object:  Default [DF__transacti__statu__20C1E124]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[transactionsum] ADD  DEFAULT ('0') FOR [status]
GO
/****** Object:  Default [DF__transacti__batch__21B6055D]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[transactionsum] ADD  DEFAULT (NULL) FOR [batchAmount]
GO
/****** Object:  Default [DF__transacti__bankN__22AA2996]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[transactionsum] ADD  DEFAULT (NULL) FOR [bankName2]
GO
/****** Object:  Default [DF__transacti__bankN__239E4DCF]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[transactionsum] ADD  DEFAULT (NULL) FOR [bankName3]
GO
/****** Object:  Default [DF__transacti__provi__24927208]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[transactionsum] ADD  DEFAULT (NULL) FOR [province]
GO
/****** Object:  Default [DF__transactio__city__25869641]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[transactionsum] ADD  DEFAULT (NULL) FOR [city]
GO
/****** Object:  Default [DF__transaction__tel__267ABA7A]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[transactionsum] ADD  DEFAULT (NULL) FOR [tel]
GO
/****** Object:  Default [DF__transacti__batch__276EDEB3]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[transactionsum] ADD  DEFAULT (NULL) FOR [batchCurrnum]
GO
/****** Object:  Default [DF__transacti__batch__286302EC]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[transactionsum] ADD  DEFAULT (NULL) FOR [batchCount]
GO
/****** Object:  Default [DF__transacti__faren__29572725]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[transactionsum] ADD  DEFAULT (NULL) FOR [faren]
GO
/****** Object:  Default [DF__transacti__shang__2A4B4B5E]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[transactionsum] ADD  DEFAULT (NULL) FOR [shanghuName]
GO
/****** Object:  Default [DF__transacti__bankA__2B3F6F97]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[transactionsum] ADD  DEFAULT (NULL) FOR [bankAccount]
GO
/****** Object:  Default [DF__transacti__sourc__2C3393D0]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[transactionsum] ADD  DEFAULT (NULL) FOR [sourceAccount]
GO
/****** Object:  Default [DF_transactionsum_sum2Id]    Script Date: 08/26/2015 22:22:51 ******/
ALTER TABLE [dbo].[transactionsum] ADD  CONSTRAINT [DF_transactionsum_sum2Id]  DEFAULT ((0)) FOR [sum2Id]
GO
