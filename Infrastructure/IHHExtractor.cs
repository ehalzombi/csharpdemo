using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using HHParser.Models;

namespace HHParser.Infrastructure
{
    public interface IHHExtractor
    {
        HtmlDocument LoadDocument(Uri uri = null);

        List<Category> ExtractCategoryList(HtmlDocument htmlCategoryListDoc);
        List<Subcategory> ExtractSubcategoryList(HtmlDocument htmlSubcategoryListDoc);

        List<Uri> ExtractPaginationLinks(HtmlDocument doc);
        List<Vacancy> ExtractVacancyList(HtmlDocument htmlVacancyListDoc, List<HtmlDocument> paginationPages);
    }
}
