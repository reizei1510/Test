using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.DBModels;

namespace Test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult> GetProducts()
        {
            List<Product> productList = await _db.Products.ToListAsync();

            return Ok(productList);
        }

        [HttpPost]
        public async Task<ActionResult> AddProduct(Product product)
        {
            Product? findProduct = await _db.Products
                .Where(p => p.Name == product.Name)
                .FirstOrDefaultAsync();

            if (findProduct == null)
            {
                // если продукт новый, создаем его
                _db.Products.Add(product);
            }
            else
            {
                // если нет, добавляем количество на склад
                findProduct.Count += product.Count;
                _db.Products.Update(findProduct);
            }
            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            Product? product = await _db.Products
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
