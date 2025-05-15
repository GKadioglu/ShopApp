using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;

namespace _1_ShopApp.Models
{
    public class ProductDetailModel
    {
        public Product Products { get; set; }
        public List<Category> Categories { get; set; }
    }
}