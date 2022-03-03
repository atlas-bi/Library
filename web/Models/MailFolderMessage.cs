using System;
using System.Collections.Generic;

namespace Atlas_Web.Models
{
    public partial class MailFolderMessage
    {
        public int Id { get; set; }
        public int? FolderId { get; set; }
        public int? MessageId { get; set; }

        public virtual MailFolder Folder { get; set; }
        public virtual MailMessage Message { get; set; }
    }
}
