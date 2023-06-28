using System;
using System.Collections.Generic;
using System.Linq;

namespace Atlas_Web.Models
{
    public class ResultModel
    {
        public ReportObject report { get; set; }
        public Collection collection { get; set; }
        public Initiative initiative { get; set; }
        public Term term { get; set; }
        public User user { get; set; }
        public UserGroup group { get; set; }
        public string Id { get; set; }
    };

    public record SearchResult(
        List<ReportObjectAttachment> AttachedFiles,
        SolrAtlas Result,
        string RunUrl
    );

    public record FilterFields(string Key, string FriendlyName);

    public record HighlightValueModel(string Key, string Value)
    {
        public string FriendlyName =>
            System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(
                Key.Replace("_", " ").Replace("text", "").Trim()
            );
    };

    public record FacetValueModel(string Value, int Count)
    {
        public string FriendlyName => FriendlyNameBuilder(Value);

        private static string FriendlyNameBuilder(string Name)
        {
            string NewName =
                System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(
                    Name.Replace("_", " ").Replace("text", "").Trim()
                );
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

    public record HighlightModel(string Key, IReadOnlyList<HighlightValueModel> Values);

    public record FacetModel(string Key, IReadOnlyList<FacetValueModel> Values)
    {
        public string FriendlyName =>
            System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(
                Key.Replace("_", " ").Replace("text", "").Trim()
            );
    };

    public record SolrAtlasResults(
        IReadOnlyList<ResultModel> Results,
        IReadOnlyList<FacetModel> FacetFields,
        IReadOnlyList<HighlightModel> Highlights,
        IReadOnlyList<FilterFields> FilterFields,
        int NumFound,
        int QTime,
        SolrAtlasParameters Parameters,
        string Advanced
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
