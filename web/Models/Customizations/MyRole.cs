using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas_Web.Models
{
    [NotMapped]
    public class MyRole
    {
        public string Url { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
