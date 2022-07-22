namespace Atlas_Web.Models
{
    public partial class UserFavoriteFolder
    {
        public UserFavoriteFolder()
        {
            StarredCollections = new HashSet<StarredCollection>();
            StarredGroups = new HashSet<StarredGroup>();
            StarredInitiatives = new HashSet<StarredInitiative>();
            StarredReports = new HashSet<StarredReport>();
            StarredSearches = new HashSet<StarredSearch>();
            StarredTerms = new HashSet<StarredTerm>();
            StarredUsers = new HashSet<StarredUser>();
        }

        public int UserFavoriteFolderId { get; set; }
        public string FolderName { get; set; }
        public int? UserId { get; set; }
        public int? FolderRank { get; set; }

        public virtual ICollection<StarredCollection> StarredCollections { get; set; }
        public virtual ICollection<StarredGroup> StarredGroups { get; set; }
        public virtual ICollection<StarredInitiative> StarredInitiatives { get; set; }
        public virtual ICollection<StarredReport> StarredReports { get; set; }
        public virtual ICollection<StarredSearch> StarredSearches { get; set; }
        public virtual ICollection<StarredTerm> StarredTerms { get; set; }
        public virtual ICollection<StarredUser> StarredUsers { get; set; }
    }
}
