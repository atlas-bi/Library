namespace Atlas_Web.Models
{
    public partial class MailRecipient
    {
        public int Id { get; set; }
        public int? MessageId { get; set; }
        public int? ToUserId { get; set; }
        public DateTime? ReadDate { get; set; }
        public int? AlertDisplayed { get; set; }
        public int? ToGroupId { get; set; }

        public virtual MailMessage Message { get; set; }
        public virtual UserGroup ToGroup { get; set; }
        public virtual User ToUser { get; set; }
    }
}
