using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shopapp.entity;

namespace _1_ShopApp.Models
{
    public class CategoryModel
    {

        
        public int CategoryId { get; set; }

        [Required(ErrorMessage ="Kategori adı zorunludur")]
        [StringLength(100,MinimumLength=5, ErrorMessage = "Kategori 5-100 arasında değer almalıdır.")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Url alanı zorunludur")]
        [StringLength(100,MinimumLength=5, ErrorMessage = "Url 5-100 arasında değer almalıdır.")]
        public string? Url { get; set; }
        
        
        public List<Product>? Products { get; set; }
        
    }
}