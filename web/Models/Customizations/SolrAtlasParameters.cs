using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Atlas_Web.Models
{
    [NotMapped]
    public class SolrAtlasParameters
    {
        public string Query { get; set; }
        public int? PageIndex { get; set; }

        public Dictionary<string, string> Filters { get; set; } = new();

        public readonly int PageSize = 20;

        public int EffectivePageIndex => PageIndex ?? 1;
        public int Start => (EffectivePageIndex - 1) * PageSize;

#pragma warning disable S2365
        public IReadOnlyList<string> FilteredFields =>
            Filters?.Select(f => f.Key).ToArray() ?? Array.Empty<string>();
#pragma warning restore S2365
        public override string ToString() =>
            $"{nameof(Query)}: {Query}, "
            + $"{nameof(PageIndex)}: {PageIndex}, "
            + $"{nameof(Filters)}: {string.Join(",", Filters.Select(f => $"{f.Key}={f.Value}"))}";
    }
}
