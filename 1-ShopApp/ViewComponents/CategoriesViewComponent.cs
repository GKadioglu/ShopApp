using System.Collections.Generic;
using System.Formats.Asn1;
using Microsoft.AspNetCore.Mvc;
using shopapp.business.Abstract;



namespace _1_ShopApp.ViewComponents
{
    public class CategoriesViewComponent : ViewComponent 
    {

        private ICategoryService _categoryService;
        public CategoriesViewComponent(ICategoryService categoryService)
        {
            this._categoryService = categoryService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
             if(RouteData.Values["category"]!=null)
             ViewBag.SelectedCategory = RouteData?.Values["category"];
             return View(await _categoryService.GetAll());
        }
    }
}