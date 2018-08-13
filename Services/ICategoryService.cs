using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace HHParser.Models
{
    public interface ICategoryService
    {
        IQueryable<Category> Categories { get; }
        IQueryable<Category> FullCategories { get; }
        IQueryable<Subcategory> Subcategories { get; }
        IQueryable<Vacancy> Vacancies { get; }

        IQueryable<Category> FilteredCategories(string search_str = null);

        IEnumerable<Category> LoadCategoryList();
        IEnumerable<Subcategory> LoadSubcategoryList(Category category);
        IEnumerable<Vacancy> LoadVacancyList(Subcategory subcategory);

        void ResetVacancies();
    }
}
