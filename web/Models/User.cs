using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class User
    {
        public User()
        {
            Analytics = new HashSet<Analytic>();
            DpAgreementUserLastUpdateUserNavigations = new HashSet<DpAgreementUser>();
            DpAgreementUserUsers = new HashSet<DpAgreementUser>();
            DpAgreements = new HashSet<DpAgreement>();
            DpDataInitiativeExecutiveOwners = new HashSet<DpDataInitiative>();
            DpDataInitiativeLastUpdateUserNavigations = new HashSet<DpDataInitiative>();
            DpDataInitiativeOperationOwners = new HashSet<DpDataInitiative>();
            DpDataProjectAnalyticsOwners = new HashSet<DpDataProject>();
            DpDataProjectConversationMessages = new HashSet<DpDataProjectConversationMessage>();
            DpDataProjectDataManagers = new HashSet<DpDataProject>();
            DpDataProjectExecutiveOwners = new HashSet<DpDataProject>();
            DpDataProjectLastUpdateUserNavigations = new HashSet<DpDataProject>();
            DpDataProjectOperationOwners = new HashSet<DpDataProject>();
            DpMilestoneChecklistCompleteds = new HashSet<DpMilestoneChecklistCompleted>();
            DpMilestoneFrequencies = new HashSet<DpMilestoneFrequency>();
            DpMilestoneTaskLastUpdateUserNavigations = new HashSet<DpMilestoneTask>();
            DpMilestoneTaskOwners = new HashSet<DpMilestoneTask>();
            DpMilestoneTemplates = new HashSet<DpMilestoneTemplate>();
            MailDrafts = new HashSet<MailDraft>();
            MailFolders = new HashSet<MailFolder>();
            MailMessages = new HashSet<MailMessage>();
            MailRecipients = new HashSet<MailRecipient>();
            MailRecipientsDeleteds = new HashSet<MailRecipientsDeleted>();
            MaintenanceLogs = new HashSet<MaintenanceLog>();
            ReportObjectAuthorUsers = new HashSet<ReportObject>();
            ReportObjectConversationMessageDocs = new HashSet<ReportObjectConversationMessageDoc>();
            ReportObjectDocOperationalOwnerUsers = new HashSet<ReportObjectDoc>();
            ReportObjectDocRequesterNavigations = new HashSet<ReportObjectDoc>();
            ReportObjectDocUpdatedByNavigations = new HashSet<ReportObjectDoc>();
            ReportObjectLastModifiedByUsers = new HashSet<ReportObject>();
            ReportObjectRunData = new HashSet<ReportObjectRunDatum>();
            ReportObjectRunTimes = new HashSet<ReportObjectRunTime>();
            ReportObjectSubscriptions = new HashSet<ReportObjectSubscription>();
            ReportObjectTopRuns = new HashSet<ReportObjectTopRun>();
            SharedItemSharedFromUsers = new HashSet<SharedItem>();
            SharedItemSharedToUsers = new HashSet<SharedItem>();
            StarredCollections = new HashSet<StarredCollection>();
            StarredGroups = new HashSet<StarredGroup>();
            StarredInitiatives = new HashSet<StarredInitiative>();
            StarredReports = new HashSet<StarredReport>();
            StarredSearches = new HashSet<StarredSearch>();
            StarredTerms = new HashSet<StarredTerm>();
            StarredUserOwners = new HashSet<StarredUser>();
            StarredUserUsers = new HashSet<StarredUser>();
            TermApprovedByUsers = new HashSet<Term>();
            TermConversationMessages = new HashSet<TermConversationMessage>();
            TermUpdatedByUsers = new HashSet<Term>();
            UserFavorites = new HashSet<UserFavorite>();
            UserGroupsMemberships = new HashSet<UserGroupsMembership>();
            UserPreferences = new HashSet<UserPreference>();
            UserRoleLinks = new HashSet<UserRoleLink>();
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string EmployeeId { get; set; }
        public string AccountName { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public string Title { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Base { get; set; }
        public string EpicId { get; set; }
        public DateTime? LastLoadDate { get; set; }
        public DateTime? LastLogin { get; set; }
        public string FullnameCalc { get; set; }
        public string FirstnameCalc { get; set; }

        public virtual UserNameDatum UserNameDatum { get; set; }
        public virtual ICollection<Analytic> Analytics { get; set; }
        public virtual ICollection<DpAgreementUser> DpAgreementUserLastUpdateUserNavigations { get; set; }
        public virtual ICollection<DpAgreementUser> DpAgreementUserUsers { get; set; }
        public virtual ICollection<DpAgreement> DpAgreements { get; set; }
        public virtual ICollection<DpDataInitiative> DpDataInitiativeExecutiveOwners { get; set; }
        public virtual ICollection<DpDataInitiative> DpDataInitiativeLastUpdateUserNavigations { get; set; }
        public virtual ICollection<DpDataInitiative> DpDataInitiativeOperationOwners { get; set; }
        public virtual ICollection<DpDataProject> DpDataProjectAnalyticsOwners { get; set; }
        public virtual ICollection<DpDataProjectConversationMessage> DpDataProjectConversationMessages { get; set; }
        public virtual ICollection<DpDataProject> DpDataProjectDataManagers { get; set; }
        public virtual ICollection<DpDataProject> DpDataProjectExecutiveOwners { get; set; }
        public virtual ICollection<DpDataProject> DpDataProjectLastUpdateUserNavigations { get; set; }
        public virtual ICollection<DpDataProject> DpDataProjectOperationOwners { get; set; }
        public virtual ICollection<DpMilestoneChecklistCompleted> DpMilestoneChecklistCompleteds { get; set; }
        public virtual ICollection<DpMilestoneFrequency> DpMilestoneFrequencies { get; set; }
        public virtual ICollection<DpMilestoneTask> DpMilestoneTaskLastUpdateUserNavigations { get; set; }
        public virtual ICollection<DpMilestoneTask> DpMilestoneTaskOwners { get; set; }
        public virtual ICollection<DpMilestoneTemplate> DpMilestoneTemplates { get; set; }
        public virtual ICollection<MailDraft> MailDrafts { get; set; }
        public virtual ICollection<MailFolder> MailFolders { get; set; }
        public virtual ICollection<MailMessage> MailMessages { get; set; }
        public virtual ICollection<MailRecipient> MailRecipients { get; set; }
        public virtual ICollection<MailRecipientsDeleted> MailRecipientsDeleteds { get; set; }
        public virtual ICollection<MaintenanceLog> MaintenanceLogs { get; set; }
        public virtual ICollection<ReportObject> ReportObjectAuthorUsers { get; set; }
        public virtual ICollection<ReportObjectConversationMessageDoc> ReportObjectConversationMessageDocs { get; set; }
        public virtual ICollection<ReportObjectDoc> ReportObjectDocOperationalOwnerUsers { get; set; }
        public virtual ICollection<ReportObjectDoc> ReportObjectDocRequesterNavigations { get; set; }
        public virtual ICollection<ReportObjectDoc> ReportObjectDocUpdatedByNavigations { get; set; }
        public virtual ICollection<ReportObject> ReportObjectLastModifiedByUsers { get; set; }
        public virtual ICollection<ReportObjectRunDatum> ReportObjectRunData { get; set; }
        public virtual ICollection<ReportObjectRunTime> ReportObjectRunTimes { get; set; }
        public virtual ICollection<ReportObjectSubscription> ReportObjectSubscriptions { get; set; }
        public virtual ICollection<ReportObjectTopRun> ReportObjectTopRuns { get; set; }
        public virtual ICollection<SharedItem> SharedItemSharedFromUsers { get; set; }
        public virtual ICollection<SharedItem> SharedItemSharedToUsers { get; set; }
        public virtual ICollection<StarredCollection> StarredCollections { get; set; }
        public virtual ICollection<StarredGroup> StarredGroups { get; set; }
        public virtual ICollection<StarredInitiative> StarredInitiatives { get; set; }
        public virtual ICollection<StarredReport> StarredReports { get; set; }
        public virtual ICollection<StarredSearch> StarredSearches { get; set; }
        public virtual ICollection<StarredTerm> StarredTerms { get; set; }
        public virtual ICollection<StarredUser> StarredUserOwners { get; set; }
        public virtual ICollection<StarredUser> StarredUserUsers { get; set; }
        public virtual ICollection<Term> TermApprovedByUsers { get; set; }
        public virtual ICollection<TermConversationMessage> TermConversationMessages { get; set; }
        public virtual ICollection<Term> TermUpdatedByUsers { get; set; }
        public virtual ICollection<UserFavorite> UserFavorites { get; set; }
        public virtual ICollection<UserGroupsMembership> UserGroupsMemberships { get; set; }
        public virtual ICollection<UserPreference> UserPreferences { get; set; }
        public virtual ICollection<UserRoleLink> UserRoleLinks { get; set; }
    }
}
