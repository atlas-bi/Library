
CREATE DATABASE [atlas]CONTAINMENT = NONE  ;
GO
ALTER DATABASE [atlas] SET COMPATIBILITY_LEVEL = 130
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
ALTER DATABASE [atlas] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [atlas] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [atlas] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [atlas] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [atlas] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [atlas] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [atlas] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [atlas] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [atlas] SET ALLOW_SNAPSHOT_ISOLATION ON
GO
ALTER DATABASE [atlas] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [atlas] SET READ_COMMITTED_SNAPSHOT ON
GO
ALTER DATABASE [atlas] SET  MULTI_USER
GO
ALTER DATABASE [atlas] SET ENCRYPTION ON
GO
ALTER DATABASE [atlas] SET QUERY_STORE = ON
GO
ALTER DATABASE [atlas] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 2048, QUERY_CAPTURE_MODE = ALL, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200)
GO
/*** The scripts of database scoped configurations in Azure should be executed inside the target database connection. ***/
GO
-- ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 8;
GO
/****** Object:  Schema [app]    Script Date: 4/7/2022 2:25:43 PM ******/
CREATE SCHEMA [app]
GO
/****** Object:  Table [app].[Analytics]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[AnalyticsTrace]    Script Date: 4/7/2022 2:25:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[AnalyticsTrace](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[Level] [int] NULL,
	[Message] [nvarchar](max) NULL,
	[Logger] [nvarchar](max) NULL,
	[logDateTime] [datetime] NULL,
	[logId] [nvarchar](max) NULL,
	[handled] [int] NULL,
	[updateTime] [datetime] NULL,
	[userAgent] [nvarchar](max) NULL,
	[referer] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[DP_Agreement]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[DP_AgreementUsers]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[DP_Attachments]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[DP_Contact]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[DP_Contact_Links]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[DP_DataInitiative]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[DP_DataProject]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[Dp_DataProjectConversation]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[Dp_DataProjectConversationMessage]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[DP_MilestoneChecklist]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[DP_MilestoneChecklistCompleted]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[DP_MilestoneFrequency]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[DP_MilestoneTasks]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[DP_MilestoneTasksCompleted]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[DP_MilestoneTemplates]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[DP_ReportAnnotation]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[DP_TermAnnotation]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[EstimatedRunFrequency]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[FinancialImpact]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[Fragility]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[FragilityTag]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[GlobalSiteSettings]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[Mail_Conversations]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[Mail_Drafts]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[Mail_FolderMessages]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[Mail_Folders]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[Mail_Messages]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[Mail_MessageType]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[Mail_Recipients]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[Mail_Recipients_Deleted]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[MaintenanceLog]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[MaintenanceLogStatus]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[MaintenanceSchedule]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[OrganizationalValue]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[ReportManageEngineTickets]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[ReportObject_doc]    Script Date: 4/7/2022 2:25:43 PM ******/
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
	[DeveloperNotes] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED
