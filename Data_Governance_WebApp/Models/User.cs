using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class User
    {
        public User()
        {
            Analytics = new HashSet<Analytics>();
            DpAgreement = new HashSet<DpAgreement>();
            DpAgreementUsersLastUpdateUserNavigation = new HashSet<DpAgreementUsers>();
            DpAgreementUsersUser = new HashSet<DpAgreementUsers>();
            DpDataInitiativeExecutiveOwner = new HashSet<DpDataInitiative>();
            DpDataInitiativeLastUpdateUserNavigation = new HashSet<DpDataInitiative>();
            DpDataInitiativeOperationOwner = new HashSet<DpDataInitiative>();
            DpDataProjectAnalyticsOwner = new HashSet<DpDataProject>();
            DpDataProjectConversationMessage = new HashSet<DpDataProjectConversationMessage>();
            DpDataProjectDataManager = new HashSet<DpDataProject>();
            DpDataProjectExecutiveOwner = new HashSet<DpDataProject>();
            DpDataProjectLastUpdateUserNavigation = new HashSet<DpDataProject>();
            DpDataProjectOperationOwner = new HashSet<DpDataProject>();
            DpMilestoneChecklistCompleted = new HashSet<DpMilestoneChecklistCompleted>();
            DpMilestoneFrequency = new HashSet<DpMilestoneFrequency>();
            DpMilestoneTasksLastUpdateUserNavigation = new HashSet<DpMilestoneTasks>();
            DpMilestoneTasksOwner = new HashSet<DpMilestoneTasks>();
            DpMilestoneTemplates = new HashSet<DpMilestoneTemplates>();
            MailDrafts = new HashSet<MailDrafts>();
            MailFolders = new HashSet<MailFolders>();
            MailMessages = new HashSet<MailMessages>();
            MailRecipients = new HashSet<MailRecipients>();
            MailRecipientsDeleted = new HashSet<MailRecipientsDeleted>();
            MaintenanceLog = new HashSet<MaintenanceLog>();
            ReportObjectAuthorUser = new HashSet<ReportObject>();
            ReportObjectConversationMessageDoc = new HashSet<ReportObjectConversationMessageDoc>();
            ReportObjectDocOperationalOwnerUser = new HashSet<ReportObjectDoc>();
            ReportObjectDocRequesterNavigation = new HashSet<ReportObjectDoc>();
            ReportObjectDocUpdatedByNavigation = new HashSet<ReportObjectDoc>();
            ReportObjectLastModifiedByUser = new HashSet<ReportObject>();
            ReportObjectRunData = new HashSet<ReportObjectRunData>();
            ReportObjectRunTime = new HashSet<ReportObjectRunTime>();
            ReportObjectSubscriptions = new HashSet<ReportObjectSubscriptions>();
            ReportObjectTopRuns = new HashSet<ReportObjectTopRuns>();
            SharedItemsSharedFromUser = new HashSet<SharedItems>();
            SharedItemsSharedToUser = new HashSet<SharedItems>();
            TermApprovedByUser = new HashSet<Term>();
            TermConversationMessage = new HashSet<TermConversationMessage>();
            TermUpdatedByUser = new HashSet<Term>();
            UserFavorites = new HashSet<UserFavorites>();
            UserGroupsMembership = new HashSet<UserGroupsMembership>();
            UserPreferences = new HashSet<UserPreferences>();
            UserRoleLinks = new HashSet<UserRoleLinks>();
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

        public virtual UserNameData UserNameData { get; set; }
        public virtual ICollection<Analytics> Analytics { get; set; }
        public virtual ICollection<DpAgreement> DpAgreement { get; set; }
        public virtual ICollection<DpAgreementUsers> DpAgreementUsersLastUpdateUserNavigation { get; set; }
        public virtual ICollection<DpAgreementUsers> DpAgreementUsersUser { get; set; }
        public virtual ICollection<DpDataInitiative> DpDataInitiativeExecutiveOwner { get; set; }
        public virtual ICollection<DpDataInitiative> DpDataInitiativeLastUpdateUserNavigation { get; set; }
        public virtual ICollection<DpDataInitiative> DpDataInitiativeOperationOwner { get; set; }
        public virtual ICollection<DpDataProject> DpDataProjectAnalyticsOwner { get; set; }
        public virtual ICollection<DpDataProjectConversationMessage> DpDataProjectConversationMessage { get; set; }
        public virtual ICollection<DpDataProject> DpDataProjectDataManager { get; set; }
        public virtual ICollection<DpDataProject> DpDataProjectExecutiveOwner { get; set; }
        public virtual ICollection<DpDataProject> DpDataProjectLastUpdateUserNavigation { get; set; }
        public virtual ICollection<DpDataProject> DpDataProjectOperationOwner { get; set; }
        public virtual ICollection<DpMilestoneChecklistCompleted> DpMilestoneChecklistCompleted { get; set; }
        public virtual ICollection<DpMilestoneFrequency> DpMilestoneFrequency { get; set; }
        public virtual ICollection<DpMilestoneTasks> DpMilestoneTasksLastUpdateUserNavigation { get; set; }
        public virtual ICollection<DpMilestoneTasks> DpMilestoneTasksOwner { get; set; }
        public virtual ICollection<DpMilestoneTemplates> DpMilestoneTemplates { get; set; }
        public virtual ICollection<MailDrafts> MailDrafts { get; set; }
        public virtual ICollection<MailFolders> MailFolders { get; set; }
        public virtual ICollection<MailMessages> MailMessages { get; set; }
        public virtual ICollection<MailRecipients> MailRecipients { get; set; }
        public virtual ICollection<MailRecipientsDeleted> MailRecipientsDeleted { get; set; }
        public virtual ICollection<MaintenanceLog> MaintenanceLog { get; set; }
        public virtual ICollection<ReportObject> ReportObjectAuthorUser { get; set; }
        public virtual ICollection<ReportObjectConversationMessageDoc> ReportObjectConversationMessageDoc { get; set; }
        public virtual ICollection<ReportObjectDoc> ReportObjectDocOperationalOwnerUser { get; set; }
        public virtual ICollection<ReportObjectDoc> ReportObjectDocRequesterNavigation { get; set; }
        public virtual ICollection<ReportObjectDoc> ReportObjectDocUpdatedByNavigation { get; set; }
        public virtual ICollection<ReportObject> ReportObjectLastModifiedByUser { get; set; }
        public virtual ICollection<ReportObjectRunData> ReportObjectRunData { get; set; }
        public virtual ICollection<ReportObjectRunTime> ReportObjectRunTime { get; set; }
        public virtual ICollection<ReportObjectSubscriptions> ReportObjectSubscriptions { get; set; }
        public virtual ICollection<ReportObjectTopRuns> ReportObjectTopRuns { get; set; }
        public virtual ICollection<SharedItems> SharedItemsSharedFromUser { get; set; }
        public virtual ICollection<SharedItems> SharedItemsSharedToUser { get; set; }
        public virtual ICollection<Term> TermApprovedByUser { get; set; }
        public virtual ICollection<TermConversationMessage> TermConversationMessage { get; set; }
        public virtual ICollection<Term> TermUpdatedByUser { get; set; }
        public virtual ICollection<UserFavorites> UserFavorites { get; set; }
        public virtual ICollection<UserGroupsMembership> UserGroupsMembership { get; set; }
        public virtual ICollection<UserPreferences> UserPreferences { get; set; }
        public virtual ICollection<UserRoleLinks> UserRoleLinks { get; set; }
    }
}
