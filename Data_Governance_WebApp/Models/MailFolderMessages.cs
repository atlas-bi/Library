using System;
using System.Collections.Generic;

namespace Data_Governance_WebApp.Models
{
    public partial class MailFolderMessages
    {
        public int Id { get; set; }
        public int? FolderId { get; set; }
        public int? MessageId { get; set; }

        public virtual MailFolders Folder { get; set; }
        public virtual MailMessages Message { get; set; }
    }
}
