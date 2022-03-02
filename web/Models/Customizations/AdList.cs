using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas_Web.Models
{
    [NotMapped]
    public class AdList
    {
        public string Url { get; set; }
        public int Column { get; set; }
        public string Params { get; set; }
    }
}
