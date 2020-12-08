using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Data_Governance_WebApp.Models
{
    public partial class Data_GovernanceContext : DbContext
    {
        public Data_GovernanceContext()
        {
        }

        public Data_GovernanceContext(DbContextOptions<Data_GovernanceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Analytics> Analytics { get; set; }
        public virtual DbSet<DpAgreement> DpAgreement { get; set; }
        public virtual DbSet<DpAgreementUsers> DpAgreementUsers { get; set; }
        public virtual DbSet<DpAttachments> DpAttachments { get; set; }
        public virtual DbSet<DpContact> DpContact { get; set; }
        public virtual DbSet<DpContactLinks> DpContactLinks { get; set; }
        public virtual DbSet<DpDataInitiative> DpDataInitiative { get; set; }
        public virtual DbSet<DpDataProject> DpDataProject { get; set; }
        public virtual DbSet<DpDataProjectConversation> DpDataProjectConversation { get; set; }
        public virtual DbSet<DpDataProjectConversationMessage> DpDataProjectConversationMessage { get; set; }
        public virtual DbSet<DpMilestoneChecklist> DpMilestoneChecklist { get; set; }
        public virtual DbSet<DpMilestoneChecklistCompleted> DpMilestoneChecklistCompleted { get; set; }
        public virtual DbSet<DpMilestoneFrequency> DpMilestoneFrequency { get; set; }
        public virtual DbSet<DpMilestoneTasks> DpMilestoneTasks { get; set; }
        public virtual DbSet<DpMilestoneTasksCompleted> DpMilestoneTasksCompleted { get; set; }
        public virtual DbSet<DpMilestoneTemplates> DpMilestoneTemplates { get; set; }
        public virtual DbSet<DpReportAnnotation> DpReportAnnotation { get; set; }
        public virtual DbSet<DpTermAnnotation> DpTermAnnotation { get; set; }
        public virtual DbSet<EstimatedRunFrequency> EstimatedRunFrequency { get; set; }
        public virtual DbSet<FinancialImpact> FinancialImpact { get; set; }
        public virtual DbSet<Fragility> Fragility { get; set; }
        public virtual DbSet<FragilityTag> FragilityTag { get; set; }
        public virtual DbSet<GlobalSiteSettings> GlobalSiteSettings { get; set; }
        public virtual DbSet<MailConversations> MailConversations { get; set; }
        public virtual DbSet<MailDrafts> MailDrafts { get; set; }
        public virtual DbSet<MailFolderMessages> MailFolderMessages { get; set; }
        public virtual DbSet<MailFolders> MailFolders { get; set; }
        public virtual DbSet<MailMessageType> MailMessageType { get; set; }
        public virtual DbSet<MailMessages> MailMessages { get; set; }
        public virtual DbSet<MailRecipients> MailRecipients { get; set; }
        public virtual DbSet<MailRecipientsDeleted> MailRecipientsDeleted { get; set; }
        public virtual DbSet<MaintenanceLog> MaintenanceLog { get; set; }
        public virtual DbSet<MaintenanceLogStatus> MaintenanceLogStatus { get; set; }
        public virtual DbSet<MaintenanceSchedule> MaintenanceSchedule { get; set; }
        public virtual DbSet<OrganizationalValue> OrganizationalValue { get; set; }
        public virtual DbSet<ReportGroupsMemberships> ReportGroupsMemberships { get; set; }
        public virtual DbSet<ReportManageEngineTickets> ReportManageEngineTickets { get; set; }
        public virtual DbSet<ReportObject> ReportObject { get; set; }
        public virtual DbSet<ReportObjectConversationDoc> ReportObjectConversationDoc { get; set; }
        public virtual DbSet<ReportObjectConversationMessageDoc> ReportObjectConversationMessageDoc { get; set; }
        public virtual DbSet<ReportObjectDoc> ReportObjectDoc { get; set; }
        public virtual DbSet<ReportObjectDocFragilityTags> ReportObjectDocFragilityTags { get; set; }
        public virtual DbSet<ReportObjectDocMaintenanceLogs> ReportObjectDocMaintenanceLogs { get; set; }
        public virtual DbSet<ReportObjectDocTerms> ReportObjectDocTerms { get; set; }
        public virtual DbSet<ReportObjectHierarchy> ReportObjectHierarchy { get; set; }
        public virtual DbSet<ReportObjectImagesDoc> ReportObjectImagesDoc { get; set; }
        public virtual DbSet<ReportObjectQuery> ReportObjectQuery { get; set; }
        public virtual DbSet<ReportObjectRunData> ReportObjectRunData { get; set; }
        public virtual DbSet<ReportObjectRunTime> ReportObjectRunTime { get; set; }
        public virtual DbSet<ReportObjectSubscriptions> ReportObjectSubscriptions { get; set; }
        public virtual DbSet<ReportObjectTopRuns> ReportObjectTopRuns { get; set; }
        public virtual DbSet<ReportObjectType> ReportObjectType { get; set; }
        public virtual DbSet<RolePermissionLinks> RolePermissionLinks { get; set; }
        public virtual DbSet<RolePermissions> RolePermissions { get; set; }
        public virtual DbSet<SearchBasicSearchData> SearchBasicSearchData { get; set; }
        public virtual DbSet<SearchBasicSearchDataSmall> SearchBasicSearchDataSmall { get; set; }
        public virtual DbSet<SearchReportObjectSearchData> SearchReportObjectSearchData { get; set; }
        public virtual DbSet<SearchTable> SearchTable { get; set; }
        public virtual DbSet<SharedItems> SharedItems { get; set; }
        public virtual DbSet<StrategicImportance> StrategicImportance { get; set; }
        public virtual DbSet<Term> Term { get; set; }
        public virtual DbSet<TermConversation> TermConversation { get; set; }
        public virtual DbSet<TermConversationMessage> TermConversationMessage { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserFavoriteFolders> UserFavoriteFolders { get; set; }
        public virtual DbSet<UserFavorites> UserFavorites { get; set; }
        public virtual DbSet<UserGroups> UserGroups { get; set; }
        public virtual DbSet<UserGroupsMembership> UserGroupsMembership { get; set; }
        public virtual DbSet<UserNameData> UserNameData { get; set; }
        public virtual DbSet<UserPreferences> UserPreferences { get; set; }
        public virtual DbSet<UserRoleLinks> UserRoleLinks { get; set; }
        public virtual DbSet<UserRoles> UserRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=rhbidb01;Database=Data_Governance;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Entity<Analytics>(entity =>
            {
                entity.ToTable("Analytics", "app");

                entity.Property(e => e.AccessDateTime)
                    .HasColumnName("accessDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.AppCodeName).HasColumnName("appCodeName");

                entity.Property(e => e.AppName).HasColumnName("appName");

                entity.Property(e => e.AppVersion).HasColumnName("appVersion");

                entity.Property(e => e.CookieEnabled).HasColumnName("cookieEnabled");

                entity.Property(e => e.Hash).HasColumnName("hash");

                entity.Property(e => e.Host).HasColumnName("host");

                entity.Property(e => e.Hostname).HasColumnName("hostname");

                entity.Property(e => e.Href).HasColumnName("href");

                entity.Property(e => e.Language).HasColumnName("language");

                entity.Property(e => e.LoadTime).HasColumnName("loadTime");

                entity.Property(e => e.Origin).HasColumnName("origin");

                entity.Property(e => e.Oscpu).HasColumnName("oscpu");

                entity.Property(e => e.PageId).HasColumnName("pageId");

                entity.Property(e => e.PageTime).HasColumnName("pageTime");

                entity.Property(e => e.Pathname).HasColumnName("pathname");

                entity.Property(e => e.Platform).HasColumnName("platform");

                entity.Property(e => e.Protocol).HasColumnName("protocol");

                entity.Property(e => e.Referrer).HasColumnName("referrer");

                entity.Property(e => e.ScreenHeight).HasColumnName("screenHeight");

                entity.Property(e => e.ScreenWidth).HasColumnName("screenWidth");

                entity.Property(e => e.Search).HasColumnName("search");

                entity.Property(e => e.SessionId).HasColumnName("sessionId");

                entity.Property(e => e.SessionTime).HasColumnName("sessionTime");

                entity.Property(e => e.Title).HasColumnName("title");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("updateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserAgent).HasColumnName("userAgent");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Analytics)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Analytics_User");
            });

            modelBuilder.Entity<DpAgreement>(entity =>
            {
                entity.HasKey(e => e.AgreementId)
                    .HasName("PK__DP_Agree__0A309D2318E98A83");

                entity.ToTable("DP_Agreement", "app");

                entity.Property(e => e.AgreementId).HasColumnName("AgreementID");

                entity.Property(e => e.EffectiveDate).HasColumnType("datetime");

                entity.Property(e => e.LastUpdateDate).HasColumnType("datetime");

                entity.Property(e => e.MeetingDate).HasColumnType("datetime");

                entity.HasOne(d => d.DataProject)
                    .WithMany(p => p.DpAgreement)
                    .HasForeignKey(d => d.DataProjectId)
                    .HasConstraintName("FK_DP_Agreement_DP_DataProject");

                entity.HasOne(d => d.LastUpdateUserNavigation)
                    .WithMany(p => p.DpAgreement)
                    .HasForeignKey(d => d.LastUpdateUser)
                    .HasConstraintName("FK_DP_Agreement_WebAppUsers");
            });

            modelBuilder.Entity<DpAgreementUsers>(entity =>
            {
                entity.HasKey(e => e.AgreementUsersId)
                    .HasName("PK__DP_Agree__3DA9AA218A1E6C55");

                entity.ToTable("DP_AgreementUsers", "app");

                entity.Property(e => e.AgreementUsersId).HasColumnName("AgreementUsersID");

                entity.Property(e => e.AgreementId).HasColumnName("AgreementID");

                entity.Property(e => e.LastUpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Agreement)
                    .WithMany(p => p.DpAgreementUsers)
                    .HasForeignKey(d => d.AgreementId)
                    .HasConstraintName("FK_DP_AgreementUsers_DP_Agreement");

                entity.HasOne(d => d.LastUpdateUserNavigation)
                    .WithMany(p => p.DpAgreementUsersLastUpdateUserNavigation)
                    .HasForeignKey(d => d.LastUpdateUser)
                    .HasConstraintName("FK_DP_AgreementUsers_WebAppUsers");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DpAgreementUsersUser)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_DP_AgreementUsers_User");
            });

            modelBuilder.Entity<DpAttachments>(entity =>
            {
                entity.HasKey(e => e.AttachmentId)
                    .HasName("PK__DP_Attac__442C64BE2CE337F1");

                entity.ToTable("DP_Attachments", "app");

                entity.Property(e => e.AttachmentData).IsRequired();

                entity.Property(e => e.AttachmentName).IsUnicode(false);

                entity.Property(e => e.AttachmentType)
                    .IsRequired()
                    .IsUnicode(false);

                entity.HasOne(d => d.DataProject)
                    .WithMany(p => p.DpAttachments)
                    .HasForeignKey(d => d.DataProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DP_Attachments_DP_DataProject");
            });

            modelBuilder.Entity<DpContact>(entity =>
            {
                entity.HasKey(e => e.ContactId)
                    .HasName("PK__DP_Conta__5C6625BB13B948C2");

                entity.ToTable("DP_Contact", "app");

                entity.Property(e => e.ContactId).HasColumnName("ContactID");

                entity.Property(e => e.Phone).HasMaxLength(55);
            });

            modelBuilder.Entity<DpContactLinks>(entity =>
            {
                entity.HasKey(e => e.LinkId)
                    .HasName("PK__DP_Conta__2D1221358626B09F");

                entity.ToTable("DP_Contact_Links", "app");

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.DpContactLinks)
                    .HasForeignKey(d => d.ContactId)
                    .HasConstraintName("FK_DP_Contact_Links_DP_Contact");

                entity.HasOne(d => d.Initiative)
                    .WithMany(p => p.DpContactLinks)
                    .HasForeignKey(d => d.InitiativeId)
                    .HasConstraintName("FK_DP_Contact_Links_DP_DataInitiative");
            });

            modelBuilder.Entity<DpDataInitiative>(entity =>
            {
                entity.HasKey(e => e.DataInitiativeId)
                    .HasName("PK__DP_DataI__1EFC948C3A83A845");

                entity.ToTable("DP_DataInitiative", "app");

                entity.Property(e => e.DataInitiativeId).HasColumnName("DataInitiativeID");

                entity.Property(e => e.ExecutiveOwnerId).HasColumnName("ExecutiveOwnerID");

                entity.Property(e => e.LastUpdateDate).HasColumnType("datetime");

                entity.Property(e => e.OperationOwnerId).HasColumnName("OperationOwnerID");

                entity.HasOne(d => d.ExecutiveOwner)
                    .WithMany(p => p.DpDataInitiativeExecutiveOwner)
                    .HasForeignKey(d => d.ExecutiveOwnerId)
                    .HasConstraintName("FK_DP_DataInitiative_User");

                entity.HasOne(d => d.FinancialImpactNavigation)
                    .WithMany(p => p.DpDataInitiative)
                    .HasForeignKey(d => d.FinancialImpact)
                    .HasConstraintName("FK_DP_DataInitiative_FinancialImpact");

                entity.HasOne(d => d.LastUpdateUserNavigation)
                    .WithMany(p => p.DpDataInitiativeLastUpdateUserNavigation)
                    .HasForeignKey(d => d.LastUpdateUser)
                    .HasConstraintName("FK_DP_DataInitiative_WebAppUsers");

                entity.HasOne(d => d.OperationOwner)
                    .WithMany(p => p.DpDataInitiativeOperationOwner)
                    .HasForeignKey(d => d.OperationOwnerId)
                    .HasConstraintName("FK_DP_DataInitiative_User1");

                entity.HasOne(d => d.StrategicImportanceNavigation)
                    .WithMany(p => p.DpDataInitiative)
                    .HasForeignKey(d => d.StrategicImportance)
                    .HasConstraintName("FK_DP_DataInitiative_StrategicImportance");
            });

            modelBuilder.Entity<DpDataProject>(entity =>
            {
                entity.HasKey(e => e.DataProjectId)
                    .HasName("PK__DP_DataP__E8D09D08794EBFAD");

                entity.ToTable("DP_DataProject", "app");

                entity.Property(e => e.DataProjectId).HasColumnName("DataProjectID");

                entity.Property(e => e.AnalyticsOwnerId).HasColumnName("AnalyticsOwnerID");

                entity.Property(e => e.DataInitiativeId).HasColumnName("DataInitiativeID");

                entity.Property(e => e.DataManagerId).HasColumnName("DataManagerID");

                entity.Property(e => e.ExecutiveOwnerId).HasColumnName("ExecutiveOwnerID");

                entity.Property(e => e.ExternalDocumentationUrl).HasColumnName("ExternalDocumentationURL");

                entity.Property(e => e.LastUpdateDate).HasColumnType("datetime");

                entity.Property(e => e.OperationOwnerId).HasColumnName("OperationOwnerID");

                entity.HasOne(d => d.AnalyticsOwner)
                    .WithMany(p => p.DpDataProjectAnalyticsOwner)
                    .HasForeignKey(d => d.AnalyticsOwnerId)
                    .HasConstraintName("FK_DP_DataProject_WebAppUsers1");

                entity.HasOne(d => d.DataManager)
                    .WithMany(p => p.DpDataProjectDataManager)
                    .HasForeignKey(d => d.DataManagerId)
                    .HasConstraintName("FK_DP_DataProject_User2");

                entity.HasOne(d => d.ExecutiveOwner)
                    .WithMany(p => p.DpDataProjectExecutiveOwner)
                    .HasForeignKey(d => d.ExecutiveOwnerId)
                    .HasConstraintName("FK_DP_DataProject_User");

                entity.HasOne(d => d.FinancialImpactNavigation)
                    .WithMany(p => p.DpDataProject)
                    .HasForeignKey(d => d.FinancialImpact)
                    .HasConstraintName("FK_DP_DataProject_FinancialImpact");

                entity.HasOne(d => d.LastUpdateUserNavigation)
                    .WithMany(p => p.DpDataProjectLastUpdateUserNavigation)
                    .HasForeignKey(d => d.LastUpdateUser)
                    .HasConstraintName("FK_DP_DataProject_WebAppUsers");

                entity.HasOne(d => d.OperationOwner)
                    .WithMany(p => p.DpDataProjectOperationOwner)
                    .HasForeignKey(d => d.OperationOwnerId)
                    .HasConstraintName("FK_DP_DataProject_User1");

                entity.HasOne(d => d.StrategicImportanceNavigation)
                    .WithMany(p => p.DpDataProject)
                    .HasForeignKey(d => d.StrategicImportance)
                    .HasConstraintName("FK_DP_DataProject_StrategicImportance");
            });

            modelBuilder.Entity<DpDataProjectConversation>(entity =>
            {
                entity.HasKey(e => e.DataProjectConversationId)
                    .HasName("PK__Dp_DataP__A555F1EB255B0952");

                entity.ToTable("Dp_DataProjectConversation", "app");

                entity.HasOne(d => d.DataProject)
                    .WithMany(p => p.DpDataProjectConversation)
                    .HasForeignKey(d => d.DataProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Dp_DataProjectConversation_DP_DataProject");
            });

            modelBuilder.Entity<DpDataProjectConversationMessage>(entity =>
            {
                entity.HasKey(e => e.DataProjectConversationMessageId)
                    .HasName("PK__Dp_DataP__06C6EA493F53AEBA");

                entity.ToTable("Dp_DataProjectConversationMessage", "app");

                entity.Property(e => e.MessageText).HasMaxLength(4000);

                entity.Property(e => e.PostDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.DataProjectConversation)
                    .WithMany(p => p.DpDataProjectConversationMessage)
                    .HasForeignKey(d => d.DataProjectConversationId)
                    .HasConstraintName("FK_Dp_DataProjectConversationMessage_Dp_DataProjectConversation");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DpDataProjectConversationMessage)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Dp_DataProjectConversationMessage_User");
            });

            modelBuilder.Entity<DpMilestoneChecklist>(entity =>
            {
                entity.HasKey(e => e.MilestoneChecklistId)
                    .HasName("PK__DP_Miles__53ECAE4A5F858065");

                entity.ToTable("DP_MilestoneChecklist", "app");

                entity.HasOne(d => d.MilestoneTask)
                    .WithMany(p => p.DpMilestoneChecklist)
                    .HasForeignKey(d => d.MilestoneTaskId)
                    .HasConstraintName("FK_DP_MilestoneChecklist_DP_MilestoneTasks");
            });

            modelBuilder.Entity<DpMilestoneChecklistCompleted>(entity =>
            {
                entity.HasKey(e => e.MilestoneChecklistCompletedId)
                    .HasName("PK__DP_Miles__E081AA701711E585");

                entity.ToTable("DP_MilestoneChecklistCompleted", "app");

                entity.Property(e => e.ChecklistStatus).HasDefaultValueSql("((0))");

                entity.Property(e => e.CompletionDate).HasColumnType("datetime");

                entity.Property(e => e.TaskDate).HasColumnType("datetime");

                entity.HasOne(d => d.CompletionUserNavigation)
                    .WithMany(p => p.DpMilestoneChecklistCompleted)
                    .HasForeignKey(d => d.CompletionUser)
                    .HasConstraintName("FK_DP_MilestoneChecklistCompleted_User");

                entity.HasOne(d => d.DataProject)
                    .WithMany(p => p.DpMilestoneChecklistCompleted)
                    .HasForeignKey(d => d.DataProjectId)
                    .HasConstraintName("FK_DP_MilestoneChecklistCompleted_DP_DataProject");
            });

            modelBuilder.Entity<DpMilestoneFrequency>(entity =>
            {
                entity.HasKey(e => e.MilestoneTypeId)
                    .HasName("PK__DP_Miles__B88C49912ECA633F");

                entity.ToTable("DP_MilestoneFrequency", "app");

                entity.Property(e => e.LastUpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.LastUpdateUserNavigation)
                    .WithMany(p => p.DpMilestoneFrequency)
                    .HasForeignKey(d => d.LastUpdateUser)
                    .HasConstraintName("FK_DP_MilestoneTypes_WebAppUsers");
            });

            modelBuilder.Entity<DpMilestoneTasks>(entity =>
            {
                entity.HasKey(e => e.MilestoneTaskId)
                    .HasName("PK__DP_Miles__64647109FB6B4EDB");

                entity.ToTable("DP_MilestoneTasks", "app");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.LastUpdateDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.HasOne(d => d.DataProject)
                    .WithMany(p => p.DpMilestoneTasks)
                    .HasForeignKey(d => d.DataProjectId)
                    .HasConstraintName("FK_DP_MilestoneTasks_DP_DataProject");

                entity.HasOne(d => d.LastUpdateUserNavigation)
                    .WithMany(p => p.DpMilestoneTasksLastUpdateUserNavigation)
                    .HasForeignKey(d => d.LastUpdateUser)
                    .HasConstraintName("FK_DP_MilestoneTasks_LastUpdateUser");

                entity.HasOne(d => d.MilestoneTemplate)
                    .WithMany(p => p.DpMilestoneTasks)
                    .HasForeignKey(d => d.MilestoneTemplateId)
                    .HasConstraintName("FK_DP_MilestoneTasks_DP_MilestoneTemplates");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.DpMilestoneTasksOwner)
                    .HasForeignKey(d => d.OwnerId)
                    .HasConstraintName("FK_DP_MilestoneTasks_User");
            });

            modelBuilder.Entity<DpMilestoneTasksCompleted>(entity =>
            {
                entity.HasKey(e => e.MilestoneTaskCompletedId)
                    .HasName("PK__DP_Miles__3226EEDD7DAFACC3");

                entity.ToTable("DP_MilestoneTasksCompleted", "app");

                entity.Property(e => e.CompletionDate).HasColumnType("datetime");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.HasOne(d => d.DataProject)
                    .WithMany(p => p.DpMilestoneTasksCompleted)
                    .HasForeignKey(d => d.DataProjectId)
                    .HasConstraintName("FK_DP_MilestoneTasksCompleted_DP_DataProject");
            });

            modelBuilder.Entity<DpMilestoneTemplates>(entity =>
            {
                entity.HasKey(e => e.MilestoneTemplateId)
                    .HasName("PK__DP_Miles__6A72A86C4B768C43");

                entity.ToTable("DP_MilestoneTemplates", "app");

                entity.Property(e => e.LastUpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.LastUpdateUserNavigation)
                    .WithMany(p => p.DpMilestoneTemplates)
                    .HasForeignKey(d => d.LastUpdateUser)
                    .HasConstraintName("FK_DP_MilestoneTemplates_WebAppUsers");

                entity.HasOne(d => d.MilestoneType)
                    .WithMany(p => p.DpMilestoneTemplates)
                    .HasForeignKey(d => d.MilestoneTypeId)
                    .HasConstraintName("FK_DP_MilestoneTemplates_DP_MilestoneTypes");
            });

            modelBuilder.Entity<DpReportAnnotation>(entity =>
            {
                entity.HasKey(e => e.ReportAnnotationId)
                    .HasName("PK__DP_Repor__84AFA7F30D34E922");

                entity.ToTable("DP_ReportAnnotation", "app");

                entity.Property(e => e.ReportAnnotationId).HasColumnName("ReportAnnotationID");

                entity.HasOne(d => d.DataProject)
                    .WithMany(p => p.DpReportAnnotation)
                    .HasForeignKey(d => d.DataProjectId)
                    .HasConstraintName("FK_DP_ReportAnnotation_DP_DataProject");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.DpReportAnnotation)
                    .HasForeignKey(d => d.ReportId)
                    .HasConstraintName("FK_DP_ReportAnnotation_ReportObject");
            });

            modelBuilder.Entity<DpTermAnnotation>(entity =>
            {
                entity.HasKey(e => e.TermAnnotationId)
                    .HasName("PK__DP_TermA__1BB492E32D415E15");

                entity.ToTable("DP_TermAnnotation", "app");

                entity.Property(e => e.TermAnnotationId).HasColumnName("TermAnnotationID");

                entity.HasOne(d => d.DataProject)
                    .WithMany(p => p.DpTermAnnotation)
                    .HasForeignKey(d => d.DataProjectId)
                    .HasConstraintName("FK_DP_TermAnnotation_DP_DataProject");

                entity.HasOne(d => d.Term)
                    .WithMany(p => p.DpTermAnnotation)
                    .HasForeignKey(d => d.TermId)
                    .HasConstraintName("FK_DP_TermAnnotation_Term");
            });

            modelBuilder.Entity<EstimatedRunFrequency>(entity =>
            {
                entity.ToTable("EstimatedRunFrequency", "app");

                entity.Property(e => e.EstimatedRunFrequencyId).HasColumnName("EstimatedRunFrequencyID");
            });

            modelBuilder.Entity<FinancialImpact>(entity =>
            {
                entity.ToTable("FinancialImpact", "app");
            });

            modelBuilder.Entity<Fragility>(entity =>
            {
                entity.ToTable("Fragility", "app");

                entity.Property(e => e.FragilityId).HasColumnName("FragilityID");
            });

            modelBuilder.Entity<FragilityTag>(entity =>
            {
                entity.ToTable("FragilityTag", "app");

                entity.Property(e => e.FragilityTagId).HasColumnName("FragilityTagID");
            });

            modelBuilder.Entity<GlobalSiteSettings>(entity =>
            {
                entity.ToTable("GlobalSiteSettings", "app");
            });

            modelBuilder.Entity<MailConversations>(entity =>
            {
                entity.HasKey(e => e.ConversationId)
                    .HasName("PK__Mail_Con__C050D8770DFC66D7");

                entity.ToTable("Mail_Conversations", "app");

                entity.HasOne(d => d.Message)
                    .WithMany(p => p.MailConversations)
                    .HasForeignKey(d => d.MessageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Mail_Conversations_Mail_Messages");
            });

            modelBuilder.Entity<MailDrafts>(entity =>
            {
                entity.HasKey(e => e.DraftId);

                entity.ToTable("Mail_Drafts", "app");

                entity.Property(e => e.EditDate).HasColumnType("datetime");

                entity.HasOne(d => d.FromUser)
                    .WithMany(p => p.MailDrafts)
                    .HasForeignKey(d => d.FromUserId)
                    .HasConstraintName("FK_Mail_Drafts_User");
            });

            modelBuilder.Entity<MailFolderMessages>(entity =>
            {
                entity.ToTable("Mail_FolderMessages", "app");

                entity.HasOne(d => d.Folder)
                    .WithMany(p => p.MailFolderMessages)
                    .HasForeignKey(d => d.FolderId)
                    .HasConstraintName("FK_Mail_FolderMessages_Mail_Folders");

                entity.HasOne(d => d.Message)
                    .WithMany(p => p.MailFolderMessages)
                    .HasForeignKey(d => d.MessageId)
                    .HasConstraintName("FK_Mail_FolderMessages_Mail_Messages");
            });

            modelBuilder.Entity<MailFolders>(entity =>
            {
                entity.HasKey(e => e.FolderId)
                    .HasName("PK__Mail_Fol__ACD7107FA5BAA87B");

                entity.ToTable("Mail_Folders", "app");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MailFolders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Mail_Folders_User");
            });

            modelBuilder.Entity<MailMessageType>(entity =>
            {
                entity.HasKey(e => e.MessageTypeId)
                    .HasName("PK__Mail_Mes__9BA1E2BAEE19569E");

                entity.ToTable("Mail_MessageType", "app");
            });

            modelBuilder.Entity<MailMessages>(entity =>
            {
                entity.HasKey(e => e.MessageId)
                    .HasName("PK__Mail_Mes__C87C0C9CF29221A9");

                entity.ToTable("Mail_Messages", "app");

                entity.Property(e => e.SendDate).HasColumnType("datetime");

                entity.HasOne(d => d.FromUser)
                    .WithMany(p => p.MailMessages)
                    .HasForeignKey(d => d.FromUserId)
                    .HasConstraintName("FK_Mail_Messages_User");

                entity.HasOne(d => d.MessageType)
                    .WithMany(p => p.MailMessages)
                    .HasForeignKey(d => d.MessageTypeId)
                    .HasConstraintName("FK_Mail_Messages_Mail_MessageType");
            });

            modelBuilder.Entity<MailRecipients>(entity =>
            {
                entity.ToTable("Mail_Recipients", "app");

                entity.Property(e => e.ReadDate).HasColumnType("datetime");

                entity.HasOne(d => d.Message)
                    .WithMany(p => p.MailRecipients)
                    .HasForeignKey(d => d.MessageId)
                    .HasConstraintName("FK_Mail_Recipients_Mail_Messages");

                entity.HasOne(d => d.ToGroup)
                    .WithMany(p => p.MailRecipients)
                    .HasForeignKey(d => d.ToGroupId)
                    .HasConstraintName("FK_Mail_Recipients_UserLDAPGroups");

                entity.HasOne(d => d.ToUser)
                    .WithMany(p => p.MailRecipients)
                    .HasForeignKey(d => d.ToUserId)
                    .HasConstraintName("FK_Mail_Recipients_User");
            });

            modelBuilder.Entity<MailRecipientsDeleted>(entity =>
            {
                entity.ToTable("Mail_Recipients_Deleted", "app");

                entity.Property(e => e.ReadDate).HasColumnType("datetime");

                entity.HasOne(d => d.ToUser)
                    .WithMany(p => p.MailRecipientsDeleted)
                    .HasForeignKey(d => d.ToUserId)
                    .HasConstraintName("FK_Mail_Recipients_User1");
            });

            modelBuilder.Entity<MaintenanceLog>(entity =>
            {
                entity.ToTable("MaintenanceLog", "app");

                entity.Property(e => e.MaintenanceLogId).HasColumnName("MaintenanceLogID");

                entity.Property(e => e.MaintainerId).HasColumnName("MaintainerID");

                entity.Property(e => e.MaintenanceDate).HasColumnType("datetime");

                entity.Property(e => e.MaintenanceLogStatusId).HasColumnName("MaintenanceLogStatusID");

                entity.HasOne(d => d.Maintainer)
                    .WithMany(p => p.MaintenanceLog)
                    .HasForeignKey(d => d.MaintainerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Maintenan__Maint__65F62111");

                entity.HasOne(d => d.MaintenanceLogStatus)
                    .WithMany(p => p.MaintenanceLog)
                    .HasForeignKey(d => d.MaintenanceLogStatusId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Maintenan__Maint__251C81ED");
            });

            modelBuilder.Entity<MaintenanceLogStatus>(entity =>
            {
                entity.ToTable("MaintenanceLogStatus", "app");

                entity.Property(e => e.MaintenanceLogStatusId).HasColumnName("MaintenanceLogStatusID");

                entity.Property(e => e.MaintenanceLogStatusName).IsRequired();
            });

            modelBuilder.Entity<MaintenanceSchedule>(entity =>
            {
                entity.ToTable("MaintenanceSchedule", "app");

                entity.Property(e => e.MaintenanceScheduleId).HasColumnName("MaintenanceScheduleID");

                entity.Property(e => e.MaintenanceScheduleName).IsRequired();
            });

            modelBuilder.Entity<OrganizationalValue>(entity =>
            {
                entity.ToTable("OrganizationalValue", "app");

                entity.Property(e => e.OrganizationalValueId).HasColumnName("OrganizationalValueID");
            });

            modelBuilder.Entity<ReportGroupsMemberships>(entity =>
            {
                entity.HasKey(e => e.MembershipId)
                    .HasName("PK__ReportGr__92A786790B03128D");

                entity.Property(e => e.LastLoadDate).HasColumnType("datetime");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.ReportGroupsMemberships)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReportGroupsMemberships_UserGroups");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.ReportGroupsMemberships)
                    .HasForeignKey(d => d.ReportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReportGroupsMemberships_ReportObject");
            });

            modelBuilder.Entity<ReportManageEngineTickets>(entity =>
            {
                entity.HasKey(e => e.ManageEngineTicketsId)
                    .HasName("PK__ReportMa__97EB8BADB02592C9");

                entity.ToTable("ReportManageEngineTickets", "app");

                entity.HasOne(d => d.ReportObject)
                    .WithMany(p => p.ReportManageEngineTickets)
                    .HasForeignKey(d => d.ReportObjectId)
                    .HasConstraintName("FK_ReportManageEngineTickets_ReportObject");
            });

            modelBuilder.Entity<ReportObject>(entity =>
            {
                entity.Property(e => e.ReportObjectId).HasColumnName("ReportObjectID");

                entity.Property(e => e.AuthorUserId).HasColumnName("AuthorUserID");

                entity.Property(e => e.DefaultVisibilityYn)
                    .HasColumnName("DefaultVisibilityYN")
                    .HasMaxLength(1);

                entity.Property(e => e.EpicMasterFile).HasMaxLength(3);

                entity.Property(e => e.EpicRecordId)
                    .HasColumnName("EpicRecordID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.EpicReportTemplateId).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.LastLoadDate).HasColumnType("datetime");

                entity.Property(e => e.LastModifiedByUserId).HasColumnName("LastModifiedByUserID");

                entity.Property(e => e.LastModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.OrphanedReportObjectYn)
                    .HasColumnName("OrphanedReportObjectYN")
                    .HasMaxLength(1)
                    .HasDefaultValueSql("('N')");

                entity.Property(e => e.ReportObjectTypeId).HasColumnName("ReportObjectTypeID");

                entity.Property(e => e.ReportObjectUrl).HasColumnName("ReportObjectURL");

                entity.Property(e => e.ReportServerCatalogId)
                    .HasColumnName("ReportServerCatalogID")
                    .HasMaxLength(50);

                entity.Property(e => e.SourceDb)
                    .IsRequired()
                    .HasColumnName("SourceDB")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceServer)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.SourceTable)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasOne(d => d.AuthorUser)
                    .WithMany(p => p.ReportObjectAuthorUser)
                    .HasForeignKey(d => d.AuthorUserId)
                    .HasConstraintName("FK__ReportObj__Autho__35682A19");

                entity.HasOne(d => d.LastModifiedByUser)
                    .WithMany(p => p.ReportObjectLastModifiedByUser)
                    .HasForeignKey(d => d.LastModifiedByUserId)
                    .HasConstraintName("FK__ReportObj__LastM__365C4E52");

                entity.HasOne(d => d.ReportObjectType)
                    .WithMany(p => p.ReportObject)
                    .HasForeignKey(d => d.ReportObjectTypeId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__ReportObj__Repor__3750728B");
            });

            modelBuilder.Entity<ReportObjectConversationDoc>(entity =>
            {
                entity.HasKey(e => e.ConversationId)
                    .HasName("PK__ReportOb__C050D8972E11C321");

                entity.ToTable("ReportObjectConversation_doc", "app");

                entity.Property(e => e.ConversationId).HasColumnName("ConversationID");

                entity.Property(e => e.ReportObjectId).HasColumnName("ReportObjectID");

                entity.HasOne(d => d.ReportObject)
                    .WithMany(p => p.ReportObjectConversationDoc)
                    .HasForeignKey(d => d.ReportObjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ReportObj__Repor__3B21036F");
            });

            modelBuilder.Entity<ReportObjectConversationMessageDoc>(entity =>
            {
                entity.HasKey(e => e.MessageId)
                    .HasName("PK__ReportOb__C87C037C3C86464B");

                entity.ToTable("ReportObjectConversationMessage_doc", "app");

                entity.Property(e => e.MessageId).HasColumnName("MessageID");

                entity.Property(e => e.ConversationId).HasColumnName("ConversationID");

                entity.Property(e => e.MessageText).IsRequired();

                entity.Property(e => e.PostDateTime).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Conversation)
                    .WithMany(p => p.ReportObjectConversationMessageDoc)
                    .HasForeignKey(d => d.ConversationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ReportObj__Conve__4E53A1AA");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ReportObjectConversationMessageDoc)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReportObjectConversationMessage_doc_User");
            });

            modelBuilder.Entity<ReportObjectDoc>(entity =>
            {
                entity.HasKey(e => e.ReportObjectId)
                    .HasName("PK__ReportOb__B7A74135D2A44EFC");

                entity.ToTable("ReportObject_doc", "app");

                entity.Property(e => e.ReportObjectId)
                    .HasColumnName("ReportObjectID")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.DoNotPurge).HasMaxLength(1);

                entity.Property(e => e.EnabledForHyperspace).HasMaxLength(1);

                entity.Property(e => e.EstimatedRunFrequencyId).HasColumnName("EstimatedRunFrequencyID");

                entity.Property(e => e.ExecutiveVisibilityYn)
                    .HasColumnName("ExecutiveVisibilityYN")
                    .HasMaxLength(1);

                entity.Property(e => e.FragilityId).HasColumnName("FragilityID");

                entity.Property(e => e.GitLabBlobUrl).HasColumnName("GitLabBlobURL");

                entity.Property(e => e.GitLabProjectUrl).HasColumnName("GitLabProjectURL");

                entity.Property(e => e.GitLabTreeUrl).HasColumnName("GitLabTreeURL");

                entity.Property(e => e.Hidden).HasMaxLength(1);

                entity.Property(e => e.LastUpdateDateTime).HasColumnType("datetime");

                entity.Property(e => e.MaintenanceScheduleId).HasColumnName("MaintenanceScheduleID");

                entity.Property(e => e.OperationalOwnerUserId).HasColumnName("OperationalOwnerUserID");

                entity.Property(e => e.OrganizationalValueId).HasColumnName("OrganizationalValueID");

                entity.HasOne(d => d.EstimatedRunFrequency)
                    .WithMany(p => p.ReportObjectDoc)
                    .HasForeignKey(d => d.EstimatedRunFrequencyId)
                    .HasConstraintName("FK__ReportObj__Estim__477199F1");

                entity.HasOne(d => d.Fragility)
                    .WithMany(p => p.ReportObjectDoc)
                    .HasForeignKey(d => d.FragilityId)
                    .HasConstraintName("FK__ReportObj__Fragi__4865BE2A");

                entity.HasOne(d => d.MaintenanceSchedule)
                    .WithMany(p => p.ReportObjectDoc)
                    .HasForeignKey(d => d.MaintenanceScheduleId)
                    .HasConstraintName("FK__ReportObj__Maint__4A4E069C");

                entity.HasOne(d => d.OperationalOwnerUser)
                    .WithMany(p => p.ReportObjectDocOperationalOwnerUser)
                    .HasForeignKey(d => d.OperationalOwnerUserId)
                    .HasConstraintName("FK__ReportObj__Opera__4B422AD5");

                entity.HasOne(d => d.OrganizationalValue)
                    .WithMany(p => p.ReportObjectDoc)
                    .HasForeignKey(d => d.OrganizationalValueId)
                    .HasConstraintName("FK__ReportObj__Organ__4C364F0E");

                entity.HasOne(d => d.ReportObject)
                    .WithOne(p => p.ReportObjectDoc)
                    .HasForeignKey<ReportObjectDoc>(d => d.ReportObjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ReportObj__Repor__3938BAFD");

                entity.HasOne(d => d.RequesterNavigation)
                    .WithMany(p => p.ReportObjectDocRequesterNavigation)
                    .HasForeignKey(d => d.Requester)
                    .HasConstraintName("FK__ReportObj__Reque__4E1E9780");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.ReportObjectDocUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK_ReportObject_doc_User");
            });

            modelBuilder.Entity<ReportObjectDocFragilityTags>(entity =>
            {
                entity.HasKey(e => new { e.ReportObjectId, e.FragilityTagId })
                    .HasName("PK__ReportOb__A068477FF5CC7648");

                entity.ToTable("ReportObjectDocFragilityTags", "app");

                entity.Property(e => e.ReportObjectId).HasColumnName("ReportObjectID");

                entity.Property(e => e.FragilityTagId).HasColumnName("FragilityTagID");

                entity.HasOne(d => d.FragilityTag)
                    .WithMany(p => p.ReportObjectDocFragilityTags)
                    .HasForeignKey(d => d.FragilityTagId)
                    .HasConstraintName("FK__ReportObj__Fragi__50FB042B");

                entity.HasOne(d => d.ReportObject)
                    .WithMany(p => p.ReportObjectDocFragilityTags)
                    .HasForeignKey(d => d.ReportObjectId)
                    .HasConstraintName("FK__ReportObj__Repor__51EF2864");
            });

            modelBuilder.Entity<ReportObjectDocMaintenanceLogs>(entity =>
            {
                entity.HasKey(e => new { e.ReportObjectId, e.MaintenanceLogId })
                    .HasName("PK__ReportOb__90514770A7C801F6");

                entity.ToTable("ReportObjectDocMaintenanceLogs", "app");

                entity.Property(e => e.ReportObjectId).HasColumnName("ReportObjectID");

                entity.Property(e => e.MaintenanceLogId).HasColumnName("MaintenanceLogID");

                entity.HasOne(d => d.MaintenanceLog)
                    .WithMany(p => p.ReportObjectDocMaintenanceLogs)
                    .HasForeignKey(d => d.MaintenanceLogId)
                    .HasConstraintName("FK__ReportObj__Maint__54CB950F");

                entity.HasOne(d => d.ReportObject)
                    .WithMany(p => p.ReportObjectDocMaintenanceLogs)
                    .HasForeignKey(d => d.ReportObjectId)
                    .HasConstraintName("FK__ReportObj__Repor__55BFB948");
            });

            modelBuilder.Entity<ReportObjectDocTerms>(entity =>
            {
                entity.HasKey(e => new { e.ReportObjectId, e.TermId })
                    .HasName("PK__ReportOb__F3B7E32FEC52DF5C");

                entity.ToTable("ReportObjectDocTerms", "app");

                entity.Property(e => e.ReportObjectId).HasColumnName("ReportObjectID");

                entity.HasOne(d => d.ReportObject)
                    .WithMany(p => p.ReportObjectDocTerms)
                    .HasForeignKey(d => d.ReportObjectId)
                    .HasConstraintName("FK__ReportObj__Repor__589C25F3");

                entity.HasOne(d => d.Term)
                    .WithMany(p => p.ReportObjectDocTerms)
                    .HasForeignKey(d => d.TermId)
                    .HasConstraintName("FK__ReportObj__TermI__59904A2C");
            });

            modelBuilder.Entity<ReportObjectHierarchy>(entity =>
            {
                entity.HasKey(e => new { e.ParentReportObjectId, e.ChildReportObjectId })
                    .HasName("PK__ReportOb__913B66516CC9406D");

                entity.Property(e => e.ParentReportObjectId).HasColumnName("ParentReportObjectID");

                entity.Property(e => e.ChildReportObjectId).HasColumnName("ChildReportObjectID");

                entity.Property(e => e.LastLoadDate).HasColumnType("datetime");

                entity.HasOne(d => d.ChildReportObject)
                    .WithMany(p => p.ReportObjectHierarchyChildReportObject)
                    .HasForeignKey(d => d.ChildReportObjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ReportObj__Child__3C1527A8");

                entity.HasOne(d => d.ParentReportObject)
                    .WithMany(p => p.ReportObjectHierarchyParentReportObject)
                    .HasForeignKey(d => d.ParentReportObjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ReportObj__Paren__3D094BE1");
            });

            modelBuilder.Entity<ReportObjectImagesDoc>(entity =>
            {
                entity.HasKey(e => e.ImageId)
                    .HasName("PK__ReportOb__7516F4ECD36AFC26");

                entity.ToTable("ReportObjectImages_doc", "app");

                entity.Property(e => e.ImageId).HasColumnName("ImageID");

                entity.Property(e => e.ImageData).IsRequired();

                entity.Property(e => e.ImageOrdinal).HasDefaultValueSql("((1))");

                entity.Property(e => e.ReportObjectId).HasColumnName("ReportObjectID");

                entity.HasOne(d => d.ReportObject)
                    .WithMany(p => p.ReportObjectImagesDoc)
                    .HasForeignKey(d => d.ReportObjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ReportObj__Repor__3A2CDF36");
            });

            modelBuilder.Entity<ReportObjectQuery>(entity =>
            {
                entity.Property(e => e.LastLoadDate).HasColumnType("datetime");

                entity.HasOne(d => d.ReportObject)
                    .WithMany(p => p.ReportObjectQuery)
                    .HasForeignKey(d => d.ReportObjectId)
                    .HasConstraintName("FK_ReportObjectQuery_ReportObject");
            });

            modelBuilder.Entity<ReportObjectRunData>(entity =>
            {
                entity.HasKey(e => new { e.ReportObjectId, e.RunId });

                entity.Property(e => e.ReportObjectId).HasColumnName("ReportObjectID");

                entity.Property(e => e.RunId).HasColumnName("RunID");

                entity.Property(e => e.LastLoadDate).HasColumnType("datetime");

                entity.Property(e => e.RunStartTime).HasColumnType("datetime");

                entity.Property(e => e.RunStatus).HasMaxLength(100);

                entity.Property(e => e.RunUserId).HasColumnName("RunUserID");

                entity.HasOne(d => d.ReportObject)
                    .WithMany(p => p.ReportObjectRunData)
                    .HasForeignKey(d => d.ReportObjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ReportObj__Repor__7A35830E");

                entity.HasOne(d => d.RunUser)
                    .WithMany(p => p.ReportObjectRunData)
                    .HasForeignKey(d => d.RunUserId)
                    .HasConstraintName("FK__ReportObj__RunUs__7B29A747");
            });

            modelBuilder.Entity<ReportObjectRunTime>(entity =>
            {
                entity.ToTable("ReportObjectRunTime", "app");

                entity.Property(e => e.RunTime).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.RunWeek).HasColumnType("datetime");

                entity.HasOne(d => d.RunUser)
                    .WithMany(p => p.ReportObjectRunTime)
                    .HasForeignKey(d => d.RunUserId)
                    .HasConstraintName("fk_userruntime");
            });

            modelBuilder.Entity<ReportObjectSubscriptions>(entity =>
            {
                entity.Property(e => e.LastLoadDate).HasColumnType("datetime");

                entity.Property(e => e.LastRunTime).HasColumnType("datetime");

                entity.HasOne(d => d.ReportObject)
                    .WithMany(p => p.ReportObjectSubscriptions)
                    .HasForeignKey(d => d.ReportObjectId)
                    .HasConstraintName("FK_ReportObjectSubscriptions_ReportObject");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ReportObjectSubscriptions)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_ReportObjectSubscriptions_User");
            });

            modelBuilder.Entity<ReportObjectTopRuns>(entity =>
            {
                entity.ToTable("ReportObjectTopRuns", "app");

                entity.Property(e => e.RunTime).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.ReportObject)
                    .WithMany(p => p.ReportObjectTopRuns)
                    .HasForeignKey(d => d.ReportObjectId)
                    .HasConstraintName("FK_ReportObjectTopRuns_ReportObject");

                entity.HasOne(d => d.RunUser)
                    .WithMany(p => p.ReportObjectTopRuns)
                    .HasForeignKey(d => d.RunUserId)
                    .HasConstraintName("fk_user");
            });

            modelBuilder.Entity<ReportObjectType>(entity =>
            {
                entity.Property(e => e.ReportObjectTypeId).HasColumnName("ReportObjectTypeID");

                entity.Property(e => e.DefaultEpicMasterFile).HasMaxLength(3);

                entity.Property(e => e.LastLoadDate).HasColumnType("datetime");

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<RolePermissionLinks>(entity =>
            {
                entity.ToTable("RolePermissionLinks", "app");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RolePermissionLinks)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_RolePermissionLinks_UserRoles");

                entity.HasOne(d => d.RolePermissions)
                    .WithMany(p => p.RolePermissionLinks)
                    .HasForeignKey(d => d.RolePermissionsId)
                    .HasConstraintName("FK_RolePermissionLinks_RolePermissions");
            });

            modelBuilder.Entity<RolePermissions>(entity =>
            {
                entity.ToTable("RolePermissions", "app");
            });

            modelBuilder.Entity<SearchBasicSearchData>(entity =>
            {
                entity.ToTable("Search_BasicSearchData", "app");

                entity.Property(e => e.ItemType).HasMaxLength(100);

                entity.Property(e => e.SearchFieldDescription).HasMaxLength(100);
            });

            modelBuilder.Entity<SearchBasicSearchDataSmall>(entity =>
            {
                entity.ToTable("Search_BasicSearchData_Small", "app");

                entity.Property(e => e.ItemType).HasMaxLength(100);

                entity.Property(e => e.SearchFieldDescription).HasMaxLength(100);
            });

            modelBuilder.Entity<SearchReportObjectSearchData>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("Search_ReportObjectSearchData_PK");

                entity.ToTable("Search_ReportObjectSearchData", "app");

                entity.Property(e => e.Pk).HasColumnName("pk");

                entity.Property(e => e.DefaultVisibilityYn)
                    .HasColumnName("DefaultVisibilityYN")
                    .HasMaxLength(1);

                entity.Property(e => e.DocCreated).HasColumnType("datetime");

                entity.Property(e => e.DocDoNotPurge).HasMaxLength(1);

                entity.Property(e => e.DocExecVis).HasMaxLength(1);

                entity.Property(e => e.DocHidden).HasMaxLength(1);

                entity.Property(e => e.DocHypeEnabled).HasMaxLength(1);

                entity.Property(e => e.DocLastUpdated).HasColumnType("datetime");

                entity.Property(e => e.EpicMasterFile).HasMaxLength(3);

                entity.Property(e => e.EpicReportTemplateId).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.LastModifiedByUserId).HasColumnName("LastModifiedByUserID");

                entity.Property(e => e.LastModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.OrphanedReportObjectYn)
                    .HasColumnName("OrphanedReportObjectYN")
                    .HasMaxLength(1);

                entity.Property(e => e.ReportObjectTypeId).HasColumnName("ReportObjectTypeID");

                entity.Property(e => e.SourceDb)
                    .IsRequired()
                    .HasColumnName("SourceDB")
                    .HasMaxLength(255);

                entity.Property(e => e.SourceServer)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.SourceTable)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<SearchTable>(entity =>
            {
                entity.ToTable("SearchTable", "app");

                entity.Property(e => e.ItemType).HasMaxLength(100);

                entity.Property(e => e.SearchFieldDescription).HasMaxLength(100);
            });

            modelBuilder.Entity<SharedItems>(entity =>
            {
                entity.ToTable("SharedItems", "app");

                entity.Property(e => e.ShareDate).HasColumnType("datetime");

                entity.HasOne(d => d.SharedFromUser)
                    .WithMany(p => p.SharedItemsSharedFromUser)
                    .HasForeignKey(d => d.SharedFromUserId)
                    .HasConstraintName("FK_SharedItems_User");

                entity.HasOne(d => d.SharedToUser)
                    .WithMany(p => p.SharedItemsSharedToUser)
                    .HasForeignKey(d => d.SharedToUserId)
                    .HasConstraintName("FK_SharedItems_User1");
            });

            modelBuilder.Entity<StrategicImportance>(entity =>
            {
                entity.ToTable("StrategicImportance", "app");
            });

            modelBuilder.Entity<Term>(entity =>
            {
                entity.ToTable("Term", "app");

                entity.HasIndex(e => e.ApprovedYn)
                    .HasName("approved");

                entity.HasIndex(e => e.ValidFromDateTime)
                    .HasName("validfrom");

                entity.Property(e => e.ApprovalDateTime).HasColumnType("datetime");

                entity.Property(e => e.ApprovedYn)
                    .HasColumnName("ApprovedYN")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.ExternalStandardUrl).HasMaxLength(4000);

                entity.Property(e => e.HasExternalStandardYn)
                    .HasColumnName("HasExternalStandardYN")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Summary).HasMaxLength(4000);

                entity.Property(e => e.ValidFromDateTime).HasColumnType("datetime");

                entity.Property(e => e.ValidToDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.ApprovedByUser)
                    .WithMany(p => p.TermApprovedByUser)
                    .HasForeignKey(d => d.ApprovedByUserId)
                    .HasConstraintName("FK_Term_WebAppUsers1");

                entity.HasOne(d => d.UpdatedByUser)
                    .WithMany(p => p.TermUpdatedByUser)
                    .HasForeignKey(d => d.UpdatedByUserId)
                    .HasConstraintName("FK_Term_WebAppUsers");
            });

            modelBuilder.Entity<TermConversation>(entity =>
            {
                entity.ToTable("TermConversation", "app");

                entity.HasOne(d => d.Term)
                    .WithMany(p => p.TermConversation)
                    .HasForeignKey(d => d.TermId)
                    .HasConstraintName("FK__TermConve__TermI__7C4F7684");
            });

            modelBuilder.Entity<TermConversationMessage>(entity =>
            {
                entity.ToTable("TermConversationMessage", "app");

                entity.Property(e => e.TermConversationMessageId).HasColumnName("TermConversationMessageID");

                entity.Property(e => e.MessageText)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.PostDateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.TermConversation)
                    .WithMany(p => p.TermConversationMessage)
                    .HasForeignKey(d => d.TermConversationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TermConversationMessage_TermConversation");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TermConversationMessage)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TermConversationMessage_User");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");

                entity.Property(e => e.LastLoadDate).HasColumnType("datetime");

                entity.Property(e => e.Username).IsRequired();
            });

            modelBuilder.Entity<UserFavoriteFolders>(entity =>
            {
                entity.HasKey(e => e.UserFavoriteFolderId)
                    .HasName("PK__UserFavo__C27B7BFA921AEE69");

                entity.ToTable("UserFavoriteFolders", "app");
            });

            modelBuilder.Entity<UserFavorites>(entity =>
            {
                entity.ToTable("UserFavorites", "app");

                entity.HasOne(d => d.Folder)
                    .WithMany(p => p.UserFavorites)
                    .HasForeignKey(d => d.FolderId)
                    .HasConstraintName("FK_UserFavorites_UserFavoriteFolders");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserFavorites)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserFavorites_User");
            });

            modelBuilder.Entity<UserGroups>(entity =>
            {
                entity.HasKey(e => e.GroupId);

                entity.Property(e => e.LastLoadDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<UserGroupsMembership>(entity =>
            {
                entity.HasKey(e => e.MembershipId);

                entity.Property(e => e.LastLoadDate).HasColumnType("datetime");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.UserGroupsMembership)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_UserGroupsMembership_UserGroups");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserGroupsMembership)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserGroupsMembership_User");
            });

            modelBuilder.Entity<UserNameData>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__User_Nam__1788CC4CF297F9FC");

                entity.ToTable("User_NameData", "app");

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.HasOne(d => d.User)
                    .WithOne(p => p.UserNameData)
                    .HasForeignKey<UserNameData>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_NameData_User");
            });

            modelBuilder.Entity<UserPreferences>(entity =>
            {
                entity.HasKey(e => e.UserPreferenceId)
                    .HasName("PK__UserPref__25771DD8735A40C9");

                entity.ToTable("UserPreferences", "app");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserPreferences)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserPreferences_User");
            });

            modelBuilder.Entity<UserRoleLinks>(entity =>
            {
                entity.ToTable("UserRoleLinks", "app");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoleLinks)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserRoleLinks_User");

                entity.HasOne(d => d.UserRoles)
                    .WithMany(p => p.UserRoleLinks)
                    .HasForeignKey(d => d.UserRolesId)
                    .HasConstraintName("FK_UserRoleLinks_UserRoles");
            });

            modelBuilder.Entity<UserRoles>(entity =>
            {
                entity.ToTable("UserRoles", "app");
            });
        }
    }
}
