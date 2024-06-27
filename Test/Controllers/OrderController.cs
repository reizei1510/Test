using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.DBModels;
using Test.Dto;

namespace Test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }

        // возвращает заказы в виде "id | дата | список названий продуктов | список количества продуктов"
        [HttpGet]
        public async Task<ActionResult> GetOrders()
        {
            List<Order> orders = await _db.Orders.ToListAsync();
            List<OrderDto> ordersDto = new List<OrderDto>();

            foreach (Order order in orders)
            {
                OrderDto orderDto = new OrderDto()
                {
                    Id = order.Id,
                    OrderDate = order.OrderDate,
                    ProductsList = await _db.Products
                        .Where(p => _db.OrderProducts
                        .Any(op => op.Order == order && op.Product == p))
                        .OrderBy(p => p.Id)
                        .Select(p => p.Name)
                        .ToListAsync(),
                    CountsList = await _db.OrderProducts
                        .Where(op => op.Order == order)
                        .OrderBy(op => op.ProductId)
                        .Select(op => op.Count)
                        .ToListAsync()
                };

                ordersDto.Add(orderDto);
            }

            return Ok(ordersDto);
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrder(OrderDto orderModel)
        {
            // создаем новый заказ
            Order order = new Order()
            {
                OrderDate = DateTime.Now
            };
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            for (int i = 0; i < orderModel.ProductsList.Count; i++)
            {
                // ищем продукт из списка
                Product? product = await _db.Products
                        .Where(p => p.Name == orderModel.ProductsList[i])
                        .FirstOrDefaultAsync();

                // если продукт не найден или на складе недостаточное количество, отказ
                if (product == null || product.Count < orderModel.CountsList[i])
                {
                    _db.Remove(order);
                    await _db.SaveChangesAsync();

                    return BadRequest();
                }

                // добавляем продукт в заказ
                OrderProduct orderProducts = new OrderProduct()
                {
                    Product = product,
                    Order = order,
                    Count = orderModel.CountsList[i]
                };
                await _db.OrderProducts.AddAsync(orderProducts);

                // обновляем остатки на складе
                product.Count -= orderProducts.Count;
                _db.Products.Update(product);
            }

            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            Order? order = await _db.Orders
                .Where(o => o.Id == id)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
