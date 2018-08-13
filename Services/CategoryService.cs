using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using LinqKit;

using HHParser.Infrastructure;

namespace HHParser.Models
{

    public class CategoryService : ICategoryService
    {
        private HHDatabaseContext context;
        private IHHLoader hhLoader;

        public CategoryService(HHDatabaseContext ctx, IHHLoader ldr)
        {
            context = ctx;
            hhLoader = ldr;
        }

        public IQueryable<Category> Categories => context.Categories;
        public IQueryable<Subcategory> Subcategories => context.Subcategories;
        public IQueryable<Vacancy> Vacancies => context.Vacancies;

        public IQueryable<Category> FullCategories => context.Categories
                    .Include(category => category.Subcategories)
                        .ThenInclude(subcategory => subcategory.Vacancies);

        public IQueryable<Category> FilteredCategories(string search_str = null)
        {
            var filters = PredicateBuilder.New<VacancyEntry>(true);

            if (!String.IsNullOrEmpty(search_str))
            {
                filters.And(entity => entity.Vacancy.Title.Contains(search_str));
            }

            List<VacancyEntry> filteredFlatVacancyList = context.Vacancies
                .Join(
                    context.Subcategories,
                    vac => vac.Subcategory.Id,
                    subcat => subcat.Id,
                    (vac, subcat) => new { vac, subcat }
                )
                .Join(
                    context.Categories,
                    vacsubcat => vacsubcat.subcat.Category.Id,
                    cat => cat.Id,
                    (vacsubcat, cat) => new VacancyEntry
                    {
                        Vacancy = vacsubcat.vac,
                        Subcategory = vacsubcat.subcat,
                        Category = cat
                    }
                 )
                 .Where(filters)
                 .ToList(); 

            var filteredVacancyTree = HeapifyVacancyList(filteredFlatVacancyList);
          
            return filteredVacancyTree.AsQueryable();
        }

        private IEnumerable<VacancyEntry> FlatVacancyList(IEnumerable<Category> categories)
        {
            /*
                Строит плоский список вакансий из древовидного
             */
            return from category in categories
                   from subcat in category.Subcategories
                   from vac in subcat.Vacancies
                   select new VacancyEntry
                   {
                       Category = category,
                       Subcategory = subcat,
                       Vacancy = vac
                   };
        }

        private ICollection<Category> HeapifyVacancyList(IEnumerable<VacancyEntry> vacancyList)
        {
            /*
                Строит древовидный список вакансий из вложенного
             */
            return (
                       from VacancyEntry entry in vacancyList
                       group entry by entry.Category into category_list
                       select new Category
                       {
                           Id = category_list.First().Category.Id,
                           Title = category_list.First().Category.Title,
                           Url = category_list.First().Category.Url,

                           Subcategories = (
                               from subcat in category_list
                               group subcat by subcat.Subcategory into subcategory_list
                               select new Subcategory
                               {
                                   Id = subcategory_list.First().Subcategory.Id,
                                   Title = subcategory_list.First().Subcategory.Title,
                                   Url = subcategory_list.First().Subcategory.Url,
                                   Vacancies = (
                                     from vac in subcategory_list
                                     select new Vacancy
                                     {
                                         Id = vac.Vacancy.Id,
                                         Title = vac.Vacancy.Title,
                                         Url = vac.Vacancy.Url,
                                         InitOffer = vac.Vacancy.InitOffer,
                                         FinalOffer = vac.Vacancy.FinalOffer,

                                     }
                                   ).ToList()
                               }
                           ).ToList()


                       }).ToList();
        }

        public void Clear()
        {
            context.Vacancies.RemoveRange(context.Vacancies);
            context.Subcategories.RemoveRange(context.Subcategories);
            context.Categories.RemoveRange(context.Categories);

            context.SaveChanges();
        }

        public ICollection<Category> FilterVacancyList(IEnumerable<Category> categories, double deviation = .3f)
        {
            /*
                отбрасывает вакансии, для которых начальное И конечное предложение отличается от средней по 
                подкатегории в deviation раз
             */

            IEnumerable<VacancyEntry> noNullVacancies = FlatVacancyList(categories);
            List<VacancyEntry> filtered = new List<VacancyEntry>();
            foreach (var g_subcat in noNullVacancies.GroupBy(e => e.Subcategory.Title))
            {
                var filters = PredicateBuilder.New<VacancyEntry>(true);
                
                List<float> initOffer = g_subcat
                    .Where(entry => entry.Vacancy.InitOffer.HasValue)
                    .Select(entry => (float)entry.Vacancy.InitOffer.Value).ToList();
                float initOfferAvg = initOffer.DefaultIfEmpty(0).Average();

                List<float> finalOffer = g_subcat
                    .Where(entry => entry.Vacancy.FinalOffer.HasValue)
                    .Select(entry => (float)entry.Vacancy.FinalOffer.Value).ToList();
                float finalOfferAvg = finalOffer.DefaultIfEmpty(0).Average();
                
                filters.And(
                    entry => !entry.Vacancy.InitOffer.HasValue
                        || MathF.Abs(entry.Vacancy.InitOffer.Value - initOfferAvg) / initOfferAvg < deviation);

                filters.And(
                    entry => !entry.Vacancy.FinalOffer.HasValue
                        || MathF.Abs(entry.Vacancy.FinalOffer.Value - finalOfferAvg) / finalOfferAvg < deviation);
                  
                filtered.AddRange(g_subcat.Where(filters).ToList());

            }

            return HeapifyVacancyList(filtered);

        }

        public void ResetVacancies()
        {
            IEnumerable<Category> categories = LoadCategoryList();
            ICollection<Category> filteredCategories = FilterVacancyList(categories);

            Clear();
            context.Categories.AddRange(filteredCategories);
            context.SaveChanges();

        }

        public IEnumerable<Category> LoadCategoryList()
        {

            List<Category> categories = hhLoader.LoadCategoryList()
                .Select(cat =>
                {
                    cat.Subcategories = (List<Subcategory>)LoadSubcategoryList(cat);
                    return cat;
                }).ToList();

            return categories;
        }

        public IEnumerable<Subcategory> LoadSubcategoryList(Category category)
        {
            return hhLoader.LoadSubcategoryList(category)
                .Select(subcat =>
                {
                    subcat.Vacancies = (List<Vacancy>)LoadVacancyList(subcat);
                    return subcat;
                })
                .ToList();
        }

        public IEnumerable<Vacancy> LoadVacancyList(Subcategory subcategory)
        {
            return hhLoader.LoadVacancyList(subcategory);
        }
    }
}
