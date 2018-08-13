using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HHParser.Models;

namespace HHParser.Infrastructure
{
    public interface IHHLoader
    {
        List<Category> LoadCategoryList();
        List<Subcategory> LoadSubcategoryList(Category category);
        List<Vacancy> LoadVacancyList(Subcategory subcategory);
    }
}
