using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace HHParser.Models
{
    public class Subcategory
    {
        public long Id { get; set; }

        [JsonProperty("Slug")]
        public string Slug { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [DataType(DataType.Url)]
        [JsonProperty("Url")]
        public Uri Url { get; set; }

        [JsonProperty("Vacancies")]
        public ICollection<Vacancy> Vacancies { get; set; }

        [JsonIgnore]
        public Category Category { get; set; }
    }
}
