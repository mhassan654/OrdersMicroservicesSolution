using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using MongoDB.Driver;

namespace DataAccessLayer.Repositories
{
    public class OrderRepository : IOrdersRepository
    {
        private readonly IMongoCollection<Order> _ordersCollection;
        private readonly string _collectionName = "orders";
        public OrderRepository(IMongoDatabase mongoDatabase)
        {
             _ordersCollection = mongoDatabase.GetCollection<Order>(_collectionName);

        }
        public async Task<Order?> AddOrder(Order order)
        {
            order.OrderId = Guid.NewGuid();
            await _ordersCollection.InsertOneAsync(order);
            return order;
        }

        public async Task<bool?> DeleteOrder(Guid orderId)
        {
           FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(temp=>temp.OrderId,orderId);

            Order? existingOrder = (await _ordersCollection.FindAsync(filter)).FirstOrDefault();

            if (existingOrder == null)
            {
                return false;
            }
           DeleteResult deleteResult= await _ordersCollection.DeleteOneAsync(filter);
            return deleteResult.DeletedCount > 0;
        }

        public async Task<Order?> GetOrderByCondition(FilterDefinition<Order> filter)
        {
            return (await _ordersCollection.FindAsync(filter)).FirstOrDefault();
        }

        public async Task<IEnumerable<Order>> GetOrders()
        {
            return (await _ordersCollection.FindAsync(
                Builders<Order>.Filter.Empty)).ToList();
        }

        public async Task<IEnumerable<Order?>> GetOrdersByCondition(FilterDefinition<Order> filter)
        {
            return (await _ordersCollection.FindAsync(filter)).ToList();
        }

        public async Task<Order?> UpdateOrder(Order order)
        {
            FilterDefinition<Order> filter = 
                Builders<Order>.Filter.Eq(temp => temp.OrderId, order.OrderId);

            Order? existingOrder =
                (await _ordersCollection.FindAsync(filter)).FirstOrDefault();

            if (existingOrder == null)
            {
                return null;
            }
           ReplaceOneResult replaceOneResult= await  _ordersCollection.ReplaceOneAsync(filter, order);
            return order;
        }
    }
}
