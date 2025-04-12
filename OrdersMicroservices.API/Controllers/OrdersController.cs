using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace OrdersMicroservices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersServices _ordersServices;

        public OrdersController(IOrdersServices ordersServices)
        {
            _ordersServices = ordersServices;
        }

        // GET: /api/orders
        [HttpGet]
        public async Task<IEnumerable<OrderResponse?>> Get()
        {
            List<OrderResponse?> orders = await _ordersServices.GetAllOrdersAsync();
            return orders;
        }

        // GET: /api/orders/search/orderid/{orderId}
        [HttpGet("search/orderid/{orderId}")]
        public async Task<OrderResponse?> GetByOrderId(Guid orderId)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(x => x.OrderId, orderId);
            OrderResponse? order = await _ordersServices.GetOrderByCondition(filter);
            return order;
        }

        //GET: /api/orders/search/productid/{productid}
        [HttpGet("search/productid/{productid}")]
        public async Task<IEnumerable<OrderResponse?>> GetByProductId(Guid productId)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.ElemMatch(
                x => x.OrderItems,
                Builders<OrderItem>.Filter.Eq(x => x.ProductId, productId));

            List<OrderResponse?> orders = await (_ordersServices.GetOrdersByCondtion(filter));
            return orders;
        }

        //GET: /api/orders/search/orderDate/{orderDate}
        [HttpGet("search/orderDate/{orderDate}")]
        public async Task<IEnumerable<OrderResponse?>> GetByOrderDate(DateTime orderDate)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(
                x => x.OrderDate.ToString("yyyy-MM-dd"), 
                orderDate.ToString("yyyy-MM-dd"));

            List<OrderResponse?> orders = await (_ordersServices.GetOrdersByCondtion(filter));
            return orders;
        }

        //POST: api/orders
        [HttpPost]
        public async Task<IActionResult> StoreOrder(OrderAddRequest orderAddRequest)
        {
            if (orderAddRequest == null) {
                return BadRequest("Invalid order data");
            }


            var orderResponse = await _ordersServices.AddOrder(orderAddRequest);

            if (orderResponse == null) {
                return Problem("Error in adding order");
            }

            return Created($"api/Orders/search/orderid/{orderResponse?.OrderID}", orderResponse);
        }

        //PUT: api/orders/{orderId}
        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrder(Guid orderId, OrderUpdateRequest orderUpdateRequest)
        {
            if (orderUpdateRequest == null)
            {
                return BadRequest("Invalid order data");
            }

            if (orderId != orderUpdateRequest.OrderID)
            {
                return BadRequest("OrderID in the URL doesn't match with the" +
                    "OrderID in the requet body");
            }

            var orderResponse = await _ordersServices.UpdateOrder(orderUpdateRequest);

            if (orderResponse == null)
            {
                return Problem("Error in updating order");
            }

            return Ok(orderResponse);
        }

        //DELETE: api/orders/{orderId}
        [HttpDelete("{orderID}")]
        public async Task<IActionResult> Delete(Guid orderID)
        {
            if (orderID == Guid.Empty)
            {
                return BadRequest("Invalid order dataID");
            }

           bool isDeleted = await _ordersServices.DeleteOrder(orderID);

            if (!isDeleted)
            {
                return Problem("Error in deleting order");
            }

            return Ok(isDeleted);
        }

        //GET: /api/orders/search/userId/{userID}
        [HttpGet("search/userid/{userId}")]
        public async Task<IEnumerable<OrderResponse?>> GetByOrderByUserId(Guid userId)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(
                x => x.UserId,userId);

            List<OrderResponse?> orders = await (_ordersServices.GetOrdersByCondtion(filter));
            return orders;
        }
    }
}
