using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using shopapp.entity;

namespace shopapp.data.Concrete.EfCore
{
    public static class SeedDatabase
    {
        public static void Seed()
        {
            // var context = new ShopContext();
            // if(context.Database.GetPendingMigrations().Count() == 0)
            // {
            //     if(context.Categories.Count() ==0)
            //     {
            //         context.Categories.AddRange(Categories);
            //     }
            // }

            // if(context.Products.Count() ==0)
            //     {
            //         context.AddRange(ProductCategories);
            //         context.Products.AddRange(Products);
            //     }
            //     context.SaveChanges();
        }

        private static Category[] Categories= {
            new Category(){Name = "Telefon", Url = "telefon"}, 
            new Category(){Name = "Bilgisayar", Url = "bilgisayar"},
            new Category(){Name = "Elektronik", Url = "elektronik"},
            new Category(){Name = "Konsol", Url = "konsol"},
            new Category(){Name = "Televizyon", Url = "televizyon"},
            new Category(){Name = "Beyaz Eşya", Url = "beyaz-esya"}



        };

         private static Product[] Products= {
            new Product(){Name = "Iphone 15", Url = "iphone-15",Price = 60000, Description = "2024 model telefon", ImageUrl = "3.jpg", IsApproved = true},
            new Product(){Name = "Atari Oyun Konsol ", Url = "atari-oyun-konsol", Price = 10000, Description = "Nostalji Kuşağı Oyun Konsolu", ImageUrl = "1.jpg", IsApproved = true},
            new Product(){Name = "Lenovo Laptop", Url = "lenovo-laptop", Price = 25000, Description = "16 GB Lenovo, 14 İnç Katlanabilir Model", ImageUrl = "5.jpg", IsApproved = true},
            new Product(){Name = "Dijitsu Televizyon ", Url = "dijitsu-televizyon", Price = 18000, Description = "2 yıl garantili televizyon", ImageUrl = "4.jpg", IsApproved = true},
            new  Product(){Name = "Samsung S13", Url = "samsuns-s13", Price = 13000, Description = "Çift kameralı samsung model telefon", ImageUrl = "3.jpg", IsApproved = true}
        };

         private static ProductCategory[] ProductCategories= {
            new ProductCategory(){Product = Products[0],Category=Categories[0]},
            new ProductCategory(){Product = Products[0],Category=Categories[2]},
            new ProductCategory(){Product = Products[1],Category=Categories[2]},
            new ProductCategory(){Product = Products[1],Category=Categories[3]},
            new ProductCategory(){Product = Products[2],Category=Categories[1]},
             new ProductCategory(){Product = Products[2],Category=Categories[2]},
            new ProductCategory(){Product = Products[3],Category=Categories[3]},
            new ProductCategory(){Product = Products[3],Category=Categories[4]},
            new ProductCategory(){Product = Products[4],Category=Categories[0]},
            new ProductCategory(){Product = Products[4],Category=Categories[2]},


         };
        
    }
}