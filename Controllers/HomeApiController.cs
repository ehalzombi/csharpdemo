using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting.Internal;

using HHParser.Models;
using HHParser.Infrastructure;

namespace HHParser.Controllers
{
    [Route("api/[controller]")]
    public class HomeApiController : Controller
    {
        private ICategoryService categoryService;

        public HomeApiController(ICategoryService catSrv)
        {
            categoryService = catSrv;
        }

        [HttpGet("[action]")]
        public IEnumerable<Category> CategoryList([FromQuery(Name = "search_str")] string search_str)
        {

            return categoryService.FilteredCategories(search_str ?? null);
        }

        [HttpGet("[action]")]
        public IEnumerable<Category> UpdateCategoryList()
        {
            categoryService.ResetVacancies();
            return categoryService.FilteredCategories();
        }
    }
}
