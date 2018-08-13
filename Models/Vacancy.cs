using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace HHParser.Models
{
    public class Vacancy
    {
        public long Id { get; set; }

        [JsonProperty("Title")]
        [Required]
        public string Title { get; set; }

        [DataType(DataType.Url)]
        [JsonProperty("Url")]
        public Uri Url { get; set; }

        [JsonProperty("InitOffer")]
        public int? InitOffer { get; set; }

        [JsonProperty("FinalOffer")]
        public int? FinalOffer { get; set; }

        [JsonIgnore]
        public Subcategory Subcategory { get; set; }
    }
}
