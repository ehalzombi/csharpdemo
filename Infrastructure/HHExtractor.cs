using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using HHParser.Models;


namespace HHParser.Infrastructure
{
    public class HHExtractor : IHHExtractor
    {
        private Uri BaseUri { get; set; }
        private Uri EntryUri { get; set; }
        private HtmlWeb Web { get; set; }

        public HHExtractor(string uri)
        {
            BaseUri = new Uri(uri);
            EntryUri = new Uri(BaseUri, "catalog");
            Web = new HtmlWeb();
        }

        public HtmlDocument LoadDocument(Uri parentUri = null)
        {
            parentUri = parentUri ?? EntryUri;
            return Web.Load(parentUri);
        }

        public List<Category> ExtractCategoryList(HtmlDocument htmlCategoryListDoc)
        {
            /*
                Извлекает первые три  категории из документа
             */
            string item_selector = "//a[@class='catalog__item-link']";
            List<Category> categories = new List<Category>();

            Parallel.ForEach(
                htmlCategoryListDoc
                .DocumentNode
                .SelectNodes(item_selector)
                .ToList()
                .Take(3),
                (HtmlNode node) => {
                    if (node == null)
                        return;

                    Uri category_url = new Uri(BaseUri, node.GetAttributeValue("href", ""));
                    categories.Add(
                        new Category
                        {
                            Title = node.InnerText,
                            Url = category_url,
                        });
                }
                );

            return categories;
        }

        public List<Subcategory> ExtractSubcategoryList(HtmlDocument htmlSubcategoryListDoc)
        {
            /*
                Извлекает первые 3 подкатегории из документа
             */
            string item_selector = "//div[@class='bloko-toggle__expandable-reverse']//a[@class='catalog__item-link']";
            List<Subcategory> subcategories = new List<Subcategory>();

            Parallel.ForEach(htmlSubcategoryListDoc
                .DocumentNode
                .SelectNodes(item_selector)
                .ToList()
                .Take(3),
                (HtmlNode node) => {
                    if (node == null)
                        return;

                    Uri subcategoryUrl = new Uri(BaseUri, node.GetAttributeValue("href", ""));
                    subcategories.Add(
                        new Subcategory
                        {
                            Title = node.InnerText,
                            Url = subcategoryUrl,
                        }
                        );
                });

            return subcategories;
        }

        public List<Vacancy> ExtractVacancyList(
            HtmlDocument subcategoryDoc,
            List<HtmlDocument> paginationPages)
        {
            /*
                Извлекает вакансии из подкатегории с учётом пагинации 
             */
            List<Vacancy> vacancies = new List<Vacancy>();

            vacancies = ExtractVacancyListFromPage(subcategoryDoc);
            Parallel.ForEach(paginationPages,
                (HtmlDocument page) => {
                    vacancies.Concat(ExtractVacancyListFromPage(page));

                });
            return vacancies;
        }

        public List<Uri> ExtractPaginationLinks(HtmlDocument doc)
        {
            string link_selector = "//a[@data-qa='pager-page']";
            var linkNodeList = doc.DocumentNode.SelectNodes(link_selector);
            List<Uri> linkList = new List<Uri>();

            foreach (var node in linkNodeList?.ToList() ?? new List<HtmlNode>())
            {
                linkList.Append(new Uri(BaseUri, node.GetAttributeValue("href", null)));
            }

            return linkList;
        }

        public List<Vacancy> ExtractVacancyListFromPage(HtmlDocument page)
        {
            string vacancy_card_selector = "//div[@data-qa='vacancy-serp__vacancy']";
            string vacancy_title_selector = ".//a[@data-qa='vacancy-serp__vacancy-title']";
            string vacancy_compensation_selector = ".//div[@data-qa='vacancy-serp__vacancy-compensation']";

            List<Vacancy> vacancies = new List<Vacancy>();

            Parallel.ForEach(page.DocumentNode.SelectNodes(vacancy_card_selector),
                (HtmlNode node) =>
                {
                    string title = node.SelectSingleNode(vacancy_title_selector).InnerHtml;
                    string url = node.SelectSingleNode(vacancy_title_selector).GetAttributeValue("href", null);
                    string compensation_str = node.SelectSingleNode(vacancy_compensation_selector)?.InnerHtml ?? "";

                    (int? initComp, int? finalComp) = ExtractCompensation(compensation_str);
                    Vacancy vac = new Vacancy
                    {
                        Title = title,
                        Url = new Uri(BaseUri, url),
                    };
                    if (initComp != null)
                        vac.InitOffer = initComp;
                    if (finalComp != null)
                        vac.FinalOffer = finalComp;
                    vacancies.Add(vac);
                });

            return vacancies;
        }

        private int StrToDecimal(string number)
        {
            number = Regex.Replace(number, "\\s", "");
            int result;
            int.TryParse(number, out result);
            return result;
        }

        private (int? init, int? final) ExtractCompensation(string comp)
        {
            string initPattern = @"от (?<init>[\d\s]+) руб\.";
            string finalPattern = @"до (?<final>[\d\s]+) руб\.";
            string initFinalPattern = @"(?<init>[\d\s]+)-(?<final>[\d\s]+) руб\.";

            if (Regex.Match(comp, initPattern).Success)
            {
                Match result = Regex.Match(comp, initPattern);
                return (StrToDecimal(result.Groups["init"].Value), null);
            }
            else if (Regex.Match(comp, finalPattern).Success)
            {
                Match result = Regex.Match(comp, finalPattern);
                return (null, StrToDecimal(result.Groups["final"].Value));
            }
            else if (Regex.Match(comp, initFinalPattern).Success)
            {
                Match result = Regex.Match(comp, initFinalPattern);
                return (
                    StrToDecimal(result.Groups["init"].Value),
                    StrToDecimal(result.Groups["final"].Value)
                );
            }
            else
            {
                return (null, null);
            }

        }
    }
}
