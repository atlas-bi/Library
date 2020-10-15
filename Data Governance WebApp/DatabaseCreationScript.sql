/*
    Atlas of Information Management business intelligence library and documentation database.
    Copyright (C) 2020  Riverside Healthcare, Kankakee, IL

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

USE [master]
GO
/****** Object:  Database [Data_Governance_Dev]    Script Date: 3/18/2020 2:40:00 PM ******/
CREATE DATABASE [Data_Governance_Dev]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Data_Governance', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS01\MSSQL\DATA\Data_Governance_Dev.mdf' , SIZE = 401408KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Data_Governance_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS01\MSSQL\DATA\Data_Governance_Dev_log.ldf' , SIZE = 860160KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [Data_Governance_Dev] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Data_Governance_Dev].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Data_Governance_Dev] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Data_Governance_Dev] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Data_Governance_Dev] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Data_Governance_Dev] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Data_Governance_Dev] SET ARITHABORT OFF 
GO
ALTER DATABASE [Data_Governance_Dev] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Data_Governance_Dev] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Data_Governance_Dev] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Data_Governance_Dev] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Data_Governance_Dev] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Data_Governance_Dev] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Data_Governance_Dev] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Data_Governance_Dev] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Data_Governance_Dev] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Data_Governance_Dev] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Data_Governance_Dev] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Data_Governance_Dev] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Data_Governance_Dev] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Data_Governance_Dev] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Data_Governance_Dev] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Data_Governance_Dev] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Data_Governance_Dev] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Data_Governance_Dev] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Data_Governance_Dev] SET  MULTI_USER 
GO
ALTER DATABASE [Data_Governance_Dev] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Data_Governance_Dev] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Data_Governance_Dev] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Data_Governance_Dev] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Data_Governance_Dev] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Data_Governance_Dev] SET QUERY_STORE = OFF
GO
USE [Data_Governance_Dev]
GO

