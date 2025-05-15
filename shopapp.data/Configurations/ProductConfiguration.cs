using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using shopapp.entity;

namespace shopapp.data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(m=>m.ProductId);
            builder.Property(m=>m.Name). IsRequired().HasMaxLength(100);
            builder.Property(m=>m.DateAdded).HasDefaultValueSql("getdate()");
            
            builder.HasData(
                new Product(){ProductId= 1, Name = "Iphone 15", Url = "iphone-15",Price = 60000, Description = "2024 model telefon", ImageUrl = "3.jpg", IsApproved = true},
                new Product(){ProductId= 2, Name = "Atari Oyun Konsol ", Url = "atari-oyun-konsol", Price = 10000, Description = "Nostalji Kuşağı Oyun Konsolu", ImageUrl = "1.jpg", IsApproved = true},
                new Product(){ProductId= 3, Name = "Lenovo Laptop", Url = "lenovo-laptop", Price = 25000, Description = "16 GB Lenovo, 14 İnç Katlanabilir Model", ImageUrl = "5.jpg", IsApproved = true},
                new Product(){ProductId= 4, Name = "Dijitsu Televizyon ", Url = "dijitsu-televizyon", Price = 18000, Description = "2 yıl garantili televizyon", ImageUrl = "4.jpg", IsApproved = true},
                new Product(){ProductId= 5, Name = "Samsung S13", Url = "samsuns-s13", Price = 13000, Description = "Çift kameralı samsung model telefon", ImageUrl = "3.jpg", IsApproved = true});
        }
    }
}