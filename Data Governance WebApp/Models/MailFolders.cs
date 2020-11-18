using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class MailFolders
    {
        public MailFolders()
        {
            MailFolderMessages = new HashSet<MailFolderMessages>();
        }

        public int FolderId { get; set; }
        public int? ParentFolderId { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
        public int? Rank { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<MailFolderMessages> MailFolderMessages { get; set; }
    }
}
