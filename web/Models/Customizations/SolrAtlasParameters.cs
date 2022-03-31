using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas_Web.Models
{
    [NotMapped]
    public class SolrAtlasParameters
    {
        public SolrAtlasParameters() { }

        public string Query { get; set; }
        public int? PageIndex { get; set; }

        public Dictionary<string, string> Filters { get; set; } = new();

        private int PageSize = 20;

        public int EffectivePageIndex => PageIndex ?? 1;
        public int Start => (EffectivePageIndex - 1) * PageSize;

        public IReadOnlyList<string> FilteredFields =>
            Filters?.Select(f => f.Key).ToArray() ?? Array.Empty<string>();

        public override string ToString() =>
            $"{nameof(Query)}: {Query}, "
            + $"{nameof(PageIndex)}: {PageIndex}, "
            + $"{nameof(Filters)}: {string.Join(",", Filters.Select(f => $"{f.Key}={f.Value}"))}";
    }
}
