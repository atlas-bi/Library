using SolrNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas_Web.Models
{
    [NotMapped]
    public class SolrAtlasLookups
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }

        [SolrField("item_name")]
        public string Name { get; set; }

        [SolrField("item_type")]
        public string Type { get; set; }

        [SolrField("atlas_id")]
        public int AtlasId { get; set; }
    }
}
