USE [master]
GO

CREATE DATABASE [atlas]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'atlas', FILENAME = N'E:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\atlas.mdf' , SIZE = 8331264KB , MAXSIZE = 10485760KB , FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'atlas_log', FILENAME = N'E:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\atlas_log.ldf' , SIZE = 10485760KB , MAXSIZE = 10485760KB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [atlas] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [atlas].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [atlas] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [atlas] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [atlas] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [atlas] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [atlas] SET ARITHABORT OFF 
GO
ALTER DATABASE [atlas] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [atlas] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [atlas] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [atlas] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [atlas] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [atlas] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [atlas] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [atlas] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [atlas] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [atlas] SET  DISABLE_BROKER 
GO
ALTER DATABASE [atlas] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [atlas] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [atlas] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [atlas] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [atlas] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [atlas] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [atlas] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [atlas] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [atlas] SET  MULTI_USER 
GO
ALTER DATABASE [atlas] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [atlas] SET DB_CHAINING OFF 
GO
ALTER DATABASE [atlas] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [atlas] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [atlas] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [atlas] SET QUERY_STORE = ON
GO
ALTER DATABASE [atlas] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 2048, QUERY_CAPTURE_MODE = ALL, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200)
GO
USE [atlas]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
USE [atlas]
GO

CREATE USER [datagov] FOR LOGIN [datagov] WITH DEFAULT_SCHEMA=[app]
GO
ALTER ROLE [db_owner] ADD MEMBER [datagov]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [datagov]
GO
ALTER ROLE [db_datareader] ADD MEMBER [datagov]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [datagov]
GO


CREATE SCHEMA [app]
GO

CREATE FULLTEXT CATALOG [ReportObjectDocs] WITH ACCENT_SENSITIVITY = ON
GO

CREATE FULLTEXT CATALOG [ReportObjects] WITH ACCENT_SENSITIVITY = OFF
AS DEFAULT
GO

CREATE FULLTEXT CATALOG [Search] WITH ACCENT_SENSITIVITY = OFF
GO

CREATE FULLTEXT CATALOG [Search_Small] WITH ACCENT_SENSITIVITY = OFF
GO

CREATE FULLTEXT CATALOG [Terms] WITH ACCENT_SENSITIVITY = ON
GO

CREATE FULLTEXT CATALOG [User_NameData] WITH ACCENT_SENSITIVITY = OFF
GO

CREATE FULLTEXT CATALOG [UserGroups] WITH ACCENT_SENSITIVITY = OFF
GO

CREATE PARTITION FUNCTION [ifts_comp_fragment_partition_function_7BCB2FBE](varbinary(128)) AS RANGE LEFT FOR VALUES (0x0066006F0072)
GO

