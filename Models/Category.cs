using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using HtmlAgilityPack;

namespace HHParser.Models
{
    public class Category
    {
        public long Id { get; set; }

        [JsonProperty("Slug")]
        public string Slug { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; } 

        [DataType(DataType.Url)]
        [JsonProperty("Url")]
        public Uri Url { get; set; }

        [JsonProperty("Subcategories")]
        public ICollection<Subcategory> Subcategories { get; set; }
    }
}
