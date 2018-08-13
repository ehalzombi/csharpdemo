using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using HHParser.Models;

namespace HHParser.Infrastructure
{
    public class HHLoader : IHHLoader
    {
        private IHHExtractor extractor;

        public HHLoader(IHHExtractor ext)
        {
            extractor = ext;
        }

        public List<Category> LoadCategoryList()
        {
            HtmlDocument doc = extractor.LoadDocument();
            return extractor.ExtractCategoryList(doc);
        }

        public List<Subcategory> LoadSubcategoryList(Category category)
        {
            HtmlDocument doc = extractor.LoadDocument(category.Url);
            return extractor.ExtractSubcategoryList(doc);
        }

        public List<Vacancy> LoadVacancyList(Subcategory subcategory)
        {
            HtmlDocument doc = extractor.LoadDocument(subcategory.Url);
            List<Uri> paginationLinks = extractor.ExtractPaginationLinks(doc);
            List<HtmlDocument> pages = paginationLinks.Select(link => extractor.LoadDocument(link)).ToList();
                
            return extractor.ExtractVacancyList(doc, pages);
        }
    }
}
