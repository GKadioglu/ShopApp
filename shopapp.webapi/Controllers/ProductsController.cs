using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shopapp.business.Abstract;
using shopapp.business.Concrete;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Iyzipay.Model.V2.Subscription;
using shopapp.entity;
using shopapp.webapi.DTO;

namespace shopapp.webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ProductsController: ControllerBase
    {
        private IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {       
            var products = await _productService.GetAll();

            var productsDTO = new List<ProductDTO>();
            
            foreach(var p in products)
            {
                productsDTO.Add(ProductToDTO(p));
            }

            return Ok(productsDTO);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var p = await _productService.GetById(id);
            if(p == null)
            {
                return NotFound();
            }
            return Ok(ProductToDTO(p));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(entity.Product entity)
        {
            await _productService.CreateAsync(entity);
            return CreatedAtAction(nameof(GetProduct),new {id=entity.ProductId},ProductToDTO(entity));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, entity.Product entity)
        {
            if(id!=entity.ProductId)
            {
                return BadRequest();
            }
            
            var products = await _productService.GetById(id);
            if(products == null)
            {
                return NotFound();
            } 
            await _productService.UpdateAsync(products,entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productService.GetById(id);

            if(product == null)
            {
                return NotFound ();
            }
            await _productService.DeleteAsync(product);
            return NoContent();
        }

        private static ProductDTO ProductToDTO(entity.Product p)
        {
            return new ProductDTO{

                    ProductId = p.ProductId,
                    Name = p.Name,
                    Url = p.Url,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl
                };
        }
    }
}