/****** Object:  User [datagov]    Script Date: 3/18/2020 2:40:00 PM ******/
CREATE USER [datagov] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [datagov]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [datagov]
GO
ALTER ROLE [db_datareader] ADD MEMBER [datagov]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [datagov]
GO
/****** Object:  Schema [app]    Script Date: 3/18/2020 2:40:00 PM ******/
CREATE SCHEMA [app]
GO
/****** Object:  FullTextCatalog [ReportObjectDocs]    Script Date: 3/18/2020 2:40:01 PM ******/
CREATE FULLTEXT CATALOG [ReportObjectDocs] WITH ACCENT_SENSITIVITY = ON
GO
/****** Object:  FullTextCatalog [ReportObjects]    Script Date: 3/18/2020 2:40:01 PM ******/
CREATE FULLTEXT CATALOG [ReportObjects] WITH ACCENT_SENSITIVITY = OFF
AS DEFAULT
GO
/****** Object:  FullTextCatalog [Search]    Script Date: 3/18/2020 2:40:01 PM ******/
CREATE FULLTEXT CATALOG [Search] WITH ACCENT_SENSITIVITY = OFF
GO
/****** Object:  FullTextCatalog [Terms]    Script Date: 3/18/2020 2:40:01 PM ******/
CREATE FULLTEXT CATALOG [Terms] WITH ACCENT_SENSITIVITY = ON
GO
/****** Object:  UserDefinedFunction [dbo].[ToProperCase]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[ToProperCase](@string VARCHAR(255)) RETURNS VARCHAR(255)
AS
BEGIN
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
/****** Object:  Table [app].[Analytics]    Script Date: 3/18/2020 2:40:01 PM ******/
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
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[BusinessApplication_doc]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[BusinessApplication_doc](
	[BusinessApplicationID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[BusinessApplicationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[DP_Agreement]    Script Date: 3/18/2020 2:40:01 PM ******/
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
PRIMARY KEY CLUSTERED 
(
	[AgreementID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[DP_AgreementUsers]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[DP_Contact]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[DP_Contact_Links]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[DP_DataInitiative]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[DP_DataProject]    Script Date: 3/18/2020 2:40:01 PM ******/
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
PRIMARY KEY CLUSTERED 
(
	[DataProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[Dp_DataProjectConversation]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[Dp_DataProjectConversationMessage]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[DP_MilestoneChecklist]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[DP_MilestoneChecklistCompleted]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[DP_MilestoneFrequency]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[DP_MilestoneTasks]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[DP_MilestoneTasksCompleted]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[DP_MilestoneTemplates]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[DP_ReportAnnotation]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[DP_ReportAnnotation](
	[ReportAnnotationID] [int] IDENTITY(1,1) NOT NULL,
	[Annotation] [nvarchar](max) NULL,
	[ReportId] [int] NULL,
	[DataProjectId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ReportAnnotationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[DP_TermAnnotation]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[DP_TermAnnotation](
	[TermAnnotationID] [int] IDENTITY(1,1) NOT NULL,
	[Annotation] [nvarchar](max) NULL,
	[TermId] [int] NULL,
	[DataProjectId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[TermAnnotationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[EstimatedRunFrequency]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[FinancialImpact]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[Fragility]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[FragilityTag]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[GlobalSiteSettings]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[MaintenanceLog]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[MaintenanceLogStatus]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[MaintenanceSchedule]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[OrganizationalValue]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[ReportManageEngineTickets]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[ReportObject_doc]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[ReportObjectConversation_doc]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[ReportObjectConversationMessage_doc]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[ReportObjectDocFragilityTags]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[ReportObjectDocMaintenanceLogs]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[ReportObjectDocTerms]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[ReportObjectImages_doc]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[ReportObjectImages_doc](
	[ImageID] [int] IDENTITY(1,1) NOT NULL,
	[ReportObjectID] [int] NOT NULL,
	[ImageOrdinal] [int] NOT NULL,
	[ImageData] [varbinary](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ImageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[ReportObjectRunTime]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[ReportObjectTopRuns]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[RolePermissionLinks]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[RolePermissions]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[SearchTable]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[StrategicImportance]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[Term]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[Term](
	[TermId] [int] IDENTITY(1,1) NOT NULL,
	[ParentTermId] [int] NULL,
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
/****** Object:  Table [app].[TermCodeExamples]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[TermCodeExamples](
	[TermCodeExampleId] [int] IDENTITY(1,1) NOT NULL,
	[TermId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TermCodeExampleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[TermCodeExamplesHistory]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[TermCodeExamplesHistory](
	[TermCodeExamplesHistoryId] [int] IDENTITY(1,1) NOT NULL,
	[TermId] [int] NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[Code] [nvarchar](max) NULL,
	[FormatAs] [nvarchar](25) NOT NULL,
	[UpdatedByUserId] [int] NOT NULL,
	[ValidFromDateTime] [datetime] NOT NULL,
	[ValidToDateTime] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TermCodeExamplesHistoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[TermConversation]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[TermConversationMessage]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[TermHistory]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[TermHistory](
	[TermHistoryId] [int] IDENTITY(1,1) NOT NULL,
	[TermId] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Summary] [nvarchar](4000) NULL,
	[TechnicalDefinition] [nvarchar](max) NULL,
	[ApprovedYN] [char](1) NOT NULL,
	[ApprovalDateTime] [datetime] NULL,
	[ApprovedByUserId] [int] NULL,
	[HasExternalStandardYN] [char](1) NOT NULL,
	[ExternalStandardUrl] [nvarchar](4000) NULL,
	[ValidFromDateTime] [datetime] NOT NULL,
	[ValidToDateTime] [datetime] NOT NULL,
	[UpdatedByUserId] [int] NOT NULL,
	[LastUpdatedDateTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[TermHistoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[UserFavoriteFolders]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[UserFavorites]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[UserLDAPGroupMembership]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[UserLDAPGroupMembership](
	[MembershipId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[GroupId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[MembershipId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[UserLDAPGroups]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[UserLDAPGroups](
	[GroupId] [int] IDENTITY(1,1) NOT NULL,
	[AccountName] [nvarchar](max) NULL,
	[GroupName] [nvarchar](max) NULL,
	[GroupEmail] [nvarchar](max) NULL,
	[GroupType] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[UserPreferences]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[UserRoleLinks]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [app].[UserRoles]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 3/18/2020 2:40:01 PM ******/
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
/****** Object:  Table [dbo].[ReportObject]    Script Date: 3/18/2020 2:40:01 PM ******/
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
	[NULLColumnNumeric] [numeric](18, 0) NULL,
PRIMARY KEY CLUSTERED 
(
	[ReportObjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportObjectHierarchy]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportObjectHierarchy](
	[ParentReportObjectID] [int] NOT NULL,
	[ChildReportObjectID] [int] NOT NULL,
	[Line] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ParentReportObjectID] ASC,
	[ChildReportObjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportObjectQuery]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportObjectQuery](
	[ReportObjectQueryId] [int] IDENTITY(1,1) NOT NULL,
	[ReportObjectId] [int] NULL,
	[Query] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ReportObjectQueryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportObjectRunData]    Script Date: 3/18/2020 2:40:01 PM ******/
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
 CONSTRAINT [PK_ReportObjectRunData] PRIMARY KEY CLUSTERED 
(
	[ReportObjectID] ASC,
	[RunID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportObjectSubscriptions]    Script Date: 3/18/2020 2:40:01 PM ******/
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
PRIMARY KEY CLUSTERED 
(
	[ReportObjectSubscriptionsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportObjectType]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportObjectType](
	[ReportObjectTypeID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[DefaultEpicMasterFile] [nvarchar](3) NULL,
PRIMARY KEY CLUSTERED 
(
	[ReportObjectTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 3/18/2020 2:40:01 PM ******/
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
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [approved]    Script Date: 3/18/2020 2:40:01 PM ******/
CREATE NONCLUSTERED INDEX [approved] ON [app].[Term]
(
	[ApprovedYN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [validfrom]    Script Date: 3/18/2020 2:40:01 PM ******/
CREATE NONCLUSTERED INDEX [validfrom] ON [app].[Term]
(
	[ValidFromDateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [app].[DP_MilestoneChecklistCompleted] ADD  DEFAULT ((0)) FOR [ChecklistStatus]
GO
ALTER TABLE [app].[ReportObjectImages_doc] ADD  DEFAULT ((1)) FOR [ImageOrdinal]
GO
ALTER TABLE [app].[TermCodeExamplesHistory] ADD  DEFAULT (N'SQL') FOR [FormatAs]
GO
ALTER TABLE [app].[TermCodeExamplesHistory] ADD  DEFAULT (getdate()) FOR [ValidFromDateTime]
GO
ALTER TABLE [app].[TermCodeExamplesHistory] ADD  DEFAULT ('12-31-9999') FOR [ValidToDateTime]
GO
ALTER TABLE [app].[TermConversationMessage] ADD  DEFAULT (getdate()) FOR [PostDateTime]
GO
ALTER TABLE [app].[TermHistory] ADD  DEFAULT (N'N') FOR [ApprovedYN]
GO
ALTER TABLE [app].[TermHistory] ADD  DEFAULT (N'N') FOR [HasExternalStandardYN]
GO
ALTER TABLE [app].[TermHistory] ADD  DEFAULT (getdate()) FOR [ValidFromDateTime]
GO
ALTER TABLE [app].[TermHistory] ADD  DEFAULT ('12-31-9999') FOR [ValidToDateTime]
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
ALTER TABLE [app].[DP_DataProject]  WITH CHECK ADD  CONSTRAINT [FK_DP_DataProject_DP_DataInitiative] FOREIGN KEY([OperationOwnerID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[DP_DataProject] CHECK CONSTRAINT [FK_DP_DataProject_DP_DataInitiative]
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
ALTER TABLE [app].[DP_MilestoneTasks]  WITH CHECK ADD  CONSTRAINT [FK_DP_MilestoneTasks_User] FOREIGN KEY([OwnerId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[DP_MilestoneTasks] CHECK CONSTRAINT [FK_DP_MilestoneTasks_User]
GO
ALTER TABLE [app].[DP_MilestoneTasks]  WITH CHECK ADD  CONSTRAINT [FK_DP_MilestoneTasks_WebAppUsers] FOREIGN KEY([LastUpdateUser])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[DP_MilestoneTasks] CHECK CONSTRAINT [FK_DP_MilestoneTasks_WebAppUsers]
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
ALTER TABLE [app].[Term]  WITH CHECK ADD FOREIGN KEY([ParentTermId])
REFERENCES [app].[Term] ([TermId])
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
ALTER TABLE [app].[TermCodeExamples]  WITH CHECK ADD FOREIGN KEY([TermId])
REFERENCES [app].[Term] ([TermId])
ON DELETE CASCADE
GO
ALTER TABLE [app].[TermCodeExamplesHistory]  WITH CHECK ADD FOREIGN KEY([TermId])
REFERENCES [app].[Term] ([TermId])
ON DELETE CASCADE
GO
ALTER TABLE [app].[TermCodeExamplesHistory]  WITH CHECK ADD  CONSTRAINT [FK__TermCodeE__Updat__531856C7] FOREIGN KEY([UpdatedByUserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[TermCodeExamplesHistory] CHECK CONSTRAINT [FK__TermCodeE__Updat__531856C7]
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
ALTER TABLE [app].[TermHistory]  WITH CHECK ADD  CONSTRAINT [FK__TermHisto__Appro__56E8E7AB] FOREIGN KEY([ApprovedByUserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[TermHistory] CHECK CONSTRAINT [FK__TermHisto__Appro__56E8E7AB]
GO
ALTER TABLE [app].[TermHistory]  WITH CHECK ADD  CONSTRAINT [FK__TermHisto__Updat__57DD0BE4] FOREIGN KEY([UpdatedByUserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[TermHistory] CHECK CONSTRAINT [FK__TermHisto__Updat__57DD0BE4]
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
ALTER TABLE [app].[UserLDAPGroupMembership]  WITH CHECK ADD  CONSTRAINT [FK_UserLDAPGroupMembership_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [app].[UserLDAPGroupMembership] CHECK CONSTRAINT [FK_UserLDAPGroupMembership_User]
GO
ALTER TABLE [app].[UserLDAPGroupMembership]  WITH CHECK ADD  CONSTRAINT [FK_UserLDAPGroupMembership_UserLDAPGroups] FOREIGN KEY([GroupId])
REFERENCES [app].[UserLDAPGroups] ([GroupId])
GO
ALTER TABLE [app].[UserLDAPGroupMembership] CHECK CONSTRAINT [FK_UserLDAPGroupMembership_UserLDAPGroups]
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
/****** Object:  StoredProcedure [app].[CalculateReportRunData]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Christopher Pickering
-- Create date: 1/3/2020
-- Description:	Used to update SearchTable
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

	select d.ReportObjectId, r.Name, RunUserId, Count(*) as Runs, Cast(ROUND(Avg(cast(RunDurationSeconds as decimal)),2)as decimal(10,2))  as RunTime,convert(nvarchar(MAX),  Max(d.RunStartTime), 101) as LastRun 
	into #myTemp
	from ReportObjectRunData as d
	inner join ReportObject as r on r.ReportObjectId = d.ReportObjectId
	where RunStatus = 'Success'
	group by d.ReportObjectId, RunUserId, r.Name
		
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
			LastRun
		from 
			#myTemp
		except (select ReportObjectId,
			Name,
			RunUserId,
			Runs,
			cast(RunTime as decimal(10,2)),
			LastRun from app.ReportObjectTopRuns)
	;

	-- delete old records from table
	delete app.ReportObjectTopRuns from app.ReportObjectTopRuns  l
	left join #myTemp as t on l.ReportObjectId = t.ReportObjectId
		and l.Name = t.Name
		and l.RunUserId = t.RunUserId
		and l.Runs = t.Runs
		and l.RunTime = t.RunTime
		and l.LastRun = t.LastRun
	where t.ReportObjectID is null

	;

	drop table if exists #myTemp;

End
GO
/****** Object:  StoredProcedure [app].[CalculateReportRunTimeData]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Christopher Pickering
-- Create date: 1/3/2020
-- Description:	Used to update SearchTable
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

	select RunUserId, Count(*) as Runs, Cast(ROUND(Avg(cast(RunDurationSeconds as decimal)),2)as decimal(10,2))  as RunTime,DATEADD(dd, DATEPART(DW,RunStartTime)*-1+1, GETDATE()) as RunWeek, convert(nvarchar(MAX),  DATEADD(dd, DATEPART(DW,RunStartTime)*-1+1, GETDATE()), 101) as RunWeekString
	into #myTemp
	from ReportObjectRunData as d
	
	where RunStatus = 'Success'
	group by DATEADD(dd, DATEPART(DW,RunStartTime)*-1+1, GETDATE()), RunUserId
		
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

End
GO
/****** Object:  StoredProcedure [app].[UpdateSearchTable]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Christopher Pickering
-- Create date: 1/3/2020
-- Description:	Used to update SearchTable
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
		where (ro.Hidden is null or ro.Hidden = 'N')		
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
/****** Object:  StoredProcedure [dbo].[BasicDirectorSearch]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Christopher Pickering
-- Create date: 11/13/19
-- Description:	Used to quickly search for users
--              for moving a "user" to a webuser
--              need full username :/ 
-- =============================================
CREATE PROCEDURE [dbo].[BasicDirectorSearch] 
	-- Add the parameters for the stored procedure here
	@searchTerm varchar(1000)
,	@exclude varchar(1000) = ''
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

      SET @searchTerm = REPLACE(REPLACE(LTRIM(RTRIM(@searchTerm)),' ','%'),'*','%');

SELECT TOP(10) Email, FullName 
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
/****** Object:  StoredProcedure [dbo].[BasicProjectSearch]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Christopher Pickering
-- Create date: 10/3/19
-- Description:	Used by Data Project to quickly search for report objects
-- =============================================
CREATE PROCEDURE [dbo].[BasicProjectSearch] 
	-- Add the parameters for the stored procedure here
	@searchTerm varchar(1000)
,	@exclude varchar(1000) = ''
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


   SET @searchTerm = REPLACE(REPLACE(LTRIM(RTRIM(@searchTerm)),' ','%'),'*','%');

	SELECT TOP (10) r.DataProjectID
	,			r.Name
	,			r.Description
		from		app.DP_DataProject r
	WHERE r.Name like concat('%', @searchTerm, '%')
	AND r.DataProjectID NOT IN (SELECT * FROM STRING_SPLIT(@exclude,','))
END
GO
/****** Object:  StoredProcedure [dbo].[BasicRealUserSearch]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Christopher Pickering
-- Create date: 11/13/19
-- Description:	Used to quickly search for users
--              for moving a "user" to a webuser
--              need full username :/ 
-- =============================================
CREATE PROCEDURE [dbo].[BasicRealUserSearch] 
	-- Add the parameters for the stored procedure here
	@searchTerm varchar(1000)
,	@exclude varchar(1000) = ''
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   SET @searchTerm = REPLACE(REPLACE(LTRIM(RTRIM(@searchTerm)),' ','%'),'*','%');

SELECT TOP(10) UserId, Username FROM 	dbo.[User] r
	WHERE r.Username NOT LIKE '%S.%'
	AND r.Username NOT LIKE ','

	and Username like concat('%', @searchTerm, '%')
	AND UserID NOT IN (SELECT * FROM STRING_SPLIT(@exclude,','))
	ORDER BY 2 asc
END
GO
/****** Object:  StoredProcedure [dbo].[BasicReportObjectSearch]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Christopher Pickering
-- Create date: 10/3/19
-- Description:	Used by Data Project to quickly search for report objects
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
	,			r.Name  as ReportObjectName
	,			r.Description
	
	from		ReportObject r
	inner join	ReportObjectType rt on rt.ReportObjectTypeID = r.ReportObjectTypeID
	where OrphanedReportObjectYN = 'N'
	and DefaultVisibilityYN = 'Y'
	and rt.ReportObjectTypeID IN (3,17,20,21,28)--Crystal, RW, DB, component, SSRS
	and r.Name like concat('%', @searchTerm, '%')
END
GO
/****** Object:  StoredProcedure [dbo].[BasicReportSearch]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Christopher Pickering
-- Create date: 10/3/19
-- Description:	Used by Data Project to quickly search for report objects
-- =============================================
CREATE PROCEDURE [dbo].[BasicReportSearch] 
	-- Add the parameters for the stored procedure here
	@searchTerm varchar(1000)
,	@exclude varchar(1000) = ''
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   SET @searchTerm = REPLACE(REPLACE(LTRIM(RTRIM(@searchTerm)),' ','%'),'*','%');

	SELECT TOP (10) r.ReportObjectID
	,			r.Name  as ReportObjectName
	,			r.Description
	
	from		ReportObject r
	inner join	ReportObjectType rt on rt.ReportObjectTypeID = r.ReportObjectTypeID
	where OrphanedReportObjectYN = 'N'
	and DefaultVisibilityYN = 'Y'
	and rt.ReportObjectTypeID IN (3,17,20,21,28)--Crystal, RW, DB, component, SSRS
	and r.Name like concat('%', @searchTerm, '%')
	AND r.ReportObjectID NOT IN (SELECT * FROM STRING_SPLIT(@exclude,','))
END
GO
/****** Object:  StoredProcedure [dbo].[BasicTermObjectSearch]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Christopher Pickering
-- Create date: 10/3/19
-- Description:	Used by Data Project to quickly search for report objects
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
	,			r.Name  
	,			r.Summary
	
	FROM [app].[Term] AS r
	where 1=1

	and r.name like concat('%', @searchTerm, '%')
END
GO
/****** Object:  StoredProcedure [dbo].[BasicTermSearch]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Christopher Pickering
-- Create date: 10/3/19
-- Description:	Used by Data Project to quickly search for report objects
-- =============================================
CREATE PROCEDURE [dbo].[BasicTermSearch] 
	-- Add the parameters for the stored procedure here
	@searchTerm varchar(1000)
,	@exclude varchar(1000) = ''
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   SET @searchTerm = REPLACE(REPLACE(LTRIM(RTRIM(@searchTerm)),' ','%'),'*','%');

	SELECT TOP (10) r.TermId
	,			r.Name  
	,			r.Summary
	
	FROM [app].[Term] AS r
	where 1=1

	and r.name like concat('%', @searchTerm, '%')
	AND r.TermId NOT IN (SELECT * FROM STRING_SPLIT(@exclude,','))
END
GO
/****** Object:  StoredProcedure [dbo].[BasicUserSearch]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Christopher Pickering
-- Create date: 10/24/19
-- Description:	Used to quickly search for users
-- =============================================
CREATE PROCEDURE [dbo].[BasicUserSearch] 
	-- Add the parameters for the stored procedure here
	@searchTerm varchar(1000)
,	@exclude varchar(1000) = ''
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   SET @searchTerm = REPLACE(REPLACE(LTRIM(RTRIM(@searchTerm)),' ','%'),'*','%');

	
SELECT TOP(10) * FROM (
SELECT r.UserID
		,dbo.ToProperCase(CASE WHEN r.Username LIKE '%,%' THEN SUBSTRING(CASE WHEN CHARINDEX(' ', REPLACE(r.Username,', ',',')) = 0 THEN REPLACE(r.Username,', ',',') ELSE SUBSTRING(REPLACE(r.Username,', ',','),0,
                CHARINDEX(' ', REPLACE(r.Username,', ',','))) END, CHARINDEX(',', CASE WHEN CHARINDEX(' ', REPLACE(r.Username,', ',',')) = 0 THEN REPLACE(r.Username,', ',',') ELSE SUBSTRING(REPLACE(r.Username,', ',','),0,
                CHARINDEX(' ', REPLACE(r.Username,', ',','))) END, 0) +1, 500) + ' ' + 
	SUBSTRING(CASE WHEN CHARINDEX(' ', REPLACE(r.Username,', ',',')) = 0 THEN REPLACE(r.Username,', ',',') ELSE SUBSTRING(REPLACE(r.Username,', ',','),0,
                CHARINDEX(' ', REPLACE(r.Username,', ',','))) END,0, CHARINDEX(',', CASE WHEN CHARINDEX(' ', REPLACE(r.Username,', ',',')) = 0 THEN REPLACE(r.Username,', ',',') ELSE SUBSTRING(REPLACE(r.Username,', ',','),0,
                CHARINDEX(' ', REPLACE(r.Username,', ',','))) END , 0) )
	ELSE 
	
		REPLACE(REPLACE(SUBSTRING(REPLACE(r.Username,' ',''),CHARINDEX('\',REPLACE(r.Username,' ',''))+1,500),'-',' '),'S.','')
	END) AS Username
	           
		from		dbo.[User] r
	WHERE r.Username NOT LIKE '%S.%'
	AND r.Username NOT LIKE ','
) AS table1
	WHERE table1.Username like concat('%', @searchTerm, '%')
	AND table1.UserID NOT IN (SELECT * FROM STRING_SPLIT(@exclude,','))
	ORDER BY 2 asc
END
GO
/****** Object:  StoredProcedure [dbo].[RelatedReports]    Script Date: 3/18/2020 2:40:01 PM ******/
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
			or s.SubscriptionTo = replace(lower(s2.SubscriptionTo),'domain','rhc')
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
	r.Name as Name,
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
group by Id, r.Name, f.ItemId,
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
/****** Object:  StoredProcedure [dbo].[Search]    Script Date: 3/18/2020 2:40:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE procedure [dbo].[Search]
    @searchTerm varchar(100)
,   @currentPage int = 1
,   @pageSize int = 20
,   @reportObjectTypes varchar(100) = '0'
as
begin


--Declare    @searchTerm varchar(100) = 'readmission'
--Declare   @currentPage int = 1
--Declare   @pageSize int = 20
--Declare   @reportObjectTypes varchar(100) = '0'
/**********************************************************************************************
Name: Report Object search for Atlas
Description: Returns report objects based on full text index searching. 
			

Auth: Scott Manley & Christopher Pickering
**********************************************************************************************/


/*
	remove characters from input 
		- anything no a-Z
		- anything not 0-9
		- anything not space

	then replace all double space with single space

*/
declare @ResultSize int = @pageSize * @currentPage + @pageSize * 4000
declare @originalSearch varchar(100) = @searchTerm
declare @i int = patindex('%[^a-zA-Z0-9 ]%', @searchTerm)

while @i > 0
      begin
            set @searchTerm = STUFF(@searchTerm, @i, 1, ' ')
            set @i = patindex('%[^a-zA-Z0-9 ]%', @searchTerm)
      end
;

set @searchTerm = REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(@searchTerm)),' ','CHAR(17)CHAR(18)'),'CHAR(18)CHAR(17)',''),'CHAR(17)CHAR(18)',' ') 

--declare @predicate varchar(100) = @searchTerm;
--declare @nearpredicate varchar(190) = '("' + REPLACE(@searchTerm, ' ', '*" NEAR "') + '*")';
--declare @andpredicate varchar(190) = REPLACE(@searchTerm, ' ', ' AND ')
declare @totalRecords int;
declare @q varchar(max);
drop table if exists #results
drop table if exists #reportTypes;
drop table if exists #matches;

select * into #reportTypes from string_split(Replace(@reportObjectTypes,' ',''), ',');

-- rank 1 (report object name with exact match to search input)
select *,0 As SearchRank into #matches from containstable(app.SearchTable,SearchField,'a',0) where 1=2;
set @q = 'select s.[Id] as [KEY], 1 as RANK, 1 as SearchRank from app.SearchTable s where SearchFieldDescription = ''Name'' and SearchField LIKE ''%' + @originalSearch + '%''';
insert into #matches exec (@q);


-- rank 2
if ((select count(1) from #matches) < @ResultSize)
      begin
			set @q = 'select g.*,2 as SearchRank from containstable (app.SearchTable, SearchField, ' + '''("' + REPLACE(@searchTerm, ' ', '*" AND "') + '*")''' + ') as g
			where not exists (select * from #matches as t where t.[Key] = g.[Key]);';
			insert into #matches exec (@q);
			--select g.*,1 as SearchRank into #matches from containstable (app.SearchTable, SearchField, @andPredicate, @ResultSize) as g;
      end

-- rank 3
if ((select count(1) from #matches) < @ResultSize)
      begin
			Set @q = 'select g.*,3 as SearchRank from containstable (app.SearchTable, SearchField, ' + '''("' + REPLACE(@searchTerm, ' ', '*" NEAR "') + '*")''' + ') as g
			where not exists (select * from #matches as t where t.[Key] = g.[Key]);';
	        --insert into #matches select g.*,2 as SearchRank from containstable (app.SearchTable, SearchField, CONCAT('("' ,REPLACE(@searchTerm, ' ', '*" NEAR "') , '*")') , @ResultSize) as g
            --where not exists (select * from #matches as t where t.[Key] = g.[Key]);
			insert into #matches exec (@q) ;
      end

-- rank 4
if ((select count(1) from #matches as s) < @ResultSize )
      begin
		Set @q = 'select g.*,4 as SearchRank from freetexttable (app.SearchTable, SearchField, ''' + @searchTerm + ''' ) as g
			where not exists (select * from #matches as t where t.[Key] = g.[Key]);';
            --insert into #matches select g.*,3 as SearchRank from freetexttable (app.SearchTable, SearchField, @predicate, @ResultSize) as g
            --where not exists (select * from #matches as t where t.[Key] = g.[Key])
			insert into #matches exec (@q) ;
            ;
      end

--insert into #results 
select * into #results from (
      select
            s.ItemId,
			s.ItemType,
			--s.SearchFieldDescription,
            min(s.ItemRank) as ItemRank,
            min(ct.RANK) as RANK,
            min(SearchRank) as SearchRank
      from
            app.SearchTable s
            inner join #matches as ct on s.Id = ct.[KEY]
			inner join #reportTypes	rot on rot.value = 0 or rot.value = s.TypeId
      group by 
            s.ItemId,
			s.ItemType
			--s.SearchFieldDescription
) as t;
                        
set @totalRecords = (select count(*)from #results);

if (@currentPage = 1)
      begin
            select top(@pageSize) 
			
                  b.ItemId,
				  b.ItemType,
				  'SearchFieldDescription' as SearchField,
				   @totalRecords as TotalRecords,
                  r.name as Name,
				  r.EpicRecordId,
				  r.EpicMasterFile,
				  r.SourceServer,
				  r.ReportServerPath,
				  case when d.DeveloperDescription is not null then d.DeveloperDescription 
					   when r.Description is not null then r.Description
                       when r.DetailedDescription is not null then r.DetailedDescription
                       when d.KeyAssumptions is not null then d.KeyAssumptions
                       else null end as Description,
				  t.Name as ReportType,
				  case when d.reportObjectId is null then 0 else 1 end as Documented,
				  r.ReportObjectURL,
				  d.EnabledForHyperspace,
				  r.EpicReportTemplateId
                

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
				  'SearchFieldDescription' as SearchField,
				   @totalRecords as TotalRecords,
                  r.name as Name,
				  r.EpicRecordId,
				  r.EpicMasterFile,
				  r.SourceServer,
				  r.ReportServerPath,
				  case when d.DeveloperDescription != null then d.DeveloperDescription 
					   when r.Description != null then r.Description
                       when r.DetailedDescription != null then r.DetailedDescription
                       when d.KeyAssumptions != null then d.KeyAssumptions
                       else 'Unknown' end as Description,
				  t.Name as ReportType,
				  case when d.reportObjectId is null then 0 else 1 end as Documented,
				  r.ReportObjectURL,
				  d.EnabledForHyperspace,
				  r.EpicReportTemplateId
                

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
/****** Object:  StoredProcedure [dbo].[SearchReportObjects]    Script Date: 3/18/2020 2:40:01 PM ******/
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
,	2 as SearchRank
,	4 as SearchNum
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
,	3 as SearchRank
,	5 as SearchNum
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
,	r.Name  as ReportObjectName
,	r.Description
,	SUM(ft.RANK) as MatchRank
,	3 as SearchRank
,	6 as SearchNum
from app.Term t
	inner join app.ReportObjectDocTerms	dt on t.TermId = dt.Termid
	inner join ReportObject	r on dt.ReportObjectID = r.ReportObjectID
	inner join freetexttable (app.Term, *, @nearpredicate)			as ft on t.TermId = ft.[KEY]
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
,	r.Name  as ReportObjectName
,	r.Description
,	ft.RANK as MatchRank
,	3 as SearchRank
,	7 as SearchNum
from app.ReportObject_doc d
	inner join ReportObject	r on d.ReportObjectID = r.ReportObjectID
	inner join freetexttable (app.ReportObject_doc, *, @nearpredicate)	as ft on r.ReportObjectID = ft.[KEY]
	inner join #reportTypes	rot on rot.value = 0 or rot.value = r.ReportObjectTypeID
where OrphanedReportObjectYN = 'N'
	and DefaultVisibilityYN = 'Y'
	and r.ReportObjectID not in (select distinct ReportObjectID from #results)
	and r.ReportObjectTypeID IN (3,17,20,21,28)--Crystal, RW, DB, component, SSRS
;	


--Group reports together and sum text rank within the search rank groups 
select		r.ReportObjectID
,			r.SearchRank
, r.ReportName
,			SUM(r.RANK) MyRANK
,			COALESCE((SELECT 1 FROM app.ReportObject_doc b WHERE r.reportobjectid = b.ReportObjectID AND (b.DeveloperDescription IS NOT NULL OR b.KeyAssumptions IS NOT NULL)),CASE WHEN r.description IS NULL THEN 3 ELSE 2 end) PRANK
into		#output
from		#results r
group by	r.ReportObjectID
,			r.SearchRank
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
/****** Object:  StoredProcedure [dbo].[SearchReportObjects_Original]    Script Date: 3/18/2020 2:40:01 PM ******/
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
ALTER DATABASE [Data_Governance_Dev] SET  READ_WRITE 
GO
