using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HHParser.Models
{
    public class VacancyEntry
    {
        public Vacancy Vacancy { get; set; }
        public Subcategory Subcategory { get; set; }
        public Category Category { get; set; }
    }
}
