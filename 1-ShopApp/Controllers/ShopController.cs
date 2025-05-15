using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _1_ShopApp.Models;
using _1_ShopApp.ViewModels;
using Azure;
using Microsoft.AspNetCore.Mvc;
using shopapp.business.Abstract;
using shopapp.entity;

namespace _1_ShopApp.Controllers
{
    public class ShopController:Controller
    {
        private IProductService _productService;
        public ShopController(IProductService productService)
        {
            this._productService = productService;
        }


        // localhost/products/telefon?page=1
        public IActionResult List(string category, int page=1) // varsayılan olarak 1 olsun page
        {
            const int pageSize= 2;
            var productViewModel = new ProductListModel()
            {
                PageInfo = new PageInfo()
                {
                    TotalItems = _productService.GetCountByCategory(category), 
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    CurrentCategory = category
                },
                Products = _productService.GetProductsByCategory(category, page, pageSize)
            };
            return View(productViewModel);
        }

        public IActionResult Details(string url)
        {
            if(url==null)
            {
                return NotFound();
            }
            Product product = _productService.GetProductDetails(url);
            if(product == null)
            {
                return NotFound();
            }
            return View(new ProductDetailModel{
                Products = product,
                Categories = product.ProductCategories.Select(i=>i.Category).ToList()
            });
        }

        public IActionResult Search(string q)
        {
            var productViewModel = new ProductListModel()
            {
               Products = _productService.GetSearchResult(q)
            };
            return View(productViewModel);
        }

    }
}