USE [master]
GO

/****** Object:  Database [DG_Staging]    Script Date: 2/8/2021 9:32:41 AM ******/
CREATE DATABASE [DG_Staging]
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DG_Staging].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [DG_Staging] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [DG_Staging] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [DG_Staging] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [DG_Staging] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [DG_Staging] SET ARITHABORT OFF 
GO

ALTER DATABASE [DG_Staging] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [DG_Staging] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [DG_Staging] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [DG_Staging] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [DG_Staging] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [DG_Staging] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [DG_Staging] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [DG_Staging] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [DG_Staging] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [DG_Staging] SET  DISABLE_BROKER 
GO

ALTER DATABASE [DG_Staging] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [DG_Staging] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [DG_Staging] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [DG_Staging] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [DG_Staging] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [DG_Staging] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [DG_Staging] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [DG_Staging] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [DG_Staging] SET  MULTI_USER 
GO

ALTER DATABASE [DG_Staging] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [DG_Staging] SET DB_CHAINING OFF 
GO

ALTER DATABASE [DG_Staging] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [DG_Staging] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [DG_Staging] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [DG_Staging] SET QUERY_STORE = OFF
GO

USE [DG_Staging]
GO

ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO

ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO

ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO

ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO

ALTER DATABASE [DG_Staging] SET  READ_WRITE 
GO



