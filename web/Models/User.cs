using Microsoft.AspNetCore.Identity;

namespace Atlas_Web.Models
{
    public partial class User //: IdentityUser
    {
        public User()
        {
            Analytics = new HashSet<Analytic>();
            AnalyticsTraces = new HashSet<AnalyticsTrace>();
            AnalyticsErrors = new HashSet<AnalyticsError>();
            DpDataInitiativeExecutiveOwners = new HashSet<Initiative>();
            DpDataInitiativeLastUpdateUserNavigations = new HashSet<Initiative>();
            DpDataInitiativeOperationOwners = new HashSet<Initiative>();
            DpDataProjectAnalyticsOwners = new HashSet<Collection>();
            DpDataProjectDataManagers = new HashSet<Collection>();
            DpDataProjectExecutiveOwners = new HashSet<Collection>();
            DpDataProjectLastUpdateUserNavigations = new HashSet<Collection>();
            DpDataProjectOperationOwners = new HashSet<Collection>();
            MailDrafts = new HashSet<MailDraft>();
            MailFolders = new HashSet<MailFolder>();
            MailMessages = new HashSet<MailMessage>();
            MailRecipients = new HashSet<MailRecipient>();
            MailRecipientsDeleteds = new HashSet<MailRecipientsDeleted>();
            MaintenanceLogs = new HashSet<MaintenanceLog>();
            ReportObjectAuthorUsers = new HashSet<ReportObject>();
            ReportObjectDocOperationalOwnerUsers = new HashSet<ReportObjectDoc>();
            ReportObjectDocRequesterNavigations = new HashSet<ReportObjectDoc>();
            ReportObjectDocUpdatedByNavigations = new HashSet<ReportObjectDoc>();
            ReportObjectLastModifiedByUsers = new HashSet<ReportObject>();
            ReportObjectRunDatas = new HashSet<ReportObjectRunData>();
            ReportObjectSubscriptions = new HashSet<ReportObjectSubscription>();
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
            TermUpdatedByUsers = new HashSet<Term>();
            UserGroupsMemberships = new HashSet<UserGroupsMembership>();
            UserPreferences = new HashSet<UserPreference>();
            UserRoleLinks = new HashSet<UserRoleLink>();
            UserSettings = new HashSet<UserSetting>();
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
        public string ProfilePhoto { get; set; }
        public DateTime? LastLoadDate { get; set; }
        public DateTime? LastLogin { get; set; }
        public string FullnameCalc { get; set; }
        public string FirstnameCalc { get; set; }
        public virtual ICollection<Analytic> Analytics { get; set; }
        public virtual ICollection<AnalyticsTrace> AnalyticsTraces { get; set; }
        public virtual ICollection<AnalyticsError> AnalyticsErrors { get; set; }
        public virtual ICollection<Initiative> DpDataInitiativeExecutiveOwners { get; set; }
        public virtual ICollection<Initiative> DpDataInitiativeLastUpdateUserNavigations { get; set; }
        public virtual ICollection<Initiative> DpDataInitiativeOperationOwners { get; set; }
        public virtual ICollection<Collection> DpDataProjectAnalyticsOwners { get; set; }
        public virtual ICollection<Collection> DpDataProjectDataManagers { get; set; }
        public virtual ICollection<Collection> DpDataProjectExecutiveOwners { get; set; }
        public virtual ICollection<Collection> DpDataProjectLastUpdateUserNavigations { get; set; }
        public virtual ICollection<Collection> DpDataProjectOperationOwners { get; set; }
        public virtual ICollection<MailDraft> MailDrafts { get; set; }
        public virtual ICollection<MailFolder> MailFolders { get; set; }
        public virtual ICollection<MailMessage> MailMessages { get; set; }
        public virtual ICollection<MailRecipient> MailRecipients { get; set; }
        public virtual ICollection<MailRecipientsDeleted> MailRecipientsDeleteds { get; set; }
        public virtual ICollection<MaintenanceLog> MaintenanceLogs { get; set; }
        public virtual ICollection<ReportObject> ReportObjectAuthorUsers { get; set; }
        public virtual ICollection<ReportObjectDoc> ReportObjectDocOperationalOwnerUsers { get; set; }
        public virtual ICollection<ReportObjectDoc> ReportObjectDocRequesterNavigations { get; set; }
        public virtual ICollection<ReportObjectDoc> ReportObjectDocUpdatedByNavigations { get; set; }
        public virtual ICollection<ReportObject> ReportObjectLastModifiedByUsers { get; set; }
        public virtual ICollection<ReportObjectRunData> ReportObjectRunDatas { get; set; }
        public virtual ICollection<ReportObjectSubscription> ReportObjectSubscriptions { get; set; }
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
        public virtual ICollection<Term> TermUpdatedByUsers { get; set; }
        public virtual ICollection<UserGroupsMembership> UserGroupsMemberships { get; set; }
        public virtual ICollection<UserPreference> UserPreferences { get; set; }
        public virtual ICollection<UserRoleLink> UserRoleLinks { get; set; }
        public virtual ICollection<UserSetting> UserSettings { get; set; }
    }
}
