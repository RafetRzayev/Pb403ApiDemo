using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pb403ApiDemo.DataContext;
using Pb403ApiDemo.Dtos;
using Pb403ApiDemo.Models;

namespace Pb403ApiDemo.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public ProductsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _dbContext.Products
                .Include(x => x.Category)
                .Include(x => x.ProductTags)
                .ThenInclude(x => x.Tag).ToList();

            var productDtos = products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                CategoryName = p.Category != null ? p.Category.Name : null,
                TagNames = p.ProductTags.Select(pt => pt.Tag!.Name).ToList()
            }).ToList();

            return Ok(productDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _dbContext.Products
                .Include(x => x.Category)
                .Include(x => x.ProductTags)
                .ThenInclude(x => x.Tag)
                .FirstOrDefault(x => x.Id == id);
           
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CreateProductDto createDto)
        {
            var product = new Product
            {
                Name = createDto.Name,
                Price = createDto.Price,
                CategoryId = createDto.CategoryId,
                ProductTags = createDto.TagIds.Select(tagId => new ProductTag { TagId = tagId }).ToList()
            };

            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, createDto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateProductDto updateDto)
        {
            var product = _dbContext.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            product.Name = updateDto.Name;
            product.Price = updateDto.Price;
            product.CategoryId = updateDto.CategoryId;
            _dbContext.SaveChanges();
            return NoContent();
        }

        [HttpPost]
        [Route("{productId}/add-tag/{tagId}")]
        public IActionResult AddTagToProduct(int productId, int tagId)
        {
            var product = _dbContext.Products
                .Include(p => p.ProductTags)
                .FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return NotFound();
            }
            if (product.ProductTags.Any(pt => pt.TagId == tagId))
            {
                return BadRequest("Tag already associated with the product.");
            }

            if (!_dbContext.Tags.Any(t => t.Id == tagId))
            {
                return NotFound("Tag not found.");
            }

            product.ProductTags.Add(new ProductTag { ProductId = productId, TagId = tagId });
            _dbContext.SaveChanges();
            return NoContent();
        }

        [HttpPost]
        [Route("{productId}/remove-tag/{tagId}")]
        public IActionResult RemoveTagToProduct(int productId, int tagId)
        {
            var product = _dbContext.Products
                .Include(p => p.ProductTags)
                .FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return NotFound();
            }

            if (!product.ProductTags.Any(pt => pt.TagId == tagId))
            {
                return BadRequest("Tag not found");
            }

            var productTag = product.ProductTags.First(pt => pt.TagId == tagId);
            product.ProductTags.Remove(productTag);
            _dbContext.SaveChanges();
            return NoContent();
        }
    }
}
