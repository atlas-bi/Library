using Microsoft.EntityFrameworkCore;

namespace Atlas_Web.Models
{
    public partial class Atlas_WebContext : DbContext
    {
        public Atlas_WebContext(DbContextOptions<Atlas_WebContext> options)
            : base(options) { }

        public virtual DbSet<Analytic> Analytics { get; set; }
        public virtual DbSet<AnalyticsTrace> AnalyticsTraces { get; set; }
        public virtual DbSet<AnalyticsError> AnalyticsErrors { get; set; }
        public virtual DbSet<Initiative> Initiatives { get; set; }
        public virtual DbSet<Collection> Collections { get; set; }
        public virtual DbSet<CollectionReport> CollectionReports { get; set; }
        public virtual DbSet<CollectionTerm> CollectionTerms { get; set; }
        public virtual DbSet<EstimatedRunFrequency> EstimatedRunFrequencies { get; set; }
        public virtual DbSet<FinancialImpact> FinancialImpacts { get; set; }
        public virtual DbSet<Fragility> Fragilities { get; set; }
        public virtual DbSet<FragilityTag> FragilityTags { get; set; }
        public virtual DbSet<GlobalSiteSetting> GlobalSiteSettings { get; set; }
        public virtual DbSet<MailConversation> MailConversations { get; set; }
        public virtual DbSet<MailDraft> MailDrafts { get; set; }
        public virtual DbSet<MailFolder> MailFolders { get; set; }
        public virtual DbSet<MailFolderMessage> MailFolderMessages { get; set; }
        public virtual DbSet<MailMessage> MailMessages { get; set; }
        public virtual DbSet<MailMessageType> MailMessageTypes { get; set; }
        public virtual DbSet<MailRecipient> MailRecipients { get; set; }
        public virtual DbSet<MailRecipientsDeleted> MailRecipientsDeleteds { get; set; }
        public virtual DbSet<MaintenanceLog> MaintenanceLogs { get; set; }
        public virtual DbSet<MaintenanceLogStatus> MaintenanceLogStatuses { get; set; }
        public virtual DbSet<MaintenanceSchedule> MaintenanceSchedules { get; set; }
        public virtual DbSet<OrganizationalValue> OrganizationalValues { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<ReportTagLink> ReportTagLinks { get; set; }
        public virtual DbSet<ReportGroupsMembership> ReportGroupsMemberships { get; set; }
        public virtual DbSet<ReportServiceRequest> ReportServiceRequests { get; set; }
        public virtual DbSet<ReportObject> ReportObjects { get; set; }
        public virtual DbSet<ReportObjectAttachment> ReportObjectAttachments { get; set; }
        public virtual DbSet<ReportObjectDoc> ReportObjectDocs { get; set; }
        public virtual DbSet<ReportObjectDocFragilityTag> ReportObjectDocFragilityTags { get; set; }
        public virtual DbSet<ReportObjectDocTerm> ReportObjectDocTerms { get; set; }
        public virtual DbSet<ReportObjectHierarchy> ReportObjectHierarchies { get; set; }
        public virtual DbSet<ReportObjectImagesDoc> ReportObjectImagesDocs { get; set; }
        public virtual DbSet<ReportObjectParameter> ReportObjectParameters { get; set; }
        public virtual DbSet<ReportObjectQuery> ReportObjectQueries { get; set; }
        public virtual DbSet<ReportObjectRunData> ReportObjectRunDatas { get; set; }
        public virtual DbSet<ReportObjectRunDataBridge> ReportObjectRunDataBridges { get; set; }
        public virtual DbSet<ReportObjectSubscription> ReportObjectSubscriptions { get; set; }
        public virtual DbSet<ReportObjectTag> ReportObjectTags { get; set; }
        public virtual DbSet<ReportObjectTagMembership> ReportObjectTagMemberships { get; set; }
        public virtual DbSet<ReportObjectType> ReportObjectTypes { get; set; }
        public virtual DbSet<RolePermission> RolePermissions { get; set; }
        public virtual DbSet<RolePermissionLink> RolePermissionLinks { get; set; }
        public virtual DbSet<SharedItem> SharedItems { get; set; }
        public virtual DbSet<StarredCollection> StarredCollections { get; set; }
        public virtual DbSet<StarredGroup> StarredGroups { get; set; }
        public virtual DbSet<StarredInitiative> StarredInitiatives { get; set; }
        public virtual DbSet<StarredReport> StarredReports { get; set; }
        public virtual DbSet<StarredSearch> StarredSearches { get; set; }
        public virtual DbSet<StarredTerm> StarredTerms { get; set; }
        public virtual DbSet<StarredUser> StarredUsers { get; set; }
        public virtual DbSet<StrategicImportance> StrategicImportances { get; set; }
        public virtual DbSet<Term> Terms { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserFavoriteFolder> UserFavoriteFolders { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<UserGroupsMembership> UserGroupsMemberships { get; set; }
        public virtual DbSet<UserPreference> UserPreferences { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<UserRoleLink> UserRoleLinks { get; set; }
        public virtual DbSet<GroupRoleLink> GroupRoleLinks { get; set; }
        public virtual DbSet<UserSetting> UserSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // unused configuration, leftover from scafolding days.
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Analytic>(entity =>
            {
                entity.ToTable("Analytics", "app");

                entity.HasIndex(e => e.AccessDateTime, "accessdatetime");
                entity
                    .HasIndex(
                        e => new { e.UserId, e.AccessDateTime },
                        "user_access_load_page_session_path"
                    )
                    .IncludeProperties(
                        p =>
                            new
                            {
                                p.LoadTime,
                                p.PageId,
                                p.SessionId,
                                p.Pathname
                            }
                    );
                entity
                    .HasIndex(e => e.AccessDateTime, "accessdatetime_session_width_agent")
                    .IncludeProperties(
                        p =>
                            new
                            {
                                p.PageId,
                                p.SessionId,
                                p.ScreenWidth,
                                p.UserAgent
                            }
                    );

                entity.HasIndex(e => e.UserId, "userid");

                entity.Property(e => e.AccessDateTime).HasColumnType("datetime");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity
                    .HasOne(d => d.User)
                    .WithMany(p => p.Analytics)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Analytics_User");
            });

            modelBuilder.Entity<AnalyticsTrace>(entity =>
            {
                entity.ToTable("AnalyticsTrace", "app");

                entity.Property(e => e.LogDateTime).HasColumnType("datetime");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity
                    .HasOne(d => d.User)
                    .WithMany(p => p.AnalyticsTraces)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Analytics_Trace_User");
            });

            modelBuilder.Entity<AnalyticsError>(entity =>
            {
                entity.ToTable("AnalyticsError", "app");

                entity.Property(e => e.LogDateTime).HasColumnType("datetime");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity
                    .HasOne(d => d.User)
                    .WithMany(p => p.AnalyticsErrors)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Analytics_Error_User");
            });

            modelBuilder.Entity<Initiative>(entity =>
            {
                entity.HasKey(e => e.InitiativeId).HasName("PK__DP_DataI__1EFC948C3A83A845");

                entity.ToTable("Initiative", "app");

                entity.HasIndex(e => e.ExecutiveOwnerId, "executiveownerid");

                entity.HasIndex(e => e.FinancialImpact, "financialimpact");

                entity.HasIndex(e => e.InitiativeId, "initiativeid");

                entity.HasIndex(e => e.LastUpdateDate, "lastupdatedate");

                entity.HasIndex(e => e.LastUpdateUser, "lastupdateuser");

                entity.HasIndex(e => e.OperationOwnerId, "operationownderid");

                entity.HasIndex(e => e.StrategicImportance, "strategicimportance");

                entity.Property(e => e.Hidden).HasMaxLength(1).IsFixedLength();

                entity
                    .HasOne(d => d.ExecutiveOwner)
                    .WithMany(p => p.DpDataInitiativeExecutiveOwners)
                    .HasForeignKey(d => d.ExecutiveOwnerId)
                    .HasConstraintName("FK_DP_DataInitiative_User");

                entity
                    .HasOne(d => d.FinancialImpactNavigation)
                    .WithMany(p => p.Initiatives)
                    .HasForeignKey(d => d.FinancialImpact)
                    .HasConstraintName("FK_DP_DataInitiative_FinancialImpact");

                entity
                    .HasOne(d => d.LastUpdateUserNavigation)
                    .WithMany(p => p.DpDataInitiativeLastUpdateUserNavigations)
                    .HasForeignKey(d => d.LastUpdateUser)
                    .HasConstraintName("FK_DP_DataInitiative_WebAppUsers");

                entity
                    .HasOne(d => d.OperationOwner)
                    .WithMany(p => p.DpDataInitiativeOperationOwners)
                    .HasForeignKey(d => d.OperationOwnerId)
                    .HasConstraintName("FK_DP_DataInitiative_User1");

                entity
                    .HasOne(d => d.StrategicImportanceNavigation)
                    .WithMany(p => p.Initiatives)
                    .HasForeignKey(d => d.StrategicImportance)
                    .HasConstraintName("FK_DP_DataInitiative_StrategicImportance");
            });

            modelBuilder.Entity<Collection>(entity =>
            {
                entity.HasKey(e => e.CollectionId).HasName("PK__DP_DataP__E8D09D08794EBFAD");

                entity.ToTable("Collection", "app");

                entity.HasIndex(e => e.AnalyticsOwnerId, "analyticsownerid");

                entity.HasIndex(e => e.CollectionId, "collectionid");

                entity.HasIndex(e => e.DataManagerId, "datamanagerid");

                entity.HasIndex(e => e.ExecutiveOwnerId, "executiveownerid");

                entity.HasIndex(e => e.FinancialImpact, "financialimpact");

                entity.HasIndex(e => e.InitiativeId, "initiativeid");

                entity.HasIndex(e => e.LastUpdateDate, "lastupdatedate");

                entity.HasIndex(e => e.LastUpdateUser, "lastupdateuser");

                entity.HasIndex(e => e.OperationOwnerId, "operationownerid");

                entity.HasIndex(e => e.StrategicImportance, "strategicimportance");

                entity.Property(e => e.Hidden).HasMaxLength(1).IsFixedLength();

                entity.Property(e => e.LastUpdateDate).HasColumnType("datetime");

                entity
                    .HasOne(d => d.AnalyticsOwner)
                    .WithMany(p => p.DpDataProjectAnalyticsOwners)
                    .HasForeignKey(d => d.AnalyticsOwnerId)
                    .HasConstraintName("FK_DP_DataProject_WebAppUsers1");

                entity
                    .HasOne(d => d.Initiative)
                    .WithMany(p => p.Collections)
                    .HasForeignKey(d => d.InitiativeId)
                    .HasConstraintName("FK_DP_DataProject_DP_DataInitiative");

                entity
                    .HasOne(d => d.DataManager)
                    .WithMany(p => p.DpDataProjectDataManagers)
                    .HasForeignKey(d => d.DataManagerId)
                    .HasConstraintName("FK_DP_DataProject_User2");

                entity
                    .HasOne(d => d.ExecutiveOwner)
                    .WithMany(p => p.DpDataProjectExecutiveOwners)
                    .HasForeignKey(d => d.ExecutiveOwnerId)
                    .HasConstraintName("FK_DP_DataProject_User");

                entity
                    .HasOne(d => d.FinancialImpactNavigation)
                    .WithMany(p => p.Collections)
                    .HasForeignKey(d => d.FinancialImpact)
                    .HasConstraintName("FK_DP_DataProject_FinancialImpact");

                entity
                    .HasOne(d => d.LastUpdateUserNavigation)
                    .WithMany(p => p.DpDataProjectLastUpdateUserNavigations)
                    .HasForeignKey(d => d.LastUpdateUser)
                    .HasConstraintName("FK_DP_DataProject_WebAppUsers");

                entity
                    .HasOne(d => d.OperationOwner)
                    .WithMany(p => p.DpDataProjectOperationOwners)
                    .HasForeignKey(d => d.OperationOwnerId)
                    .HasConstraintName("FK_DP_DataProject_User1");

                entity
                    .HasOne(d => d.StrategicImportanceNavigation)
                    .WithMany(p => p.Collections)
                    .HasForeignKey(d => d.StrategicImportance)
                    .HasConstraintName("FK_DP_DataProject_StrategicImportance");
            });

            modelBuilder.Entity<CollectionReport>(entity =>
            {
                entity.HasKey(e => e.LinkId).HasName("PK__DP_Repor__84AFA7F30D34E922");

                entity.ToTable("CollectionReport", "app");

                entity.HasIndex(e => new { e.ReportId, e.CollectionId }, "reportid+collectionid");

                entity
                    .HasOne(d => d.DataProject)
                    .WithMany(p => p.CollectionReports)
                    .HasForeignKey(d => d.CollectionId)
                    .HasConstraintName("FK_DP_ReportAnnotation_DP_DataProject");

                entity
                    .HasOne(d => d.Report)
                    .WithMany(p => p.CollectionReports)
                    .HasForeignKey(d => d.ReportId)
                    .HasConstraintName("FK_DP_ReportAnnotation_ReportObject");
            });

            modelBuilder.Entity<CollectionTerm>(entity =>
            {
                entity.HasKey(e => e.LinkId).HasName("PK__DP_TermA__1BB492E32D415E15");

                entity.ToTable("CollectionTerm", "app");

                entity.HasIndex(e => new { e.TermId, e.CollectionId }, "termid+collectionid");

                entity
                    .HasOne(d => d.DataProject)
                    .WithMany(p => p.CollectionTerms)
                    .HasForeignKey(d => d.CollectionId)
                    .HasConstraintName("FK_DP_TermAnnotation_DP_DataProject");

                entity
                    .HasOne(d => d.Term)
                    .WithMany(p => p.CollectionTerms)
                    .HasForeignKey(d => d.TermId)
                    .HasConstraintName("FK_DP_TermAnnotation_Term");
            });

            modelBuilder.Entity<EstimatedRunFrequency>(entity =>
            {
                entity.ToTable("EstimatedRunFrequency", "app");

                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<FinancialImpact>(entity =>
            {
                entity.ToTable("FinancialImpact", "app");

                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<Fragility>(entity =>
            {
                entity.ToTable("Fragility", "app");

                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<FragilityTag>(entity =>
            {
                entity.ToTable("FragilityTag", "app");

                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<GlobalSiteSetting>(entity =>
            {
                entity.ToTable("GlobalSiteSettings", "app");
            });

            modelBuilder.Entity<UserSetting>(entity =>
            {
                entity.ToTable("UserSettings", "app");
                entity.HasIndex(e => new { e.UserId, e.Name }, "user_setting");
                entity.HasIndex(e => e.UserId, "user");
                entity
                    .HasOne(d => d.User)
                    .WithMany(p => p.UserSettings)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserSettings_User");
            });

            modelBuilder.Entity<MailConversation>(entity =>
            {
                entity.HasKey(e => e.ConversationId).HasName("PK__Mail_Con__C050D8770DFC66D7");

                entity.ToTable("Mail_Conversations", "app");

                entity
                    .HasOne(d => d.Message)
                    .WithMany(p => p.MailConversations)
                    .HasForeignKey(d => d.MessageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Mail_Conversations_Mail_Messages");
            });

            modelBuilder.Entity<MailDraft>(entity =>
            {
                entity.HasKey(e => e.DraftId);

                entity.ToTable("Mail_Drafts", "app");

                entity.Property(e => e.EditDate).HasColumnType("datetime");

                entity
                    .HasOne(d => d.FromUser)
                    .WithMany(p => p.MailDrafts)
                    .HasForeignKey(d => d.FromUserId)
                    .HasConstraintName("FK_Mail_Drafts_User");
            });

            modelBuilder.Entity<MailFolder>(entity =>
            {
                entity.HasKey(e => e.FolderId).HasName("PK__Mail_Fol__ACD7107FA5BAA87B");

                entity.ToTable("Mail_Folders", "app");

                entity
                    .HasOne(d => d.User)
                    .WithMany(p => p.MailFolders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Mail_Folders_User");
            });

            modelBuilder.Entity<MailFolderMessage>(entity =>
            {
                entity.ToTable("Mail_FolderMessages", "app");

                entity
                    .HasOne(d => d.Folder)
                    .WithMany(p => p.MailFolderMessages)
                    .HasForeignKey(d => d.FolderId)
                    .HasConstraintName("FK_Mail_FolderMessages_Mail_Folders");

                entity
                    .HasOne(d => d.Message)
                    .WithMany(p => p.MailFolderMessages)
                    .HasForeignKey(d => d.MessageId)
                    .HasConstraintName("FK_Mail_FolderMessages_Mail_Messages");
            });

            modelBuilder.Entity<MailMessage>(entity =>
            {
                entity.HasKey(e => e.MessageId).HasName("PK__Mail_Mes__C87C0C9CF29221A9");

                entity.ToTable("Mail_Messages", "app");

                entity.Property(e => e.SendDate).HasColumnType("datetime");

                entity
                    .HasOne(d => d.FromUser)
                    .WithMany(p => p.MailMessages)
                    .HasForeignKey(d => d.FromUserId)
                    .HasConstraintName("FK_Mail_Messages_User");

                entity
                    .HasOne(d => d.MessageType)
                    .WithMany(p => p.MailMessages)
                    .HasForeignKey(d => d.MessageTypeId)
                    .HasConstraintName("FK_Mail_Messages_Mail_MessageType");
            });

            modelBuilder.Entity<MailMessageType>(entity =>
            {
                entity.HasKey(e => e.MessageTypeId).HasName("PK__Mail_Mes__9BA1E2BAEE19569E");

                entity.ToTable("Mail_MessageType", "app");
            });

            modelBuilder.Entity<MailRecipient>(entity =>
            {
                entity.ToTable("Mail_Recipients", "app");

                entity.Property(e => e.ReadDate).HasColumnType("datetime");

                entity
                    .HasOne(d => d.Message)
                    .WithMany(p => p.MailRecipients)
                    .HasForeignKey(d => d.MessageId)
                    .HasConstraintName("FK_Mail_Recipients_Mail_Messages");

                entity
                    .HasOne(d => d.ToGroup)
                    .WithMany(p => p.MailRecipients)
                    .HasForeignKey(d => d.ToGroupId)
                    .HasConstraintName("FK_Mail_Recipients_UserLDAPGroups");

                entity
                    .HasOne(d => d.ToUser)
                    .WithMany(p => p.MailRecipients)
                    .HasForeignKey(d => d.ToUserId)
                    .HasConstraintName("FK_Mail_Recipients_User");
            });

            modelBuilder.Entity<MailRecipientsDeleted>(entity =>
            {
                entity.ToTable("Mail_Recipients_Deleted", "app");

                entity.Property(e => e.ReadDate).HasColumnType("datetime");

                entity
                    .HasOne(d => d.ToUser)
                    .WithMany(p => p.MailRecipientsDeleteds)
                    .HasForeignKey(d => d.ToUserId)
                    .HasConstraintName("FK_Mail_Recipients_User1");
            });

            modelBuilder.Entity<MaintenanceLog>(entity =>
            {
                entity.ToTable("MaintenanceLog", "app");

                entity.HasIndex(e => e.MaintenanceLogId, "logid");

                entity.HasIndex(e => e.MaintainerId, "maintainerid");

                entity.HasIndex(e => e.MaintenanceDate, "maintenancedate");

                entity.HasIndex(e => e.MaintenanceLogStatusId, "maintenancelogstatusid");

                entity.Property(e => e.MaintenanceDate).HasColumnType("datetime");

                entity
                    .HasOne(d => d.Maintainer)
                    .WithMany(p => p.MaintenanceLogs)
                    .HasForeignKey(d => d.MaintainerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Maintenan__Maint__65F62111");

                entity
                    .HasOne(d => d.MaintenanceLogStatus)
                    .WithMany(p => p.MaintenanceLogs)
                    .HasForeignKey(d => d.MaintenanceLogStatusId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Maintenan__Maint__251C81ED");

                entity
                    .HasOne(d => d.ReportObjectDoc)
                    .WithMany(p => p.MaintenanceLogs)
                    .HasForeignKey(d => d.ReportId)
                    .HasConstraintName("FK__ReportObj__Repor__72E3DB65");
            });

            modelBuilder.Entity<MaintenanceLogStatus>(entity =>
            {
                entity.ToTable("MaintenanceLogStatus", "app");

                entity.HasIndex(e => e.Id);

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<MaintenanceSchedule>(entity =>
            {
                entity.ToTable("MaintenanceSchedule", "app");

                entity.HasIndex(e => e.Id);

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<OrganizationalValue>(entity =>
            {
                entity.ToTable("OrganizationalValue", "app");

                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("Tags", "dbo");
                entity.HasKey(e => e.TagId);

                entity.HasIndex(e => e.TagId, "tagid");
                entity.HasIndex(e => e.Name, "tagname");
            });

            modelBuilder.Entity<ReportTagLink>(entity =>
            {
                entity.ToTable("ReportTagLinks", "dbo");
                entity.HasKey(e => e.ReportTagLinkId);
                entity.HasIndex(e => new { e.ReportId, e.TagId }, "report_tag");
                entity
                    .HasOne(d => d.Report)
                    .WithMany(p => p.ReportTagLinks)
                    .HasForeignKey(d => d.ReportId);
                entity
                    .HasOne(d => d.Tag)
                    .WithMany(p => p.ReportTagLinks)
                    .HasForeignKey(d => d.TagId);
            });

            modelBuilder.Entity<ReportGroupsMembership>(entity =>
            {
                entity.ToTable("ReportGroupsMemberships", "dbo");
                entity.HasKey(e => e.MembershipId).HasName("PK__ReportGr__92A786790B03128D");

                entity.HasIndex(e => new { e.GroupId, e.ReportId }, "groupid+reportid");

                entity.HasIndex(e => e.ReportId, "reportid");

                entity.Property(e => e.LastLoadDate).HasColumnType("datetime");

                entity
                    .HasOne(d => d.Group)
                    .WithMany(p => p.ReportGroupsMemberships)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReportGroupsMemberships_UserGroups");

                entity
                    .HasOne(d => d.Report)
                    .WithMany(p => p.ReportGroupsMemberships)
                    .HasForeignKey(d => d.ReportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReportGroupsMemberships_ReportObject");
            });

            modelBuilder.Entity<ReportServiceRequest>(entity =>
            {
                entity.ToTable("ReportServiceRequests", "app");
                entity.HasKey(e => e.ServiceRequestId).HasName("PK__ReportMa__97EB8BADB02592C9");

                entity.ToTable("ReportServiceRequests", "app");

                entity.HasIndex(e => e.ReportObjectId, "reportobjectid");

                entity
                    .HasOne(d => d.ReportObject)
                    .WithMany(p => p.ReportServiceRequests)
                    .HasForeignKey(d => d.ReportObjectId)
                    .HasConstraintName("FK_ReportServiceRequests_ReportObject");
            });

            modelBuilder.Entity<ReportObject>(entity =>
            {
                entity.ToTable("ReportObject", "dbo");

                entity.HasIndex(e => e.AuthorUserId, "authorid");

                entity.HasIndex(e => e.LastModifiedByUserId, "modifiedby");

                entity.HasIndex(e => e.ReportObjectId, "reportid").IsUnique();

                entity
                    .HasIndex(
                        e => new { e.SourceServer, e.ReportObjectTypeId },
                        "sourceserver_type_report"
                    )
                    .IncludeProperties(p => p.ReportObjectId);
                entity
                    .HasIndex(e => e.DefaultVisibilityYn, "visibility_report_masterfile")
                    .IncludeProperties(p => new { p.ReportObjectId, p.EpicMasterFile });
                entity
                    .HasIndex(
                        e => new { e.SourceDb, e.EpicMasterFile },
                        "sourcedb_masterfile_report"
                    )
                    .IncludeProperties(p => p.ReportObjectId);
                entity
                    .HasIndex(e => e.SourceServer, "sourceserver_report_masterfile")
                    .IncludeProperties(p => new { p.ReportObjectId, p.EpicMasterFile });
                entity
                    .HasIndex(e => e.SourceDb, "sourcedb_report_masterfile")
                    .IncludeProperties(p => new { p.ReportObjectId, p.EpicMasterFile });
                entity
                    .HasIndex(e => new { e.ReportObjectTypeId, e.EpicMasterFile }, "type_report")
                    .IncludeProperties(p => p.ReportObjectId);
                entity
                    .HasIndex(
                        e => new { e.SourceServer, e.EpicMasterFile },
                        "sourceserver_masterfile_report"
                    )
                    .IncludeProperties(p => p.ReportObjectId);
                entity
                    .HasIndex(e => e.ReportObjectTypeId, "type_report_masterfile")
                    .IncludeProperties(
                        p =>
                            new
                            {
                                p.ReportObjectId,
                                p.EpicMasterFile,
                                p.Name,
                                p.DisplayTitle
                            }
                    );
                entity
                    .HasIndex(e => e.EpicMasterFile, "masterfile_report_visiblity_type")
                    .IncludeProperties(
                        p =>
                            new
                            {
                                p.ReportObjectId,
                                p.DefaultVisibilityYn,
                                p.ReportObjectTypeId
                            }
                    );
                entity
                    .HasIndex(
                        e =>
                            new
                            {
                                e.EpicMasterFile,
                                e.SourceServer,
                                e.ReportObjectTypeId
                            },
                        "masterfile_sourceserver_type_report"
                    )
                    .IncludeProperties(p => p.ReportObjectId);

                entity.HasIndex(
                    e =>
                        new
                        {
                            e.DefaultVisibilityYn,
                            e.OrphanedReportObjectYn,
                            e.ReportObjectTypeId
                        },
                    "visibility + orphan + type"
                );

                entity.Property(e => e.ReportObjectId).HasColumnName("ReportObjectID");

                entity.Property(e => e.AuthorUserId).HasColumnName("AuthorUserID");

                entity
                    .Property(e => e.DefaultVisibilityYn)
                    .HasMaxLength(1)
                    .HasColumnName("DefaultVisibilityYN");

                entity.Property(e => e.EpicMasterFile).HasMaxLength(3);

                entity
                    .Property(e => e.EpicRecordId)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("EpicRecordID");

                entity.Property(e => e.EpicReleased).HasMaxLength(1);

                entity.Property(e => e.EpicReportTemplateId).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.LastLoadDate).HasColumnType("datetime");

                entity.Property(e => e.LastModifiedByUserId).HasColumnName("LastModifiedByUserID");

                entity.Property(e => e.LastModifiedDate).HasColumnType("datetime");

                entity
                    .Property(e => e.OrphanedReportObjectYn)
                    .HasMaxLength(1)
                    .HasColumnName("OrphanedReportObjectYN")
                    .HasDefaultValueSql("('N')")
                    .IsFixedLength();

                entity.Property(e => e.ReportObjectTypeId).HasColumnName("ReportObjectTypeID");

                entity.Property(e => e.ReportObjectUrl).HasColumnName("ReportObjectURL");

                entity
                    .Property(e => e.ReportServerCatalogId)
                    .HasMaxLength(50)
                    .HasColumnName("ReportServerCatalogID");

                entity
                    .Property(e => e.SourceDb)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("SourceDB");

                entity.Property(e => e.SourceServer).IsRequired().HasMaxLength(255);

                entity.Property(e => e.SourceTable).IsRequired().HasMaxLength(255);

                entity
                    .HasOne(d => d.AuthorUser)
                    .WithMany(p => p.ReportObjectAuthorUsers)
                    .HasForeignKey(d => d.AuthorUserId)
                    .HasConstraintName("FK__ReportObj__Autho__35682A19");

                entity
                    .HasOne(d => d.LastModifiedByUser)
                    .WithMany(p => p.ReportObjectLastModifiedByUsers)
                    .HasForeignKey(d => d.LastModifiedByUserId)
                    .HasConstraintName("FK__ReportObj__LastM__365C4E52");

                entity
                    .HasOne(d => d.ReportObjectType)
                    .WithMany(p => p.ReportObjects)
                    .HasForeignKey(d => d.ReportObjectTypeId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__ReportObj__Repor__3750728B");
            });

            modelBuilder.Entity<ReportObjectAttachment>(entity =>
            {
                entity.ToTable("ReportObjectAttachments", "dbo");
                entity.HasIndex(e => e.ReportObjectId, "reportid");

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.LastLoadDate).HasColumnType("datetime");

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Path).IsRequired();

                entity
                    .HasOne(d => d.ReportObject)
                    .WithMany(p => p.ReportObjectAttachments)
                    .HasForeignKey(d => d.ReportObjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReportObjectAttachments_ReportObject");
            });

            modelBuilder.Entity<ReportObjectDoc>(entity =>
            {
                entity.HasKey(e => e.ReportObjectId).HasName("PK__ReportOb__B7A74135D2A44EFC");

                entity.ToTable("ReportObject_doc", "app");

                entity
                    .HasIndex(e => e.ExecutiveVisibilityYn, "execvis_reportid")
                    .IncludeProperties(p => p.ReportObjectId);

                entity
                    .HasIndex(e => e.MaintenanceScheduleId, "maintschedule_report_updated")
                    .IncludeProperties(
                        p =>
                            new
                            {
                                p.ReportObjectId,
                                p.LastUpdateDateTime,
                                p.UpdatedBy
                            }
                    );

                entity
                    .HasIndex(e => e.MaintenanceScheduleId, "maintschedule_report_updated_created")
                    .IncludeProperties(
                        p =>
                            new
                            {
                                p.ReportObjectId,
                                p.LastUpdateDateTime,
                                p.UpdatedBy,
                                p.CreatedDateTime
                            }
                    );

                entity.HasIndex(e => e.CreatedBy, "createdby");

                entity.HasIndex(e => e.EstimatedRunFrequencyId, "estimatedrunfreqid");

                entity.HasIndex(e => e.FragilityId, "fragilityid");

                entity.HasIndex(e => e.MaintenanceScheduleId, "maintenancescheduleid");

                entity.HasIndex(e => e.OperationalOwnerUserId, "operationalownerid");

                entity.HasIndex(e => e.OrganizationalValueId, "organizationalvalueid");

                entity.HasIndex(e => e.ReportObjectId, "reportid").IsUnique();

                entity.HasIndex(e => e.Requester, "requester");

                entity.HasIndex(e => e.UpdatedBy, "updatedby");

                entity
                    .Property(e => e.ReportObjectId)
                    .ValueGeneratedNever()
                    .HasColumnName("ReportObjectID");

                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.DoNotPurge).HasMaxLength(1).IsFixedLength();

                entity.Property(e => e.EnabledForHyperspace).HasMaxLength(1).IsFixedLength();

                entity
                    .Property(e => e.EstimatedRunFrequencyId)
                    .HasColumnName("EstimatedRunFrequencyID");

                entity
                    .Property(e => e.ExecutiveVisibilityYn)
                    .HasMaxLength(1)
                    .HasColumnName("ExecutiveVisibilityYN")
                    .IsFixedLength();

                entity.Property(e => e.FragilityId).HasColumnName("FragilityID");

                entity.Property(e => e.GitLabProjectUrl).HasColumnName("GitLabProjectURL");

                entity.Property(e => e.Hidden).HasMaxLength(1).IsFixedLength();

                entity.Property(e => e.LastUpdateDateTime).HasColumnType("datetime");

                entity
                    .Property(e => e.MaintenanceScheduleId)
                    .HasColumnName("MaintenanceScheduleID");

                entity
                    .Property(e => e.OperationalOwnerUserId)
                    .HasColumnName("OperationalOwnerUserID");

                entity
                    .Property(e => e.OrganizationalValueId)
                    .HasColumnName("OrganizationalValueID");

                entity
                    .HasOne(d => d.EstimatedRunFrequency)
                    .WithMany(p => p.ReportObjectDocs)
                    .HasForeignKey(d => d.EstimatedRunFrequencyId)
                    .HasConstraintName("FK__ReportObj__Estim__477199F1");

                entity
                    .HasOne(d => d.Fragility)
                    .WithMany(p => p.ReportObjectDocs)
                    .HasForeignKey(d => d.FragilityId)
                    .HasConstraintName("FK__ReportObj__Fragi__4865BE2A");

                entity
                    .HasOne(d => d.MaintenanceSchedule)
                    .WithMany(p => p.ReportObjectDocs)
                    .HasForeignKey(d => d.MaintenanceScheduleId)
                    .HasConstraintName("FK__ReportObj__Maint__4959E263");

                entity
                    .HasOne(d => d.OperationalOwnerUser)
                    .WithMany(p => p.ReportObjectDocOperationalOwnerUsers)
                    .HasForeignKey(d => d.OperationalOwnerUserId)
                    .HasConstraintName("FK__ReportObj__Opera__4B422AD5");

                entity
                    .HasOne(d => d.OrganizationalValue)
                    .WithMany(p => p.ReportObjectDocs)
                    .HasForeignKey(d => d.OrganizationalValueId)
                    .HasConstraintName("FK__ReportObj__Organ__4C364F0E");

                entity
                    .HasOne(d => d.ReportObject)
                    .WithOne(p => p.ReportObjectDoc)
                    .HasForeignKey<ReportObjectDoc>(d => d.ReportObjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ReportObj__Repor__3938BAFD");

                entity
                    .HasOne(d => d.RequesterNavigation)
                    .WithMany(p => p.ReportObjectDocRequesterNavigations)
                    .HasForeignKey(d => d.Requester)
                    .HasConstraintName("FK__ReportObj__Reque__4E1E9780");

                entity
                    .HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.ReportObjectDocUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK_ReportObject_doc_User");
            });

            modelBuilder.Entity<ReportObjectDocFragilityTag>(entity =>
            {
                entity.HasKey(e => e.LinkId).HasName("PK__ReportOb__2D122135B03BB8CE");

                entity.ToTable("ReportObjectDocFragilityTags", "app");

                entity
                    .HasOne(d => d.FragilityTag)
                    .WithMany(p => p.ReportObjectDocFragilityTags)
                    .HasForeignKey(d => d.FragilityTagId)
                    .HasConstraintName("FK__ReportObj__Fragi__71EFB72C");

                entity
                    .HasOne(d => d.ReportObject)
                    .WithMany(p => p.ReportObjectDocFragilityTags)
                    .HasForeignKey(d => d.ReportObjectId)
                    .HasConstraintName("FK__ReportObj__Repor__72E3DB67");
            });

            modelBuilder.Entity<ReportObjectDocTerm>(entity =>
            {
                entity.HasKey(e => e.LinkId).HasName("PK__ReportOb__2D122135AFCD5E79");

                entity.ToTable("ReportObjectDocTerms", "app");

                entity
                    .HasOne(d => d.ReportObject)
                    .WithMany(p => p.ReportObjectDocTerms)
                    .HasForeignKey(d => d.ReportObjectId)
                    .HasConstraintName("FK__ReportObj__Repor__6A4E9564");

                entity
                    .HasOne(d => d.Term)
                    .WithMany(p => p.ReportObjectDocTerms)
                    .HasForeignKey(d => d.TermId)
                    .HasConstraintName("FK__ReportObj__TermI__6B42B99D");
            });

            modelBuilder.Entity<ReportObjectHierarchy>(entity =>
            {
                entity
                    .HasKey(e => new { e.ParentReportObjectId, e.ChildReportObjectId })
                    .HasName("PK__ReportOb__913B66516CC9406D");

                entity.ToTable("ReportObjectHierarchy", "dbo");

                entity.HasIndex(e => e.ChildReportObjectId, "childid");

                entity.HasIndex(
                    e => new { e.ParentReportObjectId, e.ChildReportObjectId },
                    "parent+child"
                );

                entity.HasIndex(e => e.ParentReportObjectId, "parentid");

                entity.Property(e => e.ParentReportObjectId).HasColumnName("ParentReportObjectID");

                entity.Property(e => e.ChildReportObjectId).HasColumnName("ChildReportObjectID");

                entity.Property(e => e.LastLoadDate).HasColumnType("datetime");

                entity
                    .HasOne(d => d.ChildReportObject)
                    .WithMany(p => p.ReportObjectHierarchyChildReportObjects)
                    .HasForeignKey(d => d.ChildReportObjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ReportObj__Child__3C1527A8");

                entity
                    .HasOne(d => d.ParentReportObject)
                    .WithMany(p => p.ReportObjectHierarchyParentReportObjects)
                    .HasForeignKey(d => d.ParentReportObjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ReportObj__Paren__3D094BE1");
            });

            modelBuilder.Entity<ReportObjectImagesDoc>(entity =>
            {
                entity.HasKey(e => e.ImageId).HasName("PK__ReportOb__7516F4ECD36AFC26");

                entity.ToTable("ReportObjectImages_doc", "app");

                entity.HasIndex(e => e.ReportObjectId, "reportid");

                entity.Property(e => e.ImageData).IsRequired();

                entity.Property(e => e.ImageOrdinal).HasDefaultValueSql("((1))");

                entity
                    .HasOne(d => d.ReportObject)
                    .WithMany(p => p.ReportObjectImagesDocs)
                    .HasForeignKey(d => d.ReportObjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ReportObj__Repor__3A2CDF36");
            });

            modelBuilder.Entity<ReportObjectParameter>(entity =>
            {
                entity.ToTable("ReportObjectParameters", "dbo");
                entity.HasIndex(e => e.ReportObjectId, "reportobjectid");

                entity
                    .HasOne(d => d.ReportObject)
                    .WithMany(p => p.ReportObjectParameters)
                    .HasForeignKey(d => d.ReportObjectId)
                    .HasConstraintName("FK_ReportObjectParameters1_ReportObject");
            });

            modelBuilder.Entity<ReportObjectQuery>(entity =>
            {
                entity.ToTable("ReportObjectQuery", "dbo");

                entity.HasIndex(e => e.ReportObjectId, "NonClusteredIndex-20220324-104152");

                entity.HasIndex(e => e.ReportObjectQueryId, "queryid");

                entity.HasIndex(e => e.ReportObjectId, "reportobjectid");

                entity.Property(e => e.LastLoadDate).HasColumnType("datetime");

                entity
                    .HasOne(d => d.ReportObject)
                    .WithMany(p => p.ReportObjectQueries)
                    .HasForeignKey(d => d.ReportObjectId)
                    .HasConstraintName("FK_ReportObjectQuery_ReportObject");
            });

            modelBuilder.Entity<ReportObjectRunDataBridge>(entity =>
            {
                entity.HasKey(e => e.BridgeId).HasName("PK__ReportOb__2D122135AFCD5E790");

                entity.ToTable("ReportObjectRunDataBridge", "dbo");

                entity.Property(x => x.Inherited).HasDefaultValue(0);

                entity
                    .HasIndex(e => e.ReportObjectId, "reportid_runid_runs")
                    .IncludeProperties(p => new { p.RunId, p.Runs });

                entity
                    .HasIndex(
                        e => new { e.ReportObjectId, e.Inherited },
                        "reportid_inheritied_runid_runs"
                    )
                    .IncludeProperties(p => new { p.RunId, p.Runs });

                entity
                    .HasOne(d => d.ReportObject)
                    .WithMany(p => p.ReportObjectRunDataBridges)
                    .HasForeignKey(d => d.ReportObjectId)
                    .HasPrincipalKey(k => k.ReportObjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ReportObj__Repor__7B2284FD");

                entity
                    .HasOne(d => d.RunData)
                    .WithMany(p => p.ReportObjectRunDataBridges)
                    .HasForeignKey(d => d.RunId)
                    .HasPrincipalKey(k => k.RunDataId)
                    .HasConstraintName("FK__ReportObj__Run__6A4E9564");
            });

            modelBuilder.Entity<ReportObjectRunData>(entity =>
            {
                entity.ToTable("ReportObjectRunData", "dbo");
                entity.HasKey(e => new { e.RunId });
                entity
                    .HasIndex(
                        e => new { e.RunUserId, e.RunStartTime },
                        "runstart_user_duration_status"
                    )
                    .IncludeProperties(
                        p =>
                            new
                            {
                                p.RunDataId,
                                p.RunDurationSeconds,
                                p.RunStatus
                            }
                    );
                entity
                    .HasIndex(
                        e => new { e.RunUserId, e.RunStartTime_Year },
                        "runstartyear_user_duration_status"
                    )
                    .IncludeProperties(
                        p =>
                            new
                            {
                                p.RunDataId,
                                p.RunDurationSeconds,
                                p.RunStatus
                            }
                    );
                entity
                    .HasIndex(
                        e => new { e.RunUserId, e.RunStartTime_Month },
                        "runstartmonth_user_duration_status"
                    )
                    .IncludeProperties(
                        p =>
                            new
                            {
                                p.RunDataId,
                                p.RunDurationSeconds,
                                p.RunStatus
                            }
                    );
                entity
                    .HasIndex(
                        e => new { e.RunUserId, e.RunStartTime_Day },
                        "runstartday_user_duration_status"
                    )
                    .IncludeProperties(
                        p =>
                            new
                            {
                                p.RunDataId,
                                p.RunDurationSeconds,
                                p.RunStatus
                            }
                    );
                entity
                    .HasIndex(
                        e => new { e.RunUserId, e.RunStartTime_Hour },
                        "runstarthour_user_duration_status"
                    )
                    .IncludeProperties(
                        p =>
                            new
                            {
                                p.RunDataId,
                                p.RunDurationSeconds,
                                p.RunStatus
                            }
                    );

                entity
                    .HasIndex(
                        e => new { e.RunStatus, e.RunStartTime },
                        "runstart_status_duration_user"
                    )
                    .IncludeProperties(
                        p =>
                            new
                            {
                                p.RunDataId,
                                p.RunDurationSeconds,
                                p.RunUserId
                            }
                    );
                entity
                    .HasIndex(
                        e => new { e.RunStatus, e.RunStartTime_Year },
                        "runstartyear_status_duration_user"
                    )
                    .IncludeProperties(
                        p =>
                            new
                            {
                                p.RunDataId,
                                p.RunDurationSeconds,
                                p.RunUserId
                            }
                    );
                entity
                    .HasIndex(
                        e => new { e.RunStatus, e.RunStartTime_Month },
                        "runstartmonth_status_duration_user"
                    )
                    .IncludeProperties(
                        p =>
                            new
                            {
                                p.RunDataId,
                                p.RunDurationSeconds,
                                p.RunUserId
                            }
                    );
                entity
                    .HasIndex(
                        e => new { e.RunStatus, e.RunStartTime_Day },
                        "runstartday_status_duration_user"
                    )
                    .IncludeProperties(
                        p =>
                            new
                            {
                                p.RunDataId,
                                p.RunDurationSeconds,
                                p.RunUserId
                            }
                    );
                entity
                    .HasIndex(
                        e => new { e.RunStatus, e.RunStartTime_Hour },
                        "runstarthour_status_duration_user"
                    )
                    .IncludeProperties(
                        p =>
                            new
                            {
                                p.RunDataId,
                                p.RunDurationSeconds,
                                p.RunUserId
                            }
                    );

                entity.Property(e => e.LastLoadDate).HasColumnType("datetime");

                entity.Property(e => e.RunStartTime).HasColumnType("datetime");

                entity.Property(e => e.RunStatus).HasMaxLength(100);

                entity.Property(e => e.RunUserId).HasColumnName("RunUserID");

                entity
                    .HasOne(d => d.RunUser)
                    .WithMany(p => p.ReportObjectRunDatas)
                    .HasForeignKey(d => d.RunUserId)
                    .HasConstraintName("FK__ReportObj__RunUs__7C16A936");
            });

            modelBuilder.Entity<ReportObjectSubscription>(entity =>
            {
                entity.ToTable("ReportObjectSubscriptions", "dbo");
                entity
                    .HasKey(e => e.ReportObjectSubscriptionsId)
                    .HasName("PK__ReportOb__1AA55D23FE572619");

                entity.HasIndex(e => new { e.ReportObjectId, e.UserId }, "reportid + userid");

                entity.Property(e => e.LastLoadDate).HasColumnType("datetime");

                entity.Property(e => e.LastRunTime).HasColumnType("datetime");

                entity
                    .HasOne(d => d.ReportObject)
                    .WithMany(p => p.ReportObjectSubscriptions)
                    .HasForeignKey(d => d.ReportObjectId)
                    .HasConstraintName("FK_ReportObjectSubscriptions_ReportObject");

                entity
                    .HasOne(d => d.User)
                    .WithMany(p => p.ReportObjectSubscriptions)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_ReportObjectSubscriptions_User");
            });

            modelBuilder.Entity<ReportObjectTag>(entity =>
            {
                entity.ToTable("ReportObjectTags", "dbo");
                entity.HasKey(e => e.TagId);

                entity.HasIndex(e => e.TagId, "tagid");

                entity.Property(e => e.TagId).HasColumnName("TagID");

                entity
                    .Property(e => e.EpicTagId)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("EpicTagID");

                entity.Property(e => e.TagName).HasMaxLength(200).IsUnicode(false);
            });

            modelBuilder.Entity<ReportObjectTagMembership>(entity =>
            {
                entity.ToTable("ReportObjectTagMemberships", "dbo");
                entity.HasKey(e => e.TagMembershipId);

                entity.HasIndex(e => new { e.ReportObjectId, e.TagId }, "tagid+reportid");

                entity.Property(e => e.TagMembershipId).HasColumnName("TagMembershipID");

                entity.Property(e => e.ReportObjectId).HasColumnName("ReportObjectID");

                entity.Property(e => e.TagId).HasColumnName("TagID");

                entity
                    .HasOne(d => d.ReportObject)
                    .WithMany(p => p.ReportObjectTagMemberships)
                    .HasForeignKey(d => d.ReportObjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReportObjectTagMemberships_ReportObject");

                entity
                    .HasOne(d => d.Tag)
                    .WithMany(p => p.ReportObjectTagMemberships)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReportObjectTagMemberships_ReportObjectTags");
            });

            modelBuilder.Entity<ReportObjectType>(entity =>
            {
                entity.ToTable("ReportObjectType", "dbo");

                entity.HasIndex(e => e.ReportObjectTypeId, "typeid");

                entity.Property(e => e.ReportObjectTypeId).HasColumnName("ReportObjectTypeID");

                entity.Property(e => e.DefaultEpicMasterFile).HasMaxLength(3);

                entity.Property(e => e.LastLoadDate).HasColumnType("datetime");

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Visible).HasMaxLength(1);
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => e.RolePermissionsId).HasName("PK__RolePerm__18B281E0AD8AB9C1");

                entity.ToTable("RolePermissions", "app");
            });

            modelBuilder.Entity<RolePermissionLink>(entity =>
            {
                entity
                    .HasKey(e => e.RolePermissionLinksId)
                    .HasName("PK__UserPerm__40D89C0194D8BDD6");

                entity.ToTable("RolePermissionLinks", "app");

                entity
                    .HasOne(d => d.Role)
                    .WithMany(p => p.RolePermissionLinks)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_RolePermissionLinks_UserRoles");

                entity
                    .HasOne(d => d.RolePermissions)
                    .WithMany(p => p.RolePermissionLinks)
                    .HasForeignKey(d => d.RolePermissionsId)
                    .HasConstraintName("FK_RolePermissionLinks_RolePermissions");
            });

            modelBuilder.Entity<SharedItem>(entity =>
            {
                entity.ToTable("SharedItems", "app");

                entity.HasIndex(
                    e =>
                        new
                        {
                            e.SharedFromUserId,
                            e.SharedToUserId,
                            e.ShareDate
                        },
                    "from + to + date"
                );

                entity.Property(e => e.ShareDate).HasColumnType("datetime");

                entity
                    .HasOne(d => d.SharedFromUser)
                    .WithMany(p => p.SharedItemSharedFromUsers)
                    .HasForeignKey(d => d.SharedFromUserId)
                    .HasConstraintName("FK_SharedItems_User");

                entity
                    .HasOne(d => d.SharedToUser)
                    .WithMany(p => p.SharedItemSharedToUsers)
                    .HasForeignKey(d => d.SharedToUserId)
                    .HasConstraintName("FK_SharedItems_User1");
            });

            modelBuilder.Entity<StarredCollection>(entity =>
            {
                entity.HasKey(e => e.StarId).HasName("PK__StarredC__88222DCEA294BB15");

                entity.ToTable("StarredCollections", "app");

                entity.HasIndex(e => new { e.Collectionid, e.Ownerid }, "collectionid + ownerid");

                entity.HasIndex(e => e.StarId, "starid").IsUnique();

                entity
                    .HasOne(d => d.Collection)
                    .WithMany(p => p.StarredCollections)
                    .HasForeignKey(d => d.Collectionid)
                    .HasConstraintName("FK_StarredCollections_DP_DataProject");

                entity
                    .HasOne(d => d.Folder)
                    .WithMany(p => p.StarredCollections)
                    .HasForeignKey(d => d.Folderid)
                    .HasConstraintName("FK_StarredCollections_UserFavoriteFolders");

                entity
                    .HasOne(d => d.Owner)
                    .WithMany(p => p.StarredCollections)
                    .HasForeignKey(d => d.Ownerid)
                    .HasConstraintName("FK_StarredCollections_User");
            });

            modelBuilder.Entity<StarredGroup>(entity =>
            {
                entity.HasKey(e => e.StarId).HasName("PK__StarredG__88222DCE76B4AC11");

                entity.ToTable("StarredGroups", "app");

                entity.HasIndex(e => e.Folderid, "folderid");

                entity.HasIndex(e => new { e.Groupid, e.Ownerid }, "groupid + ownerid");

                entity.HasIndex(e => e.StarId, "starid").IsUnique();

                entity
                    .HasOne(d => d.Folder)
                    .WithMany(p => p.StarredGroups)
                    .HasForeignKey(d => d.Folderid)
                    .HasConstraintName("FK_StarredGroups_UserFavoriteFolders");

                entity
                    .HasOne(d => d.Group)
                    .WithMany(p => p.StarredGroups)
                    .HasForeignKey(d => d.Groupid)
                    .HasConstraintName("FK_StarredGroups_UserGroups");

                entity
                    .HasOne(d => d.Owner)
                    .WithMany(p => p.StarredGroups)
                    .HasForeignKey(d => d.Ownerid)
                    .HasConstraintName("FK_StarredGroups_User");
            });

            modelBuilder.Entity<StarredInitiative>(entity =>
            {
                entity.HasKey(e => e.StarId).HasName("PK__StarredI__88222DCEFCC5A8E5");

                entity.ToTable("StarredInitiatives", "app");

                entity.HasIndex(e => e.Folderid, "folderid");

                entity.HasIndex(e => new { e.Initiativeid, e.Ownerid }, "initiativeid + ownerid");

                entity.HasIndex(e => e.StarId, "starid").IsUnique();

                entity
                    .HasOne(d => d.Folder)
                    .WithMany(p => p.StarredInitiatives)
                    .HasForeignKey(d => d.Folderid)
                    .HasConstraintName("FK_StarredInitiatives_UserFavoriteFolders");

                entity
                    .HasOne(d => d.Initiative)
                    .WithMany(p => p.StarredInitiatives)
                    .HasForeignKey(d => d.Initiativeid)
                    .HasConstraintName("FK_StarredInitiatives_DP_DataInitiative");

                entity
                    .HasOne(d => d.Owner)
                    .WithMany(p => p.StarredInitiatives)
                    .HasForeignKey(d => d.Ownerid)
                    .HasConstraintName("FK_StarredInitiatives_User");
            });

            modelBuilder.Entity<StarredReport>(entity =>
            {
                entity.HasKey(e => e.StarId).HasName("PK__StarredR__88222DCE157D560E");

                entity.ToTable("StarredReports", "app");

                entity.HasIndex(e => e.Folderid, "folderid");

                entity.HasIndex(e => new { e.Reportid, e.Ownerid }, "reportid + ownerid");

                entity.HasIndex(e => e.StarId, "starid").IsUnique();

                entity
                    .HasOne(d => d.Folder)
                    .WithMany(p => p.StarredReports)
                    .HasForeignKey(d => d.Folderid)
                    .HasConstraintName("FK_StarredReports_UserFavoriteFolders");

                entity
                    .HasOne(d => d.Owner)
                    .WithMany(p => p.StarredReports)
                    .HasForeignKey(d => d.Ownerid)
                    .HasConstraintName("FK_StarredReports_User");

                entity
                    .HasOne(d => d.Report)
                    .WithMany(p => p.StarredReports)
                    .HasForeignKey(d => d.Reportid)
                    .HasConstraintName("FK_StarredReports_ReportObject");
            });

            modelBuilder.Entity<StarredSearch>(entity =>
            {
                entity.HasKey(e => e.StarId).HasName("PK__StarredS__88222DCE6D6FEE1D");

                entity.ToTable("StarredSearches", "app");

                entity.HasIndex(e => e.Ownerid, "ownerid");

                entity.HasIndex(e => e.StarId, "starid").IsUnique();

                entity
                    .HasOne(d => d.Folder)
                    .WithMany(p => p.StarredSearches)
                    .HasForeignKey(d => d.Folderid)
                    .HasConstraintName("FK_StarredSearches_UserFavoriteFolders");

                entity
                    .HasOne(d => d.Owner)
                    .WithMany(p => p.StarredSearches)
                    .HasForeignKey(d => d.Ownerid)
                    .HasConstraintName("FK_StarredSearches_User");
            });

            modelBuilder.Entity<StarredTerm>(entity =>
            {
                entity.HasKey(e => e.StarId).HasName("PK__StarredT__88222DCE11EE382D");

                entity.ToTable("StarredTerms", "app");

                entity.HasIndex(e => e.Folderid, "folderid");

                entity.HasIndex(e => e.StarId, "starid").IsUnique();

                entity.HasIndex(e => new { e.Termid, e.Ownerid }, "termid + ownerid");

                entity
                    .HasOne(d => d.Folder)
                    .WithMany(p => p.StarredTerms)
                    .HasForeignKey(d => d.Folderid)
                    .HasConstraintName("FK_StarredTerms_UserFavoriteFolders");

                entity
                    .HasOne(d => d.Owner)
                    .WithMany(p => p.StarredTerms)
                    .HasForeignKey(d => d.Ownerid)
                    .HasConstraintName("FK_StarredTerms_User");

                entity
                    .HasOne(d => d.Term)
                    .WithMany(p => p.StarredTerms)
                    .HasForeignKey(d => d.Termid)
                    .HasConstraintName("FK_StarredTerms_Term");
            });

            modelBuilder.Entity<StarredUser>(entity =>
            {
                entity.HasKey(e => e.StarId).HasName("PK__StarredU__88222DCE446292A3");

                entity.ToTable("StarredUsers", "app");

                entity.HasIndex(e => e.Folderid, "folderid");

                entity.HasIndex(e => new { e.Userid, e.Ownerid }, "ownerid + userid");

                entity.HasIndex(e => e.StarId, "starid").IsUnique();

                entity
                    .HasOne(d => d.Folder)
                    .WithMany(p => p.StarredUsers)
                    .HasForeignKey(d => d.Folderid)
                    .HasConstraintName("FK_StarredUsers_UserFavoriteFolders");

                entity
                    .HasOne(d => d.Owner)
                    .WithMany(p => p.StarredUserOwners)
                    .HasForeignKey(d => d.Ownerid)
                    .HasConstraintName("FK_StarredUsers_User_owner");

                entity
                    .HasOne(d => d.User)
                    .WithMany(p => p.StarredUserUsers)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasForeignKey(d => d.Userid)
                    .HasConstraintName("FK_StarredUsers_User");
            });

            modelBuilder.Entity<StrategicImportance>(entity =>
            {
                entity.ToTable("StrategicImportance", "app");

                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<Term>(entity =>
            {
                entity.ToTable("Term", "app");

                entity.HasIndex(e => e.ApprovedYn, "approved");

                entity.HasIndex(e => e.ApprovedByUserId, "approvedby");

                entity.HasIndex(e => e.TermId, "termid").IsUnique();

                entity.HasIndex(e => e.UpdatedByUserId, "updatedby");

                entity.HasIndex(e => e.ValidFromDateTime, "validfrom");

                entity.Property(e => e.ApprovalDateTime).HasColumnType("datetime");

                entity
                    .Property(e => e.ApprovedYn)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("ApprovedYN")
                    .IsFixedLength();

                entity.Property(e => e.ExternalStandardUrl).HasMaxLength(4000);

                entity
                    .Property(e => e.HasExternalStandardYn)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("HasExternalStandardYN")
                    .IsFixedLength();

                entity.Property(e => e.LastUpdatedDateTime).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Summary).HasMaxLength(4000);

                entity.Property(e => e.ValidFromDateTime).HasColumnType("datetime");

                entity.Property(e => e.ValidToDateTime).HasColumnType("datetime");

                entity
                    .HasOne(d => d.ApprovedByUser)
                    .WithMany(p => p.TermApprovedByUsers)
                    .HasForeignKey(d => d.ApprovedByUserId)
                    .HasConstraintName("FK_Term_WebAppUsers1");

                entity
                    .HasOne(d => d.UpdatedByUser)
                    .WithMany(p => p.TermUpdatedByUsers)
                    .HasForeignKey(d => d.UpdatedByUserId)
                    .HasConstraintName("FK_Term_WebAppUsers");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "dbo");
                entity.HasKey(k => k.UserId);

                entity.HasIndex(e => e.UserId, "userid").IsUnique();

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");

                entity.Property(e => e.FirstnameCalc).HasColumnName("Firstname_calc");

                entity.Property(e => e.FullnameCalc).HasColumnName("Fullname_calc");

                entity.Property(e => e.LastLoadDate).HasColumnType("datetime");

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.Username).IsRequired();
            });

            modelBuilder.Entity<UserFavoriteFolder>(entity =>
            {
                entity.ToTable("UserFavoriteFolders", "app");

                entity.HasIndex(e => e.UserId, "userid");
            });

            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.ToTable("UserGroups", "dbo");
                entity.HasKey(e => e.GroupId);

                entity.HasIndex(e => e.GroupId, "groupid").IsUnique();

                entity.Property(e => e.LastLoadDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<UserGroupsMembership>(entity =>
            {
                entity.HasKey(e => e.MembershipId);

                entity.ToTable("UserGroupsMembership", "dbo");

                entity.HasIndex(e => e.GroupId, "groupdid + userid");

                entity.HasIndex(e => new { e.UserId, e.GroupId }, "userid+groupid");

                entity.Property(e => e.LastLoadDate).HasColumnType("datetime");

                entity
                    .HasOne(d => d.Group)
                    .WithMany(p => p.UserGroupsMemberships)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_UserGroupsMembership_UserGroups");

                entity
                    .HasOne(d => d.User)
                    .WithMany(p => p.UserGroupsMemberships)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserGroupsMembership_User");
            });

            modelBuilder.Entity<UserPreference>(entity =>
            {
                entity.ToTable("UserPreferences", "app");

                entity.HasIndex(
                    e =>
                        new
                        {
                            e.ItemValue,
                            e.ItemId,
                            e.UserId
                        },
                    "itemvalue + itemid + userid"
                );

                entity
                    .HasOne(d => d.User)
                    .WithMany(p => p.UserPreferences)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserPreferences_User");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.UserRolesId).HasName("PK__UserRole__41D8EF2F71AEFA0B");

                entity.ToTable("UserRoles", "app");

                entity.HasIndex(e => e.UserRolesId, "roleid");
            });

            modelBuilder.Entity<UserRoleLink>(entity =>
            {
                entity.HasKey(e => e.UserRoleLinksId).HasName("PK__UserRole__41D8EF2F662663D0");

                entity.ToTable("UserRoleLinks", "app");

                entity.HasIndex(e => new { e.UserId, e.UserRolesId }, "userid+roleid");

                entity
                    .HasOne(d => d.User)
                    .WithMany(p => p.UserRoleLinks)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserRoleLinks_User");

                entity
                    .HasOne(d => d.UserRoles)
                    .WithMany(p => p.UserRoleLinks)
                    .HasForeignKey(d => d.UserRolesId)
                    .HasConstraintName("FK_UserRoleLinks_UserRoles");
            });
            modelBuilder.Entity<GroupRoleLink>(entity =>
            {
                entity.HasKey(e => e.GroupRoleLinksId).HasName("PK__GroupRole__LinkId");

                entity.ToTable("GroupRoleLinks", "app");

                entity.HasIndex(e => new { e.GroupId, e.UserRolesId }, "groupid+roleid");

                entity
                    .HasOne(d => d.Group)
                    .WithMany(p => p.GroupRoleLinks)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_GroupRoleLinks_Group");

                entity
                    .HasOne(d => d.UserRoles)
                    .WithMany(p => p.GroupRoleLinks)
                    .HasForeignKey(d => d.UserRolesId)
                    .HasConstraintName("FK_GroupRoleLinks_UserRoles");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