(
	[ReportObjectID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[ReportObjectConversation_doc]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[ReportObjectConversationMessage_doc]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[ReportObjectDocFragilityTags]    Script Date: 4/7/2022 2:25:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[ReportObjectDocFragilityTags](
	[LinkId] [int] IDENTITY(1,1) NOT NULL,
	[ReportObjectID] [int] NOT NULL,
	[FragilityTagID] [int] NOT NULL,
PRIMARY KEY CLUSTERED
(
	[LinkId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[ReportObjectDocMaintenanceLogs]    Script Date: 4/7/2022 2:25:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[ReportObjectDocMaintenanceLogs](
	[LinkId] [int] IDENTITY(1,1) NOT NULL,
	[ReportObjectID] [int] NOT NULL,
	[MaintenanceLogID] [int] NOT NULL,
PRIMARY KEY CLUSTERED
(
	[LinkId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[ReportObjectDocTerms]    Script Date: 4/7/2022 2:25:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[ReportObjectDocTerms](
	[LinkId] [int] IDENTITY(1,1) NOT NULL,
	[ReportObjectID] [int] NOT NULL,
	[TermId] [int] NOT NULL,
PRIMARY KEY CLUSTERED
(
	[LinkId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[ReportObjectImages_doc]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[ReportObjectReportRunTime]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[ReportObjectRunTime]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[ReportObjectTopRuns]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[ReportObjectWeightedRunRank]    Script Date: 4/7/2022 2:25:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[ReportObjectWeightedRunRank](
	[reportobjectid] [int] NOT NULL,
	[weighted_run_rank] [numeric](12, 4) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [app].[RolePermissionLinks]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[RolePermissions]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[Search_BasicSearchData]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[Search_BasicSearchData_Small]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[Search_ReportObjectSearchData]    Script Date: 4/7/2022 2:25:43 PM ******/
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
	[certificationTag] [nvarchar](max) NULL,
	[TwoYearRuns] [int] NULL,
	[OneYearRuns] [int] NULL,
	[SixMonthsRuns] [int] NULL,
	[OneMonthRuns] [int] NULL,
 CONSTRAINT [Search_ReportObjectSearchData_PK] PRIMARY KEY CLUSTERED
(
	[pk] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[SearchTable]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[SharedItems]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[StarredCollections]    Script Date: 4/7/2022 2:25:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[StarredCollections](
	[star_id] [int] IDENTITY(1,1) NOT NULL,
	[rank] [int] NULL,
	[collectionid] [int] NULL,
	[ownerid] [int] NULL,
	[folderid] [int] NULL,
PRIMARY KEY CLUSTERED
(
	[star_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[StarredGroups]    Script Date: 4/7/2022 2:25:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[StarredGroups](
	[star_id] [int] IDENTITY(1,1) NOT NULL,
	[rank] [int] NULL,
	[groupid] [int] NULL,
	[ownerid] [int] NULL,
	[folderid] [int] NULL,
PRIMARY KEY CLUSTERED
(
	[star_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[StarredInitiatives]    Script Date: 4/7/2022 2:25:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[StarredInitiatives](
	[star_id] [int] IDENTITY(1,1) NOT NULL,
	[rank] [int] NULL,
	[initiativeid] [int] NULL,
	[ownerid] [int] NULL,
	[folderid] [int] NULL,
PRIMARY KEY CLUSTERED
(
	[star_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[StarredReports]    Script Date: 4/7/2022 2:25:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[StarredReports](
	[star_id] [int] IDENTITY(1,1) NOT NULL,
	[rank] [int] NULL,
	[reportid] [int] NULL,
	[ownerid] [int] NULL,
	[folderid] [int] NULL,
PRIMARY KEY CLUSTERED
(
	[star_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[StarredSearches]    Script Date: 4/7/2022 2:25:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[StarredSearches](
	[star_id] [int] IDENTITY(1,1) NOT NULL,
	[rank] [int] NULL,
	[search] [nvarchar](max) NULL,
	[ownerid] [int] NULL,
	[folderid] [int] NULL,
PRIMARY KEY CLUSTERED
(
	[star_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[StarredTerms]    Script Date: 4/7/2022 2:25:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[StarredTerms](
	[star_id] [int] IDENTITY(1,1) NOT NULL,
	[rank] [int] NULL,
	[termid] [int] NULL,
	[ownerid] [int] NULL,
	[folderid] [int] NULL,
PRIMARY KEY CLUSTERED
(
	[star_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[StarredUsers]    Script Date: 4/7/2022 2:25:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[StarredUsers](
	[star_id] [int] IDENTITY(1,1) NOT NULL,
	[rank] [int] NULL,
	[userid] [int] NULL,
	[ownerid] [int] NULL,
	[folderid] [int] NULL,
PRIMARY KEY CLUSTERED
(
	[star_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[StrategicImportance]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[Term]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[TermConversation]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[TermConversationMessage]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[User_NameData]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[UserFavoriteFolders]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[UserFavorites]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[UserPreferences]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[UserRoleLinks]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[UserRoles]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportCertificationTags]    Script Date: 4/7/2022 2:25:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportCertificationTags](
	[Cert_ID] [int] NOT NULL,
	[CertName] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Priority] [int] NULL,
 CONSTRAINT [PK_ReportCertificationTags] PRIMARY KEY CLUSTERED
(
	[Cert_ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportGroupsMemberships]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportObject]    Script Date: 4/7/2022 2:25:43 PM ******/
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
	[CertificationTag] [nvarchar](max) NULL,
	[Availability] [nvarchar](max) NULL,
	[CertificationTagId] [int] NULL,
	[Runs] [int] NULL,
PRIMARY KEY CLUSTERED
(
	[ReportObjectID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportObjectAttachments]    Script Date: 4/7/2022 2:25:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportObjectAttachments](
	[ReportObjectAttachmentId] [int] IDENTITY(1,1) NOT NULL,
	[ReportObjectId] [int] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Path] [nvarchar](max) NOT NULL,
	[CreationDate] [datetime] NULL,
	[Source] [nvarchar](max) NULL,
	[Type] [nvarchar](max) NULL,
	[LastLoadDate] [datetime] NULL,
PRIMARY KEY CLUSTERED
(
	[ReportObjectAttachmentId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportObjectHierarchy]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportObjectParameters]    Script Date: 4/7/2022 2:25:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportObjectParameters](
	[ReportObjectParameterId] [int] IDENTITY(1,1) NOT NULL,
	[ReportObjectID] [int] NULL,
	[ParameterName] [nvarchar](max) NULL,
	[ParameterValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_ReportObjectParameters] PRIMARY KEY CLUSTERED
(
	[ReportObjectParameterId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportObjectQuery]    Script Date: 4/7/2022 2:25:43 PM ******/
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
	[Language] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED
(
	[ReportObjectQueryId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportObjectRunData]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportObjectSubscriptions]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportObjectTagMemberships]    Script Date: 4/7/2022 2:25:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportObjectTagMemberships](
	[TagMembershipID] [int] IDENTITY(1,1) NOT NULL,
	[ReportObjectID] [int] NOT NULL,
	[TagID] [int] NOT NULL,
	[Line] [int] NULL,
 CONSTRAINT [PK_ReportObjectTagMemberships] PRIMARY KEY CLUSTERED
(
	[TagMembershipID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportObjectTags]    Script Date: 4/7/2022 2:25:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportObjectTags](
	[TagID] [int] IDENTITY(1,1) NOT NULL,
	[EpicTagID] [numeric](18, 0) NULL,
	[TagName] [varchar](200) NULL,
 CONSTRAINT [PK_ReportObjectTags] PRIMARY KEY CLUSTERED
(
	[TagID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportObjectType]    Script Date: 4/7/2022 2:25:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportObjectType](
	[ReportObjectTypeID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[DefaultEpicMasterFile] [nvarchar](3) NULL,
	[LastLoadDate] [datetime] NULL,
	[ShortName] [nvarchar](max) NULL,
	[Visible] [nvarchar](1) NULL,
PRIMARY KEY CLUSTERED
(
	[ReportObjectTypeID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 4/7/2022 2:25:43 PM ******/
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
	[Fullname_calc] [nvarchar](max) NULL,
	[Firstname_calc] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED
(
	[UserID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserGroups]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserGroupsMembership]    Script Date: 4/7/2022 2:25:43 PM ******/
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
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [approved]    Script Date: 4/7/2022 2:25:43 PM ******/
CREATE NONCLUSTERED INDEX [approved] ON [app].[Term]
(
	[ApprovedYN] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [validfrom]    Script Date: 4/7/2022 2:25:43 PM ******/
CREATE NONCLUSTERED INDEX [validfrom] ON [app].[Term]
(
	[ValidFromDateTime] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
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
ALTER TABLE [app].[AnalyticsTrace]  WITH CHECK ADD  CONSTRAINT [FK_Analytics_Trace_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[AnalyticsTrace] CHECK CONSTRAINT [FK_Analytics_Trace_User]
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
ALTER TABLE [app].[StarredCollections]  WITH CHECK ADD  CONSTRAINT [FK_StarredCollections_DP_DataProject] FOREIGN KEY([collectionid])
REFERENCES [app].[DP_DataProject] ([DataProjectID])
GO
ALTER TABLE [app].[StarredCollections] CHECK CONSTRAINT [FK_StarredCollections_DP_DataProject]
GO
ALTER TABLE [app].[StarredCollections]  WITH CHECK ADD  CONSTRAINT [FK_StarredCollections_User] FOREIGN KEY([ownerid])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[StarredCollections] CHECK CONSTRAINT [FK_StarredCollections_User]
GO
ALTER TABLE [app].[StarredCollections]  WITH CHECK ADD  CONSTRAINT [FK_StarredCollections_UserFavoriteFolders] FOREIGN KEY([folderid])
REFERENCES [app].[UserFavoriteFolders] ([UserFavoriteFolderId])
GO
ALTER TABLE [app].[StarredCollections] CHECK CONSTRAINT [FK_StarredCollections_UserFavoriteFolders]
GO
ALTER TABLE [app].[StarredGroups]  WITH CHECK ADD  CONSTRAINT [FK_StarredGroups_User] FOREIGN KEY([ownerid])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[StarredGroups] CHECK CONSTRAINT [FK_StarredGroups_User]
GO
ALTER TABLE [app].[StarredGroups]  WITH CHECK ADD  CONSTRAINT [FK_StarredGroups_UserFavoriteFolders] FOREIGN KEY([folderid])
REFERENCES [app].[UserFavoriteFolders] ([UserFavoriteFolderId])
GO
ALTER TABLE [app].[StarredGroups] CHECK CONSTRAINT [FK_StarredGroups_UserFavoriteFolders]
GO
ALTER TABLE [app].[StarredGroups]  WITH CHECK ADD  CONSTRAINT [FK_StarredGroups_UserGroups] FOREIGN KEY([groupid])
REFERENCES [dbo].[UserGroups] ([GroupId])
GO
ALTER TABLE [app].[StarredGroups] CHECK CONSTRAINT [FK_StarredGroups_UserGroups]
GO
ALTER TABLE [app].[StarredInitiatives]  WITH CHECK ADD  CONSTRAINT [FK_StarredInitiatives_DP_DataInitiative] FOREIGN KEY([initiativeid])
REFERENCES [app].[DP_DataInitiative] ([DataInitiativeID])
GO
ALTER TABLE [app].[StarredInitiatives] CHECK CONSTRAINT [FK_StarredInitiatives_DP_DataInitiative]
GO
ALTER TABLE [app].[StarredInitiatives]  WITH CHECK ADD  CONSTRAINT [FK_StarredInitiatives_User] FOREIGN KEY([ownerid])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[StarredInitiatives] CHECK CONSTRAINT [FK_StarredInitiatives_User]
GO
ALTER TABLE [app].[StarredInitiatives]  WITH CHECK ADD  CONSTRAINT [FK_StarredInitiatives_UserFavoriteFolders] FOREIGN KEY([folderid])
REFERENCES [app].[UserFavoriteFolders] ([UserFavoriteFolderId])
GO
ALTER TABLE [app].[StarredInitiatives] CHECK CONSTRAINT [FK_StarredInitiatives_UserFavoriteFolders]
GO
ALTER TABLE [app].[StarredReports]  WITH CHECK ADD  CONSTRAINT [FK_StarredReports_ReportObject] FOREIGN KEY([reportid])
REFERENCES [dbo].[ReportObject] ([ReportObjectID])
GO
ALTER TABLE [app].[StarredReports] CHECK CONSTRAINT [FK_StarredReports_ReportObject]
GO
ALTER TABLE [app].[StarredReports]  WITH CHECK ADD  CONSTRAINT [FK_StarredReports_User] FOREIGN KEY([ownerid])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[StarredReports] CHECK CONSTRAINT [FK_StarredReports_User]
GO
ALTER TABLE [app].[StarredReports]  WITH CHECK ADD  CONSTRAINT [FK_StarredReports_UserFavoriteFolders] FOREIGN KEY([folderid])
REFERENCES [app].[UserFavoriteFolders] ([UserFavoriteFolderId])
GO
ALTER TABLE [app].[StarredReports] CHECK CONSTRAINT [FK_StarredReports_UserFavoriteFolders]
GO
ALTER TABLE [app].[StarredSearches]  WITH CHECK ADD  CONSTRAINT [FK_StarredSearches_User] FOREIGN KEY([ownerid])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[StarredSearches] CHECK CONSTRAINT [FK_StarredSearches_User]
GO
ALTER TABLE [app].[StarredSearches]  WITH CHECK ADD  CONSTRAINT [FK_StarredSearches_UserFavoriteFolders] FOREIGN KEY([folderid])
REFERENCES [app].[UserFavoriteFolders] ([UserFavoriteFolderId])
GO
ALTER TABLE [app].[StarredSearches] CHECK CONSTRAINT [FK_StarredSearches_UserFavoriteFolders]
GO
ALTER TABLE [app].[StarredTerms]  WITH CHECK ADD  CONSTRAINT [FK_StarredTerms_Term] FOREIGN KEY([termid])
REFERENCES [app].[Term] ([TermId])
GO
ALTER TABLE [app].[StarredTerms] CHECK CONSTRAINT [FK_StarredTerms_Term]
GO
ALTER TABLE [app].[StarredTerms]  WITH CHECK ADD  CONSTRAINT [FK_StarredTerms_User] FOREIGN KEY([ownerid])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[StarredTerms] CHECK CONSTRAINT [FK_StarredTerms_User]
GO
ALTER TABLE [app].[StarredTerms]  WITH CHECK ADD  CONSTRAINT [FK_StarredTerms_UserFavoriteFolders] FOREIGN KEY([folderid])
REFERENCES [app].[UserFavoriteFolders] ([UserFavoriteFolderId])
GO
ALTER TABLE [app].[StarredTerms] CHECK CONSTRAINT [FK_StarredTerms_UserFavoriteFolders]
GO
ALTER TABLE [app].[StarredUsers]  WITH CHECK ADD  CONSTRAINT [FK_StarredUsers_User] FOREIGN KEY([userid])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[StarredUsers] CHECK CONSTRAINT [FK_StarredUsers_User]
GO
ALTER TABLE [app].[StarredUsers]  WITH CHECK ADD  CONSTRAINT [FK_StarredUsers_User_owner] FOREIGN KEY([ownerid])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[StarredUsers] CHECK CONSTRAINT [FK_StarredUsers_User_owner]
GO
ALTER TABLE [app].[StarredUsers]  WITH CHECK ADD  CONSTRAINT [FK_StarredUsers_UserFavoriteFolders] FOREIGN KEY([folderid])
REFERENCES [app].[UserFavoriteFolders] ([UserFavoriteFolderId])
GO
ALTER TABLE [app].[StarredUsers] CHECK CONSTRAINT [FK_StarredUsers_UserFavoriteFolders]
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
ALTER TABLE [dbo].[ReportObject]  WITH CHECK ADD FOREIGN KEY([AuthorUserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[ReportObject]  WITH CHECK ADD FOREIGN KEY([AuthorUserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[ReportObject]  WITH CHECK ADD FOREIGN KEY([LastModifiedByUserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[ReportObject]  WITH CHECK ADD FOREIGN KEY([LastModifiedByUserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[ReportObject]  WITH CHECK ADD FOREIGN KEY([LastModifiedByUserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[ReportObject]  WITH CHECK ADD FOREIGN KEY([ReportObjectTypeID])
REFERENCES [dbo].[ReportObjectType] ([ReportObjectTypeID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[ReportObjectAttachments]  WITH CHECK ADD  CONSTRAINT [FK_ReportObjectAttachments_ReportObject] FOREIGN KEY([ReportObjectId])
REFERENCES [dbo].[ReportObject] ([ReportObjectID])
GO
ALTER TABLE [dbo].[ReportObjectAttachments] CHECK CONSTRAINT [FK_ReportObjectAttachments_ReportObject]
GO
ALTER TABLE [dbo].[ReportObjectHierarchy]  WITH CHECK ADD FOREIGN KEY([ChildReportObjectID])
REFERENCES [dbo].[ReportObject] ([ReportObjectID])
GO
ALTER TABLE [dbo].[ReportObjectHierarchy]  WITH CHECK ADD FOREIGN KEY([ParentReportObjectID])
REFERENCES [dbo].[ReportObject] ([ReportObjectID])
GO
ALTER TABLE [dbo].[ReportObjectParameters]  WITH CHECK ADD  CONSTRAINT [FK_ReportObjectParameters1_ReportObject] FOREIGN KEY([ReportObjectID])
REFERENCES [dbo].[ReportObject] ([ReportObjectID])
GO
ALTER TABLE [dbo].[ReportObjectParameters] CHECK CONSTRAINT [FK_ReportObjectParameters1_ReportObject]
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
ALTER TABLE [dbo].[ReportObjectTagMemberships]  WITH CHECK ADD  CONSTRAINT [FK_ReportObjectTagMemberships_ReportObjectTags] FOREIGN KEY([TagID])
REFERENCES [dbo].[ReportObjectTags] ([TagID])
GO
ALTER TABLE [dbo].[ReportObjectTagMemberships] CHECK CONSTRAINT [FK_ReportObjectTagMemberships_ReportObjectTags]
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
ALTER DATABASE [atlas] SET  READ_WRITE
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
