namespace Atlas_Web.Models
{
    public partial class MailFolder
    {
        public MailFolder()
        {
            MailFolderMessages = new HashSet<MailFolderMessage>();
        }

        public int FolderId { get; set; }
        public int? ParentFolderId { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
        public int? Rank { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<MailFolderMessage> MailFolderMessages { get; set; }
    }
}