USE [DG_Staging]
GO
/****** Object:  Schema [raw]    Script Date: 2/8/2021 9:34:01 AM ******/
CREATE SCHEMA [raw]
GO
/****** Object:  Schema [stage]    Script Date: 2/8/2021 9:34:01 AM ******/
CREATE SCHEMA [stage]
GO
/****** Object:  Table [raw].[Clarity_Username_Domainname_Links]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[Clarity_Username_Domainname_Links](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[User_Id] [nvarchar](255) NULL,
	[Name] [nvarchar](255) NULL,
	[DomainName] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity-metric-query]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity-metric-query](
	[idn id] [nvarchar](max) NULL,
	[query] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity-user-groups]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity-user-groups](
	[USER_ID] [nvarchar](max) NULL,
	[GroupName] [nvarchar](max) NULL,
	[GroupSource] [nvarchar](max) NULL,
	[GroupId] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarity-component-groups]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarity-component-groups](
	[COMPONENT_ID] [nvarchar](42) NULL,
	[group_id] [nvarchar](67) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarity-dashboard-roles]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarity-dashboard-roles](
	[dashboard_id] [nvarchar](max) NULL,
	[user_roles] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarity-dashboard-run-data]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarity-dashboard-run-data](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RunId] [int] NULL,
	[SourceServer] [nvarchar](250) NULL,
	[SourceDB] [nvarchar](250) NULL,
	[SourceTable] [nvarchar](250) NULL,
	[Name] [nvarchar](max) NULL,
	[ReportObjectType] [nvarchar](250) NULL,
	[EpicMasterFile] [nvarchar](3) NULL,
	[EpicRecordID] [nvarchar](100) NULL,
	[RunUserId] [nvarchar](100) NULL,
	[RunStartTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarity-dashboard-types]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarity-dashboard-types](
	[dashboard_id] [nvarchar](max) NULL,
	[user_types] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-clarity_emp]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-clarity_emp](
	[USER_ID] [varchar](18) NULL,
	[NAME] [varchar](160) NULL,
	[PROV_ID] [varchar](18) NULL,
	[EPIC_EMP_ID] [varchar](18) NULL,
	[MC_DEPARTMENT_ID] [numeric](18, 0) NULL,
	[CR_USER_NAME] [varchar](254) NULL,
	[PB_DEF_CLS_NM] [varchar](40) NULL,
	[CONF_SEC_CLS_NM] [varchar](40) NULL,
	[DFLT_SEC_CLASS_C] [varchar](18) NULL,
	[EPR_SEC_CLASS_C] [varchar](18) NULL,
	[MR_CLASS_C] [varchar](18) NULL,
	[USER_CONFIG_ID] [varchar](18) NULL,
	[MC_DEF_SEC_LEVEL_C] [varchar](18) NULL,
	[RFL_DEF_CLS_C] [varchar](18) NULL,
	[IB_SEC_CLASS_ID] [varchar](18) NULL,
	[SHARED_SEC_CL_ID] [varchar](18) NULL,
	[CUST_SVC_DEF_CLS] [varchar](18) NULL,
	[DEL_STATUS_C] [int] NULL,
	[USER_NAME_EXT] [varchar](160) NULL,
	[USER_STATUS_C] [int] NULL,
	[ADDRESS] [varchar](254) NULL,
	[CITY] [varchar](60) NULL,
	[STATE_PROVINCE] [varchar](50) NULL,
	[ZIP_CODE] [varchar](50) NULL,
	[PHONE] [varchar](50) NULL,
	[LAST_PW_UPDATE] [datetime] NULL,
	[SQL_ECL_ID] [varchar](18) NULL,
	[ES_AUTH_ALLSA_YN] [varchar](1) NULL,
	[CAD0_OTH_DEP_C] [int] NULL,
	[CAD0_DEPARTMENT_ID] [numeric](18, 0) NULL,
	[CAD1_OTH_DEP_C] [int] NULL,
	[CAD1_DEPARTMENT_ID] [numeric](18, 0) NULL,
	[ES_DSKTP_ACSS_YN] [varchar](1) NULL,
	[ES_RPT_SEC_PNT_C] [int] NULL,
	[CT_DFLT_CLS_C] [varchar](18) NULL,
	[CT_DSKTP_ACSS_YN] [varchar](1) NULL,
	[AR_DFLT_FACLTY_C] [int] NULL,
	[AR_DF_SERV_AREA_ID] [numeric](18, 0) NULL,
	[AR_DFLT_LOC_ID] [numeric](18, 0) NULL,
	[AR_DEPARTMENT_ID] [numeric](18, 0) NULL,
	[DFLT_ECL_ID] [varchar](18) NULL,
	[MR_RESTR_ACCS_YN] [varchar](1) NULL,
	[ENBL_RSLT_REV_YN] [varchar](1) NULL,
	[PRF_LST_PX_C] [int] NULL,
	[PRF_LST_COMDX_C] [int] NULL,
	[PRF_LST_MEDS_C] [int] NULL,
	[PRF_LST_RFV_C] [int] NULL,
	[MPI_SEC_CLS_C] [varchar](18) NULL,
	[MAIL_SYSTEM_C] [int] NULL,
	[LGIN_DEPARTMENT_ID] [numeric](18, 0) NULL,
	[EL_ACCS_C] [int] NULL,
	[EW_USER_CLS_C] [varchar](18) NULL,
	[CR_DFLT_ECL_ID] [varchar](18) NULL,
	[APCLM_DEF_ECL_ID] [varchar](18) NULL,
	[CASE_DEF_ECL_ID] [varchar](18) NULL,
	[AP_DEF_ECL_ID] [varchar](18) NULL,
	[CAPRR_DEF_ECL_ID] [varchar](18) NULL,
	[CAPPAY_DEF_ECL_ID] [varchar](18) NULL,
	[LAST_ACCS_DATETIME] [datetime] NULL,
	[DFLT_LOC_YN] [varchar](1) NULL,
	[RESTR_ACCS_REV_YN] [varchar](1) NULL,
	[LAST_USER_ID] [varchar](18) NULL,
	[MILLIMAN_USA_UNAME] [varchar](255) NULL,
	[MR_LOGON_DEPT_ID] [numeric](18, 0) NULL,
	[CAD_PRV_OTH_DPT_YN] [varchar](254) NULL,
	[CAD_GUI_BKDROP_YN] [varchar](254) NULL,
	[CAD_GUI_FRM_SZE_C] [int] NULL,
	[IS_DFLT_DEPT_YN] [varchar](254) NULL,
	[DISPLAY_ERR_RPT_C] [varchar](254) NULL,
	[IS_COLLECTOR_YN] [varchar](254) NULL,
	[USER_ALIAS] [varchar](254) NULL,
	[CM_PHY_OWNER_ID] [varchar](25) NULL,
	[CM_LOG_OWNER_ID] [varchar](25) NULL,
	[SYSTEM_LOGIN] [varchar](254) NULL,
	[RPT_GRP_ONE] [varchar](254) NULL,
	[RPT_GRP_TWO] [varchar](254) NULL,
	[RPT_GRP_THREE] [varchar](254) NULL,
	[LAB_DEFAULT_ECL_ID] [varchar](18) NULL,
	[LAB_WORKBENCH_YN] [varchar](254) NULL,
	[SUPERVISOR_YN] [varchar](1) NULL,
	[LICENSE_USRTYPE_C] [int] NULL,
	[EW_ACCS_C] [int] NULL,
	[EL_ACCS_PP_GRP_YN] [varchar](1) NULL,
	[EL_NOTFY_EMAIL_YN] [varchar](1) NULL,
	[EL_DAYS_BTN_EMAIL] [int] NULL,
	[EL_LAST_EMAIL_DT] [datetime] NULL,
	[EL_GRP_NOTIFY_YN] [varchar](1) NULL,
	[EL_USR_AFFECT_YN] [varchar](1) NULL,
	[EL_TRMS_ACPT_INST] [datetime] NULL,
	[EL_ACCS_PROG_PNT] [varchar](254) NULL,
	[WEB_EXT_IDENTIFIER] [varchar](254) NULL,
	[ME_PORTAL_DEF_ID] [varchar](184) NULL,
	[ST_PASTE_C] [int] NULL,
	[ME_ADMIN_FLAG_YN] [varchar](1) NULL,
	[ME_ACCESS_C] [int] NULL,
	[FORCE_PWD_CHANGE_YN] [varchar](1) NULL,
	[OR_SYSTEM_CLASS_ID] [varchar](18) NULL,
	[RX_SEC_CLASS_ID] [varchar](18) NULL,
	[OR_DEF_LOC_SECUR_ID] [varchar](18) NULL,
	[INP_EMR_SEC_CLS_ID] [varchar](18) NULL,
	[DFLT_ACCT_WQ_ID] [numeric](18, 0) NULL,
	[SP_FAC_YN] [varchar](1) NULL,
	[SP_AFF_YN] [varchar](1) NULL,
	[SP_OUT_YN] [varchar](1) NULL,
	[HB_DFLT_LGN_DEP_ID] [numeric](18, 0) NULL,
	[CDT_DFLT_ECL_ID] [varchar](18) NULL,
	[ROI_DFLT_ECL_ID] [varchar](18) NULL,
	[NTCM_DFLT_ECL_ID] [varchar](18) NULL,
	[HH_DFLT_ECL_ID] [varchar](18) NULL,
	[ER_DFLT_ECL_ID] [varchar](18) NULL,
	[LAB_DFLT_TST_ECL_ID] [varchar](18) NULL,
	[PEAR_DFLT_ECL_ID] [varchar](18) NULL,
	[OB_DFLT_ECL_ID] [varchar](18) NULL,
	[CHSYNC_DFLT_ECL_ID] [varchar](18) NULL,
	[CDA_DFLT_ECL_ID] [varchar](18) NULL,
	[CTM_DFLT_ECL_ID] [varchar](18) NULL,
	[CE_DFLT_ECL_ID] [varchar](18) NULL,
	[HNDHLD_DFLT_ECL_ID] [varchar](18) NULL,
	[PRM_BIL_DFLT_ECL_ID] [varchar](18) NULL,
	[CONF_DFLT_ECL_ID] [varchar](18) NULL,
	[MR_INIT_PRAC_C] [varchar](66) NULL,
	[ADT_DFLT_ECL_ID] [varchar](18) NULL,
	[EDI_DFLT_ECL_ID] [varchar](18) NULL,
	[HB_DFLT_ECL_ID] [varchar](18) NULL,
	[RIS_DFLT_ECL_ID] [varchar](18) NULL,
	[CARD_DFLT_ECL_ID] [varchar](18) NULL,
	[EMFI_DFLT_ECL_ID] [varchar](18) NULL,
	[DC_DFLT_ECL_ID] [varchar](18) NULL,
	[LOGIN_BLOCKED_C] [int] NULL,
	[EMP_RECORD_TYPE_C] [int] NULL,
	[LNK_SEC_TEMPLT_ID] [varchar](18) NULL,
	[LAB_BIL_DFLT_ECL_ID] [varchar](18) NULL,
	[LOGIN_BLOCKED_C_CMT] [varchar](254) NULL,
	[PPL_TYPE_C] [int] NULL,
	[CM_DFLT_SPT_USER_ID] [varchar](18) NULL,
	[CM_DFLT_SPT_POOL_ID] [numeric](18, 0) NULL,
	[CM_DFLT_SPT_FREETXT] [varchar](254) NULL,
	[MR_ADMIN_VIEW_ONLY] [int] NULL,
	[SYS_OVERV_YN] [varchar](1) NULL,
	[FOCUS_DEPT_FIELD_YN] [varchar](1) NULL,
	[DFLT_SCHED_VIEW_C] [int] NULL,
	[OR_DEF_CASE_LOC_ID] [numeric](18, 0) NULL,
	[OR_DEF_CASE_SVC_C] [varchar](66) NULL,
	[OR_DEF_CASE_SRGN_ID] [varchar](18) NULL,
	[USR_LOG_OR_SCRPT_ID] [varchar](18) NULL,
	[RFL_REP_SECPT_C] [int] NULL,
	[LAB_LAST_DRAWTYPE_C] [int] NULL,
	[SHOW_UNREL_RES_YN] [varchar](1) NULL,
	[SHOW_ERR_RPT_C] [int] NULL,
	[PTSRCH_DEF_FAC_YN] [varchar](1) NULL,
	[IS_SUP_PROV_REQ_C] [int] NULL,
	[FAC_FILTER_TYPE_C] [int] NULL,
	[LAB_RESCREEN_FACTOR] [numeric](6, 2) NULL,
	[IGNORE_LIGHT_M_YN] [varchar](1) NULL,
	[HIM_REP_SEC_PT_C] [int] NULL,
	[HIM_ADMIN_ACCESS_YN] [varchar](1) NULL,
	[PREF_LIST_SET_ORX_C] [int] NULL,
	[DFLT_PROB_PRI_C] [int] NULL,
	[PROB_PRI_OFF_PREF_C] [int] NULL,
	[PROB_PRI_ON_PREF_C] [int] NULL,
	[PROB_LST_PREF_L_ID] [numeric](18, 0) NULL,
	[USR_SORT_PROB_YN] [varchar](1) NULL,
	[LAST_PATIENT_LIST] [varchar](254) NULL,
	[IP_UNIQUE_ID] [varchar](192) NULL,
	[IPEMR_DEF_RESTR_YN] [varchar](1) NULL,
	[IP_DEF_RES_ACC_YN] [varchar](1) NULL,
	[IP_PATLST_DEFLST_ID] [varchar](18) NULL,
	[PTSRCH_SHOW_LST_YN] [varchar](1) NULL,
	[DSB_FRM_SEC_ID] [varchar](18) NULL,
	[EFF_FROM_DATE] [datetime] NULL,
	[EFF_TO_DATE] [datetime] NULL,
	[DEACTIVATE_DAYS] [int] NULL,
	[CAD_OTH_DEP_ECL_ID] [varchar](18) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-clarity_rpt]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-clarity_rpt](
	[REPORT_ID] [numeric](18, 0) NOT NULL,
	[REPORT_NAME] [varchar](192) NULL,
	[REPORT_DESC_ONE] [varchar](255) NULL,
	[REPORT_DESC_TWO] [varchar](255) NULL,
	[REPORT_TYPE] [varchar](60) NULL,
	[REPORT_SELECT_TYPE] [varchar](20) NULL,
	[INPUT_FILE_NAME] [varchar](254) NULL,
	[INFO_FOLDER] [varchar](254) NULL,
	[SELECTION_STRING] [varchar](60) NULL,
	[REPORT_OUTPUT_FMT] [varchar](40) NULL,
	[CM_PHY_OWNER_ID] [varchar](25) NULL,
	[CM_LOG_OWNER_ID] [varchar](25) NULL,
	[ASSOC_REPORT_ID] [numeric](18, 0) NULL,
	[REPORT_TYPE_C] [varchar](66) NULL,
	[REPORT_OVRD_DB_CONN_ID] [numeric](18, 0) NULL,
	[RECORD_STATUS_C] [int] NULL,
	[REPORT_CLASS_C] [varchar](66) NULL,
	[OVR_SUB_INACT_DAYS] [int] NULL,
	[OVR_OUTPUT_FORMAT_C] [int] NULL,
	[OVR_FREQ_C] [varchar](66) NULL,
	[OVR_PRIORITY_C] [int] NULL,
	[OVR_DAYS_KEEP_INST] [int] NULL,
	[OVR_PUBLISHED_FOLDER] [varchar](50) NULL,
	[OVR_USE_RPT_LOGIN_YN] [varchar](1) NULL,
	[OVR_HRS_KEEP_RSLT] [int] NULL,
	[HIDE_FROM_LIBRARY_YN] [varchar](1) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-clarity_rpt_groups]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-clarity_rpt_groups](
	[REPORT_ID] [numeric](18, 0) NOT NULL,
	[LINE] [int] NOT NULL,
	[CM_PHY_OWNER_ID] [varchar](25) NULL,
	[CM_LOG_OWNER_ID] [varchar](25) NULL,
	[REPORT_GROUP_C] [varchar](66) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-component_desc]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-component_desc](
	[COMPONENT_ID] [numeric](18, 0) NULL,
	[CM_PHY_OWNER_ID] [varchar](25) NULL,
	[CM_LOG_OWNER_ID] [varchar](25) NULL,
	[RECORD_DESC] [varchar](1024) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-component_info]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-component_info](
	[COMPONENT_ID] [numeric](18, 0) NULL,
	[CM_PHY_OWNER_ID] [varchar](25) NULL,
	[CM_LOG_OWNER_ID] [varchar](25) NULL,
	[COMPONENT_NAME] [varchar](200) NULL,
	[RECORD_STATUS_C] [int] NULL,
	[DISPLAY_FORMAT_C] [int] NULL,
	[DATA_SOURCE_C] [int] NULL,
	[RECORD_TYPE_C] [int] NULL,
	[PARENT_COMPON_ID] [numeric](18, 0) NULL,
	[PARENT_COMPON_UNIQ] [varchar](50) NULL,
	[OWNING_APPL_ID] [numeric](18, 0) NULL,
	[READY_FOR_USE_YN] [varchar](1) NULL,
	[AVAIL_TO_USER_YN] [varchar](1) NULL,
	[USER_ID] [varchar](18) NULL,
	[DASHBOARD_ID] [numeric](18, 0) NULL,
	[REFRESH_RATE] [int] NULL,
	[SHOW_REFR_BTTN_YN] [varchar](1) NULL,
	[RPT_TEMPLATE_C] [varchar](66) NULL,
	[SHOW_RPT_COMP_TM_YN] [varchar](1) NULL,
	[VIEW_REPORT_YN] [varchar](1) NULL,
	[RUN_REPORT_YN] [varchar](1) NULL,
	[EXP_RSLT_YN] [varchar](1) NULL,
	[ENABLE_DRILLDOWN_YN] [varchar](1) NULL,
	[REPORT_ID] [numeric](18, 0) NULL,
	[EXTENSION_ID] [numeric](18, 0) NULL,
	[INIT_PARAM] [varchar](254) NULL,
	[SHOW_DATA_TIME_YN] [varchar](1) NULL,
	[CODE_TEMPLATE_ID] [numeric](18, 0) NULL,
	[EMBD_CONTENT_HEIGHT] [int] NULL,
	[EMBD_SRC_URL] [varchar](2048) NULL,
	[MULT_DATA_RES_C] [int] NULL,
	[PERIOD_INTERVAL_C] [int] NULL,
	[NUM_PERIODS] [int] NULL,
	[SUMMARY_LEVEL_C] [int] NULL,
	[DYN_SUM_LOCS_C] [int] NULL,
	[METRIC_TYPE_C] [int] NULL,
	[SHOW_YTD_YN] [varchar](1) NULL,
	[SHOW_QTR_TO_DT_YN] [varchar](1) NULL,
	[SHOW_MO_TO_DT_YN] [varchar](1) NULL,
	[SHOW_WEEK_TO_DT_YN] [varchar](1) NULL,
	[DISPLAY_TITLE] [varchar](254) NULL,
	[COMPON_COLOR_C] [int] NULL,
	[LAUNCH_ACTIVITY] [varchar](254) NULL,
	[SHOW_UPDATE_TIME_YN] [varchar](1) NULL,
	[HEADER_ICON] [varchar](254) NULL,
	[ACTIVITY_TOOLTIP] [varchar](254) NULL,
	[RECORD_CREATION_DT] [datetime] NULL,
	[INSTANT_OF_UPD_DTTM] [datetime] NULL,
	[SHOW_EXT_BENCHMRK_YN] [varchar](1) NULL,
	[SUMMARY_INDEX] [int] NULL,
	[PERIODS_IN_SPARKLN] [int] NULL,
	[INCLUDE_ZERO_IN_GRAPH_YN] [varchar](1) NULL,
	[ENABLE_BKGLOADING_YN] [varchar](1) NULL,
	[SHOW_TODAY_YN] [varchar](1) NULL,
	[SHOW_FISC_YTD_YN] [varchar](1) NULL,
	[RPT_TEMPLATE_ID] [numeric](18, 0) NULL,
	[OVERRIDE_GT_LABEL] [varchar](254) NULL,
	[OVERRIDE_GT_ROWS_YN] [varchar](1) NULL,
	[EXCL_XTD_SPARKLN_YN] [varchar](1) NULL,
	[MODEL_ID] [numeric](18, 0) NULL,
	[EXCL_FUT_DATA_YN] [varchar](1) NULL,
	[SLICERDICER_REPORT_INFO_ID] [numeric](18, 0) NULL,
	[MAX_HEIGHT] [int] NULL,
	[ACTIVITY] [varchar](140) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-component-list]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-component-list](
	[DASHBOARD_ID] [numeric](18, 0) NOT NULL,
	[LINE] [int] NOT NULL,
	[CM_PHY_OWNER_ID] [varchar](25) NULL,
	[CM_LOG_OWNER_ID] [varchar](25) NULL,
	[REGION] [int] NULL,
	[COMPONENT_UNIQ_ID] [varchar](50) NULL,
	[COMPONENT_ID] [numeric](18, 0) NULL,
	[TITLE_OVERRIDE] [varchar](254) NULL,
	[WIDTH] [numeric](18, 2) NULL,
	[MAX_HEIGHT] [int] NULL,
	[CAN_REMOVE_YN] [varchar](1) NULL,
	[CAN_EDIT_YN] [varchar](1) NULL,
	[CAN_COLLAPSE_YN] [varchar](1) NULL,
	[START_COLLAPSED_YN] [varchar](1) NULL,
	[COMPONENT_STATUS_C] [int] NULL,
	[OVERRIDE_COLOR_C] [int] NULL,
	[SOURCE_REGION] [int] NULL,
	[SOURCE_INDEX] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-dashboard_desc]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-dashboard_desc](
	[DASHBOARD_ID] [numeric](18, 0) NULL,
	[LINE] [int] NULL,
	[CM_PHY_OWNER_ID] [varchar](25) NULL,
	[CM_LOG_OWNER_ID] [varchar](25) NULL,
	[RECORD_DESC] [varchar](300) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-dashboard_info]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-dashboard_info](
	[DASHBOARD_ID] [numeric](18, 0) NULL,
	[CM_PHY_OWNER_ID] [varchar](25) NULL,
	[CM_LOG_OWNER_ID] [varchar](25) NULL,
	[DASHBOARD_NAME] [varchar](200) NULL,
	[RECORD_STATUS_C] [int] NULL,
	[RECORD_TYPE_C] [int] NULL,
	[PARENT_DSHBD_ID] [numeric](18, 0) NULL,
	[OWNING_APPL_ID] [numeric](18, 0) NULL,
	[READY_FOR_USE_YN] [varchar](1) NULL,
	[ENABLED_YN] [varchar](1) NULL,
	[USER_ID] [varchar](18) NULL,
	[LAYOUT_ID] [numeric](18, 0) NULL,
	[CAN_ADD_COMPON_YN] [varchar](1) NULL,
	[NO_GRP_COMPON_YN] [varchar](1) NULL,
	[PRIM_PERSONLZTN_YN] [varchar](1) NULL,
	[DISPLAY_TITLE] [varchar](254) NULL,
	[RECORD_CREATION_DT] [datetime] NULL,
	[INSTANT_OF_UPD_DTTM] [datetime] NULL,
	[GROUP_COMPONENTS_YN] [varchar](1) NULL,
	[OVRIDE_STATUS_C] [int] NULL,
	[OVRIDE_CONTEXT] [varchar](62) NULL,
	[OVRIDE_PARENT_DB_ID] [numeric](18, 0) NULL,
	[SOURCE_LAYOUT_ID] [numeric](18, 0) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-emp_basic_info]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-emp_basic_info](
	[USER_ID] [varchar](18) NULL,
	[CM_PHY_OWNER_ID] [varchar](25) NULL,
	[CM_LOG_OWNER_ID] [varchar](25) NULL,
	[USR_LOG_DOM] [varchar](67) NULL,
	[ACCESS_DISK_CAT_YN] [varchar](1) NULL,
	[ACCESS_SPOOL_CAT_YN] [varchar](1) NULL,
	[ACCESS_FAX_CAT_YN] [varchar](1) NULL,
	[ALLOW_RESTR_LOGI_YN] [varchar](1) NULL,
	[AUTHEN_CONFIG_ID] [numeric](18, 0) NULL,
	[MAX_PW_AGE] [numeric](11, 2) NULL,
	[MIN_PW_AGE] [numeric](11, 2) NULL,
	[ASK_WORKSTATION_C] [int] NULL,
	[WINDOWS_MODE_C] [int] NULL,
	[EPIC_ID_KEY] [varchar](10) NULL,
	[LDAP_OVERRIDE_ID] [varchar](254) NULL,
	[PROMPT_EPIC_ID_KEY_YN] [varchar](1) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-filter_definitions]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-filter_definitions](
	[FILTER_ID] [numeric](18, 0) NULL,
	[CM_PHY_OWNER_ID] [varchar](25) NULL,
	[CM_LOG_OWNER_ID] [varchar](25) NULL,
	[FILTER_NAME] [varchar](200) NULL,
	[RECORD_STATUS_C] [int] NULL,
	[OVERRIDE_REC_STS_C] [int] NULL,
	[OVERRIDE_CONTEXT_C] [int] NULL,
	[BASE_RECORD_ID] [numeric](18, 0) NULL,
	[OVERRIDE_CMPL_INST_DTTM] [datetime] NULL,
	[FILTER_CAT_C] [int] NULL,
	[FILTER_DATA_TYPE_C] [int] NULL,
	[FILTER_LIST_YN] [varchar](1) NULL,
	[FILTER_LOWER_BOUND] [numeric](18, 2) NULL,
	[FILTER_UPPER_BOUND] [numeric](18, 2) NULL,
	[GRANULARITY] [numeric](18, 2) NULL,
	[OVERTIME_YN] [varchar](1) NULL,
	[COPY_FORWARD_YN] [varchar](1) NULL,
	[SENTENCE_PREFIX] [varchar](254) NULL,
	[SENTENCE_PREFIX_NEG] [varchar](254) NULL,
	[SENTENCE_PREFIX_PAST] [varchar](254) NULL,
	[SENTENCE_PREFIX_PAST_NEG] [varchar](254) NULL,
	[SENTENCE_PREFIX_PROG] [varchar](254) NULL,
	[OPTION_LIST_TABLE] [varchar](254) NULL,
	[OPTION_LIST_COLUMN] [varchar](254) NULL,
	[TABLE_PARAM_TYPE] [varchar](254) NULL,
	[WAREHOUSE_TABLE_NAME_C] [int] NULL,
	[RECORD_CREATION_DT] [datetime] NULL,
	[INSTANT_OF_UPDATE_DTTM] [datetime] NULL,
	[UNIT] [varchar](100) NULL,
	[FILTER_INACTIVE_YN] [varchar](1) NULL,
	[METRIC_ID] [varchar](18) NULL,
	[FILTER_NUMBER] [numeric](18, 0) NULL,
	[NOADD_YN] [varchar](1) NULL,
	[FLO_MEAS_ID] [varchar](18) NULL,
	[LOOKUP_TYPE_NAME] [varchar](254) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-metric_desc]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-metric_desc](
	[DEFINITION_ID] [numeric](18, 0) NULL,
	[LINE] [int] NULL,
	[CM_PHY_OWNER_ID] [varchar](25) NULL,
	[CM_LOG_OWNER_ID] [varchar](25) NULL,
	[RECORD_DESC] [varchar](254) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-metric_info]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-metric_info](
	[DEFINITION_ID] [numeric](18, 0) NULL,
	[CM_PHY_OWNER_ID] [varchar](25) NULL,
	[CM_LOG_OWNER_ID] [varchar](25) NULL,
	[METRIC_NAME] [varchar](200) NULL,
	[RECORD_STATUS_C] [int] NULL,
	[OVERRIDE_REC_STS_C] [int] NULL,
	[OVERRIDE_CONTEXT_C] [varchar](62) NULL,
	[PARENT_RECORD_ID] [numeric](18, 0) NULL,
	[OVERRIDE_COMP_DTTM] [datetime] NULL,
	[DISPLAY_TITLE] [varchar](200) NULL,
	[RESULT_TYPE_C] [int] NULL,
	[UNIT_TYPE_C] [int] NULL,
	[COLL_MTHD_C] [int] NULL,
	[CONFIG_TYPE_C] [int] NULL,
	[RECORD_TYPE_C] [int] NULL,
	[OWNING_APPL_ID] [numeric](18, 0) NULL,
	[LOWER_VALUE_GOOD_C] [int] NULL,
	[ACTIVE_YN] [varchar](1) NULL,
	[EXT_AGG_SUPPORT_YN] [varchar](1) NULL,
	[EXT_BNCH_FOR_DEF_ID] [numeric](18, 0) NULL,
	[RECORD_CREATION_DT] [datetime] NULL,
	[INST_OF_UPDATE_DTTM] [datetime] NULL,
	[QM_ID] [numeric](18, 0) NULL,
	[COPY_FROM_DEF_ID] [numeric](18, 0) NULL,
	[DATA_NOT_EXTRCT_YN] [varchar](1) NULL,
	[ADT_EXCL_RULE_ID] [varchar](18) NULL,
	[ADT_EXCL_EXT_ID] [numeric](18, 0) NULL,
	[ADT_EVAL_EXT_ID] [numeric](18, 0) NULL,
	[CSF_FACT_TABLE] [varchar](192) NULL,
	[CSF_AGG_PROC_ID] [numeric](18, 0) NULL,
	[MIN_AGE] [int] NULL,
	[MAX_AGE] [int] NULL,
	[IP_READMIT_INDEX_DX_ID] [varchar](18) NULL,
	[IP_READMIT_PLAN_PROC_ID] [varchar](18) NULL,
	[IP_READMIT_UNPLAN_DX_ID] [varchar](18) NULL,
	[IP_READMIT_SNGL_DAY_YN] [varchar](1) NULL,
	[ADT_DEP_CALC_MTHD_C] [int] NULL,
	[ADT_USR_CALC_MTHD_C] [int] NULL,
	[FACILITY_EXCL_YN] [varchar](1) NULL,
	[MPM_EVAL_RULE_ID] [varchar](18) NULL,
	[MPM_MEAS_GROUP_C] [int] NULL,
	[SCALE_FACTOR] [numeric](14, 6) NULL,
	[DEF_LOAD_EXT_ID] [numeric](18, 0) NULL,
	[ROLLUP_MTHD_C] [int] NULL,
	[EXT_AGG_CONTEXT_C] [int] NULL,
	[QM_SUB_MEASURE_ID] [varchar](256) NULL,
	[QM_VERSION] [int] NULL,
	[OM_SUM_TYPE_C] [int] NULL,
	[NEAR_DUE_THRESH] [int] NULL,
	[PAST_DUE_THRESH] [int] NULL,
	[LEVEL_CONFIG_LPP_ID] [numeric](18, 0) NULL,
	[BENCHMARK_SOURCE_C] [int] NULL,
	[RX_EXCL_EXT_ID] [numeric](18, 0) NULL,
	[RX_EVAL_EXT_ID] [numeric](18, 0) NULL,
	[RX_EVAL_ACT_C] [int] NULL,
	[IP_READMIT_PLAN_OR_PROC_ID] [varchar](18) NULL,
	[IP_READMIT_PLAN_ICD_PROC_ID] [varchar](18) NULL,
	[RIS_NQMBC_QM_C] [varchar](66) NULL,
	[RIS_NQMBC_RANGE_LOW] [int] NULL,
	[RIS_NQMBC_RANGE_HIGH] [int] NULL,
	[FEATURE_TRACKING_DEFINITION_ID] [numeric](18, 0) NULL,
	[NUMERATOR_LOGIC] [varchar](100) NULL,
	[DENOMINATOR_LOGIC] [varchar](100) NULL,
	[IP_METRIC_WINDOW_DAYS] [int] NULL,
	[IP_READMIT_ALWAYS_PLAN_DX_ID] [varchar](18) NULL,
	[EXT_WINDOW_ALLOW_OVERRIDES_YN] [varchar](1) NULL,
	[LOOKBACK_WINDOW] [varchar](10) NULL,
	[AUTO_SQL_TABLE_TYPE_C] [int] NULL,
	[AUTO_SQL_RUN_GROUP_ID] [numeric](18, 0) NULL,
	[AUTO_SQL_FACT_TABLE_NAME] [varchar](128) NULL,
	[AUTO_SQL_FILTER_EXPRESSION] [varchar](508) NULL,
	[AUTO_SQL_NUMER_AGGN_FUNCTION_C] [int] NULL,
	[AUTO_SQL_NUMER_EXPRESSION] [varchar](508) NULL,
	[AUTO_SQL_DENOM_AGGN_FUNCTION_C] [int] NULL,
	[AUTO_SQL_DENOM_EXPRESSION] [varchar](508) NULL,
	[AUTO_SQL_DATE_EXPRESSION] [varchar](508) NULL,
	[GOAL_ENABLE_YN] [varchar](1) NULL,
	[GOAL] [numeric](18, 2) NULL,
	[HIDE_DEPT_SCORE_YN] [varchar](1) NULL,
	[METRIC_GROUPING_C] [int] NULL,
	[BACKGROUND_ONLY_YN] [varchar](1) NULL,
	[SCALE_MIN] [numeric](18, 2) NULL,
	[SCALE_MAX] [numeric](18, 2) NULL,
	[TEMPLATE_C] [int] NULL,
	[HP_EVAL_RULE_ID] [varchar](18) NULL,
	[EXCLUSIONS_DEFINITION_ID] [numeric](18, 0) NULL,
	[JOB_CONFIGURATION_ID] [numeric](18, 0) NULL,
	[ADT_CLS_CALC_MTHD_C] [int] NULL,
	[BACKFILL_WINDOW] [varchar](10) NULL,
	[IP_MORTALITY_INDEX_DX_ID] [varchar](18) NULL,
	[IP_MORTALITY_METRIC_TYPE_C] [int] NULL,
	[NUM_UNIT_TYPE_C] [int] NULL,
	[DEN_UNIT_TYPE_C] [int] NULL,
	[REG_FAC_ROLLUP_TYPE_C] [int] NULL,
	[REG_TIME_ACT_TOT_C] [int] NULL,
	[IP_MIN_INDEX_LENGTH_OF_STAY] [int] NULL,
	[ES_INCL_OR_EXCL_PROV_C] [int] NULL,
	[ES_CNT_ADDL_ENC_TYPE_YN] [varchar](1) NULL,
	[IP_METRIC_EXCLUSION_DX_ID] [varchar](18) NULL,
	[READMIT_ALGORITHM_METHOD_C] [int] NULL,
	[IP_PRIMARY_DX_GROUPER_ID] [varchar](18) NULL,
	[IP_SECONDARY_DX_GROUPER_ID] [varchar](18) NULL,
	[IP_SECONDARY_DX_POA_YN] [varchar](1) NULL,
	[SQL_SOURCE_DATABASE_C] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-ovride_rpt_groups]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-ovride_rpt_groups](
	[REPORT_ID] [numeric](18, 0) NOT NULL,
	[LINE] [int] NOT NULL,
	[CM_PHY_OWNER_ID] [varchar](25) NULL,
	[CM_LOG_OWNER_ID] [varchar](25) NULL,
	[REPORT_GROUP_C] [varchar](66) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-report_desc]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-report_desc](
	[REPORT_INFO_ID] [numeric](18, 0) NULL,
	[LINE] [int] NULL,
	[CM_PHY_OWNER_ID] [varchar](25) NULL,
	[CM_LOG_OWNER_ID] [varchar](25) NULL,
	[REPORT_DESCRIPTION] [varchar](3500) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-report_info]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-report_info](
	[REPORT_INFO_ID] [numeric](18, 0) NULL,
	[CM_PHY_OWNER_ID] [varchar](25) NULL,
	[CM_LOG_OWNER_ID] [varchar](25) NULL,
	[REPORT_INFO_NAME] [varchar](200) NULL,
	[RPT_TYPE_C] [varchar](66) NULL,
	[PRIVATE_OR_PUBLIC_C] [int] NULL,
	[TEMP_REPORT_C] [int] NULL,
	[CURRENT_DEPT_YN] [varchar](1) NULL,
	[ANCHORED_COLUMNS] [int] NULL,
	[RECORD_TYPE_C] [int] NULL,
	[PARENT_TEMPLATE_ID] [numeric](18, 0) NULL,
	[LOGIC] [varchar](254) NULL,
	[END_DATE_STRING] [varchar](12) NULL,
	[START_DATE_STRING] [varchar](12) NULL,
	[END_TIME_STRING] [varchar](20) NULL,
	[START_TIME_STRING] [varchar](20) NULL,
	[REPORT_ID] [numeric](18, 0) NULL,
	[OWNED_BY_USER_ID] [varchar](18) NULL,
	[LAST_MOD_BY_USER_ID] [varchar](18) NULL,
	[LAST_RUN_BY_USER_ID] [varchar](18) NULL,
	[LOAD_ALL_YN] [varchar](1) NULL,
	[OVRIDE_OUTPUT_FMT_C] [int] NULL,
	[OVRIDE_RUN_FREQ_C] [varchar](66) NULL,
	[OVRIDE_BOE_FLDR] [varchar](50) NULL,
	[OVRIDE_CACHE_EXP] [int] NULL,
	[OVRIDE_REQ_PRI_C] [int] NULL,
	[OVRIDE_DAYS_KEEP] [int] NULL,
	[OVRIDE_DAYS_BEFORE_SUB_INACT] [int] NULL,
	[RECORD_STATE_C] [int] NULL,
	[CREATED_BY_USER_ID] [varchar](18) NULL,
	[INST_OF_CREATION_DTTM] [datetime] NULL,
	[INST_OF_LAST_MOD_DTTM] [datetime] NULL,
	[BI_FIX_FLAG_C] [int] NULL,
	[BI_LAST_BATCH_GUID] [varchar](36) NULL,
	[OVRIDE_SEARCH_RECS] [int] NULL,
	[OVRIDE_FIND_RECS] [int] NULL,
	[HAS_ABS_DATES_YN] [varchar](1) NULL,
	[HAS_STYLE_OVERRIDES_YN] [varchar](1) NULL,
	[HIDE_FROM_LIBRARY_YN] [varchar](1) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-rw_rpt_run_data]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-rw_rpt_run_data](
	[RUN_ID] [numeric](18, 0) NOT NULL,
	[REP_SETTINGS_ID] [numeric](18, 0) NULL,
	[SERVER_NODE_NAME] [varchar](40) NULL,
	[RUN_INSTANT] [datetime] NULL,
	[RUN_USER_ID] [varchar](18) NULL,
	[REPORT_START_INST] [datetime] NULL,
	[REPORT_END_INST] [datetime] NULL,
	[TOTAL_EXE_TIME] [numeric](18, 0) NULL,
	[VIEW_STATUS_C] [int] NULL,
	[REPORT_STATUS_C] [int] NULL,
	[RUN_NAME] [varchar](192) NULL,
	[SOURCE_REPORT_ID] [numeric](18, 0) NULL,
	[REPORT_RUN_TYPE_C] [int] NULL,
 CONSTRAINT [PK_RW_RPT_RUN_DATA] PRIMARY KEY CLUSTERED 
(
	[RUN_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 85) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-template_description]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-template_description](
	[REPORT_ID] [numeric](18, 0) NULL,
	[CONTACT_DATE_REAL] [float] NULL,
	[LINE] [int] NULL,
	[CONTACT_DATE] [datetime] NULL,
	[SEARCH_SOURCE_DESC] [varchar](3500) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-template_dynamic]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-template_dynamic](
	[REPORT_ID] [numeric](18, 0) NULL,
	[CONTACT_DATE_REAL] [float] NULL,
	[CONTACT_DATE] [datetime] NULL,
	[CONTACT_NUM] [varchar](254) NULL,
	[CM_CT_OWNER_ID] [varchar](25) NULL,
	[PARAMETER_VIEWER_ID] [numeric](18, 0) NULL,
	[INFO_REC_NAME_ID] [numeric](18, 0) NULL,
	[SUBSET_COLUMNID_ID] [numeric](18, 0) NULL,
	[SUBSET_COLUMNDAT_ID] [numeric](18, 0) NULL,
	[SUBSET_INI] [varchar](5) NULL,
	[BEFORE_ACT_PACK_YN] [varchar](1) NULL,
	[DESCRIPTION] [varchar](254) NULL,
	[PARAM_PROMPT_ID] [numeric](18, 0) NULL,
	[TEMPLATE_ID] [numeric](18, 0) NULL,
	[CACHE_EXP_IN] [int] NULL,
	[VIEW_EXP_RESULTS_YN] [varchar](1) NULL,
	[SEARCH_SOURCE_NAME] [varchar](254) NULL,
	[PRINT_CLASS_C] [varchar](66) NULL,
	[HELP_MODULE] [varchar](254) NULL,
	[HELP_FILE_C] [int] NULL,
	[SEARCH_HELP_ID] [varchar](254) NULL,
	[SEARCH_HLP_MODULE_C] [int] NULL,
	[VIEWER_HELP_ID] [varchar](254) NULL,
	[VIEWER_HLP_MODULE_C] [int] NULL,
	[REPORT_UI_C] [int] NULL,
	[SELECT_CRIT_YN] [varchar](1) NULL,
	[ALLOW_ADD_CRIT_YN] [varchar](1) NULL,
	[VIEWER_REPORT_ID] [varchar](18) NULL,
	[MATCH_LINE_ONLY_YN] [varchar](1) NULL,
	[ENABLE_CACHING_YN] [varchar](1) NULL,
	[EN_SEARCH_SUMM_YN] [varchar](1) NULL,
	[STATIC_COL_DATA_C] [int] NULL,
	[SHOW_EXPORT_YN] [varchar](1) NULL,
	[WRAP_TEXT_C] [int] NULL,
	[REP_LAYOUT_C] [varchar](66) NULL,
	[PRINT_VIEW_TAG_ID] [numeric](18, 0) NULL,
	[SPEC_VIEW_EXPEX_ID] [numeric](18, 0) NULL,
	[SETUP_DATA_PP_ID] [numeric](18, 0) NULL,
	[H_SUMM_TEMPLATE_ID] [numeric](18, 0) NULL,
	[ENABLE_PREV_YN] [varchar](1) NULL,
	[MAX_NUM_SEARCH] [int] NULL,
	[MAX_NUM_RETURN] [int] NULL,
	[PLAIN_TXT_PRN_CLS_C] [varchar](66) NULL,
	[VWR_RPT_BTG_VIEW_C] [varchar](66) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-template_info]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-template_info](
	[REPORT_ID] [numeric](18, 0) NULL,
	[CM_PHY_OWNER_ID] [varchar](25) NULL,
	[CM_LOG_OWNER_ID] [varchar](25) NULL,
	[REPORT_NAME] [varchar](254) NULL,
	[STATUS_C] [int] NULL,
	[SRC_TEMPL_ID] [numeric](18, 0) NULL,
	[UPD_VERSION_C] [numeric](8, 3) NULL,
	[REPORT_TYPE_C] [varchar](66) NULL,
	[REP_PAGE_PROG_ID] [varchar](254) NULL,
	[SHOW_DISPTAB_YN] [varchar](1) NULL,
	[DEF_REPSTG_YN] [varchar](1) NULL,
	[ENABLE_PRTBTN_YN] [varchar](1) NULL,
	[SHOW_RS_YN] [varchar](1) NULL,
	[SHOW_PRTTAB_YN] [varchar](1) NULL,
	[DEF_PRTDLG_YN] [varchar](1) NULL,
	[SUP_PRTLAYOUT_YN] [varchar](1) NULL,
	[AC_RW_SHARE_REP_YN] [varchar](1) NULL,
	[SHOW_SUMTAB_YN] [varchar](1) NULL,
	[SUP_RTFPRT_YN] [varchar](1) NULL,
	[CODE_BUILD_REP] [varchar](254) NULL,
	[PRINT_CODE] [varchar](254) NULL,
	[SUP_COLICONS_YN] [varchar](1) NULL,
	[SPT_ROWFONT_YN] [varchar](1) NULL,
	[ROW_OVERRIDE_YN] [varchar](1) NULL,
	[SUP_FILTERLPP_YN] [varchar](1) NULL,
	[SUP_REPBUILDR_YN] [varchar](1) NULL,
	[APP_TYPE_ID] [numeric](18, 0) NULL,
	[SUP_VCPROL_YN] [varchar](1) NULL,
	[REP_PAGE_PARAM] [varchar](254) NULL,
	[REP_PAGE_CAPTION] [varchar](254) NULL,
	[REPORT_TYPE_HGR_C] [int] NULL,
	[RUN_CAPTION] [varchar](254) NULL,
	[PUBLIC_ONLY_MODE_YN] [varchar](1) NULL,
	[SPRTS_HRCHY_YN] [varchar](1) NULL,
	[SPRTS_VIEWSCREENS_YN] [varchar](1) NULL,
	[RPT_SETUP_PROGID_ID] [numeric](18, 0) NULL,
	[USE_REPORT_LOGIN_YN] [varchar](1) NULL,
	[REP_PAGE_PROG_ID2_ID] [numeric](18, 0) NULL,
	[POST_RUN_LPP_ID] [numeric](18, 0) NULL,
	[SHOW_TLBRTAB_YN] [varchar](1) NULL,
	[SPT_WRPOVRD_YN] [varchar](1) NULL,
	[MESSAGE_AREA_TEXT] [varchar](4000) NULL,
	[REPORT_FREQ_C] [varchar](66) NULL,
	[REQPRIORITY_C] [int] NULL,
	[DAYS_KEEP_INSTANCE] [int] NULL,
	[WEBI_REPORT_CUID] [varchar](50) NULL,
	[WEBI_REPORT_NAME] [varchar](300) NULL,
	[ACTIVITY_DESCRIPTOR] [varchar](100) NULL,
	[WEB_ENABLED_YN] [varchar](1) NULL,
	[CRITERIA_VIEW] [varchar](254) NULL,
	[DEF_COLORS_FONTS] [varchar](254) NULL,
	[ANCHORED_COLUMNS] [int] NULL,
	[VERIFIED_CHECKSUM] [int] NULL,
	[TPL_CHECKSUM] [int] NULL,
	[ENABLE_EMFI_YN] [varchar](1) NULL,
	[CRYSTAL_DATAMODEL_C] [int] NULL,
	[SHOW_OVERRIDE_TAB_YN] [varchar](1) NULL,
	[NEED_RSLT_UPDATE_YN] [varchar](1) NULL,
	[EXTBI_VW_BEHAVIOR_C] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-template_info_2]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-template_info_2](
	[REPORT_ID] [numeric](18, 0) NULL,
	[CM_PHY_OWNER_ID] [varchar](25) NULL,
	[CM_LOG_OWNER_ID] [varchar](25) NULL,
	[CRYSTAL_FILENAME] [varchar](254) NULL,
	[OUTPUT_FORMAT_C] [int] NULL,
	[ENTERPRISE_FOLDER] [varchar](50) NULL,
	[CONTEXT_ID] [numeric](18, 0) NULL,
	[REASON_NO_CONTEXT_C] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-x_idk_info_noadd_single]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-x_idk_info_noadd_single](
	[RECORD_ID] [numeric](18, 0) NULL,
	[CM_PHY_OWNER_ID] [varchar](25) NULL,
	[CM_LOG_OWNER_ID] [varchar](25) NULL,
	[RECORD_NAME] [varchar](200) NULL,
	[RECORD_STATUS_C] [int] NULL,
	[DISPLAY_TITLE] [varchar](200) NULL,
	[Y_AXIS_TITLE] [varchar](30) NULL,
	[OVERRIDE_REC_STS_C] [int] NULL,
	[OVERRIDE_CONTEXT] [varchar](62) NULL,
	[PARENT_RECORD_ID] [numeric](18, 0) NULL,
	[OVERRIDE_COMP_INST_DTTM] [datetime] NULL,
	[RECORD_TYPE_C] [int] NULL,
	[PARENT_REC_ID] [numeric](18, 0) NULL,
	[OWN_APP_ID] [numeric](18, 0) NULL,
	[METRIC_LOAD_EXT_ID] [numeric](18, 0) NULL,
	[DRILLDOWN_EXT_ID] [numeric](18, 0) NULL,
	[USE_THRESHOLDS_DD_YN] [varchar](1) NULL,
	[SHOW_ALL_TIERS_YN] [varchar](1) NULL,
	[RPT_TMP_SRC_ID] [numeric](18, 0) NULL,
	[REPORT_DATA_SRC_ID] [numeric](18, 0) NULL,
	[REPORT_SUMMARY_IDX] [int] NULL,
	[SUMMARY_COLUMN_ID] [numeric](18, 0) NULL,
	[SUMMARY_FUNCTION_C] [int] NULL,
	[SUM_FUNC_NUM_AND_YN] [varchar](1) NULL,
	[SUM_FUNC_DEN_AND_YN] [varchar](1) NULL,
	[METRIC_DEF_ID] [numeric](18, 0) NULL,
	[RESULT_PIECE_C] [int] NULL,
	[GOAL] [numeric](18, 2) NULL,
	[MUST_EXCEED_GOAL_YN] [varchar](1) NULL,
	[COL_DFLT_DISP_FMT_C] [int] NULL,
	[COL_NUM_DECIMALS] [int] NULL,
	[ROUNDING_METHOD_C] [int] NULL,
	[DISAB_PEER_GROUP_YN] [varchar](1) NULL,
	[DISPLAY_OVRIDE_C] [int] NULL,
	[GROUP_HEADER_ID] [numeric](18, 0) NULL,
	[KEY_RES_GRP_ID] [numeric](18, 0) NULL,
	[RECORD_CREATION_DT] [datetime] NULL,
	[INSTANT_OF_UPDATE_DTTM] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-x-idb-resources]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-x-idb-resources](
	[RECORD_ID] [numeric](18, 0) NOT NULL,
	[LINE] [int] NOT NULL,
	[CM_PHY_OWNER_ID] [varchar](25) NULL,
	[CM_LOG_OWNER_ID] [varchar](25) NULL,
	[DATA_RESOURCES_ID] [numeric](18, 0) NULL,
	[PARENT_RESOURCE_ID] [numeric](18, 0) NULL,
	[EAF_ID] [numeric](18, 0) NULL,
	[LOC_TARGET_ID] [numeric](18, 0) NULL,
	[DEPT_ID] [numeric](18, 0) NULL,
	[DEPT_SPEC_TARGET_C] [varchar](66) NULL,
	[PROV_ID] [varchar](18) NULL,
	[PROV_SPEC_TARGET_C] [varchar](66) NULL,
	[TGT_USER_ID] [varchar](18) NULL,
	[CM_EMP_ID] [varchar](18) NULL,
	[LOB_TARGET_ID] [varchar](18) NULL,
	[VEN_TARGET_ID] [varchar](18) NULL,
	[RFL_TYPE_TARGET_C] [varchar](66) NULL,
	[RFL_CLS_TARGET_C] [varchar](66) NULL,
	[NCS_TOP_TARGET_C] [varchar](66) NULL,
	[BUS_SEG_TGT_ID] [numeric](18, 0) NULL,
	[PAY_TGT_ID] [numeric](18, 0) NULL,
	[EMPLOYEE_TARGET_ID] [numeric](18, 0) NULL,
	[FIN_DIV_ID] [numeric](18, 0) NULL,
	[FIN_SUBDIV_ID] [numeric](18, 0) NULL,
	[BILL_AREA_ID] [numeric](18, 0) NULL,
	[AUR_ANTIB_AGENT_ID] [numeric](18, 0) NULL,
	[AUR_ROUTE_ADMIN_ID] [numeric](18, 0) NULL,
	[PROC_CAT_TARGET_ID] [varchar](254) NULL,
	[IMG_TECH_TARGET_ID] [varchar](18) NULL,
	[BASE_PAT_CLS_TGT_C] [int] NULL,
	[MOD_PROV_TARGET_ID] [varchar](18) NULL,
	[ORD_PRIORITY_TGT_C] [int] NULL,
	[CENTER_TGT_C] [varchar](66) NULL,
	[APPT_STATUS_TGT_C] [int] NULL,
	[PB_ARR_TARGET_C] [varchar](66) NULL,
	[CASE_MGR_TGT_ID] [varchar](18) NULL,
	[EMP_GRP_TARGET_ID] [varchar](20) NULL,
	[CRM_SBTP_TRGT_VAL_C] [int] NULL,
	[CRM_SRCT_TRGT_VAL_C] [int] NULL,
	[BLOCK_TYPE_TGT_C] [varchar](66) NULL,
	[CRM_SBJT_TRGT_VAL_C] [int] NULL,
	[USR_TYP_TRGT_VAL_C] [int] NULL,
	[PHR_TARGET_ID] [numeric](18, 0) NULL,
	[PRC_ID] [varchar](18) NULL,
	[INTERFACE_KIND_C_ID] [numeric](18, 0) NULL,
	[TRANSPLANT_ORGAN_C] [int] NULL,
	[ERROR_CODE_TGT_ID] [numeric](18, 0) NULL,
	[SURGICAL_SERVICE_C] [varchar](66) NULL,
	[OR_LOCATION_ID] [numeric](18, 0) NULL,
	[LAB_TEST_ID] [varchar](18) NULL,
	[LAB_PATHO_TYPE_C] [int] NULL,
	[DASHBOARD_TARGET_ID] [numeric](18, 0) NULL,
	[COMPONENT_TARGET_ID] [numeric](18, 0) NULL,
	[OPERATING_ROOM_ID] [varchar](18) NULL,
	[CMS_MU_ID] [numeric](18, 0) NULL,
	[DEVICE_ID] [varchar](18) NULL,
	[TXP_CNTR_TGT_VAL_C] [int] NULL,
	[ABX_TARGET_ID] [numeric](18, 0) NULL,
	[ORGANISM_TARGET_ID] [numeric](18, 0) NULL,
	[RPT_BASE_CL_TGT_C] [int] NULL,
	[CLIN_CONTEXT_TGT_C] [int] NULL,
	[DG_VCG_ID] [varchar](18) NULL,
	[REG_TARGET_ID] [numeric](18, 0) NULL,
	[MED_ALT_TGT_VAL_C] [int] NULL,
	[DEP_UNIT_TYP_VAL_C] [int] NULL,
	[PT_CLASS_CL_TGT_C] [varchar](66) NULL,
	[DNB_ERR_TARGET_ID] [numeric](18, 0) NULL,
	[IMAGING_DS_SCORE_C] [varchar](66) NULL,
	[CRM_TASK_TGT_ID] [numeric](18, 0) NULL,
	[READING_PRIORITY_C] [int] NULL,
	[POOL_TGT_ID] [numeric](18, 0) NULL,
	[POOL_TYP_TGT_C] [int] NULL,
	[PRIM_SVC_TGT_C] [varchar](66) NULL,
	[FIN_CLASS_TGT_C] [varchar](66) NULL,
	[LAB_SUBMITTER_ID] [numeric](18, 0) NULL,
	[KIOSK_ID] [varchar](18) NULL,
	[KIOSK_CLUSTER_ID] [varchar](18) NULL,
	[KSK_ROOT_TARGET_ID] [varchar](18) NULL,
	[HB_CLASS_TGT_C] [int] NULL,
	[DEF_TARGET_ID] [varchar](18) NULL,
	[REG_DEF_TGT_ID] [numeric](18, 0) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-zc_record_type_24]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-zc_record_type_24](
	[RECORD_TYPE_24_C] [int] NULL,
	[NAME] [varchar](254) NULL,
	[TITLE] [varchar](254) NULL,
	[ABBR] [varchar](254) NULL,
	[INTERNAL_ID] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-clarityreport-dbo-zc_report_type_hgr]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-clarityreport-dbo-zc_report_type_hgr](
	[REPORT_TYPE_HGR_C] [int] NULL,
	[NAME] [varchar](254) NULL,
	[TITLE] [varchar](254) NULL,
	[ABBR] [varchar](254) NULL,
	[INTERNAL_ID] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-reportserver-dbo-catalog]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-reportserver-dbo-catalog](
	[ItemID] [uniqueidentifier] NULL,
	[Path] [nvarchar](425) NULL,
	[Name] [nvarchar](425) NULL,
	[ParentID] [uniqueidentifier] NULL,
	[Type] [int] NULL,
	[Content] [varbinary](max) NULL,
	[Intermediate] [uniqueidentifier] NULL,
	[SnapshotDataID] [uniqueidentifier] NULL,
	[LinkSourceID] [uniqueidentifier] NULL,
	[Property] [nvarchar](max) NULL,
	[Description] [nvarchar](512) NULL,
	[Hidden] [bit] NULL,
	[CreatedByID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModifiedByID] [uniqueidentifier] NULL,
	[ModifiedDate] [datetime] NULL,
	[MimeType] [nvarchar](260) NULL,
	[SnapshotLimit] [int] NULL,
	[Parameter] [nvarchar](max) NULL,
	[PolicyID] [uniqueidentifier] NULL,
	[PolicyRoot] [bit] NULL,
	[ExecutionFlag] [int] NULL,
	[ExecutionTime] [datetime] NULL,
	[SubType] [nvarchar](128) NULL,
	[ComponentID] [uniqueidentifier] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-reportserver-dbo-executionlogstorage]    Script Date: 2/8/2021 9:34:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-reportserver-dbo-executionlogstorage](
	[LogEntryId] [bigint] IDENTITY(1,1) NOT NULL,
	[InstanceName] [nvarchar](38) NOT NULL,
	[ReportID] [uniqueidentifier] NULL,
	[UserName] [nvarchar](260) NULL,
	[ExecutionId] [nvarchar](64) NULL,
	[RequestType] [tinyint] NOT NULL,
	[Format] [nvarchar](26) NULL,
	[Parameters] [ntext] NULL,
	[ReportAction] [tinyint] NULL,
	[TimeStart] [datetime] NOT NULL,
	[TimeEnd] [datetime] NOT NULL,
	[TimeDataRetrieval] [int] NOT NULL,
	[TimeProcessing] [int] NOT NULL,
	[TimeRendering] [int] NOT NULL,
	[Source] [tinyint] NOT NULL,
	[Status] [nvarchar](40) NOT NULL,
	[ByteCount] [bigint] NOT NULL,
	[RowCount] [bigint] NOT NULL,
	[AdditionalInfo] [xml] NULL,
PRIMARY KEY CLUSTERED 
(
	[LogEntryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-reportserver-dbo-policyuserrole]    Script Date: 2/8/2021 9:34:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-reportserver-dbo-policyuserrole](
	[ID] [uniqueidentifier] NOT NULL,
	[RoleID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[PolicyID] [uniqueidentifier] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-reportserver-dbo-subscriptions]    Script Date: 2/8/2021 9:34:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-reportserver-dbo-subscriptions](
	[SubscriptionID] [uniqueidentifier] NOT NULL,
	[OwnerID] [uniqueidentifier] NOT NULL,
	[Report_OID] [uniqueidentifier] NOT NULL,
	[Locale] [nvarchar](128) NOT NULL,
	[InactiveFlags] [int] NOT NULL,
	[ExtensionSettings] [ntext] NULL,
	[ModifiedByID] [uniqueidentifier] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[Description] [nvarchar](512) NULL,
	[LastStatus] [nvarchar](260) NULL,
	[EventType] [nvarchar](260) NOT NULL,
	[MatchData] [ntext] NULL,
	[LastRunTime] [datetime] NULL,
	[Parameters] [ntext] NULL,
	[DataSettings] [ntext] NULL,
	[DeliveryExtension] [nvarchar](260) NULL,
	[Version] [int] NOT NULL,
	[ReportZone] [int] NOT NULL,
 CONSTRAINT [PK_Subscriptions_Epcogdbp01] PRIMARY KEY CLUSTERED 
(
	[SubscriptionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-reportserver-dbo-users]    Script Date: 2/8/2021 9:34:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-reportserver-dbo-users](
	[UserID] [uniqueidentifier] NULL,
	[Sid] [binary](85) NULL,
	[UserType] [int] NULL,
	[AuthType] [int] NULL,
	[UserName] [nvarchar](260) NULL,
	[Setting] [nvarchar](max) NULL,
	[ServiceToken] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-reportserver-dbo-catalog]    Script Date: 2/8/2021 9:34:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-reportserver-dbo-catalog](
	[ItemID] [uniqueidentifier] NULL,
	[Path] [nvarchar](425) NULL,
	[Name] [nvarchar](425) NULL,
	[ParentID] [uniqueidentifier] NULL,
	[Type] [int] NULL,
	[Content] [varbinary](max) NULL,
	[Intermediate] [uniqueidentifier] NULL,
	[SnapshotDataID] [uniqueidentifier] NULL,
	[LinkSourceID] [uniqueidentifier] NULL,
	[Property] [nvarchar](max) NULL,
	[Description] [nvarchar](512) NULL,
	[Hidden] [bit] NULL,
	[CreatedByID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModifiedByID] [uniqueidentifier] NULL,
	[ModifiedDate] [datetime] NULL,
	[MimeType] [nvarchar](260) NULL,
	[SnapshotLimit] [int] NULL,
	[Parameter] [nvarchar](max) NULL,
	[PolicyID] [uniqueidentifier] NULL,
	[PolicyRoot] [bit] NULL,
	[ExecutionFlag] [int] NULL,
	[ExecutionTime] [datetime] NULL,
	[SubType] [nvarchar](128) NULL,
	[ComponentID] [uniqueidentifier] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-reportserver-dbo-executionlogstorage]    Script Date: 2/8/2021 9:34:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-reportserver-dbo-executionlogstorage](
	[LogEntryId] [bigint] IDENTITY(1,1) NOT NULL,
	[InstanceName] [nvarchar](38) NOT NULL,
	[ReportID] [uniqueidentifier] NULL,
	[UserName] [nvarchar](260) NULL,
	[ExecutionId] [nvarchar](64) NULL,
	[RequestType] [tinyint] NOT NULL,
	[Format] [nvarchar](26) NULL,
	[Parameters] [ntext] NULL,
	[ReportAction] [tinyint] NULL,
	[TimeStart] [datetime] NOT NULL,
	[TimeEnd] [datetime] NOT NULL,
	[TimeDataRetrieval] [int] NOT NULL,
	[TimeProcessing] [int] NOT NULL,
	[TimeRendering] [int] NOT NULL,
	[Source] [tinyint] NOT NULL,
	[Status] [nvarchar](40) NOT NULL,
	[ByteCount] [bigint] NOT NULL,
	[RowCount] [bigint] NOT NULL,
	[AdditionalInfo] [xml] NULL,
PRIMARY KEY CLUSTERED 
(
	[LogEntryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-reportserver-dbo-policyuserrole]    Script Date: 2/8/2021 9:34:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-reportserver-dbo-policyuserrole](
	[ID] [uniqueidentifier] NOT NULL,
	[RoleID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[PolicyID] [uniqueidentifier] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-reportserver-dbo-subscriptions]    Script Date: 2/8/2021 9:34:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-reportserver-dbo-subscriptions](
	[SubscriptionID] [uniqueidentifier] NOT NULL,
	[OwnerID] [uniqueidentifier] NOT NULL,
	[Report_OID] [uniqueidentifier] NOT NULL,
	[Locale] [nvarchar](128) NOT NULL,
	[InactiveFlags] [int] NOT NULL,
	[ExtensionSettings] [ntext] NULL,
	[ModifiedByID] [uniqueidentifier] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[Description] [nvarchar](512) NULL,
	[LastStatus] [nvarchar](260) NULL,
	[EventType] [nvarchar](260) NOT NULL,
	[MatchData] [ntext] NULL,
	[LastRunTime] [datetime] NULL,
	[Parameters] [ntext] NULL,
	[DataSettings] [ntext] NULL,
	[DeliveryExtension] [nvarchar](260) NULL,
	[Version] [int] NOT NULL,
	[ReportZone] [int] NOT NULL,
 CONSTRAINT [PK_Subscriptions] PRIMARY KEY CLUSTERED 
(
	[SubscriptionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [raw].[clarity_server-reportserver-dbo-users]    Script Date: 2/8/2021 9:34:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[clarity_server-reportserver-dbo-users](
	[UserID] [uniqueidentifier] NULL,
	[Sid] [binary](85) NULL,
	[UserType] [int] NULL,
	[AuthType] [int] NULL,
	[UserName] [nvarchar](260) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[ssrs_database-reportserver-dbo-catalog]    Script Date: 2/8/2021 9:34:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[ssrs_database-reportserver-dbo-catalog](
	[ItemID] [uniqueidentifier] NULL,
	[Path] [nvarchar](425) NULL,
	[Name] [nvarchar](425) NULL,
	[ParentID] [uniqueidentifier] NULL,
	[Type] [int] NULL,
	[Content] [varbinary](max) NULL,
	[Intermediate] [uniqueidentifier] NULL,
	[SnapshotDataID] [uniqueidentifier] NULL,
	[LinkSourceID] [uniqueidentifier] NULL,
	[Property] [nvarchar](max) NULL,
	[Description] [nvarchar](512) NULL,
	[Hidden] [bit] NULL,
	[CreatedByID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[ModifiedByID] [uniqueidentifier] NULL,
	[ModifiedDate] [datetime] NULL,
	[MimeType] [nvarchar](260) NULL,
	[SnapshotLimit] [int] NULL,
	[Parameter] [nvarchar](max) NULL,
	[PolicyID] [uniqueidentifier] NULL,
	[PolicyRoot] [bit] NULL,
	[ExecutionFlag] [int] NULL,
	[ExecutionTime] [datetime] NULL,
	[SubType] [nvarchar](128) NULL,
	[ComponentID] [uniqueidentifier] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [raw].[ssrs_database-reportserver-dbo-executionlogstorage]    Script Date: 2/8/2021 9:34:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[ssrs_database-reportserver-dbo-executionlogstorage](
	[LogEntryId] [bigint] IDENTITY(1,1) NOT NULL,
	[InstanceName] [nvarchar](38) NULL,
	[ReportID] [uniqueidentifier] NULL,
	[UserName] [nvarchar](260) NULL,
	[ExecutionId] [nvarchar](64) NULL,
	[RequestType] [tinyint] NULL,
	[Format] [nvarchar](26) NULL,
	[Parameters] [ntext] NULL,
	[ReportAction] [tinyint] NULL,
	[TimeStart] [datetime] NULL,
	[TimeEnd] [datetime] NULL,
	[TimeDataRetrieval] [int] NULL,
	[TimeProcessing] [int] NULL,
	[TimeRendering] [int] NULL,
	[Source] [tinyint] NULL,
	[Status] [nvarchar](40) NULL,
	[ByteCount] [bigint] NULL,
	[RowCount] [bigint] NULL,
	[AdditionalInfo] [xml] NULL,
PRIMARY KEY CLUSTERED 
(
	[LogEntryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [raw].[ssrs_database-reportserver-dbo-policyuserrole]    Script Date: 2/8/2021 9:34:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[ssrs_database-reportserver-dbo-policyuserrole](
	[ID] [uniqueidentifier] NOT NULL,
	[RoleID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[PolicyID] [uniqueidentifier] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [raw].[ssrs_database-reportserver-dbo-subscriptions]    Script Date: 2/8/2021 9:34:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[ssrs_database-reportserver-dbo-subscriptions](
	[SubscriptionID] [uniqueidentifier] NOT NULL,
	[OwnerID] [uniqueidentifier] NOT NULL,
	[Report_OID] [uniqueidentifier] NOT NULL,
	[Locale] [nvarchar](128) NOT NULL,
	[InactiveFlags] [int] NOT NULL,
	[ExtensionSettings] [ntext] NULL,
	[ModifiedByID] [uniqueidentifier] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[Description] [nvarchar](512) NULL,
	[LastStatus] [nvarchar](260) NULL,
	[EventType] [nvarchar](260) NOT NULL,
	[MatchData] [ntext] NULL,
	[LastRunTime] [datetime] NULL,
	[Parameters] [ntext] NULL,
	[DataSettings] [ntext] NULL,
	[DeliveryExtension] [nvarchar](260) NULL,
	[Version] [int] NOT NULL,
	[ReportZone] [int] NOT NULL,
 CONSTRAINT [PK_Subscriptions_ssrs_database] PRIMARY KEY CLUSTERED 
(
	[SubscriptionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [raw].[ssrs_database-reportserver-dbo-users]    Script Date: 2/8/2021 9:34:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [raw].[ssrs_database-reportserver-dbo-users](
	[UserID] [uniqueidentifier] NULL,
	[Sid] [binary](85) NULL,
	[UserType] [int] NULL,
	[AuthType] [int] NULL,
	[UserName] [nvarchar](260) NULL,
	[ServiceToken] [nvarchar](max) NULL,
	[Setting] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [stage].[ReportObjectHierarchyStaging]    Script Date: 2/8/2021 9:34:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [stage].[ReportObjectHierarchyStaging](
	[ParentReportObjectBizKey] [nvarchar](255) NOT NULL,
	[ChildReportObjectBizKey] [nvarchar](255) NOT NULL,
	[Line] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [stage].[ReportObjectRunDataStaging]    Script Date: 2/8/2021 9:34:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [stage].[ReportObjectRunDataStaging](
	[SourceServer] [nvarchar](255) NOT NULL,
	[SourceDB] [nvarchar](255) NOT NULL,
	[SourceTable] [nvarchar](255) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[ReportObjectType] [nvarchar](255) NOT NULL,
	[EpicMasterFile] [nvarchar](3) NULL,
	[EpicRecordID] [numeric](18, 0) NULL,
	[ReportServerCatalogID] [nvarchar](50) NULL,
	[RunID] [int] NOT NULL,
	[RunUserID] [nvarchar](100) NULL,
	[RunUserName] [nvarchar](100) NULL,
	[RunStartTime] [datetime] NULL,
	[RunDurationSeconds] [int] NULL,
	[RunStatus] [nvarchar](100) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [stage].[ReportObjectStaging]    Script Date: 2/8/2021 9:34:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [stage].[ReportObjectStaging](
	[SourceServer] [nvarchar](255) NOT NULL,
	[SourceDB] [nvarchar](255) NOT NULL,
	[SourceTable] [nvarchar](255) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[DetailedDescription] [nvarchar](max) NULL,
	[ReportObjectType] [nvarchar](255) NOT NULL,
	[Author] [nvarchar](max) NULL,
	[LastModifiedBy] [nvarchar](max) NULL,
	[LastModifiedDate] [datetime] NULL,
	[ReportObjectURL] [nvarchar](max) NULL,
	[EpicMasterFile] [nvarchar](3) NULL,
	[EpicRecordID] [numeric](18, 0) NULL,
	[ReportServerCatalogID] [nvarchar](50) NULL,
	[DefaultVisibilityYN] [nvarchar](1) NULL,
	[EpicReportTemplateId] [numeric](18, 0) NULL,
	[ReportServerPath] [nvarchar](max) NULL,
	[DisplayTitle] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
