using BusinessLogicLayer.DTO;
using DataAccessLayer.Entities;
using MongoDB.Driver;

namespace BusinessLogicLayer.ServiceContracts;

public interface IOrdersServices
{
    /// <summary>
    /// Retrieves the list of orders from the orders repository
    /// </summary>
    /// <returns>Returns list or orderRepository objects
    /// </returns>
    Task<List<OrderResponse?>> GetAllOrdersAsync();
    
    
    /// <summary>
    /// Returns list of orders matching with the condition
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    
    Task<List<OrderResponse?>> GetOrdersByCustomerIdAsync(
        FilterDefinition<Order> filter);
    
    /// <summary>
    /// Returns a single order that matches with given condition
    /// </summary>
    /// <param name="filter">Expression that represents the condition to check
    /// </param>
    /// <returns>
    /// Returns matching order object as ORderResponse; or null if not found
    /// </returns>
    Task<List<OrderResponse?>> GetOrderByCustomerIdAsync(
        FilterDefinition<Order> filter);
    
    /// <summary>
    /// Add (inersts) order into the collection using orders repository
    /// </summary>
    /// <param name="order"> Order to insert
    /// </param>
    /// <returns> Returns OrderResponse object that contains details after
    /// inserting
    /// </returns>
    Task<OrderResponse?> AddOrder(OrderAddRequest orderAddRequest);
    
    
    /// <summary>
    /// Updates the existing order based on the orderId
    /// </summary>
    /// <param name="orderUpdateRequest"> Orderdata to update
    /// </param>
    /// <returns>Returns order object after successfult updation: otherwise null
    /// </returns>
    Task<OrderResponse?> UpdateOrder(OrderUpdateRequest orderUpdateRequest);
    
    /// <summary>
    /// Deletes an existing order based on given order id
    /// </summary>
    /// <param name="orderId">OrderId to search an delete
    /// </param>
    /// <returns>Returns true if the deletion is successfult: otherwise false
    /// </returns>
    Task<bool> DeleteOrder(Guid orderId);
    
}