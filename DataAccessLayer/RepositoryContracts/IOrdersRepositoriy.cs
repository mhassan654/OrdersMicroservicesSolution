using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using MongoDB.Driver;

namespace DataAccessLayer.RepositoryContracts
{
    public interface IOrdersRepositoriy
    {
        /// <summary>
        /// Retrieves all orders asynchrounously
        /// </summary>
        /// <returns> Returns all orders from the orders collection
        /// </returns>
        Task<IEnumerable<Order>> GetOrders();

        /// <summary>
        /// Retrieves all Orders based on the specified condition asynchrounously
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Returning a collection of matching orders
        /// </returns>

        Task<IEnumerable<Order?>> GetOrdersByCondition(FilterDefinition<Order> filter);

        /// <summary>
        /// Retrieves a single order based on the specified condition asynchrounously
        /// </summary>
        /// <param name="filter">The condition to filter orders
        /// </param>
        /// <returns>Returning matching order
        /// </returns>
        Task<Order?> GetOrderByCondition(FilterDefinition<Order> filter);

        /// <summary>
        /// Adds a new Order into the orders collection asynchrounously
        /// </summary>
        /// <param name="order">The order to be added
        /// </param>
        /// <returns>Returns the added order object or null if unsuccessful
        /// </returns>

        Task<Order?> AddOrder(Order order);

        /// <summary>
        /// Updates an existing order in the orders collection asynchrounously
        /// </summary>
        /// <param name="order">The order to be added
        /// </param>
        /// <returns>Returns the updated order object or null if not found
        /// </returns>
        Task<Order?> UpdateOrder(Order order);

        /// <summary>
        /// Deletes an existing order in the orders collection asynchrounously
        /// </summary>
        /// <param name="orderId">The order id based on which the order will be deleted
        /// </param>
        /// <returns>Returns true if the order is deleted successfully, 
        /// false if not found and null if not successful
        /// </returns>
        Task<bool?> DeleteOrder(Guid orderId);
    }
}
