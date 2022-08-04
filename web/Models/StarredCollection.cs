namespace Atlas_Web.Models
{
    public partial class StarredCollection
    {
        public int StarId { get; set; }
        public int? Rank { get; set; }
        public int Collectionid { get; set; }
        public int Ownerid { get; set; }
        public int? Folderid { get; set; }

        public virtual Collection Collection { get; set; }
        public virtual UserFavoriteFolder Folder { get; set; }
        public virtual User Owner { get; set; }
    }
}
