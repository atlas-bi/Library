namespace Atlas_Web.Models
{
    public partial class CollectionTerm
    {
        public int LinkId { get; set; }
        public int TermId { get; set; }
        public int CollectionId { get; set; }
        public int? Rank { get; set; }

        public virtual Collection DataProject { get; set; }
        public virtual Term Term { get; set; }
    }
}
