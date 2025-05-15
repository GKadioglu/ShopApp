using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _1_ShopApp.ViewModels;
using AspNetCoreGeneratedDocument;
using Iyzipay.Model.V2.Subscription;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shopapp.business.Abstract;
using shopapp.data.Abstract;
using shopapp.entity;

namespace Controllers
{
    public class HomeController : Controller
    {
        private IProductService _productService;
        public HomeController(IProductService productService)
        {
            this._productService = productService;
        }
        public IActionResult Index()
        {

            var productViewModel = new ProductListModel()
            {
                Products = _productService.GetHomePageProducts()
            };
            return View(productViewModel);
        }

        public async Task <IActionResult> GetProductsFromRestApi()
        {
            var products = new List<shopapp.entity.Product>();
            using (var httpClient = new HttpClient())
            {
                using(var response = await httpClient.GetAsync("http://localhost:5029/api/products"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    products = JsonConvert.DeserializeObject<List<shopapp.entity.Product>>(apiResponse);

                }
            }
            return View(products);
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View("MyView");
        }
    }
}