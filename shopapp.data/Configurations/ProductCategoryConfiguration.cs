using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using shopapp.entity;

namespace shopapp.data.Configurations
{
    public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
             builder.HasKey(c=>new{c.CategoryId,c.ProductId});
             
             builder.HasData
             (

                new ProductCategory(){ProductId = 1,CategoryId=1},
                new ProductCategory(){ProductId = 1,CategoryId = 3},
                new ProductCategory(){ProductId = 2,CategoryId = 3},
                new ProductCategory(){ProductId = 2,CategoryId = 4},
                new ProductCategory(){ProductId = 3,CategoryId = 2},
                new ProductCategory(){ProductId = 3,CategoryId = 3},
                new ProductCategory(){ProductId = 4,CategoryId = 4},
                new ProductCategory(){ProductId = 4,CategoryId = 5},
                new ProductCategory(){ProductId = 5,CategoryId = 1},
                new ProductCategory(){ProductId = 5,CategoryId = 3}

             );
        }
    }
}