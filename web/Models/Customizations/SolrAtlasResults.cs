using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;


namespace Atlas_Web.Models
{

    public record FacetValueModel(
        string Value,
        int Count
    )
    {
        public string FriendlyName => FriendlyNameBuilder(Value);

        private static string FriendlyNameBuilder(string Name)
        {
            string NewName = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(Name.Replace("_", " ").Replace("text", "").Trim());
            if (NewName == "N")
            {
                return "No";
            }
            else if (NewName == "Y")
            {
                return "Yes";
            }
            return NewName;
        }
    };

    public record FacetModel(
        string Key,
        IReadOnlyList<FacetValueModel> Values
    )
    {
        public string FriendlyName => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(Key.Replace("_", " ").Replace("text", "").Trim());
    };

    public record SolrAtlasResults(
            IReadOnlyList<SolrAtlas> Results,
            IReadOnlyList<FacetModel> FacetFields,

            int NumFound,
            int QTime,
            SolrAtlasParameters Parameters
        )
    {
        const int PageSlide = 2;

        public IEnumerable<int> Pages
        {
            get
            {
                var pageCount = LastPage;
                var pageFrom = Math.Max(1, Parameters.EffectivePageIndex - PageSlide);
                var pageTo = Math.Min(pageCount - 1, Parameters.EffectivePageIndex + PageSlide);
                pageFrom = Math.Max(1, Math.Min(pageTo - 2 * PageSlide, pageFrom));
                pageTo = Math.Min(pageCount, Math.Max(pageFrom + 2 * PageSlide, pageTo));
                return Enumerable.Range(pageFrom, pageTo - pageFrom + 1);
            }
        }

        public int LastPage => (int)Math.Floor(((decimal)NumFound - 1) / Parameters.PageSize) + 1;

    }

}