CREATE PARTITION SCHEME [ifts_comp_fragment_data_space_7BCB2FBE] AS PARTITION [ifts_comp_fragment_partition_function_7BCB2FBE] TO ([PRIMARY], [PRIMARY])
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[ToProperCase](@string VARCHAR(255)) RETURNS VARCHAR(255)
AS
begin
  set @string = Upper(@string);
  DECLARE @i INT           -- index
  DECLARE @l INT           -- input length
  DECLARE @c NCHAR(1)      -- current char
  DECLARE @f INT           -- first letter flag (1/0)
  DECLARE @o VARCHAR(255)  -- output string
  DECLARE @w VARCHAR(10)   -- characters considered as white space

  SET @w = '[' + CHAR(13) + CHAR(10) + CHAR(9) + CHAR(160) + ' ' + ']'
  SET @i = 1
  SET @l = LEN(@string)
  SET @f = 1
  SET @o = ''

  WHILE @i <= @l
  BEGIN
    SET @c = SUBSTRING(@string, @i, 1)
    IF @f = 1 
    BEGIN
     SET @o = @o + @c
     SET @f = 0
    END
    ELSE
    BEGIN
     SET @o = @o + LOWER(@c)
    END

    IF @c LIKE @w SET @f = 1

    SET @i = @i + 1
  END

  RETURN @o
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[Analytics](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Username] [nvarchar](max) NULL,
    [appCodeName] [nvarchar](max) NULL,
    [appName] [nvarchar](max) NULL,
    [appVersion] [nvarchar](max) NULL,
    [cookieEnabled] [nvarchar](max) NULL,
    [language] [nvarchar](max) NULL,
    [oscpu] [nvarchar](max) NULL,
    [platform] [nvarchar](max) NULL,
    [userAgent] [nvarchar](max) NULL,
    [host] [nvarchar](max) NULL,
    [hostname] [nvarchar](max) NULL,
    [href] [nvarchar](max) NULL,
    [protocol] [nvarchar](max) NULL,
    [search] [nvarchar](max) NULL,
    [pathname] [nvarchar](max) NULL,
    [hash] [nvarchar](max) NULL,
    [screenHeight] [nvarchar](max) NULL,
    [screenWidth] [nvarchar](max) NULL,
    [origin] [nvarchar](max) NULL,
    [title] [nvarchar](max) NULL,
    [loadTime] [nvarchar](max) NULL,
    [accessDateTime] [datetime] NULL,
    [referrer] [nvarchar](max) NULL,
    [UserId] [int] NULL,
    [Zoom] [float] NULL,
    [Epic] [int] NULL,
    [active] [int] NULL,
    [pageId] [nvarchar](max) NULL,
    [sessionId] [nvarchar](max) NULL,
    [pageTime] [int] NULL,
    [sessionTime] [int] NULL,
    [updateTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[DP_Agreement](
    [AgreementID] [int] IDENTITY(1,1) NOT NULL,
    [Description] [nvarchar](max) NULL,
    [MeetingDate] [datetime] NULL,
    [EffectiveDate] [datetime] NULL,
    [LastUpdateDate] [datetime] NULL,
    [LastUpdateUser] [int] NULL,
    [DataProjectId] [int] NULL,
    [Rank] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [AgreementID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[DP_AgreementUsers](
    [AgreementUsersID] [int] IDENTITY(1,1) NOT NULL,
    [AgreementID] [int] NULL,
    [UserId] [int] NULL,
    [LastUpdateDate] [datetime] NULL,
    [LastUpdateUser] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [AgreementUsersID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[DP_Attachments](
    [AttachmentId] [int] IDENTITY(1,1) NOT NULL,
    [DataProjectId] [int] NOT NULL,
    [Rank] [int] NOT NULL,
    [AttachmentData] [varbinary](max) NOT NULL,
    [AttachmentType] [varchar](max) NOT NULL,
    [AttachmentName] [varchar](max) NULL,
    [AttachmentSize] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [AttachmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[DP_Contact](
    [ContactID] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](max) NULL,
    [Email] [nvarchar](max) NULL,
    [Phone] [nvarchar](55) NULL,
    [Company] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [ContactID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[DP_Contact_Links](
    [LinkId] [int] IDENTITY(1,1) NOT NULL,
    [InitiativeId] [int] NULL,
    [ContactId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [LinkId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[DP_DataInitiative](
    [DataInitiativeID] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](max) NULL,
    [Description] [nvarchar](max) NULL,
    [OperationOwnerID] [int] NULL,
    [ExecutiveOwnerID] [int] NULL,
    [FinancialImpact] [int] NULL,
    [StrategicImportance] [int] NULL,
    [LastUpdateDate] [datetime] NULL,
    [LastUpdateUser] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [DataInitiativeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[DP_DataProject](
    [DataProjectID] [int] IDENTITY(1,1) NOT NULL,
    [DataInitiativeID] [int] NULL,
    [Name] [nvarchar](max) NULL,
    [Purpose] [nvarchar](max) NULL,
    [Description] [nvarchar](max) NULL,
    [OperationOwnerID] [int] NULL,
    [ExecutiveOwnerID] [int] NULL,
    [AnalyticsOwnerID] [int] NULL,
    [DataManagerID] [int] NULL,
    [FinancialImpact] [int] NULL,
    [StrategicImportance] [int] NULL,
    [ExternalDocumentationURL] [nvarchar](max) NULL,
    [LastUpdateDate] [datetime] NULL,
    [LastUpdateUser] [int] NULL,
    [Hidden] [nchar](1) NULL,
PRIMARY KEY CLUSTERED 
(
    [DataProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[Dp_DataProjectConversation](
    [DataProjectConversationId] [int] IDENTITY(1,1) NOT NULL,
    [DataProjectId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
    [DataProjectConversationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[Dp_DataProjectConversationMessage](
    [DataProjectConversationMessageId] [int] IDENTITY(1,1) NOT NULL,
    [DataProjectConversationId] [int] NULL,
    [UserId] [int] NULL,
    [MessageText] [nvarchar](4000) NULL,
    [PostDateTime] [datetime] NULL,
    [UserName] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [DataProjectConversationMessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[DP_MilestoneChecklist](
    [MilestoneChecklistId] [int] IDENTITY(1,1) NOT NULL,
    [MilestoneTaskId] [int] NULL,
    [Item] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [MilestoneChecklistId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[DP_MilestoneChecklistCompleted](
    [MilestoneChecklistCompletedId] [int] IDENTITY(1,1) NOT NULL,
    [DataProjectId] [int] NULL,
    [TaskDate] [datetime] NULL,
    [TaskId] [int] NULL,
    [MilestoneChecklistId] [int] NULL,
    [ChecklistStatus] [bit] NULL,
    [CompletionDate] [datetime] NULL,
    [CompletionUser] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [MilestoneChecklistCompletedId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[DP_MilestoneFrequency](
    [MilestoneTypeId] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](max) NULL,
    [LastUpdateUser] [int] NULL,
    [LastUpdateDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
    [MilestoneTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[DP_MilestoneTasks](
    [MilestoneTaskId] [int] IDENTITY(1,1) NOT NULL,
    [MilestoneTemplateId] [int] NULL,
    [OwnerId] [int] NULL,
    [Description] [nvarchar](max) NULL,
    [StartDate] [datetime] NULL,
    [EndDate] [datetime] NULL,
    [LastUpdateUser] [int] NULL,
    [LastUpdateDate] [datetime] NULL,
    [DataProjectId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [MilestoneTaskId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[DP_MilestoneTasksCompleted](
    [MilestoneTaskCompletedId] [int] IDENTITY(1,1) NOT NULL,
    [DataProjectId] [int] NULL,
    [CompletionDate] [datetime] NULL,
    [CompletionUser] [nvarchar](max) NULL,
    [Comments] [nvarchar](max) NULL,
    [Owner] [nvarchar](max) NULL,
    [DueDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
    [MilestoneTaskCompletedId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[DP_MilestoneTemplates](
    [MilestoneTemplateId] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](max) NULL,
    [MilestoneTypeId] [int] NULL,
    [LastUpdateUser] [int] NULL,
    [LastUpdateDate] [datetime] NULL,
    [Interval] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [MilestoneTemplateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[DP_ReportAnnotation](
    [ReportAnnotationID] [int] IDENTITY(1,1) NOT NULL,
    [Annotation] [nvarchar](max) NULL,
    [ReportId] [int] NULL,
    [DataProjectId] [int] NULL,
    [Rank] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [ReportAnnotationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[DP_TermAnnotation](
    [TermAnnotationID] [int] IDENTITY(1,1) NOT NULL,
    [Annotation] [nvarchar](max) NULL,
    [TermId] [int] NULL,
    [DataProjectId] [int] NULL,
    [Rank] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [TermAnnotationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[EstimatedRunFrequency](
    [EstimatedRunFrequencyID] [int] IDENTITY(1,1) NOT NULL,
    [EstimatedRunFrequencyName] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [EstimatedRunFrequencyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[FinancialImpact](
    [FinancialImpactId] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [FinancialImpactId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[Fragility](
    [FragilityID] [int] IDENTITY(1,1) NOT NULL,
    [FragilityName] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [FragilityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[FragilityTag](
    [FragilityTagID] [int] IDENTITY(1,1) NOT NULL,
    [FragilityTagName] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [FragilityTagID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[GlobalSiteSettings](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](max) NULL,
    [Description] [nvarchar](max) NULL,
    [Value] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[Mail_Conversations](
    [ConversationId] [int] IDENTITY(1,1) NOT NULL,
    [MessageId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
    [ConversationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[Mail_Drafts](
    [DraftId] [int] IDENTITY(1,1) NOT NULL,
    [Subject] [nvarchar](max) NULL,
    [Message] [nvarchar](max) NULL,
    [EditDate] [datetime] NULL,
    [MessageTypeId] [int] NULL,
    [FromUserId] [int] NULL,
    [MessagePlainText] [nvarchar](max) NULL,
    [Recipients] [nvarchar](max) NULL,
    [ReplyToMessageId] [int] NULL,
    [ReplyToConvId] [int] NULL,
 CONSTRAINT [PK_Mail_Drafts] PRIMARY KEY CLUSTERED 
(
    [DraftId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[Mail_FolderMessages](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [FolderId] [int] NULL,
    [MessageId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[Mail_Folders](
    [FolderId] [int] IDENTITY(1,1) NOT NULL,
    [ParentFolderId] [int] NULL,
    [UserId] [int] NULL,
    [Name] [nvarchar](max) NULL,
    [Rank] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [FolderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[Mail_Messages](
    [MessageId] [int] IDENTITY(1,1) NOT NULL,
    [Subject] [nvarchar](max) NULL,
    [Message] [nvarchar](max) NULL,
    [SendDate] [datetime] NULL,
    [MessageTypeId] [int] NULL,
    [FromUserId] [int] NULL,
    [MessagePlainText] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [MessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[Mail_MessageType](
    [MessageTypeId] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [MessageTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[Mail_Recipients](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [MessageId] [int] NULL,
    [ToUserId] [int] NULL,
    [ReadDate] [datetime] NULL,
    [AlertDisplayed] [int] NULL,
    [ToGroupId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[Mail_Recipients_Deleted](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [MessageId] [int] NULL,
    [ToUserId] [int] NULL,
    [ReadDate] [datetime] NULL,
    [AlertDisplayed] [int] NULL,
    [ToGroupId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[MaintenanceLog](
    [MaintenanceLogID] [int] IDENTITY(1,1) NOT NULL,
    [MaintainerID] [int] NULL,
    [MaintenanceDate] [datetime] NULL,
    [Comment] [nvarchar](max) NULL,
    [MaintenanceLogStatusID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [MaintenanceLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[MaintenanceLogStatus](
    [MaintenanceLogStatusID] [int] IDENTITY(1,1) NOT NULL,
    [MaintenanceLogStatusName] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
    [MaintenanceLogStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[MaintenanceSchedule](
    [MaintenanceScheduleID] [int] IDENTITY(1,1) NOT NULL,
    [MaintenanceScheduleName] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
    [MaintenanceScheduleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[OrganizationalValue](
    [OrganizationalValueID] [int] IDENTITY(1,1) NOT NULL,
    [OrganizationalValueName] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [OrganizationalValueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[ReportManageEngineTickets](
    [ManageEngineTicketsId] [int] IDENTITY(1,1) NOT NULL,
    [TicketNumber] [int] NULL,
    [Description] [nvarchar](max) NULL,
    [ReportObjectId] [int] NULL,
    [TicketUrl] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [ManageEngineTicketsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[ReportObject_doc](
    [ReportObjectID] [int] NOT NULL,
    [OperationalOwnerUserID] [int] NULL,
    [Requester] [int] NULL,
    [GitLabProjectURL] [nvarchar](max) NULL,
    [GitLabTreeURL] [nvarchar](max) NULL,
    [GitLabBlobURL] [nvarchar](max) NULL,
    [DeveloperDescription] [nvarchar](max) NULL,
    [KeyAssumptions] [nvarchar](max) NULL,
    [OrganizationalValueID] [int] NULL,
    [EstimatedRunFrequencyID] [int] NULL,
    [FragilityID] [int] NULL,
    [ExecutiveVisibilityYN] [nchar](1) NULL,
    [MaintenanceScheduleID] [int] NULL,
    [LastUpdateDateTime] [datetime] NULL,
    [CreatedDateTime] [datetime] NULL,
    [CreatedBy] [int] NULL,
    [UpdatedBy] [int] NULL,
    [EnabledForHyperspace] [nchar](1) NULL,
    [DoNotPurge] [nchar](1) NULL,
    [Hidden] [nchar](1) NULL,
PRIMARY KEY CLUSTERED 
(
    [ReportObjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[ReportObjectConversation_doc](
    [ConversationID] [int] IDENTITY(1,1) NOT NULL,
    [ReportObjectID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
    [ConversationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[ReportObjectConversationMessage_doc](
    [MessageID] [int] IDENTITY(1,1) NOT NULL,
    [ConversationID] [int] NOT NULL,
    [UserID] [int] NOT NULL,
    [MessageText] [nvarchar](max) NOT NULL,
    [PostDateTime] [datetime] NOT NULL,
    [Username] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [MessageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[ReportObjectDocFragilityTags](
    [ReportObjectID] [int] NOT NULL,
    [FragilityTagID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
    [ReportObjectID] ASC,
    [FragilityTagID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[ReportObjectDocMaintenanceLogs](
    [ReportObjectID] [int] NOT NULL,
    [MaintenanceLogID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
    [ReportObjectID] ASC,
    [MaintenanceLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[ReportObjectDocTerms](
    [ReportObjectID] [int] NOT NULL,
    [TermId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
    [ReportObjectID] ASC,
    [TermId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[ReportObjectImages_doc](
    [ImageID] [int] IDENTITY(1,1) NOT NULL,
    [ReportObjectID] [int] NOT NULL,
    [ImageOrdinal] [int] NOT NULL,
    [ImageData] [varbinary](max) NOT NULL,
    [ImageSource] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [ImageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[ReportObjectReportRunTime](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [ReportObjectId] [int] NULL,
    [Duration] [int] NULL,
    [Runs] [int] NULL,
    [RunWeek] [datetime] NULL,
    [RunWeekString] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[ReportObjectRunTime](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [RunUserId] [int] NULL,
    [Runs] [int] NULL,
    [RunTime] [decimal](10, 2) NULL,
    [RunWeek] [datetime] NULL,
    [RunWeekString] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[ReportObjectTopRuns](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [ReportObjectId] [int] NULL,
    [Name] [nvarchar](max) NULL,
    [RunUserId] [int] NULL,
    [Runs] [int] NULL,
    [RunTime] [decimal](10, 2) NULL,
    [LastRun] [nvarchar](max) NULL,
    [ReportObjectTypeId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[ReportObjectWeightedRunRank](
    [reportobjectid] [int] NOT NULL,
    [weighted_run_rank] [numeric](12, 4) NULL
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[RolePermissionLinks](
    [RolePermissionLinksId] [int] IDENTITY(1,1) NOT NULL,
    [RoleId] [int] NULL,
    [RolePermissionsId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [RolePermissionLinksId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[RolePermissions](
    [RolePermissionsId] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](max) NULL,
    [Description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [RolePermissionsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[Search_BasicSearchData](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [ItemId] [int] NULL,
    [TypeId] [int] NULL,
    [ItemType] [nvarchar](100) NULL,
    [ItemRank] [int] NULL,
    [SearchFieldDescription] [nvarchar](100) NULL,
    [SearchField] [nvarchar](max) NULL,
    [Hidden] [int] NULL,
    [VisibleType] [int] NULL,
    [Orphaned] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[Search_BasicSearchData_Small](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [ItemId] [int] NULL,
    [TypeId] [int] NULL,
    [ItemType] [nvarchar](100) NULL,
    [ItemRank] [int] NULL,
    [SearchFieldDescription] [nvarchar](100) NULL,
    [SearchField] [nvarchar](max) NULL,
    [Hidden] [int] NULL,
    [VisibleType] [int] NULL,
    [Orphaned] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[Search_ReportObjectSearchData](
    [pk] [int] IDENTITY(1,1) NOT NULL,
    [Id] [int] NOT NULL,
    [ColumnName] [nvarchar](max) NULL,
    [Value] [nvarchar](max) NULL,
    [LastModifiedDate] [datetime] NULL,
    [EpicMasterFile] [nvarchar](3) NULL,
    [DefaultVisibilityYN] [nvarchar](1) NULL,
    [OrphanedReportObjectYN] [nchar](1) NULL,
    [ReportObjectTypeID] [int] NULL,
    [AuthorUserId] [int] NULL,
    [LastModifiedByUserID] [int] NULL,
    [EpicReportTemplateId] [numeric](18, 0) NULL,
    [SourceServer] [nvarchar](255) NOT NULL,
    [SourceDB] [nvarchar](255) NOT NULL,
    [SourceTable] [nvarchar](255) NOT NULL,
    [Documented] [int] NOT NULL,
    [DocOwnerId] [int] NULL,
    [DocRequesterId] [int] NULL,
    [DocOrgValueId] [int] NULL,
    [DocRunFreqId] [int] NULL,
    [DocFragId] [int] NULL,
    [DocExecVis] [nchar](1) NULL,
    [DocMainSchedId] [int] NULL,
    [DocLastUpdated] [datetime] NULL,
    [DocCreated] [datetime] NULL,
    [DocCreatedBy] [int] NULL,
    [DocUpdatedBy] [int] NULL,
    [DocHypeEnabled] [nchar](1) NULL,
    [DocDoNotPurge] [nchar](1) NULL,
    [DocHidden] [nchar](1) NULL,
    [TwoYearRuns] [int] NULL,
    [OneYearRuns] [int] NULL,
    [SixMonthsRuns] [int] NULL,
    [OneMonthRuns] [int] NULL,
 CONSTRAINT [Search_ReportObjectSearchData_PK] PRIMARY KEY CLUSTERED 
(
    [pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[SearchTable](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [ItemId] [int] NULL,
    [TypeId] [int] NULL,
    [ItemType] [nvarchar](100) NULL,
    [ItemRank] [int] NULL,
    [SearchFieldDescription] [nvarchar](100) NULL,
    [SearchField] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[SharedItems](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [SharedFromUserId] [int] NULL,
    [SharedToUserId] [int] NULL,
    [Url] [nvarchar](max) NULL,
    [Name] [nvarchar](max) NULL,
    [ShareDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[StrategicImportance](
    [StrategicImportanceId] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [StrategicImportanceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[Term](
    [TermId] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](255) NULL,
    [Summary] [nvarchar](4000) NULL,
    [TechnicalDefinition] [nvarchar](max) NULL,
    [ApprovedYN] [char](1) NULL,
    [ApprovalDateTime] [datetime] NULL,
    [ApprovedByUserId] [int] NULL,
    [HasExternalStandardYN] [char](1) NULL,
    [ExternalStandardUrl] [nvarchar](4000) NULL,
    [ValidFromDateTime] [datetime] NULL,
    [ValidToDateTime] [datetime] NULL,
    [UpdatedByUserId] [int] NULL,
    [LastUpdatedDateTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
    [TermId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[TermConversation](
    [TermConversationId] [int] IDENTITY(1,1) NOT NULL,
    [TermId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
    [TermConversationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[TermConversationMessage](
    [TermConversationMessageID] [int] IDENTITY(1,1) NOT NULL,
    [TermConversationId] [int] NOT NULL,
    [UserId] [int] NOT NULL,
    [MessageText] [nvarchar](4000) NOT NULL,
    [PostDateTime] [datetime] NOT NULL,
    [UserName] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [TermConversationMessageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[User_NameData](
    [UserId] [int] NOT NULL,
    [Fullname] [nvarchar](max) NULL,
    [Firstname] [nvarchar](max) NULL,
    [Lastname] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[UserFavoriteFolders](
    [UserFavoriteFolderId] [int] IDENTITY(1,1) NOT NULL,
    [FolderName] [nvarchar](max) NULL,
    [UserId] [int] NULL,
    [FolderRank] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [UserFavoriteFolderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[UserFavorites](
    [UserFavoritesId] [int] IDENTITY(1,1) NOT NULL,
    [ItemType] [nvarchar](max) NULL,
    [ItemRank] [int] NULL,
    [ItemId] [int] NULL,
    [UserId] [int] NULL,
    [ItemName] [nvarchar](max) NULL,
    [FolderId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [UserFavoritesId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[UserPreferences](
    [UserPreferenceId] [int] IDENTITY(1,1) NOT NULL,
    [ItemType] [nvarchar](max) NULL,
    [ItemValue] [int] NULL,
    [ItemId] [int] NULL,
    [UserId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [UserPreferenceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[UserRoleLinks](
    [UserRoleLinksId] [int] IDENTITY(1,1) NOT NULL,
    [UserId] [int] NULL,
    [UserRolesId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [UserRoleLinksId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[UserRoles](
    [UserRolesId] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](max) NULL,
    [Description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [UserRolesId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
    [MigrationId] [nvarchar](150) NOT NULL,
    [ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
    [MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportGroupsMemberships](
    [MembershipId] [int] IDENTITY(1,1) NOT NULL,
    [GroupId] [int] NOT NULL,
    [ReportId] [int] NOT NULL,
    [LastLoadDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
    [MembershipId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportObject](
    [ReportObjectID] [int] IDENTITY(1,1) NOT NULL,
    [ReportObjectBizKey] [nvarchar](max) NULL,
    [SourceServer] [nvarchar](255) NOT NULL,
    [SourceDB] [nvarchar](255) NOT NULL,
    [SourceTable] [nvarchar](255) NOT NULL,
    [Name] [nvarchar](max) NULL,
    [Description] [nvarchar](max) NULL,
    [DetailedDescription] [nvarchar](max) NULL,
    [ReportObjectTypeID] [int] NULL,
    [AuthorUserID] [int] NULL,
    [LastModifiedByUserID] [int] NULL,
    [LastModifiedDate] [datetime] NULL,
    [ReportObjectURL] [nvarchar](max) NULL,
    [EpicMasterFile] [nvarchar](3) NULL,
    [EpicRecordID] [numeric](18, 0) NULL,
    [ReportServerCatalogID] [nvarchar](50) NULL,
    [DefaultVisibilityYN] [nvarchar](1) NULL,
    [OrphanedReportObjectYN] [nchar](1) NULL,
    [EpicReportTemplateId] [numeric](18, 0) NULL,
    [ReportServerPath] [nvarchar](max) NULL,
    [DisplayTitle] [nvarchar](max) NULL,
    [LastLoadDate] [datetime] NULL,
    [RepositoryDescription] [nvarchar](max) NULL,
    [EpicReleased] [nvarchar](1) NULL,
PRIMARY KEY CLUSTERED 
(
    [ReportObjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportObjectHierarchy](
    [ParentReportObjectID] [int] NOT NULL,
    [ChildReportObjectID] [int] NOT NULL,
    [Line] [int] NULL,
    [LastLoadDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
    [ParentReportObjectID] ASC,
    [ChildReportObjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportObjectQuery](
    [ReportObjectQueryId] [int] IDENTITY(1,1) NOT NULL,
    [ReportObjectId] [int] NULL,
    [Query] [nvarchar](max) NULL,
    [LastLoadDate] [datetime] NULL,
    [SourceServer] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [ReportObjectQueryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportObjectRunData](
    [ReportObjectID] [int] NOT NULL,
    [RunID] [int] NOT NULL,
    [RunUserID] [int] NULL,
    [RunStartTime] [datetime] NULL,
    [RunDurationSeconds] [int] NULL,
    [RunStatus] [nvarchar](100) NULL,
    [LastLoadDate] [datetime] NULL,
 CONSTRAINT [PK_ReportObjectRunData] PRIMARY KEY CLUSTERED 
(
    [ReportObjectID] ASC,
    [RunID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportObjectSubscriptions](
    [ReportObjectSubscriptionsId] [int] IDENTITY(1,1) NOT NULL,
    [ReportObjectId] [int] NULL,
    [UserId] [int] NULL,
    [SubscriptionId] [nvarchar](max) NULL,
    [InactiveFlags] [int] NULL,
    [EmailList] [nvarchar](max) NULL,
    [Description] [nvarchar](max) NULL,
    [LastStatus] [nvarchar](max) NULL,
    [LastRunTime] [datetime] NULL,
    [SubscriptionTo] [nvarchar](max) NULL,
    [LastLoadDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
    [ReportObjectSubscriptionsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportObjectType](
    [ReportObjectTypeID] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](max) NOT NULL,
    [DefaultEpicMasterFile] [nvarchar](3) NULL,
    [LastLoadDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
    [ReportObjectTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
    [UserID] [int] IDENTITY(1,1) NOT NULL,
    [Username] [nvarchar](max) NOT NULL,
    [EmployeeID] [nvarchar](max) NULL,
    [AccountName] [nvarchar](max) NULL,
    [DisplayName] [nvarchar](max) NULL,
    [FullName] [nvarchar](max) NULL,
    [FirstName] [nvarchar](max) NULL,
    [LastName] [nvarchar](max) NULL,
    [Department] [nvarchar](max) NULL,
    [Title] [nvarchar](max) NULL,
    [Phone] [nvarchar](max) NULL,
    [Email] [nvarchar](max) NULL,
    [Base] [nvarchar](max) NULL,
    [EpicId] [nvarchar](max) NULL,
    [LastLoadDate] [datetime] NULL,
    [LastLogin] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
    [UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserGroups](
    [GroupId] [int] IDENTITY(1,1) NOT NULL,
    [AccountName] [nvarchar](max) NULL,
    [GroupName] [nvarchar](max) NULL,
    [GroupEmail] [nvarchar](max) NULL,
    [GroupType] [nvarchar](max) NULL,
    [GroupSource] [nvarchar](max) NULL,
    [LastLoadDate] [datetime] NULL,
    [EpicId] [nvarchar](max) NULL,
 CONSTRAINT [PK_UserGroups] PRIMARY KEY CLUSTERED 
(
    [GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserGroupsMembership](
    [MembershipId] [int] IDENTITY(1,1) NOT NULL,
    [UserId] [int] NULL,
    [GroupId] [int] NULL,
    [LastLoadDate] [datetime] NULL,
 CONSTRAINT [PK_UserGroupsMembership] PRIMARY KEY CLUSTERED 
(
    [MembershipId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO

CREATE NONCLUSTERED INDEX [approved] ON [app].[Term]
(
    [ApprovedYN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [validfrom] ON [app].[Term]
(
    [ValidFromDateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [app].[DP_MilestoneChecklistCompleted] ADD  DEFAULT ((0)) FOR [ChecklistStatus]
GO
ALTER TABLE [app].[ReportObjectImages_doc] ADD  DEFAULT ((1)) FOR [ImageOrdinal]
GO
ALTER TABLE [app].[TermConversationMessage] ADD  DEFAULT (getdate()) FOR [PostDateTime]
GO
ALTER TABLE [dbo].[ReportObject] ADD  DEFAULT ('N') FOR [OrphanedReportObjectYN]
GO
ALTER TABLE [app].[Analytics]  WITH CHECK ADD  CONSTRAINT [FK_Analytics_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[Analytics] CHECK CONSTRAINT [FK_Analytics_User]
GO
ALTER TABLE [app].[DP_Agreement]  WITH CHECK ADD  CONSTRAINT [FK_DP_Agreement_DP_DataProject] FOREIGN KEY([DataProjectId])
REFERENCES [app].[DP_DataProject] ([DataProjectID])
GO
ALTER TABLE [app].[DP_Agreement] CHECK CONSTRAINT [FK_DP_Agreement_DP_DataProject]
GO
ALTER TABLE [app].[DP_Agreement]  WITH CHECK ADD  CONSTRAINT [FK_DP_Agreement_WebAppUsers] FOREIGN KEY([LastUpdateUser])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[DP_Agreement] CHECK CONSTRAINT [FK_DP_Agreement_WebAppUsers]
GO
ALTER TABLE [app].[DP_AgreementUsers]  WITH CHECK ADD  CONSTRAINT [FK_DP_AgreementUsers_DP_Agreement] FOREIGN KEY([AgreementID])
REFERENCES [app].[DP_Agreement] ([AgreementID])
GO
ALTER TABLE [app].[DP_AgreementUsers] CHECK CONSTRAINT [FK_DP_AgreementUsers_DP_Agreement]
GO
ALTER TABLE [app].[DP_AgreementUsers]  WITH CHECK ADD  CONSTRAINT [FK_DP_AgreementUsers_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[DP_AgreementUsers] CHECK CONSTRAINT [FK_DP_AgreementUsers_User]
GO
ALTER TABLE [app].[DP_AgreementUsers]  WITH CHECK ADD  CONSTRAINT [FK_DP_AgreementUsers_WebAppUsers] FOREIGN KEY([LastUpdateUser])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[DP_AgreementUsers] CHECK CONSTRAINT [FK_DP_AgreementUsers_WebAppUsers]
GO
ALTER TABLE [app].[DP_Attachments]  WITH CHECK ADD  CONSTRAINT [FK_DP_Attachments_DP_DataProject] FOREIGN KEY([DataProjectId])
REFERENCES [app].[DP_DataProject] ([DataProjectID])
GO
ALTER TABLE [app].[DP_Attachments] CHECK CONSTRAINT [FK_DP_Attachments_DP_DataProject]
GO
ALTER TABLE [app].[DP_Contact_Links]  WITH CHECK ADD  CONSTRAINT [FK_DP_Contact_Links_DP_Contact] FOREIGN KEY([ContactId])
REFERENCES [app].[DP_Contact] ([ContactID])
GO
ALTER TABLE [app].[DP_Contact_Links] CHECK CONSTRAINT [FK_DP_Contact_Links_DP_Contact]
GO
ALTER TABLE [app].[DP_Contact_Links]  WITH CHECK ADD  CONSTRAINT [FK_DP_Contact_Links_DP_DataInitiative] FOREIGN KEY([InitiativeId])
REFERENCES [app].[DP_DataInitiative] ([DataInitiativeID])
GO
ALTER TABLE [app].[DP_Contact_Links] CHECK CONSTRAINT [FK_DP_Contact_Links_DP_DataInitiative]
GO
ALTER TABLE [app].[DP_DataInitiative]  WITH CHECK ADD  CONSTRAINT [FK_DP_DataInitiative_FinancialImpact] FOREIGN KEY([FinancialImpact])
REFERENCES [app].[FinancialImpact] ([FinancialImpactId])
GO
ALTER TABLE [app].[DP_DataInitiative] CHECK CONSTRAINT [FK_DP_DataInitiative_FinancialImpact]
GO
ALTER TABLE [app].[DP_DataInitiative]  WITH CHECK ADD  CONSTRAINT [FK_DP_DataInitiative_StrategicImportance] FOREIGN KEY([StrategicImportance])
REFERENCES [app].[StrategicImportance] ([StrategicImportanceId])
GO
ALTER TABLE [app].[DP_DataInitiative] CHECK CONSTRAINT [FK_DP_DataInitiative_StrategicImportance]
GO
ALTER TABLE [app].[DP_DataInitiative]  WITH CHECK ADD  CONSTRAINT [FK_DP_DataInitiative_User] FOREIGN KEY([ExecutiveOwnerID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[DP_DataInitiative] CHECK CONSTRAINT [FK_DP_DataInitiative_User]
GO
ALTER TABLE [app].[DP_DataInitiative]  WITH CHECK ADD  CONSTRAINT [FK_DP_DataInitiative_User1] FOREIGN KEY([OperationOwnerID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[DP_DataInitiative] CHECK CONSTRAINT [FK_DP_DataInitiative_User1]
GO
ALTER TABLE [app].[DP_DataInitiative]  WITH CHECK ADD  CONSTRAINT [FK_DP_DataInitiative_WebAppUsers] FOREIGN KEY([LastUpdateUser])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[DP_DataInitiative] CHECK CONSTRAINT [FK_DP_DataInitiative_WebAppUsers]
GO
ALTER TABLE [app].[DP_DataProject]  WITH CHECK ADD  CONSTRAINT [FK_DP_DataProject_FinancialImpact] FOREIGN KEY([FinancialImpact])
REFERENCES [app].[FinancialImpact] ([FinancialImpactId])
GO
ALTER TABLE [app].[DP_DataProject] CHECK CONSTRAINT [FK_DP_DataProject_FinancialImpact]
GO
ALTER TABLE [app].[DP_DataProject]  WITH CHECK ADD  CONSTRAINT [FK_DP_DataProject_StrategicImportance] FOREIGN KEY([StrategicImportance])
REFERENCES [app].[StrategicImportance] ([StrategicImportanceId])
GO
ALTER TABLE [app].[DP_DataProject] CHECK CONSTRAINT [FK_DP_DataProject_StrategicImportance]
GO
ALTER TABLE [app].[DP_DataProject]  WITH CHECK ADD  CONSTRAINT [FK_DP_DataProject_User] FOREIGN KEY([ExecutiveOwnerID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[DP_DataProject] CHECK CONSTRAINT [FK_DP_DataProject_User]
GO
ALTER TABLE [app].[DP_DataProject]  WITH CHECK ADD  CONSTRAINT [FK_DP_DataProject_User1] FOREIGN KEY([OperationOwnerID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[DP_DataProject] CHECK CONSTRAINT [FK_DP_DataProject_User1]
GO
ALTER TABLE [app].[DP_DataProject]  WITH CHECK ADD  CONSTRAINT [FK_DP_DataProject_User2] FOREIGN KEY([DataManagerID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[DP_DataProject] CHECK CONSTRAINT [FK_DP_DataProject_User2]
GO
ALTER TABLE [app].[DP_DataProject]  WITH CHECK ADD  CONSTRAINT [FK_DP_DataProject_WebAppUsers] FOREIGN KEY([LastUpdateUser])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[DP_DataProject] CHECK CONSTRAINT [FK_DP_DataProject_WebAppUsers]
GO
ALTER TABLE [app].[DP_DataProject]  WITH CHECK ADD  CONSTRAINT [FK_DP_DataProject_WebAppUsers1] FOREIGN KEY([AnalyticsOwnerID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[DP_DataProject] CHECK CONSTRAINT [FK_DP_DataProject_WebAppUsers1]
GO
ALTER TABLE [app].[Dp_DataProjectConversation]  WITH CHECK ADD  CONSTRAINT [FK_Dp_DataProjectConversation_DP_DataProject] FOREIGN KEY([DataProjectId])
REFERENCES [app].[DP_DataProject] ([DataProjectID])
GO
ALTER TABLE [app].[Dp_DataProjectConversation] CHECK CONSTRAINT [FK_Dp_DataProjectConversation_DP_DataProject]
GO
ALTER TABLE [app].[Dp_DataProjectConversationMessage]  WITH CHECK ADD  CONSTRAINT [FK_Dp_DataProjectConversationMessage_Dp_DataProjectConversation] FOREIGN KEY([DataProjectConversationId])
REFERENCES [app].[Dp_DataProjectConversation] ([DataProjectConversationId])
GO
ALTER TABLE [app].[Dp_DataProjectConversationMessage] CHECK CONSTRAINT [FK_Dp_DataProjectConversationMessage_Dp_DataProjectConversation]
GO
ALTER TABLE [app].[Dp_DataProjectConversationMessage]  WITH CHECK ADD  CONSTRAINT [FK_Dp_DataProjectConversationMessage_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[Dp_DataProjectConversationMessage] CHECK CONSTRAINT [FK_Dp_DataProjectConversationMessage_User]
GO
ALTER TABLE [app].[DP_MilestoneChecklist]  WITH CHECK ADD  CONSTRAINT [FK_DP_MilestoneChecklist_DP_MilestoneTasks] FOREIGN KEY([MilestoneTaskId])
REFERENCES [app].[DP_MilestoneTasks] ([MilestoneTaskId])
GO
ALTER TABLE [app].[DP_MilestoneChecklist] CHECK CONSTRAINT [FK_DP_MilestoneChecklist_DP_MilestoneTasks]
GO
ALTER TABLE [app].[DP_MilestoneChecklistCompleted]  WITH CHECK ADD  CONSTRAINT [FK_DP_MilestoneChecklistCompleted_DP_DataProject] FOREIGN KEY([DataProjectId])
REFERENCES [app].[DP_DataProject] ([DataProjectID])
GO
ALTER TABLE [app].[DP_MilestoneChecklistCompleted] CHECK CONSTRAINT [FK_DP_MilestoneChecklistCompleted_DP_DataProject]
GO
ALTER TABLE [app].[DP_MilestoneChecklistCompleted]  WITH CHECK ADD  CONSTRAINT [FK_DP_MilestoneChecklistCompleted_User] FOREIGN KEY([CompletionUser])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[DP_MilestoneChecklistCompleted] CHECK CONSTRAINT [FK_DP_MilestoneChecklistCompleted_User]
GO
ALTER TABLE [app].[DP_MilestoneFrequency]  WITH CHECK ADD  CONSTRAINT [FK_DP_MilestoneTypes_WebAppUsers] FOREIGN KEY([LastUpdateUser])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[DP_MilestoneFrequency] CHECK CONSTRAINT [FK_DP_MilestoneTypes_WebAppUsers]
GO
ALTER TABLE [app].[DP_MilestoneTasks]  WITH CHECK ADD  CONSTRAINT [FK_DP_MilestoneTasks_DP_DataProject] FOREIGN KEY([DataProjectId])
REFERENCES [app].[DP_DataProject] ([DataProjectID])
GO
ALTER TABLE [app].[DP_MilestoneTasks] CHECK CONSTRAINT [FK_DP_MilestoneTasks_DP_DataProject]
GO
ALTER TABLE [app].[DP_MilestoneTasks]  WITH CHECK ADD  CONSTRAINT [FK_DP_MilestoneTasks_DP_MilestoneTemplates] FOREIGN KEY([MilestoneTemplateId])
REFERENCES [app].[DP_MilestoneTemplates] ([MilestoneTemplateId])
GO
ALTER TABLE [app].[DP_MilestoneTasks] CHECK CONSTRAINT [FK_DP_MilestoneTasks_DP_MilestoneTemplates]
GO
ALTER TABLE [app].[DP_MilestoneTasks]  WITH CHECK ADD  CONSTRAINT [FK_DP_MilestoneTasks_LastUpdateUser] FOREIGN KEY([LastUpdateUser])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[DP_MilestoneTasks] CHECK CONSTRAINT [FK_DP_MilestoneTasks_LastUpdateUser]
GO
ALTER TABLE [app].[DP_MilestoneTasks]  WITH CHECK ADD  CONSTRAINT [FK_DP_MilestoneTasks_User] FOREIGN KEY([OwnerId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[DP_MilestoneTasks] CHECK CONSTRAINT [FK_DP_MilestoneTasks_User]
GO
ALTER TABLE [app].[DP_MilestoneTasksCompleted]  WITH CHECK ADD  CONSTRAINT [FK_DP_MilestoneTasksCompleted_DP_DataProject] FOREIGN KEY([DataProjectId])
REFERENCES [app].[DP_DataProject] ([DataProjectID])
GO
ALTER TABLE [app].[DP_MilestoneTasksCompleted] CHECK CONSTRAINT [FK_DP_MilestoneTasksCompleted_DP_DataProject]
GO
ALTER TABLE [app].[DP_MilestoneTemplates]  WITH CHECK ADD  CONSTRAINT [FK_DP_MilestoneTemplates_DP_MilestoneTypes] FOREIGN KEY([MilestoneTypeId])
REFERENCES [app].[DP_MilestoneFrequency] ([MilestoneTypeId])
GO
ALTER TABLE [app].[DP_MilestoneTemplates] CHECK CONSTRAINT [FK_DP_MilestoneTemplates_DP_MilestoneTypes]
GO
ALTER TABLE [app].[DP_MilestoneTemplates]  WITH CHECK ADD  CONSTRAINT [FK_DP_MilestoneTemplates_WebAppUsers] FOREIGN KEY([LastUpdateUser])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[DP_MilestoneTemplates] CHECK CONSTRAINT [FK_DP_MilestoneTemplates_WebAppUsers]
GO
ALTER TABLE [app].[DP_ReportAnnotation]  WITH CHECK ADD  CONSTRAINT [FK_DP_ReportAnnotation_DP_DataProject] FOREIGN KEY([DataProjectId])
REFERENCES [app].[DP_DataProject] ([DataProjectID])
GO
ALTER TABLE [app].[DP_ReportAnnotation] CHECK CONSTRAINT [FK_DP_ReportAnnotation_DP_DataProject]
GO
ALTER TABLE [app].[DP_ReportAnnotation]  WITH CHECK ADD  CONSTRAINT [FK_DP_ReportAnnotation_ReportObject] FOREIGN KEY([ReportId])
REFERENCES [dbo].[ReportObject] ([ReportObjectID])
GO
ALTER TABLE [app].[DP_ReportAnnotation] CHECK CONSTRAINT [FK_DP_ReportAnnotation_ReportObject]
GO
ALTER TABLE [app].[DP_TermAnnotation]  WITH CHECK ADD  CONSTRAINT [FK_DP_TermAnnotation_DP_DataProject] FOREIGN KEY([DataProjectId])
REFERENCES [app].[DP_DataProject] ([DataProjectID])
GO
ALTER TABLE [app].[DP_TermAnnotation] CHECK CONSTRAINT [FK_DP_TermAnnotation_DP_DataProject]
GO
ALTER TABLE [app].[DP_TermAnnotation]  WITH CHECK ADD  CONSTRAINT [FK_DP_TermAnnotation_Term] FOREIGN KEY([TermId])
REFERENCES [app].[Term] ([TermId])
GO
ALTER TABLE [app].[DP_TermAnnotation] CHECK CONSTRAINT [FK_DP_TermAnnotation_Term]
GO
ALTER TABLE [app].[Mail_Conversations]  WITH CHECK ADD  CONSTRAINT [FK_Mail_Conversations_Mail_Messages] FOREIGN KEY([MessageId])
REFERENCES [app].[Mail_Messages] ([MessageId])
GO
ALTER TABLE [app].[Mail_Conversations] CHECK CONSTRAINT [FK_Mail_Conversations_Mail_Messages]
GO
ALTER TABLE [app].[Mail_Drafts]  WITH CHECK ADD  CONSTRAINT [FK_Mail_Drafts_User] FOREIGN KEY([FromUserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[Mail_Drafts] CHECK CONSTRAINT [FK_Mail_Drafts_User]
GO
ALTER TABLE [app].[Mail_FolderMessages]  WITH CHECK ADD  CONSTRAINT [FK_Mail_FolderMessages_Mail_Folders] FOREIGN KEY([FolderId])
REFERENCES [app].[Mail_Folders] ([FolderId])
GO
ALTER TABLE [app].[Mail_FolderMessages] CHECK CONSTRAINT [FK_Mail_FolderMessages_Mail_Folders]
GO
ALTER TABLE [app].[Mail_FolderMessages]  WITH CHECK ADD  CONSTRAINT [FK_Mail_FolderMessages_Mail_Messages] FOREIGN KEY([MessageId])
REFERENCES [app].[Mail_Messages] ([MessageId])
GO
ALTER TABLE [app].[Mail_FolderMessages] CHECK CONSTRAINT [FK_Mail_FolderMessages_Mail_Messages]
GO
ALTER TABLE [app].[Mail_Folders]  WITH CHECK ADD  CONSTRAINT [FK_Mail_Folders_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[Mail_Folders] CHECK CONSTRAINT [FK_Mail_Folders_User]
GO
ALTER TABLE [app].[Mail_Messages]  WITH CHECK ADD  CONSTRAINT [FK_Mail_Messages_Mail_MessageType] FOREIGN KEY([MessageTypeId])
REFERENCES [app].[Mail_MessageType] ([MessageTypeId])
GO
ALTER TABLE [app].[Mail_Messages] CHECK CONSTRAINT [FK_Mail_Messages_Mail_MessageType]
GO
ALTER TABLE [app].[Mail_Messages]  WITH CHECK ADD  CONSTRAINT [FK_Mail_Messages_User] FOREIGN KEY([FromUserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[Mail_Messages] CHECK CONSTRAINT [FK_Mail_Messages_User]
GO
ALTER TABLE [app].[Mail_Recipients]  WITH CHECK ADD  CONSTRAINT [FK_Mail_Recipients_Mail_Messages] FOREIGN KEY([MessageId])
REFERENCES [app].[Mail_Messages] ([MessageId])
GO
ALTER TABLE [app].[Mail_Recipients] CHECK CONSTRAINT [FK_Mail_Recipients_Mail_Messages]
GO
ALTER TABLE [app].[Mail_Recipients]  WITH CHECK ADD  CONSTRAINT [FK_Mail_Recipients_User] FOREIGN KEY([ToUserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[Mail_Recipients] CHECK CONSTRAINT [FK_Mail_Recipients_User]
GO
ALTER TABLE [app].[Mail_Recipients]  WITH CHECK ADD  CONSTRAINT [FK_Mail_Recipients_UserLDAPGroups] FOREIGN KEY([ToGroupId])
REFERENCES [dbo].[UserGroups] ([GroupId])
GO
ALTER TABLE [app].[Mail_Recipients] CHECK CONSTRAINT [FK_Mail_Recipients_UserLDAPGroups]
GO
ALTER TABLE [app].[Mail_Recipients_Deleted]  WITH CHECK ADD  CONSTRAINT [FK_Mail_Recipients_User1] FOREIGN KEY([ToUserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[Mail_Recipients_Deleted] CHECK CONSTRAINT [FK_Mail_Recipients_User1]
GO
ALTER TABLE [app].[MaintenanceLog]  WITH CHECK ADD FOREIGN KEY([MaintenanceLogStatusID])
REFERENCES [app].[MaintenanceLogStatus] ([MaintenanceLogStatusID])
ON DELETE CASCADE
GO
ALTER TABLE [app].[MaintenanceLog]  WITH CHECK ADD  CONSTRAINT [FK__Maintenan__Maint__65F62111] FOREIGN KEY([MaintainerID])
REFERENCES [dbo].[User] ([UserID])
ON DELETE CASCADE
GO
ALTER TABLE [app].[MaintenanceLog] CHECK CONSTRAINT [FK__Maintenan__Maint__65F62111]
GO
ALTER TABLE [app].[ReportManageEngineTickets]  WITH CHECK ADD  CONSTRAINT [FK_ReportManageEngineTickets_ReportObject] FOREIGN KEY([ReportObjectId])
REFERENCES [dbo].[ReportObject] ([ReportObjectID])
GO
ALTER TABLE [app].[ReportManageEngineTickets] CHECK CONSTRAINT [FK_ReportManageEngineTickets_ReportObject]
GO
ALTER TABLE [app].[ReportObject_doc]  WITH CHECK ADD FOREIGN KEY([EstimatedRunFrequencyID])
REFERENCES [app].[EstimatedRunFrequency] ([EstimatedRunFrequencyID])
GO
ALTER TABLE [app].[ReportObject_doc]  WITH CHECK ADD FOREIGN KEY([FragilityID])
REFERENCES [app].[Fragility] ([FragilityID])
GO
ALTER TABLE [app].[ReportObject_doc]  WITH CHECK ADD FOREIGN KEY([MaintenanceScheduleID])
REFERENCES [app].[MaintenanceSchedule] ([MaintenanceScheduleID])
GO
ALTER TABLE [app].[ReportObject_doc]  WITH CHECK ADD FOREIGN KEY([OperationalOwnerUserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[ReportObject_doc]  WITH CHECK ADD FOREIGN KEY([OrganizationalValueID])
REFERENCES [app].[OrganizationalValue] ([OrganizationalValueID])
GO
ALTER TABLE [app].[ReportObject_doc]  WITH CHECK ADD FOREIGN KEY([ReportObjectID])
REFERENCES [dbo].[ReportObject] ([ReportObjectID])
GO
ALTER TABLE [app].[ReportObject_doc]  WITH CHECK ADD FOREIGN KEY([Requester])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[ReportObject_doc]  WITH CHECK ADD  CONSTRAINT [FK_ReportObject_doc_User] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[ReportObject_doc] CHECK CONSTRAINT [FK_ReportObject_doc_User]
GO
ALTER TABLE [app].[ReportObjectConversation_doc]  WITH CHECK ADD FOREIGN KEY([ReportObjectID])
REFERENCES [dbo].[ReportObject] ([ReportObjectID])
GO
ALTER TABLE [app].[ReportObjectConversationMessage_doc]  WITH CHECK ADD FOREIGN KEY([ConversationID])
REFERENCES [app].[ReportObjectConversation_doc] ([ConversationID])
GO
ALTER TABLE [app].[ReportObjectConversationMessage_doc]  WITH CHECK ADD  CONSTRAINT [FK_ReportObjectConversationMessage_doc_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[ReportObjectConversationMessage_doc] CHECK CONSTRAINT [FK_ReportObjectConversationMessage_doc_User]
GO
ALTER TABLE [app].[ReportObjectDocFragilityTags]  WITH CHECK ADD FOREIGN KEY([FragilityTagID])
REFERENCES [app].[FragilityTag] ([FragilityTagID])
ON DELETE CASCADE
GO
ALTER TABLE [app].[ReportObjectDocFragilityTags]  WITH CHECK ADD FOREIGN KEY([ReportObjectID])
REFERENCES [app].[ReportObject_doc] ([ReportObjectID])
ON DELETE CASCADE
GO
ALTER TABLE [app].[ReportObjectDocMaintenanceLogs]  WITH CHECK ADD FOREIGN KEY([MaintenanceLogID])
REFERENCES [app].[MaintenanceLog] ([MaintenanceLogID])
ON DELETE CASCADE
GO
ALTER TABLE [app].[ReportObjectDocMaintenanceLogs]  WITH CHECK ADD FOREIGN KEY([ReportObjectID])
REFERENCES [app].[ReportObject_doc] ([ReportObjectID])
ON DELETE CASCADE
GO
ALTER TABLE [app].[ReportObjectDocTerms]  WITH CHECK ADD FOREIGN KEY([ReportObjectID])
REFERENCES [app].[ReportObject_doc] ([ReportObjectID])
ON DELETE CASCADE
GO
ALTER TABLE [app].[ReportObjectDocTerms]  WITH CHECK ADD FOREIGN KEY([TermId])
REFERENCES [app].[Term] ([TermId])
ON DELETE CASCADE
GO
ALTER TABLE [app].[ReportObjectImages_doc]  WITH CHECK ADD FOREIGN KEY([ReportObjectID])
REFERENCES [dbo].[ReportObject] ([ReportObjectID])
GO
ALTER TABLE [app].[ReportObjectReportRunTime]  WITH CHECK ADD  CONSTRAINT [fk_reportruntime] FOREIGN KEY([ReportObjectId])
REFERENCES [dbo].[ReportObject] ([ReportObjectID])
GO
ALTER TABLE [app].[ReportObjectReportRunTime] CHECK CONSTRAINT [fk_reportruntime]
GO
ALTER TABLE [app].[ReportObjectRunTime]  WITH CHECK ADD  CONSTRAINT [fk_userruntime] FOREIGN KEY([RunUserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[ReportObjectRunTime] CHECK CONSTRAINT [fk_userruntime]
GO
ALTER TABLE [app].[ReportObjectTopRuns]  WITH CHECK ADD  CONSTRAINT [FK_ReportObjectTopRuns_ReportObject] FOREIGN KEY([ReportObjectId])
REFERENCES [dbo].[ReportObject] ([ReportObjectID])
GO
ALTER TABLE [app].[ReportObjectTopRuns] CHECK CONSTRAINT [FK_ReportObjectTopRuns_ReportObject]
GO
ALTER TABLE [app].[ReportObjectTopRuns]  WITH CHECK ADD  CONSTRAINT [fk_user] FOREIGN KEY([RunUserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[ReportObjectTopRuns] CHECK CONSTRAINT [fk_user]
GO
ALTER TABLE [app].[ReportObjectWeightedRunRank]  WITH CHECK ADD  CONSTRAINT [FK_ReportObjectWeightedRunRank_ReportObject] FOREIGN KEY([reportobjectid])
REFERENCES [dbo].[ReportObject] ([ReportObjectID])
GO
ALTER TABLE [app].[ReportObjectWeightedRunRank] CHECK CONSTRAINT [FK_ReportObjectWeightedRunRank_ReportObject]
GO
ALTER TABLE [app].[RolePermissionLinks]  WITH CHECK ADD  CONSTRAINT [FK_RolePermissionLinks_RolePermissions] FOREIGN KEY([RolePermissionsId])
REFERENCES [app].[RolePermissions] ([RolePermissionsId])
GO
ALTER TABLE [app].[RolePermissionLinks] CHECK CONSTRAINT [FK_RolePermissionLinks_RolePermissions]
GO
ALTER TABLE [app].[RolePermissionLinks]  WITH CHECK ADD  CONSTRAINT [FK_RolePermissionLinks_UserRoles] FOREIGN KEY([RoleId])
REFERENCES [app].[UserRoles] ([UserRolesId])
GO
ALTER TABLE [app].[RolePermissionLinks] CHECK CONSTRAINT [FK_RolePermissionLinks_UserRoles]
GO
ALTER TABLE [app].[SharedItems]  WITH CHECK ADD  CONSTRAINT [FK_SharedItems_User] FOREIGN KEY([SharedFromUserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[SharedItems] CHECK CONSTRAINT [FK_SharedItems_User]
GO
ALTER TABLE [app].[SharedItems]  WITH CHECK ADD  CONSTRAINT [FK_SharedItems_User1] FOREIGN KEY([SharedToUserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[SharedItems] CHECK CONSTRAINT [FK_SharedItems_User1]
GO
ALTER TABLE [app].[Term]  WITH CHECK ADD  CONSTRAINT [FK_Term_WebAppUsers] FOREIGN KEY([UpdatedByUserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[Term] CHECK CONSTRAINT [FK_Term_WebAppUsers]
GO
ALTER TABLE [app].[Term]  WITH CHECK ADD  CONSTRAINT [FK_Term_WebAppUsers1] FOREIGN KEY([ApprovedByUserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[Term] CHECK CONSTRAINT [FK_Term_WebAppUsers1]
GO
ALTER TABLE [app].[TermConversation]  WITH CHECK ADD FOREIGN KEY([TermId])
REFERENCES [app].[Term] ([TermId])
ON DELETE CASCADE
GO
ALTER TABLE [app].[TermConversationMessage]  WITH CHECK ADD  CONSTRAINT [FK_TermConversationMessage_TermConversation] FOREIGN KEY([TermConversationId])
REFERENCES [app].[TermConversation] ([TermConversationId])
GO
ALTER TABLE [app].[TermConversationMessage] CHECK CONSTRAINT [FK_TermConversationMessage_TermConversation]
GO
ALTER TABLE [app].[TermConversationMessage]  WITH CHECK ADD  CONSTRAINT [FK_TermConversationMessage_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[TermConversationMessage] CHECK CONSTRAINT [FK_TermConversationMessage_User]
GO
ALTER TABLE [app].[User_NameData]  WITH CHECK ADD  CONSTRAINT [FK_User_NameData_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[User_NameData] CHECK CONSTRAINT [FK_User_NameData_User]
GO
ALTER TABLE [app].[UserFavorites]  WITH CHECK ADD  CONSTRAINT [FK_UserFavorites_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[UserFavorites] CHECK CONSTRAINT [FK_UserFavorites_User]
GO
ALTER TABLE [app].[UserFavorites]  WITH CHECK ADD  CONSTRAINT [FK_UserFavorites_UserFavoriteFolders] FOREIGN KEY([FolderId])
REFERENCES [app].[UserFavoriteFolders] ([UserFavoriteFolderId])
GO
ALTER TABLE [app].[UserFavorites] CHECK CONSTRAINT [FK_UserFavorites_UserFavoriteFolders]
GO
ALTER TABLE [app].[UserPreferences]  WITH CHECK ADD  CONSTRAINT [FK_UserPreferences_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[UserPreferences] CHECK CONSTRAINT [FK_UserPreferences_User]
GO
ALTER TABLE [app].[UserRoleLinks]  WITH CHECK ADD  CONSTRAINT [FK_UserRoleLinks_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[UserRoleLinks] CHECK CONSTRAINT [FK_UserRoleLinks_User]
GO
ALTER TABLE [app].[UserRoleLinks]  WITH CHECK ADD  CONSTRAINT [FK_UserRoleLinks_UserRoles] FOREIGN KEY([UserRolesId])
REFERENCES [app].[UserRoles] ([UserRolesId])
GO
ALTER TABLE [app].[UserRoleLinks] CHECK CONSTRAINT [FK_UserRoleLinks_UserRoles]
GO
ALTER TABLE [dbo].[ReportGroupsMemberships]  WITH CHECK ADD  CONSTRAINT [FK_ReportGroupsMemberships_ReportObject] FOREIGN KEY([ReportId])
REFERENCES [dbo].[ReportObject] ([ReportObjectID])
GO
ALTER TABLE [dbo].[ReportGroupsMemberships] CHECK CONSTRAINT [FK_ReportGroupsMemberships_ReportObject]
GO
ALTER TABLE [dbo].[ReportGroupsMemberships]  WITH CHECK ADD  CONSTRAINT [FK_ReportGroupsMemberships_UserGroups] FOREIGN KEY([GroupId])
REFERENCES [dbo].[UserGroups] ([GroupId])
GO
ALTER TABLE [dbo].[ReportGroupsMemberships] CHECK CONSTRAINT [FK_ReportGroupsMemberships_UserGroups]
GO
ALTER TABLE [dbo].[ReportObject]  WITH CHECK ADD FOREIGN KEY([AuthorUserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[ReportObject]  WITH CHECK ADD FOREIGN KEY([LastModifiedByUserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[ReportObject]  WITH CHECK ADD FOREIGN KEY([ReportObjectTypeID])
REFERENCES [dbo].[ReportObjectType] ([ReportObjectTypeID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[ReportObjectHierarchy]  WITH CHECK ADD FOREIGN KEY([ChildReportObjectID])
REFERENCES [dbo].[ReportObject] ([ReportObjectID])
GO
ALTER TABLE [dbo].[ReportObjectHierarchy]  WITH CHECK ADD FOREIGN KEY([ParentReportObjectID])
REFERENCES [dbo].[ReportObject] ([ReportObjectID])
GO
ALTER TABLE [dbo].[ReportObjectQuery]  WITH CHECK ADD  CONSTRAINT [FK_ReportObjectQuery_ReportObject] FOREIGN KEY([ReportObjectId])
REFERENCES [dbo].[ReportObject] ([ReportObjectID])
GO
ALTER TABLE [dbo].[ReportObjectQuery] CHECK CONSTRAINT [FK_ReportObjectQuery_ReportObject]
GO
ALTER TABLE [dbo].[ReportObjectRunData]  WITH CHECK ADD FOREIGN KEY([ReportObjectID])
REFERENCES [dbo].[ReportObject] ([ReportObjectID])
GO
ALTER TABLE [dbo].[ReportObjectRunData]  WITH CHECK ADD FOREIGN KEY([RunUserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[ReportObjectSubscriptions]  WITH CHECK ADD  CONSTRAINT [FK_ReportObjectSubscriptions_ReportObject] FOREIGN KEY([ReportObjectId])
REFERENCES [dbo].[ReportObject] ([ReportObjectID])
GO
ALTER TABLE [dbo].[ReportObjectSubscriptions] CHECK CONSTRAINT [FK_ReportObjectSubscriptions_ReportObject]
GO
ALTER TABLE [dbo].[ReportObjectSubscriptions]  WITH CHECK ADD  CONSTRAINT [FK_ReportObjectSubscriptions_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[ReportObjectSubscriptions] CHECK CONSTRAINT [FK_ReportObjectSubscriptions_User]
GO
ALTER TABLE [dbo].[UserGroupsMembership]  WITH CHECK ADD  CONSTRAINT [FK_UserGroupsMembership_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[UserGroupsMembership] CHECK CONSTRAINT [FK_UserGroupsMembership_User]
GO
ALTER TABLE [dbo].[UserGroupsMembership]  WITH CHECK ADD  CONSTRAINT [FK_UserGroupsMembership_UserGroups] FOREIGN KEY([GroupId])
REFERENCES [dbo].[UserGroups] ([GroupId])
GO
ALTER TABLE [dbo].[UserGroupsMembership] CHECK CONSTRAINT [FK_UserGroupsMembership_UserGroups]
GO


-- create report object id

  insert into [dbo].[ReportObjectType] ([Name], [DefaultEpicMasterFile]) values
    
    ('Application Report','HRX'),
('Application Report Template','HGR'),
('Epic-Crystal Report','HRX'),
('Epic-Crystal Template','HGR'),
('Epic-WebI Report','HRX'),
('Epic-WebI Template','HGR'),
('External Crystal Template','HGR'),
('External WebI Template','HGR'),
('Other HGR Template','HGR'),
('Other HRX Report','HRX'),
('Other Radar Dashboard','IDM'),
('Personalization Radar Dashboard','IDM'),
('Personalization Radar Dashboard Component','IDB'),
('Radar Dashboard Resource','IDK'),
('Radar Metric','IDN'),
('Redirector Radar Dashboard','IDM'),
('Reporting Workbench Report','HRX'),
('Reporting Workbench Template','HGR'),
('SlicerDicer Filter','FDS'),
('Source Radar Dashboard','IDM'),
('Source Radar Dashboard Component','IDB'),
('SSRS Datasource',null),
('SSRS File',null),
('SSRS Folder',null),
('SSRS KPI',null),
('SSRS Linked Report',null),
('SSRS Mobile Report (folder)',null),
('SSRS Report',null),
('SSRS Shared Dataset',null),
('User Created Radar Dashboard','IDM'),
('SSRS Report Part',null),
('Other Radar Dashboard Component','IDB'),
('User Created Radar Dashboard Component','IDB'),
('Datalink Template','HGR'),
('Datalink Report','HRX')



/****** Object:  FullTextIndex     Script Date: 11/19/2020 1:51:21 PM ******/
/*CREATE FULLTEXT INDEX ON [app].[ReportObject_doc](
[DeveloperDescription] LANGUAGE 'English', 
[KeyAssumptions] LANGUAGE 'English')
KEY INDEX [PK__ReportOb__B7A74135D2A44EFC]ON ([ReportObjectDocs], FILEGROUP [PRIMARY])
WITH (CHANGE_TRACKING = AUTO, STOPLIST = OFF)
*/
declare @i nvarchar(max) = (select
                                'CREATE FULLTEXT INDEX ON [app].[ReportObject_doc]([DeveloperDescription] LANGUAGE ''English'',[KeyAssumptions] LANGUAGE ''English'')KEY INDEX [' + i.name + ']ON ([ReportObjectDocs], FILEGROUP [PRIMARY]) WITH (CHANGE_TRACKING = AUTO, STOPLIST = OFF)'
                            from sys.tables t
                                INNER JOIN sys.indexes i ON t.object_id = i.object_id
                            where 
                                i.index_id = 1  -- clustered index
                                and t.name = 'ReportObject_doc')


exec sp_executesql @i
GO
/****** Object:  FullTextIndex     Script Date: 11/19/2020 1:51:21 PM ******/
/*
CREATE FULLTEXT INDEX ON [app].[Search_BasicSearchData](
[SearchField] LANGUAGE 'English')
KEY INDEX [PK__Search_B__3214EC07DFB24C4E]ON ([Search], FILEGROUP [PRIMARY])
WITH (CHANGE_TRACKING = AUTO, STOPLIST = OFF)
*/
declare @i nvarchar(max) = (select
                                'CREATE FULLTEXT INDEX ON [app].[Search_BasicSearchData]([SearchField] LANGUAGE ''English'')KEY INDEX [' + i.name + ']ON ([Search], FILEGROUP [PRIMARY]) WITH (CHANGE_TRACKING = AUTO, STOPLIST = OFF)'
                            from sys.tables t
                                INNER JOIN sys.indexes i ON t.object_id = i.object_id
                            where 
                                i.index_id = 1  -- clustered index
                                and t.name = 'Search_BasicSearchData')
exec sp_executesql @i

GO
/****** Object:  FullTextIndex     Script Date: 11/19/2020 1:51:21 PM ******/
/*
CREATE FULLTEXT INDEX ON [app].[Search_BasicSearchData_Small](
[SearchField] LANGUAGE 'English')
KEY INDEX [PK__Search_B__3214EC07D8479892]ON ([Search_Small], FILEGROUP [PRIMARY])
WITH (CHANGE_TRACKING = AUTO, STOPLIST = SYSTEM)
*/
declare @i nvarchar(max) = (select
                                'CREATE FULLTEXT INDEX ON [app].[Search_BasicSearchData_Small]([SearchField] LANGUAGE ''English'')KEY INDEX [' + i.name + ']ON ([Search_Small], FILEGROUP [PRIMARY]) WITH (CHANGE_TRACKING = AUTO, STOPLIST = SYSTEM)'
                            from sys.tables t
                                INNER JOIN sys.indexes i ON t.object_id = i.object_id
                            where 
                                i.index_id = 1  -- clustered index
                                and t.name = 'Search_BasicSearchData_Small')
exec sp_executesql @i
GO
/****** Object:  FullTextIndex     Script Date: 11/19/2020 1:51:21 PM ******/
/*CREATE FULLTEXT INDEX ON [app].[Term](
[Name] LANGUAGE 'English', 
[Summary] LANGUAGE 'English', 
[TechnicalDefinition] LANGUAGE 'English')
KEY INDEX [PK__Term__410A21A5A6BE1546]ON ([Terms], FILEGROUP [PRIMARY])
WITH (CHANGE_TRACKING = AUTO, STOPLIST = OFF)
*/
declare @i nvarchar(max) = (select
                                'CREATE FULLTEXT INDEX ON [app].[Term]([Name] LANGUAGE ''English'',[Summary] LANGUAGE ''English'',[TechnicalDefinition] LANGUAGE ''English'')KEY INDEX [' + i.name + ']ON ([Terms], FILEGROUP [PRIMARY]) WITH (CHANGE_TRACKING = AUTO, STOPLIST = OFF)'
                            from sys.tables t
                                INNER JOIN sys.indexes i ON t.object_id = i.object_id
                            where 
                                i.index_id = 1  -- clustered index
                                and t.name = 'Term')
exec sp_executesql @i

GO
USE [atlas]
GO
/****** Object:  FullTextIndex     Script Date: 11/19/2020 1:51:21 PM ******/
/*CREATE FULLTEXT INDEX ON [app].[User_NameData](
[Fullname] LANGUAGE 'English')
KEY INDEX [PK__User_Nam__1788CC4CF297F9FC]ON ([User_NameData], FILEGROUP [PRIMARY])
WITH (CHANGE_TRACKING = AUTO, STOPLIST = SYSTEM)
*/
declare @i nvarchar(max) = (select
                                'CREATE FULLTEXT INDEX ON [app].[User_NameData]([Fullname] LANGUAGE ''English'')KEY INDEX [' + i.name + ']ON ([User_NameData], FILEGROUP [PRIMARY]) WITH (CHANGE_TRACKING = AUTO, STOPLIST = SYSTEM)'
                            from sys.tables t
                                INNER JOIN sys.indexes i ON t.object_id = i.object_id
                            where 
                                i.index_id = 1  -- clustered index
                                and t.name = 'User_NameData')
exec sp_executesql @i

GO

/****** Object:  FullTextIndex     Script Date: 11/19/2020 1:51:21 PM ******/
/*CREATE FULLTEXT INDEX ON [app].[UserLDAPGroups](
[GroupName] LANGUAGE 'English')
KEY INDEX [PK__UserLDAP__149AF36ADDF529F9]ON ([UserGroups], FILEGROUP [PRIMARY])
WITH (CHANGE_TRACKING = AUTO, STOPLIST = SYSTEM)
*/
declare @i nvarchar(max) = (select
                                'CREATE FULLTEXT INDEX ON [dbo].[UserGroups]([GroupName] LANGUAGE ''English'')KEY INDEX [' + i.name + ']ON ([UserGroups], FILEGROUP [PRIMARY]) WITH (CHANGE_TRACKING = AUTO, STOPLIST = SYSTEM)'
                            from sys.tables t
                                INNER JOIN sys.indexes i ON t.object_id = i.object_id
                            where 
                                i.index_id = 1  -- clustered index
                                and t.name = 'UserGroups')
exec sp_executesql @i
GO
/****** Object:  FullTextIndex     Script Date: 11/19/2020 1:51:21 PM ******/
/*CREATE FULLTEXT INDEX ON [dbo].[ReportObject](
[Description] LANGUAGE 'English', 
[DetailedDescription] LANGUAGE 'English', 
[Name] LANGUAGE 'English')
KEY INDEX [PK__ReportOb__B7A741355058A44F]ON ([ReportObjects], FILEGROUP [PRIMARY])
WITH (CHANGE_TRACKING = AUTO, STOPLIST = OFF)
*/
declare @i nvarchar(max) = (select
                                'CREATE FULLTEXT INDEX ON [dbo].[ReportObject]([Description] LANGUAGE ''English'',[DetailedDescription] LANGUAGE ''English'',[Name] LANGUAGE ''English'')KEY INDEX [' + i.name + ']ON ([ReportObjects], FILEGROUP [PRIMARY]) WITH (CHANGE_TRACKING = AUTO, STOPLIST = OFF)'
                            from sys.tables t
                                INNER JOIN sys.indexes i ON t.object_id = i.object_id
                            where 
                                i.index_id = 1  -- clustered index
                                and t.name = 'ReportObject')
exec sp_executesql @i
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:      Christopher Pickering
-- Create date: 1/3/2020
-- Description: Used to update SearchTable
-- =============================================
CREATE PROCEDURE [app].[CalculateReportRunData]
AS
BEGIN 
    -- ================================================
    /*
       package used to update the search fields used 
       by the index for Altas search.
    */
    -- ================================================
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

   

-- create view of current data
    drop table if exists #myTemp;

    select d.ReportObjectId, isnull(r.displayTitle,r.Name) as Name, RunUserId, Count(*) as Runs, Cast(ROUND(Avg(cast(RunDurationSeconds as decimal)),2)as decimal(10,2))  as RunTime,convert(nvarchar(MAX),  Max(d.RunStartTime), 101) as LastRun, r.ReportObjectTypeID
    into #myTemp
    from ReportObjectRunData as d
    inner join ReportObject as r on r.ReportObjectId = d.ReportObjectId
    where RunStatus = 'Success'
    group by d.ReportObjectId, RunUserId,isnull(r.displayTitle,r.Name), r.ReportObjectTypeId
        
    ;
            

    --drop table if exists app.ReportObjectTopRuns;
    if not exists (select * from sysobjects where name='ReportObjectTopRuns' and xtype='U')
        exec('create table app.ReportObjectTopRuns
        (
            Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
            ReportObjectId int,
            Name nvarchar(max),
            RunUserId int,
            Runs int,
            RunTime decimal(10,2),
            LastRun nvarchar(max),
            ReportObjectTypeId int null,
            constraint fk_user foreign key (RunUserId) references [User] (UserId)
        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
        ')
    ;

    -- insert new records into table.
    insert into app.ReportObjectTopRuns
        select
            ReportObjectId,
            Name,
            RunUserId,
            Runs,
            cast(RunTime as decimal(10,2)),
            LastRun,
            ReportObjectTypeId
        from 
            #myTemp
        except (select ReportObjectId,
            Name,
            RunUserId,
            Runs,
            cast(RunTime as decimal(10,2)),
            LastRun,
            ReportObjectTypeId from app.ReportObjectTopRuns)
    ;

    -- delete old records from table
    delete app.ReportObjectTopRuns from app.ReportObjectTopRuns  l
    left join #myTemp as t on l.ReportObjectId = t.ReportObjectId
        and l.Name = t.Name
        and l.RunUserId = t.RunUserId
        and l.Runs = t.Runs
        and l.RunTime = t.RunTime
        and l.LastRun = t.LastRun
        and l.ReportObjectTypeId = t.ReportObjectTypeId
    where t.ReportObjectID is null

    ;

    drop table if exists #myTemp;

    -- report run rank

    truncate table app.ReportObjectWeightedRunRank;

    insert into  app.ReportObjectWeightedRunRank
    select 
    reportobjectid
    , CAST(runs * 10.000/max(runs) over () AS NUMERIC(12,4)) weighted_run_rank
        from (
    select cast(sum(weighted_runs)/16.000 as NUMERIC(12,4)) as runs, reportobjectid
    from (select reportobjectid
     , count(1) runs
     , (16 - (DATEPART(week, getdate()) - DATEPART(week, RunStartTime))) * count(1) weighted_runs
    from dbo.ReportObjectRunData r
    where FORMAT(RunStartTime, 'yyyy-MM-01 00:00:00.000') >= dateadd(WEEK,-16,getdate())
    group by reportobjectid, (16 - (DATEPART(week, getdate()) - DATEPART(week, RunStartTime)))) as t group by reportobjectid
    ) as t
    order by 2 desc
End
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:      Christopher Pickering
-- Create date: 1/3/2020
-- Description: Used to update SearchTable
-- =============================================
CREATE PROCEDURE [app].[CalculateReportRunTimeData]
AS
BEGIN 
    -- ================================================
    /*
       package used to update the search fields used 
       by the index for Altas search.
    */
    -- ================================================
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

   

-- create view of current data
    drop table if exists #myTemp;

    select RunUserId, Count(*) as Runs, Cast(ROUND(Avg(cast(RunDurationSeconds as decimal)),2)as decimal(10,2))  as RunTime,
    DATEADD(dd, DATEPART(DW,RunStartTime)*-1+1, DATEADD(dd, DATEDIFF(dd, 0, RunStartTime), 0)) as RunWeek, 
convert(nvarchar(MAX),  DATEADD(dd, DATEPART(DW,RunStartTime)*-1+1, DATEADD(dd, DATEDIFF(dd, 0, RunStartTime), 0)), 101) as RunWeekString
    into #myTemp
    from ReportObjectRunData as d
    inner join ReportObject r on d.ReportObjectID = r.ReportObjectID
    where RunStatus = 'Success'
      and r.ReportObjectTypeId not in (20,21)
    group by DATEADD(dd, DATEPART(DW,RunStartTime)*-1+1, DATEADD(dd, DATEDIFF(dd, 0, RunStartTime), 0)), RunUserId
        
    ;
        

    --drop table if exists app.ReportObjectRunTime;
    if not exists (select * from sysobjects where name='ReportObjectRunTime' and xtype='U')
        exec('create table app.ReportObjectRunTime
        (
            Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
            RunUserId int,
            Runs int,
            RunTime decimal(10,2),
            RunWeek datetime,
            RunWeekString nvarchar(max),
            constraint fk_userruntime foreign key (RunUserId) references [User] (UserId)
        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
        ')
    ;

    -- insert new records into table.
    insert into app.ReportObjectRunTime
        select
            RunUserId,
            Runs,
            cast(RunTime as Decimal(10,2)),
            RunWeek,
            RunWeekString
        from 
            #myTemp
        except (select 
            RunUserId,
            Runs,
            cast(RunTime as Decimal(10,2)),
            RunWeek,
            RunWeekString from app.ReportObjectRunTime)
    ;

    -- delete old records from table
    delete app.ReportObjectRunTime 
    from app.ReportObjectRunTime  l
    left outer join #myTemp as t on l.RunUserId = t.RunUserId
        and l.Runs = t.Runs
        and l.RunTime = t.RunTime
        and l.RunWeek = t.RunWeek
        and l.RunWeekString = t.RunWeekString
    where t.RunUserId is null
    ;

    drop table if exists #myTemp;

    -- run data from report perspecitve
    if not exists (select * from sysobjects where name='ReportObjectReportRunTime' and xtype='U')
        exec('create table app.ReportObjectReportRunTime
        (
            Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
            ReportObjectId int,
            Duration int,
            Runs int,
            RunWeek datetime,
            RunWeekString nvarchar(max),
            constraint fk_reportruntime foreign key (ReportObjectId) references [ReportObject] (ReportObjectID)
        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
        ')
    ;
    delete from app.ReportObjectReportRunTime where 1=1;
    insert into app.ReportObjectReportRunTime
        select
              ReportObjectID,
              Avg(RunDurationSeconds) Duration,
              count(*) Runs,
              DATEADD(
                week,
                DATEDIFF(week, -1, RunStartTime),
                -1
              ) AS RunWeek,
              convert(
                nvarchar(MAX),
                DATEADD(
                  week,
                  DATEDIFF(week, -1, RunStartTime),
                  -1
                ),
                101
              ) as RunWeekString
            FROM
              [ReportObjectRunData]
            where
              RunStatus = 'Success' -- and [ReportObjectID] = 21187
            group by
              DATEADD(
                week,
                DATEDIFF(week, -1, RunStartTime),
                -1
              ),
              ReportObjectID,
              convert(
                nvarchar(MAX),
                DATEADD(
                  week,
                  DATEDIFF(week, -1, RunStartTime),
                  -1
                ),
                101
              )

    ;


End
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Christopher Pickering
-- Create date: 3/19/20
-- Description: Master package to update search data
-- =============================================
CREATE PROCEDURE [app].[Search_MasterDataUpdate]
    -- no params
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @return_value int
    EXEC    @return_value = [app].[Search_UpdateReportObjectData]
    
    EXEC    @return_value = [app].[Search_UpdateBasicSearchData]

    EXEC    @return_value = [app].[User_UpdateUsernameData]
    

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Christopher Pickering
-- Create date: 3/19/20
-- Description: Update basic search table
-- =============================================
CREATE PROCEDURE [app].[Search_UpdateBasicSearchData]
    -- no params
AS
BEGIN
    SET NOCOUNT ON;
    -- create view of current data
    drop table if exists #myTemp;

    select * into #myTemp from (
        select
            'report' as 'ItemType',
            id as ItemId,
            ReportObjectTypeID as TypeId,
            [Value] as SearchField,
            'Name' as SearchFieldDescription,
                -- rank is 1 if there are all descriptions, 2 if dev and 3 if none
                case when r.[Documented] = 1 then 1 else 2 end as ItemRank,
            case when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'N'  then 0 
                 when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'Y'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'N'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'Y'  then 0 end as [Hidden],
            case when ReportObjectTypeID IN (3, 17, 20, 21, 28) then 0 else 1 end VisibleType,
            case when OrphanedReportObjectYN = 'Y' then 1 else 0 end Orphaned
        from
            app.Search_ReportObjectSearchData as r
        where 1=1
    --      OrphanedReportObjectYN = 'N'
    --      and DefaultVisibilityYN = 'Y'
    --      and ([DocHidden] is null or [DocHidden] = 'N')
    --      and ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and [ColumnName] = 'Name'

        union all


        select
            'report' as 'ItemType',
            id as ItemId,
            ReportObjectTypeID as TypeId,
            [Value] as SearchField,
            'DisplayTitle' as SearchFieldDescription,
                -- rank is 1 if there are all descriptions, 2 if dev and 3 if none
                case when r.[Documented] = 1 then 1 else 2 end as ItemRank,
            case when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'N'  then 0 
                 when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'Y'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'N'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'Y'  then 0 end as [Hidden],
            case when ReportObjectTypeID IN (3, 17, 20, 21, 28) then 0 else 1 end VisibleType,
            case when OrphanedReportObjectYN = 'Y' then 1 else 0 end Orphaned
        from
            app.Search_ReportObjectSearchData as r
        where 1=1
    --      OrphanedReportObjectYN = 'N'
    --      and DefaultVisibilityYN = 'Y'
    --      and ([DocHidden] is null or [DocHidden] = 'N')
    --      and ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and [ColumnName] = 'DisplayTitle'

        union all

        select
            'report' as 'Type',
            r.id as Id,
            ReportObjectTypeID as TypeId,
            [Value] as SearchField,
            'Description' as SearchFieldDescription,
            4 as ItemRank,
            case when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'N'  then 0 
                 when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'Y'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'N'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'Y'  then 0 end as [Hidden],
            case when ReportObjectTypeID IN (3, 17, 20, 21, 28) then 0 else 1 end VisibleType,
            case when OrphanedReportObjectYN = 'Y' then 1 else 0 end Orphaned
        from
            app.Search_ReportObjectSearchData as r
        where 1=1
        --  OrphanedReportObjectYN = 'N'
        --  and DefaultVisibilityYN = 'Y'
        --  and (DocHidden is null or DocHidden = 'N')
        --  and r.ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and ColumnName = 'Description'

        union all

        select
            'report' as 'Type',
            r.id as Id,
            ReportObjectTypeID as TypeId,
            [Value] as SearchField,
            'Detailed Description' as SearchFieldDescription,
            4 as ItemRank,
            case when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'N'  then 0 
                 when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'Y'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'N'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'Y'  then 0 end as [Hidden],
            case when ReportObjectTypeID IN (3, 17, 20, 21, 28) then 0 else 1 end VisibleType,
            case when OrphanedReportObjectYN = 'Y' then 1 else 0 end Orphaned
        from
            app.Search_ReportObjectSearchData as r
        where 1=1
        --  OrphanedReportObjectYN = 'N'
        --  and DefaultVisibilityYN = 'Y'
        --  and (DocHidden is null or DocHidden = 'N')
        --  and r.ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and ColumnName = 'DetailedDescription'
        
        union all

        select
            'report' as 'Type',
            r.id,
            ReportObjectTypeID as TypeId,
            [Value] as SearchField,
            'Term Name' as SearchFieldDescription,
            4,
            case when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'N'  then 0 
                 when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'Y'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'N'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'Y'  then 0 end as [Hidden],
            case when ReportObjectTypeID IN (3, 17, 20, 21, 28) then 0 else 1 end VisibleType,
            case when OrphanedReportObjectYN = 'Y' then 1 else 0 end Orphaned
        from
            app.Search_ReportObjectSearchData as r
        where 1=1
        --  OrphanedReportObjectYN = 'N'
        --  and DefaultVisibilityYN = 'Y'
        --  and (DocHidden is null or DocHidden = 'N')
        --  and r.ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and ColumnName = 'Term-Name'
        group by
            r.id,
            ReportObjectTypeID,
            [Value],
            case when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'N'  then 0 
                 when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'Y'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'N'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'Y'  then 0 end ,
            case when ReportObjectTypeID IN (3, 17, 20, 21, 28) then 0 else 1 end ,
            case when OrphanedReportObjectYN = 'Y' then 1 else 0 end 

        union all

        select
            'report' as 'Type',
            r.id,
            ReportObjectTypeID as TypeId,
            [Value] as SearchField,
            'Term Summary' as SearchFieldDescription,
            5,
            case when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'N'  then 0 
                 when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'Y'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'N'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'Y'  then 0 end as [Hidden],
            case when ReportObjectTypeID IN (3, 17, 20, 21, 28) then 0 else 1 end VisibleType,
            case when OrphanedReportObjectYN = 'Y' then 1 else 0 end Orphaned
        from
            app.Search_ReportObjectSearchData as r
        where 1=1
        --  OrphanedReportObjectYN = 'N'
        --  and DefaultVisibilityYN = 'Y'
        --  and (DocHidden is null or DocHidden = 'N')
        --  and r.ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and ColumnName = 'Term-Summary'
        group by
            r.id,
            ReportObjectTypeID,
            [Value],
            case when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'N'  then 0 
                 when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'Y'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'N'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'Y'  then 0 end ,
            case when ReportObjectTypeID IN (3, 17, 20, 21, 28) then 0 else 1 end,
            case when OrphanedReportObjectYN = 'Y' then 1 else 0 end

        union all

        select
            'report' as 'Type',
            r.id,
            ReportObjectTypeID as TypeId,
            [Value] as SearchField,
            'Term Technical Definition' as SearchFieldDescription,
            5,
            case when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'N'  then 0 
                 when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'Y'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'N'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'Y'  then 0 end as [Hidden],
            case when ReportObjectTypeID IN (3, 17, 20, 21, 28) then 0 else 1 end VisibleType,
            case when OrphanedReportObjectYN = 'Y' then 1 else 0 end Orphaned
        from
            app.Search_ReportObjectSearchData as r
        where 1=1
        --  OrphanedReportObjectYN = 'N'
        --  and DefaultVisibilityYN = 'Y'
        --  and (DocHidden is null or DocHidden = 'N')
        --  and r.ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and ColumnName = 'Term-TechnicalDefinition'
        group by
            r.id,
            ReportObjectTypeID,
            [Value],
            case when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'N'  then 0 
                 when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'Y'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'N'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'Y'  then 0 end,
            case when ReportObjectTypeID IN (3, 17, 20, 21, 28) then 0 else 1 end,
            case when OrphanedReportObjectYN = 'Y' then 1 else 0 end

            union all
         select
            'report' as 'Type',
            r.id,
            ReportObjectTypeID as TypeId,
            [Value] as SearchField,
            'Project Annotation' as SearchFieldDescription,
            5,
            case when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'N'  then 0 
                 when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'Y'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'N'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'Y'  then 0 end as [Hidden],
            case when ReportObjectTypeID IN (3, 17, 20, 21, 28) then 0 else 1 end VisibleType,
            case when OrphanedReportObjectYN = 'Y' then 1 else 0 end Orphaned
        from
            app.Search_ReportObjectSearchData as r
        where 1=1
        --  OrphanedReportObjectYN = 'N'
        --  and DefaultVisibilityYN = 'Y'
        --  and (DocHidden is null or DocHidden = 'N')
        --  and r.ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and ColumnName = 'ProjectAnnotation-Annotation'
        group by
            r.id,
            ReportObjectTypeID,
            [Value],
            case when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'N'  then 0 
                 when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'Y'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'N'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'Y'  then 0 end,
            case when ReportObjectTypeID IN (3, 17, 20, 21, 28) then 0 else 1 end,
            case when OrphanedReportObjectYN = 'Y' then 1 else 0 end
        union all

        Select
            'report' as 'Type',
            r.id,
            ReportObjectTypeID as TypeId,
            [Value] as SearchField,
            'Developer Description' as SearchFieldDescription,
            4,
            case when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'N'  then 0 
                 when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'Y'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'N'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'Y'  then 0 end as [Hidden],
            case when ReportObjectTypeID IN (3, 17, 20, 21, 28) then 0 else 1 end VisibleType,
            case when OrphanedReportObjectYN = 'Y' then 1 else 0 end Orphaned
        from
            app.Search_ReportObjectSearchData as r
        where 1=1
        --  OrphanedReportObjectYN = 'N'
        --  and DefaultVisibilityYN = 'Y'
        --  and (DocHidden is null or DocHidden = 'N')
        --  and r.ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and ColumnName = 'Doc-DeveloperDescription'

        union all

        Select
            'report' as 'Type',
            r.id,
            ReportObjectTypeID as TypeId,
            [Value] as SearchField,
            'Key Assumptions' as SearchFieldDescription,
            4,
            case when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'N'  then 0 
                 when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'Y'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'N'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'Y'  then 0 end as [Hidden],
            case when ReportObjectTypeID IN (3, 17, 20, 21, 28) then 0 else 1 end VisibleType,
            case when OrphanedReportObjectYN = 'Y' then 1 else 0 end Orphaned
        from
            app.Search_ReportObjectSearchData as r
        where 1=1
        --  OrphanedReportObjectYN = 'N'
        --  and DefaultVisibilityYN = 'Y'
        --  and (DocHidden is null or DocHidden = 'N')
        --  and r.ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and ColumnName = 'Doc-KeyAssumptions'

        union all

        select 
            'report',
            r.id,
            ReportObjectTypeID as TypeId,
            r.EpicMasterFile + ' ' + [Value] as SearchField,
            'Epic Id' as SearchFieldDescription,
            4,
            case when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'N'  then 0 
                 when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'Y'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'N'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'Y'  then 0 end as [Hidden],
            case when ReportObjectTypeID IN (3, 17, 20, 21, 28) then 0 else 1 end VisibleType,
            case when OrphanedReportObjectYN = 'Y' then 1 else 0 end Orphaned
        from app.Search_ReportObjectSearchData as r
        where 1=1
        --  OrphanedReportObjectYN = 'N'
        --  and DefaultVisibilityYN = 'Y'
        --  and (DocHidden is null or DocHidden = 'N')
        --  and r.ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and r.EpicMasterFile is not null
            and ColumnName = 'EpicRecordId'

        union all 

        select 
            'report',
            r.id,
            ReportObjectTypeID as TypeId,
            'HGR ' + cast(r.EpicReportTemplateId as nvarchar),
            'Epic Template Id' as SearchFieldDescription,
            4,
            case when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'N'  then 0 
                 when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'Y'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'N'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'Y'  then 0 end as [Hidden],
            case when ReportObjectTypeID IN (3, 17, 20, 21, 28) then 0 else 1 end VisibleType,
            case when OrphanedReportObjectYN = 'Y' then 1 else 0 end Orphaned
        from app.Search_ReportObjectSearchData as r
        where 1=1
        --  OrphanedReportObjectYN = 'N'
        --  and DefaultVisibilityYN = 'Y'
        --  and (DocHidden is null or DocHidden = 'N')
        --  and r.ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and r.EpicReportTemplateId is not null
            and ColumnName = 'Name'

        union all

        select distinct
            'report',
            r.id,
            ReportObjectTypeID as TypeId,
            stuff((select ' ' + [Value] from app.Search_reportObjectSearchData as tabl1 where tabl1.id = r.id
            for xml path('')),1,1,'') as SearchField,
            'Query',
            5,
            case when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'N'  then 0 
                 when DefaultVisibilityYN = 'Y' and isnull([DocHidden],'N') = 'Y'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'N'  then 1 
                 when DefaultVisibilityYN = 'N' and isnull([DocHidden],'N') = 'Y'  then 0 end as [Hidden],
            case when ReportObjectTypeID IN (3, 17, 20, 21, 28) then 0 else 1 end VisibleType,
            case when OrphanedReportObjectYN = 'Y' then 1 else 0 end Orphaned
        from app.Search_ReportObjectSearchData as r
        where 1=1
        --  OrphanedReportObjectYN = 'N'
        --  and DefaultVisibilityYN = 'Y'
        --  and (DocHidden is null or DocHidden = 'N')
        --  and r.ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and ColumnName = 'Query-Query'
                
        ) as t

        
    --drop table if exists app.Search_BasicSearchData;
    if not exists (select * from sysobjects where name='Search_BasicSearchData' and xtype='U')
        exec('create table app.Search_BasicSearchData
        (
            Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
            ItemId int,
            TypeId int,
            ItemType nvarchar(100),
            ItemRank int,
            SearchFieldDescription nvarchar(100),
            SearchField nvarchar(max),
            Hidden int,
            VisibleType int,
            Orphaned int
        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
        ')
    ;

    --drop table if exists app.Search_BasicSearchData_Small;
    if not exists (select * from sysobjects where name='Search_BasicSearchData_Small' and xtype='U')
        exec('create table app.Search_BasicSearchData_Small
        (
            Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
            ItemId int,
            TypeId int,
            ItemType nvarchar(100),
            ItemRank int,
            SearchFieldDescription nvarchar(100),
            SearchField nvarchar(max),
            Hidden int,
            VisibleType int,
            Orphaned int
        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
        ')
    ;


    -- insert new records into table.
    insert into app.Search_BasicSearchData
        select
            ItemId,
            TypeId,
            ItemType,
            ItemRank,
            SearchFieldDescription,
            SearchField,
            [Hidden],
            VisibleType,
            Orphaned
        from 
            #myTemp
        except (select ItemId,
            TypeId,
            ItemType,
            ItemRank,
            SearchFieldDescription,
            SearchField,
            [Hidden],
            VisibleType,
            Orphaned from app.Search_BasicSearchData)
        order by
            ItemRank desc
    ;
    -- insert new records into small table.
    insert into app.Search_BasicSearchData_Small
        select
            ItemId,
            TypeId,
            ItemType,
            ItemRank,
            SearchFieldDescription,
            SearchField,
            [Hidden],
            VisibleType,
            Orphaned
        from 
            #myTemp
            where [hidden] = 0
            and orphaned = 0
            and visibletype = 0
        except (select ItemId,
            TypeId,
            ItemType,
            ItemRank,
            SearchFieldDescription,
            SearchField,
            [Hidden],
            VisibleType,
            Orphaned from app.Search_BasicSearchData_Small)
        order by
            ItemRank desc
    ;

    -- delete old records from table
    delete app.Search_BasicSearchData from app.Search_BasicSearchData  l
    left join #myTemp as t on l.ItemId = t.ItemId
        and l.TypeId = t.TypeId
        and l.ItemType = t.ItemType
        and l.ItemRank = t.ItemRank
        and l.SearchFieldDescription = t.SearchFieldDescription
        and l.SearchField = t.SearchField 
        and l.[Hidden] = t.[Hidden]
        and l.VisibleType = t.VisibleType
        and l.Orphaned = t.Orphaned
    where t.ItemId is null

    -- delete old records from small table
    delete app.Search_BasicSearchData_Small from app.Search_BasicSearchData_Small  l
    left join #myTemp as t on l.ItemId = t.ItemId
        and l.TypeId = t.TypeId
        and l.ItemType = t.ItemType
        and l.ItemRank = t.ItemRank
        and l.SearchFieldDescription = t.SearchFieldDescription
        and l.SearchField = t.SearchField 
        and l.[Hidden] = t.[Hidden]
        and l.VisibleType = t.VisibleType
        and l.Orphaned = t.Orphaned
        and t.orphaned = 0
        and t.Visibletype = 0
        and t.orphaned = 0
    where t.ItemId is null

    ;

    -- create full text index
    if not exists (select * from sys.fulltext_index_columns as fi inner JOIN sys.columns as c ON c.object_id = fi.object_id AND c.column_id = fi.column_id and c.name = 'SearchField' inner join sys.tables as t on t.object_id = fi.object_id and t.name = 'Search_BasicSearchData')
        begin
            declare @i varchar(max) = (select
                                    'create fulltext index on app.Search_BasicSearchData(SearchField) key index ' + i.name + ' On Search With Stoplist = off;'
                                from sys.tables t
                                    INNER JOIN sys.indexes i ON t.object_id = i.object_id
                                where 
                                    i.index_id = 1  -- clustered index
                                    and t.name = 'Search_BasicSearchData')

            exec(@i)
        end

    ;
    drop table if exists #myTemp;
    -- rebuild catalog
    alter fulltext catalog Search rebuild with accent_sensitivity=off;

    -- create full text index small
    if not exists (select * from sys.fulltext_index_columns as fi inner JOIN sys.columns as c ON c.object_id = fi.object_id AND c.column_id = fi.column_id and c.name = 'SearchField' inner join sys.tables as t on t.object_id = fi.object_id and t.name = 'Search_BasicSearchData_Small')
        begin
            set @i  = (select
                                    'create fulltext index on app.Search_BasicSearchData_Small(SearchField) key index ' + i.name + ' On Search With Stoplist = off;'
                                from sys.tables t
                                    INNER JOIN sys.indexes i ON t.object_id = i.object_id
                                where 
                                    i.index_id = 1  -- clustered index
                                    and t.name = 'Search_BasicSearchData_Small')

            exec(@i)
        end

    ;
    drop table if exists #myTemp;
    -- rebuild catalog
    alter fulltext catalog Search_Small rebuild with accent_sensitivity=off;

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Christopher Pickering
-- Create date: 3/19/20
-- Description: Creates/updates table with search data from reports
-- =============================================
CREATE PROCEDURE [app].[Search_UpdateReportObjectData]
    -- no params
AS
BEGIN
    SET NOCOUNT ON;

    /******* 

    set vars

    *******/

    declare @sql nvarchar(max);
    declare @col nvarchar(max);

    -- get report run data
    drop table if exists #reportrunstemp;
    select r.reportobjectid , d.cnt as cnt1,d12.cnt as cnt2,d6.cnt as cnt3, d1.cnt as cnt4 into #reportrunstemp from ReportObject r
    inner join (select ReportObjectID, count(1) as cnt from ReportObjectRunData group by ReportObjectID) as d on r.ReportObjectID = d.ReportObjectID
    left outer join (select ReportObjectID, count(1) as cnt from ReportObjectRunData  where RunStartTime > DATEADD(month,-12,getdate()) group by ReportObjectID) as d12 on r.ReportObjectID = d12.ReportObjectID
    left outer join (select ReportObjectID, count(1) as cnt from ReportObjectRunData  where RunStartTime > DATEADD(month,-6,getdate()) group by ReportObjectID) as d6 on r.ReportObjectID = d6.ReportObjectID
    left outer join (select ReportObjectID, count(1) as cnt from ReportObjectRunData  where RunStartTime > DATEADD(month,-1,getdate()) group by ReportObjectID) as d1 on r.ReportObjectID = d1.ReportObjectID

    -- get column names w/ id
    drop table if exists #mytempids;
    select
        [value] as column_name,
        ROW_NUMBER() OVER(ORDER BY [value] ASC) AS id 
    into #mytempids 
    from string_split('Name,DisplayTitle,Description,DetailedDescription,ReportObjectURL,EpicRecordID,ReportServerCatalogID,EpicReportTemplateId,ReportServerPath',',');

    -- create temp table w/ needed columns
    drop table if exists app.Search_ReportObjectSearchData;
    
    select
        pk = Identity(int,1,1),
        r.reportobjectid as Id,
        cast('col' as nvarchar(max)) as ColumnName,
        cast('col' as nvarchar(max)) as [Value],
        r.LastModifiedDate,
        r.EpicMasterFile,
        r.DefaultVisibilityYN,
        r.OrphanedReportObjectYN,
        r.ReportObjectTypeID,
        r.AuthorUserId,
        r.LastModifiedByUserID,
        r.EpicReportTemplateId,
        r.SourceServer,
        r.SourceDB,
        r.SourceTable,
        1 as Documented,
        d.OperationalOwnerUserID as DocOwnerId,
        d.Requester as DocRequesterId,
        d.OrganizationalValueID as DocOrgValueId,
        d.EstimatedRunFrequencyID as DocRunFreqId,
        d.FragilityID as DocFragId,
        d.ExecutiveVisibilityYN as DocExecVis,
        d.MaintenanceScheduleID as DocMainSchedId,
        d.LastUpdateDateTime as DocLastUpdated,
        d.CreatedDateTime as DocCreated,
        d.CreatedBy as DocCreatedBy,
        d.UpdatedBy as DocUpdatedBy,
        d.EnabledForHyperspace as DocHypeEnabled,
        d.DoNotPurge as DocDoNotPurge,
        d.[Hidden] as DocHidden,
        cast(null as int) as TwoYearRuns,
        cast(null as int) as OneYearRuns,
        cast(null as int) as SixMonthsRuns,
        cast(null as int) as OneMonthRuns
    into app.Search_ReportObjectSearchData 
    from ReportObject r
    left outer join app.ReportObject_Doc d on r.ReportObjectID = d.ReportObjectID
    where 0=1;

    ALTER TABLE app.Search_ReportObjectSearchData ADD CONSTRAINT Search_ReportObjectSearchData_PK PRIMARY KEY (pk)

    -- get fields from reportobject
    declare @id Integer = 0 -- assuming all Ids are > 0
    while exists (select * from #mytempids where id > @Id)
        begin
            select @id = Min(id) from #mytempids where id > @id;
            select @col = column_name from #mytempids where id = @id;
        
            set @SQL = 'insert into app.Search_ReportObjectSearchData 
                        (
                            Id,
                            ColumnName,
                            [Value],
                            LastModifiedDate,
                            EpicMasterFile,
                            DefaultVisibilityYN,
                            OrphanedReportObjectYN,
                            ReportObjectTypeID,
                            AuthorUserId,
                            LastModifiedByUserID,
                            EpicReportTemplateId,
                            SourceServer,
                            SourceDB,
                            SourceTable,
                            Documented,
                            DocOwnerId,
                            DocRequesterId,
                            DocOrgValueId,
                            DocRunFreqId,
                            DocFragId,
                            DocExecVis,
                            DocMainSchedId,
                            DocLastUpdated,
                            DocCreated,
                            DocCreatedBy,
                            DocUpdatedBy,
                            DocHypeEnabled,
                            DocDoNotPurge,
                            DocHidden,
                            TwoYearRuns,
                            OneYearRuns,
                            SixMonthsRuns,
                            OneMonthRuns
                        )
                        select 
                            r.reportobjectid,
                            '''+ @col + ''',
                            r.'+ @col + ',
                            r.LastModifiedDate,
                            r.EpicMasterFile,
                            r.DefaultVisibilityYN,
                            r.OrphanedReportObjectYN,
                            r.ReportObjectTypeID,
                            r.AuthorUserId,
                            r.LastModifiedByUserID,
                            r.EpicReportTemplateId,
                            r.SourceServer,
                            r.SourceDB,
                            r.SourceTable,
                            case when d.reportobjectid is null then 0 else 1 end,
                            d.OperationalOwnerUserID,
                            d.Requester,
                            d.OrganizationalValueID,
                            d.EstimatedRunFrequencyID,
                            d.FragilityID,
                            d.ExecutiveVisibilityYN,
                            d.MaintenanceScheduleID,
                            d.LastUpdateDateTime,
                            d.CreatedDateTime,
                            d.CreatedBy,
                            d.UpdatedBy,
                            d.EnabledForHyperspace,
                            d.DoNotPurge,
                            d.[Hidden],d2.cnt1,d2.cnt2,d2.cnt3,d2.cnt4
                        from ReportObject r
                        left outer join #reportrunstemp as d2 on r.ReportObjectID = d2.ReportObjectID
                        left outer join app.ReportObject_doc d on r.reportobjectid = d.reportobjectid
                        where '+ @col + ' is not null
                        and r.name not like ''%(P)%'''

            exec sp_executesql @SQL
        end
    
    -- get fields from terms
    -- get column names w/ id
    truncate table #mytempids;
    insert into #mytempids (column_name, id)
    select
        [value] as column_name,
        ROW_NUMBER() OVER(ORDER BY [value] ASC) AS id 

    from string_split('Name,Summary,TechnicalDefinition',',');

    -- build temp table of related reports
    drop table if exists #relatedreports;
    select reportobjectId, linkedId into #relatedreports from  (
select t.ReportObjectID, t.ReportObjectID as linkedId
from app.ReportObjectDocTerms t
left outer join app.ReportObject_doc d on t.ReportObjectID = d.ReportObjectID
inner join ReportObject r on t.ReportObjectID = r.ReportObjectID

union

select  c.ParentReportObjectID, t.ReportObjectID
from ReportObjectHierarchy c
inner join app.ReportObjectDocTerms t on c.ChildReportObjectID = t.ReportObjectID
left outer join app.ReportObject_doc d on t.ReportObjectID = d.ReportObjectID
inner join ReportObject r on t.ReportObjectID = r.ReportObjectID

union

select  c.ParentReportObjectID, t.ReportObjectID
from ReportObjectHierarchy c
inner join ReportObjectHierarchy gc on c.ChildReportObjectID = gc.ParentReportObjectID
inner join app.ReportObjectDocTerms t on gc.ChildReportObjectID = t.ReportObjectID
left outer join app.ReportObject_doc d on t.ReportObjectID = d.ReportObjectID
inner join ReportObject r on t.ReportObjectID = r.ReportObjectID

union

select c.ParentReportObjectID, t.ReportObjectID
from ReportObjectHierarchy c
inner join ReportObjectHierarchy gc on c.ChildReportObjectID = gc.ParentReportObjectID
inner join ReportObjectHierarchy gcc on gc.ChildReportObjectID = gcc.ParentReportObjectID
inner join app.ReportObjectDocTerms t on gcc.ChildReportObjectID = t.ReportObjectID
left outer join app.ReportObject_doc d on t.ReportObjectID = d.ReportObjectID
inner join ReportObject r on t.ReportObjectID = r.ReportObjectID
) as t;

    set @id = 0 -- assuming all Ids are > 0
    while exists (select * from #mytempids where id > @Id)
        begin
            select @id = Min(id) from #mytempids where id > @id;
            select @col = column_name from #mytempids where id = @id;
        
            set @SQL = 'insert into app.Search_ReportObjectSearchData
(
Id,
ColumnName,
[Value],
LastModifiedDate,
EpicMasterFile,
DefaultVisibilityYN,
OrphanedReportObjectYN,
ReportObjectTypeID,
AuthorUserId,
LastModifiedByUserID,
EpicReportTemplateId,
SourceServer,
SourceDB,
SourceTable,
Documented,
DocOwnerId,
DocRequesterId,
DocOrgValueId,
DocRunFreqId,
DocFragId,
DocExecVis,
DocMainSchedId,
DocLastUpdated,
DocCreated,
DocCreatedBy,
DocUpdatedBy,
DocHypeEnabled,
DocDoNotPurge,
DocHidden,
TwoYearRuns,
OneYearRuns,
SixMonthsRuns,
OneMonthRuns
)
select distinct
r.reportobjectid,
''Term-'+ @col + ''',
t.'+ @col + ',
r.LastModifiedDate,
r.EpicMasterFile,
r.DefaultVisibilityYN,
r.OrphanedReportObjectYN,
r.ReportObjectTypeID,
r.AuthorUserId,
r.LastModifiedByUserID,
r.EpicReportTemplateId,
r.SourceServer,
r.SourceDB,
r.SourceTable,
case when d.reportobjectid is null then 0 else 1 end,
d.OperationalOwnerUserID,
d.Requester,
d.OrganizationalValueID,
d.EstimatedRunFrequencyID,
d.FragilityID,
d.ExecutiveVisibilityYN,
d.MaintenanceScheduleID,
d.LastUpdateDateTime,
d.CreatedDateTime,
d.CreatedBy,
d.UpdatedBy,
d.EnabledForHyperspace,
d.DoNotPurge,
d.[Hidden],d2.cnt1,d2.cnt2,d2.cnt3,d2.cnt4
from 
reportobject r 
join #relatedreports  inheritance on r.ReportObjectID = inheritance.ReportObjectID
left outer join #reportrunstemp as d2 on r.reportobjectid = d2.ReportObjectID
left outer join app.ReportObject_doc d on r.ReportObjectID = d.reportobjectid
join app.ReportObjectDocTerms dt on dt.ReportObjectID = inheritance.linkedId
inner join app.Term t on dt.TermId = t.TermId
where t.'+ @col + ' is not null
and r.name not like ''%(P)%'''
            exec sp_executesql @SQL
        end

drop table if exists #relatedreports;
    -- get fields from reportobject_doc
    -- get column names w/ id
    truncate table #mytempids;
    insert into #mytempids (column_name, id)
    select
        [value] as column_name,
        ROW_NUMBER() OVER(ORDER BY [value] ASC) AS id 

    from string_split('GitLabProjectURL,DeveloperDescription,KeyAssumptions',',');

    set @id = 0 -- assuming all Ids are > 0
    while exists (select * from #mytempids where id > @Id)
        begin
            select @id = Min(id) from #mytempids where id > @id;
            select @col = column_name from #mytempids where id = @id;
        
            set @SQL = 'insert into app.Search_ReportObjectSearchData 
                        (
                            Id,
                            ColumnName,
                            [Value],
                            LastModifiedDate,
                            EpicMasterFile,
                            DefaultVisibilityYN,
                            OrphanedReportObjectYN,
                            ReportObjectTypeID,
                            AuthorUserId,
                            LastModifiedByUserID,
                            EpicReportTemplateId,
                            SourceServer,
                            SourceDB,
                            SourceTable,
                            Documented,
                            DocOwnerId,
                            DocRequesterId,
                            DocOrgValueId,
                            DocRunFreqId,
                            DocFragId,
                            DocExecVis,
                            DocMainSchedId,
                            DocLastUpdated,
                            DocCreated,
                            DocCreatedBy,
                            DocUpdatedBy,
                            DocHypeEnabled,
                            DocDoNotPurge,
                            DocHidden,
                            TwoYearRuns,
                            OneYearRuns,
                            SixMonthsRuns,
                            OneMonthRuns
                        )
                        select 
                            r.reportobjectid,
                            ''Doc-'+ @col + ''',
                            d.'+ @col + ',
                            r.LastModifiedDate,
                            r.EpicMasterFile,
                            r.DefaultVisibilityYN,
                            r.OrphanedReportObjectYN,
                            r.ReportObjectTypeID,
                            r.AuthorUserId,
                            r.LastModifiedByUserID,
                            r.EpicReportTemplateId,
                            r.SourceServer,
                            r.SourceDB,
                            r.SourceTable,
                            case when d.reportobjectid is null then 0 else 1 end,
                            d.OperationalOwnerUserID,
                            d.Requester,
                            d.OrganizationalValueID,
                            d.EstimatedRunFrequencyID,
                            d.FragilityID,
                            d.ExecutiveVisibilityYN,
                            d.MaintenanceScheduleID,
                            d.LastUpdateDateTime,
                            d.CreatedDateTime,
                            d.CreatedBy,
                            d.UpdatedBy,
                            d.EnabledForHyperspace,
                            d.DoNotPurge,
                            d.[Hidden],d2.cnt1,d2.cnt2,d2.cnt3,d2.cnt4
                        from ReportObject r
                        inner join app.ReportObject_doc d on r.ReportObjectID = d.ReportObjectID
                        left outer join #reportrunstemp as d2 on r.ReportObjectID = d2.ReportObjectID
                        where d.'+ @col + ' is not null
                        and r.name not like ''%(P)%'''
            exec sp_executesql @SQL
        end


    -- get fields from project > report annotations
    -- get column names w/ id
    truncate table #mytempids;
    insert into #mytempids (column_name, id)
    select
        [value] as column_name,
        ROW_NUMBER() OVER(ORDER BY [value] ASC) AS id 

    from string_split('Annotation',',');

    set @id = 0 -- assuming all Ids are > 0
    while exists (select * from #mytempids where id > @Id)
        begin
            select @id = Min(id) from #mytempids where id > @id;
            select @col = column_name from #mytempids where id = @id;
        
            set @SQL = 'insert into app.Search_ReportObjectSearchData 
                        (
                            Id,
                            ColumnName,
                            [Value],
                            LastModifiedDate,
                            EpicMasterFile,
                            DefaultVisibilityYN,
                            OrphanedReportObjectYN,
                            ReportObjectTypeID,
                            AuthorUserId,
                            LastModifiedByUserID,
                            EpicReportTemplateId,
                            SourceServer,
                            SourceDB,
                            SourceTable,
                            Documented,
                            DocOwnerId,
                            DocRequesterId,
                            DocOrgValueId,
                            DocRunFreqId,
                            DocFragId,
                            DocExecVis,
                            DocMainSchedId,
                            DocLastUpdated,
                            DocCreated,
                            DocCreatedBy,
                            DocUpdatedBy,
                            DocHypeEnabled,
                            DocDoNotPurge,
                            DocHidden,
                            TwoYearRuns,
                            OneYearRuns,
                            SixMonthsRuns,
                            OneMonthRuns
                        )
                        select distinct
                            r.reportobjectid,
                            ''ProjectAnnotation-'+ @col + ''',
                            a.' +  @col + ',
                            r.LastModifiedDate,
                            r.EpicMasterFile,
                            r.DefaultVisibilityYN,
                            r.OrphanedReportObjectYN,
                            r.ReportObjectTypeID,
                            r.AuthorUserId,
                            r.LastModifiedByUserID,
                            r.EpicReportTemplateId,
                            r.SourceServer,
                            r.SourceDB,
                            r.SourceTable,
                            case when d.reportobjectid is null then 0 else 1 end,
                            d.OperationalOwnerUserID,
                            d.Requester,
                            d.OrganizationalValueID,
                            d.EstimatedRunFrequencyID,
                            d.FragilityID,
                            d.ExecutiveVisibilityYN,
                            d.MaintenanceScheduleID,
                            d.LastUpdateDateTime,
                            d.CreatedDateTime,
                            d.CreatedBy,
                            d.UpdatedBy,
                            d.EnabledForHyperspace,
                            d.DoNotPurge,
                            d.[Hidden],d2.cnt1,d2.cnt2,d2.cnt3,d2.cnt4
                        from ReportObject r
                        inner join app.Dp_ReportAnnotation a on a.reportId = r.reportObjectId
                        left outer join app.ReportObject_doc d on r.ReportObjectID = d.ReportObjectID
                        left outer join #reportrunstemp as d2 on r.ReportObjectID = d2.ReportObjectID
                        where a.'+ @col + ' is not null
                        and r.name not like ''%(P)%'''
            exec sp_executesql @SQL
        end

    -- get fields from querys
    -- get column names w/ id
    truncate table #mytempids;
    insert into #mytempids (column_name, id)
    select
        [value] as column_name,
        ROW_NUMBER() OVER(ORDER BY [value] ASC) AS id 

    from string_split('Query',',');

    set @id = 0 -- assuming all Ids are > 0
    while exists (select * from #mytempids where id > @Id)
        begin
            select @id = Min(id) from #mytempids where id > @id;
            select @col = column_name from #mytempids where id = @id;
        
            set @SQL = 'insert into app.Search_ReportObjectSearchData 
                        (
                            Id,
                            ColumnName,
                            [Value],
                            LastModifiedDate,
                            EpicMasterFile,
                            DefaultVisibilityYN,
                            OrphanedReportObjectYN,
                            ReportObjectTypeID,
                            AuthorUserId,
                            LastModifiedByUserID,
                            EpicReportTemplateId,
                            SourceServer,
                            SourceDB,
                            SourceTable,
                            Documented,
                            DocOwnerId,
                            DocRequesterId,
                            DocOrgValueId,
                            DocRunFreqId,
                            DocFragId,
                            DocExecVis,
                            DocMainSchedId,
                            DocLastUpdated,
                            DocCreated,
                            DocCreatedBy,
                            DocUpdatedBy,
                            DocHypeEnabled,
                            DocDoNotPurge,
                            DocHidden,
                            TwoYearRuns,
                            OneYearRuns,
                            SixMonthsRuns,
                            OneMonthRuns
                        )
                        select distinct
                            r.reportobjectid,
                            ''Query-'+ @col + ''',
                            Stuff((select '' '' + Query from dbo.ReportObjectQuery q2 where q2.reportObjectId = r.reportObjectId
                            for xml path('''')),1,1,'''') as ' +  @col + ',
                            r.LastModifiedDate,
                            r.EpicMasterFile,
                            r.DefaultVisibilityYN,
                            r.OrphanedReportObjectYN,
                            r.ReportObjectTypeID,
                            r.AuthorUserId,
                            r.LastModifiedByUserID,
                            r.EpicReportTemplateId,
                            r.SourceServer,
                            r.SourceDB,
                            r.SourceTable,
                            case when d.reportobjectid is null then 0 else 1 end,
                            d.OperationalOwnerUserID,
                            d.Requester,
                            d.OrganizationalValueID,
                            d.EstimatedRunFrequencyID,
                            d.FragilityID,
                            d.ExecutiveVisibilityYN,
                            d.MaintenanceScheduleID,
                            d.LastUpdateDateTime,
                            d.CreatedDateTime,
                            d.CreatedBy,
                            d.UpdatedBy,
                            d.EnabledForHyperspace,
                            d.DoNotPurge,
                            d.[Hidden],d2.cnt1,d2.cnt2,d2.cnt3,d2.cnt4
                        from ReportObject r
                        inner join dbo.ReportObjectQuery q on q.reportObjectId = r.reportObjectId
                        left outer join app.ReportObject_doc d on r.ReportObjectID = d.ReportObjectID
                        left outer join #reportrunstemp as d2 on r.ReportObjectID = d2.ReportObjectID
                        where q.'+ @col + ' is not null
                        and r.name not like ''%(P)%'''
            exec sp_executesql @SQL
        end

    -- component queries

    -- get fields from querys
    -- get column names w/ id
    truncate table #mytempids;
    insert into #mytempids (column_name, id)
    select
        [value] as column_name,
        ROW_NUMBER() OVER(ORDER BY [value] ASC) AS id 

    from string_split('Query',',');

    set @id = 0 -- assuming all Ids are > 0
    while exists (select * from #mytempids where id > @Id)
        begin
            select @id = Min(id) from #mytempids where id > @id;
            select @col = column_name from #mytempids where id = @id;
        
            set @SQL = 'insert into app.Search_ReportObjectSearchData 
                        (
                            Id,
                            ColumnName,
                            [Value],
                            LastModifiedDate,
                            EpicMasterFile,
                            DefaultVisibilityYN,
                            OrphanedReportObjectYN,
                            ReportObjectTypeID,
                            AuthorUserId,
                            LastModifiedByUserID,
                            EpicReportTemplateId,
                            SourceServer,
                            SourceDB,
                            SourceTable,
                            Documented,
                            DocOwnerId,
                            DocRequesterId,
                            DocOrgValueId,
                            DocRunFreqId,
                            DocFragId,
                            DocExecVis,
                            DocMainSchedId,
                            DocLastUpdated,
                            DocCreated,
                            DocCreatedBy,
                            DocUpdatedBy,
                            DocHypeEnabled,
                            DocDoNotPurge,
                            DocHidden,
                            TwoYearRuns,
                            OneYearRuns,
                            SixMonthsRuns,
                            OneMonthRuns
                        )
                        select distinct
                            r.reportobjectid,
                            ''Query-'+ @col + ''',
                            Stuff((select '' '' + Query from dbo.ReportObjectQuery q2 where q2.reportObjectId = r3.reportObjectId
                            for xml path('''')),1,1,'''') as ' +  @col + ',
                            r.LastModifiedDate,
                            r.EpicMasterFile,
                            r.DefaultVisibilityYN,
                            r.OrphanedReportObjectYN,
                            r.ReportObjectTypeID,
                            r.AuthorUserId,
                            r.LastModifiedByUserID,
                            r.EpicReportTemplateId,
                            r.SourceServer,
                            r.SourceDB,
                            r.SourceTable,
                            case when d.reportobjectid is null then 0 else 1 end,
                            d.OperationalOwnerUserID,
                            d.Requester,
                            d.OrganizationalValueID,
                            d.EstimatedRunFrequencyID,
                            d.FragilityID,
                            d.ExecutiveVisibilityYN,
                            d.MaintenanceScheduleID,
                            d.LastUpdateDateTime,
                            d.CreatedDateTime,
                            d.CreatedBy,
                            d.UpdatedBy,
                            d.EnabledForHyperspace,
                            d.DoNotPurge,
                            d.[Hidden],d2.cnt1,d2.cnt2,d2.cnt3,d2.cnt4
                        from ReportObject r
                        
                        
                        left outer join #reportrunstemp as d2 on r.ReportObjectID = d2.ReportObjectID
                        join ReportObjectHierarchy h on r.ReportObjectID = h.ParentReportObjectID
                        join ReportObject r2 on h.ChildReportObjectID = r2.ReportObjectID
                        join ReportObjectHierarchy h2 on r2.ReportObjectID = h2.ParentReportObjectID
                        join ReportObject r3 on h2.ChildReportObjectID = r3.ReportObjectID
                        join ReportObjectQuery q on r3.ReportObjectID = q.ReportObjectId
                        left outer join app.ReportObject_doc d on r.ReportObjectID = d.ReportObjectID

                        where 1=1
                        and r.EpicMasterFile = ''IDB''
                        and q.'+ @col + ' is not null
                        and r.name not like ''%(P)%'''
            exec sp_executesql @SQL
        end
    
        /*
        clean up 
        */

        drop table if exists #mytempids;
        drop table if exists #reportrunstemp;

    END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:      Christopher Pickering
-- Create date: 1/3/2020
-- Description: Used to update SearchTable
-- =============================================
CREATE PROCEDURE [app].[UpdateSearchTable]
AS
BEGIN 
    -- ================================================
    /*
       package used to update the search fields used 
       by the index for Altas search.
    */
    -- ================================================
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

   

-- create view of current data
    drop table if exists #myTemp;

    select * into #myTemp from (

        select
            'report' as 'ItemType',
            r.reportobjectid as ItemId,
            ReportObjectTypeID as TypeId,
            Name as SearchField,
            'Name' as SearchFieldDescription,
             -- rank is 1 if there are all descriptions, 2 if dev and 3 if none
            COALESCE(
                (SELECT 1 FROM app.ReportObject_doc b WHERE r.ReportObjectID = b.ReportObjectID AND (b.DeveloperDescription IS NOT NULL OR b.KeyAssumptions IS NOT NULL)
                ), CASE WHEN r.description IS NULL THEN 3 ELSE 2 end
            ) as ItemRank
        from
            dbo.reportObject as r
            left outer join app.ReportObject_doc ro on r.ReportObjectID =  ro.ReportObjectID
        where
            OrphanedReportObjectYN = 'N'
            and DefaultVisibilityYN = 'Y'
            and (ro.Hidden is null or ro.Hidden = 'N')
            and r.ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and Name is not null

        union all

        select
            'report' as 'Type',
            r.reportobjectid as Id,
            ReportObjectTypeID as TypeId,
            Description as SearchField,
            'Description' as SearchFieldDescription,
            4 as ItemRank
        from
            dbo.reportObject as r
            left outer join app.ReportObject_doc ro on r.ReportObjectID = ro.ReportObjectID
        where
            OrphanedReportObjectYN = 'N'
            and DefaultVisibilityYN = 'Y'
            and (ro.Hidden is null or ro.Hidden = 'N')
            and r.ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and description is not null

        union all

        select
            'report' as 'Type',
            r.reportobjectid as Id,
            ReportObjectTypeID as TypeId,
            DetailedDescription as SearchField,
            'Detailed Description' as SearchFieldDescription,
            4 as ItemRank
        from
            dbo.reportObject as r
            left outer join app.ReportObject_doc ro on r.ReportObjectID = ro.ReportObjectID


        where
            OrphanedReportObjectYN = 'N'
            and DefaultVisibilityYN = 'Y'
            and (ro.Hidden is null or ro.Hidden = 'N')
            and r.ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and detaileddescription is not null

        union all

        select
            'report' as 'Type',
            r.ReportObjectID,
            ReportObjectTypeID as TypeId,
            t.Name,
            'Term Name' as SearchFieldDescription,
            4
        from
            app.Term t
            inner join app.ReportObjectDocTerms dt on t.TermId = dt.TermId
            inner join dbo.ReportObject r on dt.ReportObjectID = r.ReportObjectID
            left outer join app.ReportObject_doc ro on r.ReportObjectID = ro.ReportObjectID
        where
            OrphanedReportObjectYN = 'N'
            and DefaultVisibilityYN = 'Y'
            and (ro.Hidden is null or ro.Hidden = 'N')
            and r.ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and t.Name is not null
        group by
            r.ReportObjectID,
            ReportObjectTypeID,
            t.Name

        union all

        select
            'report' as 'Type',
            r.ReportObjectID,
            ReportObjectTypeID as TypeId,
            t.Summary,
            'Term Summary' as SearchFieldDescription,
            5
        from
            app.Term t
            inner join app.ReportObjectDocTerms dt on t.TermId = dt.TermId
            inner join dbo.ReportObject r on dt.ReportObjectID = r.ReportObjectID
            left outer join app.ReportObject_doc ro on r.ReportObjectID = ro.ReportObjectID
        where
            OrphanedReportObjectYN = 'N'
            and DefaultVisibilityYN = 'Y'
            and (ro.Hidden is null or ro.Hidden = 'N')
            and r.ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and t.summary is not null
        group by
            r.ReportObjectID,
            ReportObjectTypeID,
            t.Summary

        union all

        select
            'report' as 'Type',
            r.ReportObjectID,
            ReportObjectTypeID as TypeId,
            t.TechnicalDefinition,
            'Term Technical Definition' as SearchFieldDescription,
            5
        from
            app.Term t
            inner join app.ReportObjectDocTerms dt on t.TermId = dt.TermId
            inner join dbo.ReportObject r on dt.ReportObjectID = r.ReportObjectID
            left outer join app.ReportObject_doc ro on r.ReportObjectID = ro.ReportObjectID
        where
            OrphanedReportObjectYN = 'N'
            and DefaultVisibilityYN = 'Y'
            and (ro.Hidden is null or ro.Hidden = 'N')
            and r.ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and t.technicaldefinition is not null
        group by
            r.ReportObjectID,
            ReportObjectTypeID,
            t.TechnicalDefinition

        union all

        Select
            'report' as 'Type',
            r.ReportObjectId,
            ReportObjectTypeID as TypeId,
            d.DeveloperDescription,
            'Developer Description' as SearchFieldDescription,
            4
        from
            app.reportobject_doc d
            inner join dbo.reportobject r on d.reportobjectid = r.reportobjectid
        where
            OrphanedReportObjectYN = 'N'
            and DefaultVisibilityYN = 'Y'
            and (d.Hidden is null or d.Hidden = 'N')
            and r.ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and developerdescription is not null

        union all

        Select
            'report' as 'Type',
            r.ReportObjectId,
            ReportObjectTypeID as TypeId,
            d.KeyAssumptions,
            'Key Assumptions' as SearchFieldDescription,
            4
        from
            app.reportobject_doc d
            inner join dbo.reportobject r on d.reportobjectid = r.reportobjectid
        where
            OrphanedReportObjectYN = 'N'
            and DefaultVisibilityYN = 'Y'
            and (d.Hidden is null or d.Hidden = 'N')
            and r.ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and KeyAssumptions is not null
    
        union all

        select 
            'report',
            r.ReportObjectId,
            ReportObjectTypeID as TypeId,
            r.EpicMasterFile + ' ' + cast(r.EpicRecordID as nvarchar),
            'Epic Id' as SearchFieldDescription,
            4
        from dbo.reportobject r
        left outer join app.ReportObject_doc ro on r.ReportObjectID = ro.ReportObjectID
        where
            OrphanedReportObjectYN = 'N'
            and DefaultVisibilityYN = 'Y'
            and (ro.Hidden is null or ro.Hidden = 'N')
            and r.ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and (r.EpicMasterFile is not null or r.EpicRecordID is not null)

        union all 

        select 
            'report',
            r.ReportObjectId,
            ReportObjectTypeID as TypeId,
            'HGR ' + cast(r.EpicReportTemplateId as nvarchar),
            'Epic Template Id' as SearchFieldDescription,
            4
        from dbo.reportobject r
        left outer join app.ReportObject_doc ro on r.ReportObjectID = ro.ReportObjectID
        where
            OrphanedReportObjectYN = 'N'
            and DefaultVisibilityYN = 'Y'
            and (ro.Hidden is null or ro.Hidden = 'N')
            and r.ReportObjectTypeID IN (3, 17, 20, 21, 28)
            and r.EpicReportTemplateId is not null
            
        ) as cool

        union all

        select
            'report',
            r.ReportObjectId,
            ReportObjectTypeID as TypeId,
            q.Query,
            'Query',
            5
        from dbo.ReportObjectQuery q
        inner join dbo.ReportObject r on q.reportObjectId = r.reportObjectId
        left outer join app.ReportObject_doc ro on r.ReportObjectID = ro.ReportObjectID
        where OrphanedReportObjectYN = 'N'
            and DefaultVisibilityYN = 'Y'
            and (ro.Hidden is null or ro.Hidden = 'N')  
        ;
            

    --drop table if exists app.SearchTable;
    if not exists (select * from sysobjects where name='SearchTable' and xtype='U')
        exec('create table app.SearchTable
        (
            Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
            ItemId int,
            TypeId int,
            ItemType nvarchar(100),
            ItemRank int,
            SearchFieldDescription nvarchar(100),
            SearchField nvarchar(max)
        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
        ')
    ;


    -- insert new records into table.
    insert into app.SearchTable
        select
            ItemId,
            TypeId,
            ItemType,
            ItemRank,
            SearchFieldDescription,
            SearchField
        from 
            #myTemp
        except (select ItemId,
            TypeId,
            ItemType,
            ItemRank,
            SearchFieldDescription,
            SearchField from app.SearchTable)
        order by
            ItemRank desc
    ;

    -- delete old records from table
    delete app.SearchTable from app.SearchTable  l
    left join #myTemp as t on l.ItemId = t.ItemId
        and l.TypeId = t.TypeId
        and l.ItemType = t.ItemType
        and l.ItemRank = t.ItemRank
        and l.SearchFieldDescription = t.SearchFieldDescription
        and l.SearchField = t.SearchField 
    where t.ItemId is null

    ;

    -- create full text index
    if not exists (select * from sys.fulltext_index_columns as fi inner JOIN sys.columns as c ON c.object_id = fi.object_id AND c.column_id = fi.column_id and name = 'SearchField')
        begin
            declare @i varchar(max) = (select
                                    'create fulltext index on app.SearchTable(SearchField) key index ' + i.name + ' On Search With Stoplist = off;'
                                from sys.tables t
                                    INNER JOIN sys.indexes i ON t.object_id = i.object_id
                                where 
                                    i.index_id = 1  -- clustered index
                                    and t.name = 'SearchTable')

            exec(@i)
        end

    ;
    drop table if exists #myTemp;
    -- rebuild catalog
    alter fulltext catalog Search rebuild with accent_sensitivity=off;

End
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Christopher Pickering
-- Create date: 3/19/20
-- Description: Update basic search table
-- =============================================
CREATE PROCEDURE [app].[User_UpdateUsernameData]
    -- no params
AS
BEGIN
    SET NOCOUNT ON;

    drop table if exists #myUserTemp;

    select userid,
        dbo.ToProperCase(fullname) as Fullname,
        dbo.ToProperCase(case when charindex(' ',fullname) > 0 then substring(fullname,0,charindex(' ',fullname)) else fullname end) as Firstname,
        dbo.ToProperCase(case when charindex(' ',fullname) > 0 then substring(fullname,charindex(' ',fullname),len(fullname)) else null end) as Lastname 
    into #myUserTemp
    from (select 
    userid,
    lower(case when fullname is not null then fullname  
               when username is not null then 
                    case when username like '%,%' then
                        -- firsname
                        replace(substring(case when charindex(' ',replace(username,', ',',')) > 0 then substring(replace(username,', ',','),0,charindex(' ',replace(username,', ',','))) else replace(username,', ',',') end,
                        charindex(',',case when charindex(' ',replace(username,', ',',')) > 0 then substring(replace(username,', ',','),0,charindex(' ',replace(username,', ',','))) else replace(username,', ',',') end) + 1,
                        len(username)),'s.','') + ' ' + 
            
        
                        -- last name
                        substring(case when charindex(' ',replace(username,', ',',')) > 0 then substring(replace(username,', ',','),0,charindex(' ',replace(username,', ',','))) else replace(username,', ',',') end,
                            0,charindex(',',case when charindex(' ',replace(username,', ',',')) > 0 then substring(replace(username,', ',','),0,charindex(' ',replace(username,', ',','))) else replace(username,', ',',') end))
            
                        else replace(replace(substring(username,charindex('\',username)+1,len(username)),'-',' '),'s.','')
                    end 
                when accountname is not null then 
                    case when accountname like '%,%' then
                        -- firsname
                        replace(substring(case when charindex(' ',replace(accountname,', ',',')) > 0 then substring(replace(accountname,', ',','),0,charindex(' ',replace(accountname,', ',','))) else replace(accountname,', ',',') end,
                        charindex(',',case when charindex(' ',replace(accountname,', ',',')) > 0 then substring(replace(accountname,', ',','),0,charindex(' ',replace(accountname,', ',','))) else replace(accountname,', ',',') end) + 1,
                        len(accountname)),'s.','') + ' ' + 
            
        
                        -- last name
                        substring(case when charindex(' ',replace(accountname,', ',',')) > 0 then substring(replace(accountname,', ',','),0,charindex(' ',replace(accountname,', ',','))) else replace(accountname,', ',',') end,
                            0,charindex(',',case when charindex(' ',replace(accountname,', ',',')) > 0 then substring(replace(accountname,', ',','),0,charindex(' ',replace(accountname,', ',','))) else replace(accountname,', ',',') end))
            
                        else replace(replace(substring(accountname,charindex('\',accountname)+1,len(accountname)),'-',' '),'s.','')
                    end 
                else 


    null end) as fullname

    from [User]
    ) as t
    

    --drop table if exists app.User_NameData;
        if not exists (select * from sysobjects where name='User_NameData' and xtype='U')
            exec('create table app.User_NameData
            (
                UserId int NOT NULL PRIMARY KEY,
                Fullname nvarchar(max) null, 
                Firstname nvarchar(max) null, 
                Lastname nvarchar(max) null
            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
            ')
        ;

    MERGE app.User_NameData d USING #myUserTemp s
    ON s.UserId = d.UserId
    WHEN MATCHED
        THEN update set
            d.Firstname = s.Firstname,
            d.Lastname = s.Lastname,
            d.Fullname = s.Fullname
    WHEN NOT MATCHED by Target
        THEN insert (UserId, Fullname, Firstname, Lastname)
             values (s.UserId, s.Fullname, s.Firstname, s.Lastname)
    WHEN NOT MATCHED BY SOURCE
        THEN DELETE;

    drop table if exists #myUserTemp;

    -- create full text index
    if not exists (select * from sys.fulltext_index_columns as fi 
                    inner JOIN sys.columns as c ON c.object_id = fi.object_id AND c.column_id = fi.column_id 
                    INNER JOIN sys.objects o ON o.object_id = c.object_id 
                    where o.name = 'User_NameData' and c.name = 'Fullname')
        begin
            declare @i varchar(max) = (select
                                    'create fulltext index on app.User_NameData(Fullname,Firstname,Lastname) key index ' + i.name + ' On User_NameData With Stoplist = off;'
                                from sys.tables t
                                    INNER JOIN sys.indexes i ON t.object_id = i.object_id
                                where 
                                    i.index_id = 1  -- clustered index
                                    and t.name = 'User_NameData')
            exec(@i)
        end
        alter fulltext catalog User_NameData rebuild with accent_sensitivity=off;
        ;
end;
    
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Christopher Pickering
-- Create date: 11/13/19
-- Description: Used to quickly search for users
--              for moving a "user" to a webuser
--              need full username :/ 
-- =============================================
CREATE PROCEDURE [dbo].[BasicDirectorSearch] 
    -- Add the parameters for the stored procedure here
    @searchTerm varchar(1000)
,   @results int = 10
,   @exclude varchar(1000) = ''
    
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

      SET @searchTerm = REPLACE(REPLACE(LTRIM(RTRIM(@searchTerm)),' ','%'),'*','%');

SELECT TOP(@results) Email, FullName 
FROM dbo.[User] r
inner join app.UserRoleLinks l on r.UserId = l.UserId
inner join app.UserRoles u on u.UserRolesId = l.UserRolesId
    WHERE r.Username NOT LIKE '%S.%'
    AND r.Username NOT LIKE ','
    and u.Name = 'Director'
    and FullName like concat('%', @searchTerm, '%')
    AND r.UserID NOT IN (SELECT * FROM STRING_SPLIT(@exclude,','))
    ORDER BY 2 asc
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Christopher Pickering
-- Create date: 10/3/19
-- Description: Used by Data Project to quickly search for report objects
-- =============================================
CREATE PROCEDURE [dbo].[BasicProjectSearch] 
    -- Add the parameters for the stored procedure here
    @searchTerm varchar(1000)
,   @results int = 10
,   @exclude varchar(1000) = ''
    
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;


   SET @searchTerm = REPLACE(REPLACE(LTRIM(RTRIM(@searchTerm)),' ','%'),'*','%');

    SELECT TOP (@results) r.DataProjectID
    ,           r.Name
    ,           r.Description
        from        app.DP_DataProject r
    WHERE r.Name like concat('%', @searchTerm, '%')
    AND r.DataProjectID NOT IN (SELECT * FROM STRING_SPLIT(@exclude,','))
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Christopher Pickering
-- Create date: 11/13/19
-- Description: Used to quickly search for users
--              for moving a "user" to a webuser
--              need full username :/ 
-- =============================================
CREATE PROCEDURE [dbo].[BasicRealUserSearch] 
    -- Add the parameters for the stored procedure here
    @searchTerm varchar(1000)
,   @exclude varchar(1000) = ''
    
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

   SET @searchTerm = REPLACE(REPLACE(LTRIM(RTRIM(@searchTerm)),' ','%'),'*','%');

SELECT TOP(10) UserId, Username FROM    dbo.[User] r
    WHERE r.Username NOT LIKE '%S.%'
    AND r.Username NOT LIKE ','

    and Username like concat('%', @searchTerm, '%')
    AND UserID NOT IN (SELECT * FROM STRING_SPLIT(@exclude,','))
    ORDER BY 2 asc
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Christopher Pickering
-- Create date: 10/3/19
-- Description: Used by Data Project to quickly search for report objects
-- =============================================
CREATE PROCEDURE [dbo].[BasicReportObjectSearch] 
    -- Add the parameters for the stored procedure here
    @searchTerm varchar(1000)
    
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

   SET @searchTerm = REPLACE(REPLACE(LTRIM(RTRIM(@searchTerm)),' ','%'),'*','%');

    SELECT TOP (10) r.ReportObjectID
    ,           r.Name  as ReportObjectName
    ,           r.Description
    
    from        ReportObject r
    inner join  ReportObjectType rt on rt.ReportObjectTypeID = r.ReportObjectTypeID
    where OrphanedReportObjectYN = 'N'
    and DefaultVisibilityYN = 'Y'
    and rt.ReportObjectTypeID IN (3,17,20,21,28)--Crystal, RW, DB, component, SSRS
    and r.Name like concat('%', @searchTerm, '%')
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Christopher Pickering
-- Create date: 10/3/19
-- Description: Used by Data Project to quickly search for report objects
-- =============================================
CREATE PROCEDURE [dbo].[BasicReportSearch] 
    -- Add the parameters for the stored procedure here
    @searchTerm varchar(1000)
,   @results int = 10
,   @exclude varchar(1000) = ''
    
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

   SET @searchTerm = REPLACE(REPLACE(LTRIM(RTRIM(@searchTerm)),' ','%'),'*','%');

    SELECT TOP (@results) r.ReportObjectID
    ,           r.Name  as ReportObjectName
    ,           r.Description
    
    from        ReportObject r
    inner join  ReportObjectType rt on rt.ReportObjectTypeID = r.ReportObjectTypeID
    where OrphanedReportObjectYN = 'N'
    and DefaultVisibilityYN = 'Y'
    and rt.ReportObjectTypeID IN (3,17,20,21,28)--Crystal, RW, DB, component, SSRS
    and r.Name like concat('%', @searchTerm, '%')
    AND r.ReportObjectID NOT IN (SELECT * FROM STRING_SPLIT(@exclude,','))
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Christopher Pickering
-- Create date: 10/3/19
-- Description: Used by Data Project to quickly search for report objects
-- =============================================
CREATE PROCEDURE [dbo].[BasicTermObjectSearch] 
    -- Add the parameters for the stored procedure here
    @searchTerm varchar(1000)
    
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

   SET @searchTerm = REPLACE(REPLACE(LTRIM(RTRIM(@searchTerm)),' ','%'),'*','%');

    SELECT TOP (10) r.TermId
    ,           r.Name  
    ,           r.Summary
    
    FROM [app].[Term] AS r
    where 1=1

    and r.name like concat('%', @searchTerm, '%')
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Christopher Pickering
-- Create date: 10/3/19
-- Description: Used by Data Project to quickly search for report objects
-- =============================================
CREATE PROCEDURE [dbo].[BasicTermSearch] 
    -- Add the parameters for the stored procedure here
    @searchTerm varchar(1000)
,   @results int = 10
,   @exclude varchar(1000) = ''
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

   SET @searchTerm = REPLACE(REPLACE(LTRIM(RTRIM(@searchTerm)),' ','%'),'*','%');

    SELECT TOP (@results) r.TermId
    ,           r.Name  
    ,           r.Summary
    
    FROM [app].[Term] AS r
    where 1=1

    and r.name like concat('%', @searchTerm, '%')
    AND r.TermId NOT IN (SELECT * FROM STRING_SPLIT(@exclude,','))
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Christopher Pickering
-- Create date: 10/24/19
-- Description: Used to quickly search for users
-- =============================================
CREATE PROCEDURE [dbo].[BasicUserSearch] 
    -- Add the parameters for the stored procedure here
    @searchTerm varchar(1000)
,   @exclude varchar(1000) = ''
,   @results int = 10
,   @type varchar(1) = 'u'

    
AS
begin
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    --Declare   @searchTerm varchar(100) = 'chri';
    declare   @resultSize int = 10
    declare @searchTable nvarchar(100) = 'app.User_NameData';
    --declare @searchTerm varchar(1000) = 'pick'
    --declare @excluded varchar(1000) = ''

    /*
        clear up search string
    */
    declare @i int = patindex('%[^a-zA-Z0-9 ]%', @searchTerm)

    while @i > 0
        begin
            set @searchTerm = STUFF(@searchTerm, @i, 1, ' ')
            set @i = patindex('%[^a-zA-Z0-9 ]%', @searchTerm)
        end

    set @searchTerm = REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(@searchTerm)),' ','CHAR(17)CHAR(18)'),'CHAR(18)CHAR(17)',''),'CHAR(17)CHAR(18)',' ') 

    declare @q varchar(max);

    drop table if exists #matches;
    select userid, Fullname,'u' as S into #matches from app.User_NameData where 1=2;

    begin
        -- users
        Set @q = 'select s.UserId, s.Fullname,''u''  from containstable ('+@searchTable+', Fullname, ' + '''("' + REPLACE(@searchTerm, ' ', '*" NEAR "') + '*")''' + ') as g
        left join ' + @searchTable + ' s on g.[Key] = s.UserId order by rank, Firstname, Lastname';
        insert into #matches exec (@q) ;
        if(@type='a')
            begin
                -- groups
                set @q = 'select s.GroupId, s.Groupname, ''g'' from containstable (dbo.UserGroups, Groupname, ' + '''("' + REPLACE(@searchTerm, ' ', '*" NEAR "') + '*")''' + ') as g
                left join dbo.UserGroups s on g.[Key] = s.GroupId order by rank, Groupname';
                insert into #matches exec (@q) ;
            end;
    end

    if (select count(1) from #matches) < @resultSize
        begin

            set @q = 'select UserId, Fullname, ''u'' from '+@searchTable+' where 1=1 ' +
            (SELECT DISTINCT 
  
                    replace((
                        select 'and charindex('''+[value]+''', fullname) > 0 ' from String_split(@searchTerm,' ')
                        FOR XML PATH ('')
                        ),'&gt;','>')
            FROM String_split(@searchTerm,' ')) + '
            order by Firstname, Lastname';
            print(@q)
            insert into #matches exec (@q)
            if(@type='a')
                begin
                    set @q = 'select GroupId, Groupname, ''u'' from dbo.UserGroups where 1=1 ' +
                    (SELECT DISTINCT 
  
                            replace((
                                select 'and charindex('''+[value]+''', Groupname) > 0 ' from String_split(@searchTerm,' ')
                                FOR XML PATH ('')
                                ),'&gt;','>')
                    FROM String_split(@searchTerm,' ')) + '
                    order by Groupname';
            
                    insert into #matches exec (@q)
                end;

        end;

    select distinct top(@results)  userid, fullname as username, S from #matches where userid not in (select [value] from String_Split(@exclude,',')) order by fullname
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[RelatedReports]
    @Id nvarchar(max)
,   @UserId int
,   @records int = 20
as
begin

/**********************************************************************************************
Name: Related Reports
Description: Find Related Reports based in report ID and user ID.
            

Auth: Christopher Pickering
**********************************************************************************************/


/*
Declare @Id nvarchar(max) = 58392;

Declare @UserId int = 2548;
Declare @records int = 20;
*/
drop table if exists #lst;
select * into #lst from STRING_SPLIT(REPLACE(@Id,' ',''), ',');

drop table if exists #related;

select top(@records) Id, cnt into #related from (
    select Id, sum(cnt) as cnt from (
        -- favorites
        select f2.ItemId as Id, count(*) as cnt
        from (select userid from app.UserFavorites where itemid in (select * from #lst) and lower(ItemType) = 'report') as f
            INNER JOIN app.UserFavorites as f2 on f.UserId = f2.UserId
        group by f2.ItemId

        union all 

        -- other subscriptions
        select s2.ReportObjectId, count(*) as cnt
        from (select UserId, SubscriptionTo from ReportObjectSubscriptions where ReportObjectId in (select * from #lst)) s
        , ReportObjectSubscriptions s2 
         where (
                s.UserId = s2.UserId
            or s.SubscriptionTo = s2.SubscriptionTo
            or s.SubscriptionTo = replace(lower(s2.SubscriptionTo),'riversidehealthcare','rhc')
            )
        group by s2.ReportObjectId

        union all

        -- related through a project
        select a.ReportId, 1
        from (select d.DataProjectId from app.DP_ReportAnnotation d where ReportId in (select * from #lst)) r
        inner join app.DP_ReportAnnotation a on r.DataProjectId = a.DataProjectId

        union all

        -- same template
        select r.ReportObjectID, 1
        from (select EpicReportTemplateId
            from ReportObject
            where EpicReportTemplateId is not null
            and ReportObjectId in (select * from #lst)
            and DefaultVisibilityYN = 'Y' ) as t
        inner join ReportObject r on t.EpicReportTemplateId = r.EpicReportTemplateId
        where r.DefaultVisibilityYN = 'Y' 

    ) as sub
group by Id) as sub
where exists (select top(1) tr.Id from app.ReportObjectTopRuns tr where tr.ReportObjectID = sub.Id and tr.LastRun > DATEADD(month,-6,GetDate()))
order by cnt desc;
;

select 
    id as Id,
    isnull(r.DisplayTitle,r.Name) as Name,
    case when f.ItemId is null then 'no' else 'yes' end as Favorite,
    r.ReportObjectUrl,
    t.Name as ReportObjectType,
    r.EpicReportTemplateId,
    r.EpicRecordId,
    r.EpicMasterFile,
    d.EnabledForHyperspace
    --d.ReportObjectID
from #related sub
    inner join ReportObject r on sub.Id = r.ReportObjectID
    inner join ReportObjectType t on r.ReportObjectTypeID = t.ReportObjectTypeID
    left outer join app.ReportObject_doc d on sub.Id = d.ReportObjectId
    left outer join (select ItemId from app.UserFavorites where lower(ItemType) = 'report' and UserId = @UserId) f on f.ItemId = sub.Id
where sub.Id not in (select * from #lst)
  and r.DefaultVisibilityYN = 'Y'     
group by Id, isnull(r.DisplayTitle,r.Name), f.ItemId,
    r.ReportObjectUrl,
    t.Name,
    r.EpicReportTemplateId,
    r.EpicRecordId,
    r.EpicMasterFile,
    d.EnabledForHyperspace,
    d.ReportObjectID
order by case when d.ReportObjectID is null then 0 else 1 end desc

    end;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[Search]
    @searchTerm varchar(100)
,   @currentPage int = 1
,   @pageSize int = 20
,   @showHidden varchar = 0
,   @showAllTypes varchar = 0
,   @showOrphans varchar = 0
,   @reportObjectTypes varchar(100) = '0'
,   @searchField varchar(200) = ''
as
begin



--Declare   @searchTerm varchar(100) = 't';
--Declare   @currentPage int = 1
--Declare   @pageSize int = 20
--Declare   @showHidden varchar = 0
--Declare   @showAllTypes varchar = 0
--Declare   @showOrphans varchar = 0
--Declare   @reportObjectTypes varchar(100) = '0'
--Declare   @searchField varchar(200) = ''

/**********************************************************************************************
Name: Report Object search for Atlas
Description: Returns report objects based on full text index searching. 
            
Auth: Scott Manley & Christopher Pickering
**********************************************************************************************/

-- where are we searching?
declare @searchTable nvarchar(100) = 'app.Search_BasicSearchData_Small';

if(@showHidden + @showAllTypes + @showOrphans > 0)
    set @searchTable = 'app.Search_BasicSearchData'

-- return enough records for next 6 pages. big number because results are parred down later.
declare @ResultSize int = @pageSize * @currentPage + @pageSize * 4000

-- @originalSearch = @searchTerm w/ special chars removed.
declare @originalSearch varchar(100) = @searchTerm
declare @i int;
set @i = patindex('%[^a-zA-Z0-9 _-]%', @originalSearch)
while @i > 0
        begin
            set @originalSearch = STUFF(@originalSearch, @i, 1, ' ')
            set @i = patindex('%[^a-zA-Z0-9 _-]%', @originalSearch)
        end
set @originalSearch = REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(@originalSearch)),' ','CHAR(17)CHAR(18)'),'CHAR(18)CHAR(17)',''),'CHAR(17)CHAR(18)',' ') 

-- create another string that has the quotes (stuff we must have in results)
declare @match nvarchar(max) = '1=1';
if CHARINDEX('"',@searchTerm) > 0
    begin
        declare @quotestring nvarchar(100)
        declare @quotestringmaster nvarchar(100) = @searchTerm
        declare @index_one int = patindex('%"%"%', @quotestringmaster)
        declare @index_two int;
        declare @length int;

        drop table if exists #tempstrings;
        select cast(null as nvarchar(100)) as mytext into #tempstrings where 1=2;

        while @index_one > 0
              begin
                    set @index_two = charindex('"', @quotestringmaster, @index_one+1);
                    set @length = @index_two - @index_one
                    set @quotestring = substring(@quotestringmaster,@index_one+1,@length-1) 
                    -- remove all except letters, numbers, space, underscore.
                    set @i = patindex('%[^a-zA-Z0-9 _]%', @quotestring)

                    while @i > 0
                          begin
                                set @quotestring = STUFF(@quotestring, @i, 1, ' ')
                                set @i = patindex('%[^a-zA-Z0-9 _]%', @quotestring)
                          end
                    set @quotestring = REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(@quotestring)),' ','CHAR(17)CHAR(18)'),'CHAR(18)CHAR(17)',''),'CHAR(17)CHAR(18)',' ') 
                    insert into #tempstrings (mytext ) select  @quotestring where @quotestring != '' and @quotestring is not null
                    set @quotestringmaster = stuff(@quotestringmaster,@index_one,@length+1,'')
                    set @index_one = patindex('%"%"%', @quotestringmaster)
              end
      
        -- clean string  
        set @i = patindex('%[^a-zA-Z0-9 _]%', @searchTerm)

        while @i > 0
            begin
                set @searchTerm = STUFF(@searchTerm, @i, 1, ' ')
                set @i = patindex('%[^a-zA-Z0-9 _]%', @searchTerm)
            end
        ;

        set @searchTerm = REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(@searchTerm)),' ','CHAR(17)CHAR(18)'),'CHAR(18)CHAR(17)',''),'CHAR(17)CHAR(18)',' ') 

        SELECT  DISTINCT @match =
            concat(SUBSTRING(
                (
                    SELECT '%'' and SearchField LIKE ''%'+ST1.mytext  AS [text()]
                    FROM #tempstrings ST1
                    FOR XML PATH ('')
                ), 8, 1000000),'%', char(39) )
        FROM #tempstrings ST2

        if @match is null
            set @match = '1=1'
    end

declare @totalRecords int;
declare @q varchar(max);

drop table if exists #dupresults;
drop table if exists #results
drop table if exists #reportTypes;
drop table if exists #searchField;
drop table if exists #matches;

-- set report type filter
select * into #reportTypes from string_split(Replace(@reportObjectTypes,' ',''), ',');

-- set report field filter
select * into #searchField from string_split(@searchField, ',');

-- create table
select *,0 As SearchRank into #matches from containstable(app.Search_BasicSearchData,SearchField,'a',0) where 1=2;

-- rank 0 (exact matc on name or term name)
if ((select charindex(' ',@searchTerm)) = 0)
    begin
        Set @q = 'select g.*,0 as SearchRank from freetexttable ('+@searchTable+', SearchField, ''' + @searchTerm + ''' ) as g
            inner join '+@searchTable+' d on d.Id = g.[Key]
            where (d.SearchFieldDescription = ''Name'' or d.SearchFieldDescription = ''DisplayTitle'' or d.SearchFieldDescription = ''Term Name'')';
            
            --print @q
            insert into #matches exec (@q) ;

    end

-- rank 1 (report object name with exact match to search input)
if ((select count(1) from #matches) < @ResultSize)
    begin
        set @q = 'select s.[Id] as [KEY], 1 as RANK, 1 as SearchRank  from '+@searchTable+' s where (SearchFieldDescription = ''Name'' or SearchFieldDescription = ''DisplayTitle''  or SearchFieldDescription = ''Term Name'') and SearchField LIKE ''%' + @originalSearch + '%''
        and not exists (select * from #matches as t where t.[Key] = s.Id);';
    --print @q
        insert into #matches exec (@q);
    end

-- rank 2
if ((select count(1) from #matches) < @ResultSize)
      begin
            set @q = 'select g.*,2 as SearchRank from containstable ('+@searchTable+', SearchField, ' + '''("' + REPLACE(@searchTerm, ' ', '*" AND "') + '*")''' + ') as g
            where not exists (select * from #matches as t where t.[Key] = g.[Key]);';

            insert into #matches exec (@q);
      end

-- rank 3
if ((select count(1) from #matches) < @ResultSize)
      begin
            Set @q = 'select g.*,3 as SearchRank from containstable ('+@searchTable+', SearchField, ' + '''("' + REPLACE(@searchTerm, ' ', '*" NEAR "') + '*")''' + ') as g
            where not exists (select * from #matches as t where t.[Key] = g.[Key]);';
            
            insert into #matches exec (@q) ;
      end

-- rank 4
if ((select count(1) from #matches as s) < @ResultSize )
      begin
        Set @q = 'select g.*,4 as SearchRank from freetexttable ('+@searchTable+', SearchField, ''' + @searchTerm + ''' ) as g
            where not exists (select * from #matches as t where t.[Key] = g.[Key]);';
            
            insert into #matches exec (@q) ;
            ;
      end

--insert into #results 
select
    s.ItemId,
    s.ItemType,
    s.SearchFieldDescription,
    s.SearchField as [Text],
    1 as ItemRank,
    1 as RANK,
    1 as SearchRank,
    s.[Hidden] as 'Hidden',
    s.VisibleType as VisibleType,
    s.Orphaned as Orphaned
    into #results
from
    app.Search_BasicSearchData s
    inner join #matches as ct on s.Id = ct.[KEY]
    inner join #reportTypes rot on rot.value = 0 or rot.value = s.TypeId
    inner join #searchField sf on sf.value = '' or sf.value = s.SearchFieldDescription

where 1=2
      
set @q = '
      select
            s.ItemId,
            s.ItemType,
            min(
            case when s.SearchFieldDescription = ''Key Assumptions'' then ''Description''
                 when s.SearchFieldDescription = ''Developer Description'' then ''Description''
                 else s.SearchFieldDescription end) as SearchFieldDescription,
            max(s.SearchField) [Text],
            min(m.ItemRank) as ItemRank,
            min(ct.Rank) as RANK,
            min(ct.SearchRank) as SearchRank,
            s.[Hidden],
            s.VisibleType,
            s.Orphaned
      from
            '+@searchTable+' s
            inner join #matches ct on s.id=ct.[key]
            inner join #reportTypes rot on rot.value = 0 or rot.value = s.TypeId
            inner join #searchField sf on sf.value = '''' or sf.value = s.SearchFieldDescription
            inner join (select s.itemId, min(itemRank) ItemRank from #matches as ct
                        inner join '+@searchTable+' s on s.id = ct.[key]
                        group by s.itemId) as m on m.itemId = s.itemid and m.itemrank= s.itemrank

      where ' + @match + '
      and s.[Hidden] = ' + @showHidden + '
      and s.VisibleType = ' + @showAllTypes + '
      and s.Orphaned = ' + @showOrphans + '
        group by  s.ItemId,
            s.ItemType,
            --m.ItemRank,
            s.[Hidden],
            s.VisibleType,
            s.Orphaned
            --ct.Rank,
            --ct.SearchRank
'


--print(@q)
insert into #results exec(@q)

-- remove dups


/*
select r.* into #results from #dupresults  r
, (select itemid, min(rank) rank from #dupresults group by itemid ) dup 
where r.ItemId = dup.ItemId and r.RANK = dup.rank;

*/

set @totalRecords = (select count(*)from #results);

if (@currentPage = 1)
      begin
            select top(@pageSize) 
            
                  b.ItemId,
                  b.ItemType,
                  SearchFieldDescription as SearchField,
                   @totalRecords as TotalRecords,
                  isnull(r.DisplayTitle,r.name) as Name,
                  r.EpicRecordId,
                  r.EpicMasterFile,
                  case when r.EpicReleased = 'Y' then 1 else 0 end EpicReleased,
                  r.SourceServer,
                  r.ReportServerPath,
                  case when (SearchFieldDescription = 'Name'  and r.DisplayTitle is null) or  SearchFieldDescription = 'DisplayTitle'  then 
                      case when d.DeveloperDescription is not null then d.DeveloperDescription 
                           when r.Description is not null then r.Description
                           when r.DetailedDescription is not null then r.DetailedDescription
                           when d.KeyAssumptions is not null then d.KeyAssumptions
                           else null end
                    else b.[Text] end as Description,
                  t.Name as ReportType,
                  case when d.reportObjectId is null then 0 else 1 end as Documented,
                  r.ReportObjectURL,
                  d.EnabledForHyperspace,
                  r.EpicReportTemplateId,
                  b.[Hidden],
                  b.VisibleType,
                  b.Orphaned
            from
                  #results as b
                  left outer join reportobject r on b.itemid = r.reportobjectid
                  left outer join app.ReportObject_doc d on r.reportobjectid = d.reportobjectid
                  left outer join dbo.ReportObjectType t on r.ReportObjectTypeId = t.ReportObjectTypeID
            order by
                  SearchRank asc,
                  ItemRank asc,
                  RANK asc,
                  Documented,
                  Name asc
                 
      end
else
      begin
            select 
                  b.ItemId,
                  b.ItemType,
                  SearchFieldDescription as SearchField,
                   @totalRecords as TotalRecords,
                  isnull(r.DisplayTitle,r.name) as Name,
                  r.EpicRecordId,
                  r.EpicMasterFile,
                  case when r.EpicReleased = 'Y' then 1 else 0 end EpicReleased,
                  r.SourceServer,
                  r.ReportServerPath,
                  case when (SearchFieldDescription = 'Name'  and r.DisplayTitle is null) or  SearchFieldDescription = 'DisplayTitle'  then 
                      case when d.DeveloperDescription is not null then d.DeveloperDescription 
                           when r.Description is not null then r.Description
                           when r.DetailedDescription is not null then r.DetailedDescription
                           when d.KeyAssumptions is not null then d.KeyAssumptions
                           else null end
                    else b.[Text] end as Description,
                  t.Name as ReportType,
                  case when d.reportObjectId is null then 0 else 1 end as Documented,
                  r.ReportObjectURL,
                  d.EnabledForHyperspace,
                  r.EpicReportTemplateId,
                  b.[Hidden],
                  b.VisibleType,
                  b.Orphaned
            from
                  #results as b
                  left outer join reportobject r on b.itemid = r.reportobjectid
                  left outer join app.ReportObject_doc d on r.reportobjectid = d.reportobjectid
                  left outer join dbo.ReportObjectType t on r.ReportObjectTypeId = t.ReportObjectTypeID
            order by
                  SearchRank asc,
                  ItemRank asc,
                  RANK asc,
                  Documented,
                  Name asc 
                  offset (@currentPage - 1) * @pageSize rows fetch next @pageSize rows only;
      end
end;
        
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[SearchReportObjects]
    @searchTerm varchar(1000)
,   @currentPage int = 1
,   @pageSize int = 20
,   @reportObjectTypes varchar(1000) = '0'
as
-- adapted from https://www.mikesdotnetting.com/article/298/implementing-sql-server-full-text-search-in-an-asp-net-mvc-web-application-with-entity-framework
begin

/**********************************************************************************************
Name: Report Object search for Atlas
Description: Returns report objects based on full text index searching. 
            Search return order:
                Direct reportobject name match              
                Direct any text field match (ReportObject, ReportObject_doc, ReportObjectDocTerms)
                Fuzzy any text field match 

            All fuzzy matches will have fulltext rank summed, and then reportobjects ranked by that sum 

Auth: Scott Manley
**********************************************************************************************/

-- remove non alpha numberic characters and non space (needed because stop chars is disabled)
declare @i int = patindex('%[^a-zA-Z0-9 ]%', @searchTerm)
WHILE @i > 0
BEGIN
    SET @searchTerm = STUFF(@searchTerm, @i, 1, '')
    SET @i = patindex('%[^a-zA-Z0-9 ]%', @searchTerm)
END
;

-- remove extra space from string
SET @searchTerm = REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(@searchTerm)),' ','CHAR(17)CHAR(18)'),'CHAR(18)CHAR(17)',''),'CHAR(17)CHAR(18)',' ') 
print @searchTerm

declare @predicate varchar(8000) = @searchTerm;
declare @nearpredicate varchar(8000) = '(' + REPLACE(@searchTerm, ' ', ' NEAR ') + ')';
declare @andpredicate varchar(8000) = REPLACE(@searchTerm, ' ', ' AND ')
declare @totalRecords int;

drop table if exists #results
drop table if exists #output;
drop table if exists #reportTypes;

select * into #reportTypes from string_split(Replace(@reportObjectTypes,' ',''), ',');

drop table if exists #exactNameMatch;
drop table if exists #exactTermMatch;
drop table if exists #exactReportMatch;
drop table if exists #exactReportDocMatch;

select * into #exactNameMatch from containstable (ReportObject, Name, @nearpredicate,100);
select * into #exactTermMatch from containstable (app.Term, *, @nearpredicate,100);
select * into #exactReportMatch from containstable (ReportObject, *, @nearpredicate,100);
select * into #exactReportDocMatch from containstable (app.ReportObject_Doc, *, @nearpredicate,100);

drop table if exists #fuzzyReportMatch;

select * into #fuzzyReportMatch from freetexttable (ReportObject, *, @predicate);


-- Direct match to Report Name (Search 1 Rank 1)
select
    r.ReportObjectID,
    r.Name as ReportName,
    r.description,
    ct.RANK,
    1 as SearchRank,
    1 as SearchNum 
into #results
from
    ReportObject r 
    inner join #exactNameMatch as ct on r.ReportObjectID = ct.[KEY]
    inner join string_split(Replace(@reportObjectTypes,' ',''), ',') rot on rot.value = 0 or rot.value = r.ReportObjectTypeID
where
    OrphanedReportObjectYN = 'N'
    and DefaultVisibilityYN = 'Y'
    and r.ReportObjectTypeID IN (3,17,20,21,28) 
;                                                               

-- Direct match to Term (Search 2 Rank 2)
insert into #results
select
    r.ReportObjectID,
    r.Name as ReportName,
    r.description,
    SUM(ct.RANK) as MatchRank,
    2 as SearchRank,
    2 as SearchNum
from
    app.Term t
    inner join #exactTermMatch as ct on t.TermId = ct.[KEY]
    inner join app.ReportObjectDocTerms dt on t.TermId = dt.TermId
    inner join ReportObject r on dt.ReportObjectID = r.ReportObjectID
    inner join #reportTypes rot on rot.value = 0 or rot.value = r.ReportObjectTypeID
where
    r.ReportObjectID not in (
        select
            distinct ReportObjectID
        from
            #results where SearchRank = 1
    )
    and OrphanedReportObjectYN = 'N'
    and DefaultVisibilityYN = 'Y'
    and r.ReportObjectTypeID IN (3,17,20,21,28)
group by
    r.ReportObjectID,
    r.Name,
    r.description
 
 
--Direct match to ReportObject (Search 3 Rank 2)
insert into
    #results
select
    r.ReportObjectID,
    r.Name as ReportObjectName,
    r.Description,   
    ct.RANK as MatchRank,
    2 as SearchRank,
    3 as SearchNum
from
    ReportObject r
    inner join #exactReportMatch as ct on r.ReportObjectID = ct.[KEY]
    inner join #reportTypes rot on rot.value = 0 or rot.value = r.ReportObjectTypeID
where
    OrphanedReportObjectYN = 'N'
    and DefaultVisibilityYN = 'Y'
    and r.ReportObjectID not in (
        select
            distinct ReportObjectID
        from
            #results where SearchRank =1
    )
    and r.ReportObjectTypeID IN (3, 17, 20, 21, 28) --Crystal, RW, DB, component, SSRS

;                       

--Direct match to ReportObjectDoc (Search 4 Rank 2) 
insert into #results
select     
    r.ReportObjectID
,   r.Name as ReportObjectName
,   r.Description
,   ct.RANK as MatchRank
,   2 as SearchRank
,   4 as SearchNum
from  app.ReportObject_doc d
    inner join ReportObject  r on d.ReportObjectID = r.ReportObjectID
    inner join #exactReportDocMatch as ct on r.ReportObjectID = ct.[KEY]
    inner join #reportTypes rot on rot.value = 0 or rot.value = r.ReportObjectTypeID
where OrphanedReportObjectYN = 'N'
    and DefaultVisibilityYN = 'Y'
    and r.ReportObjectID not in (select distinct ReportObjectID from #results )
    and r.ReportObjectTypeID IN (3,17,20,21,28)--Crystal, RW, DB, component, SSRS
;                       

--Fuzzy match to ReportObject (Search 5 rank 3)
insert into #results
select     
    r.ReportObjectID
,   r.Name  as ReportObjectName
,   r.Description
,   ft.RANK as MatchRank
,   3 as SearchRank
,   5 as SearchNum
from ReportObject r
    inner join #fuzzyReportMatch as ft on r.ReportObjectID = ft.[KEY]
    inner join #reportTypes rot on rot.value = 0 or rot.value = r.ReportObjectTypeID
where OrphanedReportObjectYN = 'N'
    and DefaultVisibilityYN = 'Y'
    and r.ReportObjectID not in (select distinct ReportObjectID from #results ) --only use fuzzy logic if no direct/near matching was found
    and r.ReportObjectTypeID IN (3,17,20,21,28) --Crystal, RW, DB, component, SSRS
;                       

--Fuzzy match to ReportObjectTerm (Search 6 Rank 3)
insert into #results
select
    r.ReportObjectID
,   r.Name  as ReportObjectName
,   r.Description
,   SUM(ft.RANK) as MatchRank
,   3 as SearchRank
,   6 as SearchNum
from app.Term t
    inner join app.ReportObjectDocTerms dt on t.TermId = dt.Termid
    inner join ReportObject r on dt.ReportObjectID = r.ReportObjectID
    inner join freetexttable (app.Term, *, @nearpredicate)          as ft on t.TermId = ft.[KEY]
    inner join  #reportTypes rot on rot.value = 0 or rot.value = r.ReportObjectTypeID
where OrphanedReportObjectYN = 'N'
    and DefaultVisibilityYN = 'Y'
    and r.ReportObjectID not in (select distinct ReportObjectID from #results)
    and r.ReportObjectTypeID IN (3,17,20,21,28)--Crystal, RW, DB, component, SSRS
group by
    r.ReportObjectID
,   r.Name  
,   r.Description
;

--Fuzzy match to ReportObjectDoc (Search 7 Rank 3) 
insert into #results
select
    r.ReportObjectID
,   r.Name  as ReportObjectName
,   r.Description
,   ft.RANK as MatchRank
,   3 as SearchRank
,   7 as SearchNum
from app.ReportObject_doc d
    inner join ReportObject r on d.ReportObjectID = r.ReportObjectID
    inner join freetexttable (app.ReportObject_doc, *, @nearpredicate)  as ft on r.ReportObjectID = ft.[KEY]
    inner join #reportTypes rot on rot.value = 0 or rot.value = r.ReportObjectTypeID
where OrphanedReportObjectYN = 'N'
    and DefaultVisibilityYN = 'Y'
    and r.ReportObjectID not in (select distinct ReportObjectID from #results)
    and r.ReportObjectTypeID IN (3,17,20,21,28)--Crystal, RW, DB, component, SSRS
;   


--Group reports together and sum text rank within the search rank groups 
select      r.ReportObjectID
,           r.SearchRank
, r.ReportName
,           SUM(r.RANK) MyRANK
,           COALESCE((SELECT 1 FROM app.ReportObject_doc b WHERE r.reportobjectid = b.ReportObjectID AND (b.DeveloperDescription IS NOT NULL OR b.KeyAssumptions IS NOT NULL)),CASE WHEN r.description IS NULL THEN 3 ELSE 2 end) PRANK
into        #output
from        #results r
group by    r.ReportObjectID
,           r.SearchRank
            , r.ReportName
            ,r.Description



--OUTPUT
set @totalRecords = (select count(*)from #output);

    select   o.ReportObjectID as Id
    , o.ReportName as Name
    ,        @totalRecords TotalRecords
    ,        o.MyRANK
    ,        o.PRANK
    --,        o.SearchRank
    from     #output o
    order by CASE WHEN o.SearchRank = 1 THEN 1 ELSE NULL END  desc, o.PRANK asc, o.SearchRank asc, o.MyRANK desc, o.ReportName asc offset (@currentPage - 1) * @pageSize rows fetch next @pageSize rows only;


end;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--use Data_Governance_Dev;

--select *
--from app.TermHistory;

--select *
--from dbo.ReportObject r
--inner join containstable(ReportObject, *, 'PB NEAR Visit') t on r.ReportObjectID = t.[KEY]
--order by t.RANK desc;

create procedure [dbo].[SearchReportObjects_Original]
    @searchTerm varchar(1000)
,   @currentPage int = 1
,   @pageSize int = 20
,   @reportObjectTypes varchar(1000) = 'All'
as
-- adapted from https://www.mikesdotnetting.com/article/298/implementing-sql-server-full-text-search-in-an-asp-net-mvc-web-application-with-entity-framework
begin
    declare @predicate varchar(8000);
    declare @totalRecords int;

    drop table if exists #output;

    select          @predicate = coalesce(@predicate + ' ', '') + split.value
    from            string_split(@searchTerm, ' ') split
    left outer join sys.fulltext_system_stopwords on split.value = stopword
    where           stopword is null;

    select     r.ReportObjectID
    ,          rt.Name as ReportObjectType
    ,          r.Name  as ReportObjectName
    ,          r.Description
    ,          r.EpicMasterFile
    ,          r.EpicRecordID
    ,          ct.RANK
    into       #output
    from       ReportObject                                r
    inner join freetexttable (ReportObject, *, @predicate) as ct on r.ReportObjectID = ct.[KEY]
    inner join ReportObjectType                            rt on rt.ReportObjectTypeID = r.ReportObjectTypeID
    inner join string_split(@reportObjectTypes, ',')       rot on rot.value = cast(rt.ReportObjectTypeID as varchar(3))
                                                                  or rot.value = 'All';

    set @totalRecords = (select count(*)from #output);

    select   o.ReportObjectID
    ,        o.ReportObjectType
    ,        o.ReportObjectName
    ,        o.Description
    ,        o.EpicMasterFile
    ,        o.EpicRecordID
    ,        @totalRecords TotalRecords
    ,        o.RANK
    from     #output o
    order by o.RANK desc offset (@currentPage - 1) * @pageSize rows fetch next @pageSize rows only;
end;
GO
USE [master]
GO
ALTER DATABASE [atlas] SET  READ_WRITE 
GO


USE [master]
GO
ALTER DATABASE [atlas] SET  READ_WRITE 
GO

USE [atlas]
GO


/* basic seed */

insert into [atlas].dbo.[User] (Username) values ('Default')
GO
insert into [atlas].app.UserRoles (Name, Description) values
('Administrator','Administrators have the highest priveleges and are not prevented from taking any available actions.'),
('Report Writer','Report Writers can create and edit ReportObject documentation and terms, but cannot approve Terms, edit approved Terms, or delete things they do not own.'),
('Term Administrator','Term Admins can create, edit, approve and delete Terms.'),
('Term Builder','Term Builders can create and edit Term documentation, but cannot approve them, edit them after approval, or link them to ReportObjects.'),
('User','Users do not have any special permissions. They can navigate the site, comment, and view approved Terms and all non-hidden ReportObjects.')
GO
insert into [atlas].app.UserRoleLinks (UserId, UserRolesId) values (1,1)
GO
insert into [atlas].app.RolePermissions ([Name], Description) values
('Edit User Permissions', 'NULL'),
('Approve Terms', 'NULL'),
('Create New Terms', 'NULL'),
('Delete Approved Terms', 'NULL'),
('Delete Unapproved Terms', 'NULL'),
('Edit Approved ReportObjects Documentation', 'NULL'),
('Edit Approved Terms', 'NULL'),
('Edit Report Documentation', 'NULL'),
('Edit Unapproved Terms', 'NULL'),
('Link/Unlink Terms and ReportObjects', 'NULL'),
('View Hidden ReportObjects', 'NULL'),
('View Unapproved Terms', 'NULL'),
('Edit Role Permissions', 'NULL'),
('Search', 'NULL'),
('Create Initiative', 'NULL'),
('Delete Initiative', 'NULL'),
('Edit Initiative', 'NULL'),
('Create Milestone Template', 'NULL'),
('Delete Milestone Template', 'NULL'),
('Delete Comments', 'NULL'),
('Create Project', 'NULL'),
('Delete Project', 'NULL'),
('Edit Project', 'NULL'),
('Complete Milestone Task Checklist Item', 'NULL'),
('Complete Milestone Task', 'NULL'),
('Create Contacts', 'NULL'),
('Delete Contacts', 'NULL'),
('Create Parameters', 'NULL'),
('Delete Parameters', 'NULL'),
('Edit Report Purge Option', 'NULL'),
('View Site Analytics', 'NULL'),
('View Other User', 'NULL'),
('Open In Editor', 'NULL'),
('View Report Profiles', 'NULL'),
('Search For Other User', 'NULL'),
('Edit Other Users', 'NULL'),
('Uncomplete Milestone Task Checklist Item', 'NULL'),
('Edit Report Hidden Option', 'NULL'),
('Can Change Roles', 'NULL'),
('Manage Global Site Settings', 'NULL')
GO
insert into [atlas].app.RolePermissionLinks (RoleId, RolePermissionsId) values 
(3,7),
(3,10),
(3,12),
(3,13),
(3,14),
(3,16),
(3,17),
(4,6),
(4,7),
(4,9),
(4,11),
(4,13),
(4,17),
(5,17),
(5,19),
(2,19),
(3,19),
(4,19),
(5,39),
(2,37),
(3,37),
(4,37),
(5,37),
(2,39),
(3,39),
(4,39),
(2,40),
(3,40),
(4,40),
(5,40)
GO
insert into [atlas].app.OrganizationalValue (OrganizationalValueName) values ('Critical'),
('High'),
('Medium-High'),
('Medium')
GO
insert into [atlas].app.MaintenanceSchedule (MaintenanceScheduleName) values

('Quarterly'),
('Twice a Year'),
('Yearly'),
('Every Two Years'),
('Audit Only')
GO
insert into [atlas].app.MaintenanceLogStatus (MaintenanceLogStatusName) VALUES
('Approved - No Changes'),
('Approved - With Changes'),
('Recommend Retire')
GO
insert into [atlas].app.Fragility (FragilityName) VALUES
('High'),
('Medium'),
('Low'),
('Very Low')
GO
insert into [atlas].app.FragilityTag (FragilityTagName) VALUES
('Facility Build'),
('Procedure Code (CPT)'),
('Procedure Code (ICD)'),
('Procedure Code (Proc ID)'),
('Diagnosis Code'),
('Regulatory Changes'),
('Payor Plan Name'),
('Agency Build'),
('Service Area'),
('SER Build'),
('Workqueues'),
('Billing Indicators'),
('Batches'),
('M-Code'),
('Patient Class'),
('Bad Debt'),
('New Application'),
('Rules Engine'),
('Registry'),
('SQL'),
('Free Text'),
('ClarityREF Dependencies'),
('External Application'),
('Flowsheets'),
('Primary Care Provider'),
('Order Sets'),
('Workflow Dependent')
GO
insert into [atlas].app.FinancialImpact ([Name]) values 
('Medium'),
('High'),
('Critical')
GO
insert into [atlas].app.EstimatedRunFrequency (EstimatedRunFrequencyName) VALUES
('Multiple Times Per Day'),
('Daily'),
('Weekly'),
('Monthly'),
('Yearly')
GO
insert into [atlas].app.DP_MilestoneFrequency ([Name]) VALUES
('Day(s)'),
('Week(s)'),
('Month(s)'),
('Year(s)'),
('Quarter(s)')
GO
insert into [atlas].app.StrategicImportance ([Name]) VALUES
('Medium'),
('High'),
('Critical')
GO
