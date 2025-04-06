using AutoMapper;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using FluentValidation;
using FluentValidation.Results;
using MongoDB.Driver;

namespace BusinessLogicLayer.Services;

public class OrdersService:IOrdersServices
{
    private readonly IValidator<OrderAddRequest> _orderAddRequestValidator;
    private readonly IValidator<OrderUpdateRequest> _orderUpdateRequestValidator;
    private readonly IValidator<OrderItemAddRequest> _orderItemAddRequestValidator;
    private readonly IValidator<OrderItemUpdateRequest> _orderItemUpdateRequestValidator;
    
    private readonly IMapper _mapper;
    private IOrdersRepository _repository;
    
    public OrdersService(
        IOrdersRepository ordersRepository,
        IMapper mapper,
        IValidator<OrderAddRequest> orderAddRequestValidator,
        IValidator<OrderUpdateRequest> orderUpdateRequestValidator,
        IValidator<OrderItemAddRequest> orderItemAddRequestValidator,
        IValidator<OrderItemUpdateRequest> orderItemUpdateRequestValidator)
    {
        _orderAddRequestValidator = orderAddRequestValidator;
        _orderUpdateRequestValidator = orderUpdateRequestValidator;
        _orderItemAddRequestValidator = orderItemAddRequestValidator;
        _orderItemUpdateRequestValidator = orderItemUpdateRequestValidator;
        _mapper = mapper;
        _repository = ordersRepository;
    }
    
    public async Task<List<OrderResponse?>> GetAllOrdersAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<List<OrderResponse?>> GetOrdersByCustomerIdAsync(FilterDefinition<Order> filter)
    {
        throw new NotImplementedException();
    }

    public async Task<List<OrderResponse?>> GetOrderByCustomerIdAsync(FilterDefinition<Order> filter)
    {
        throw new NotImplementedException();
    }

    public async Task<OrderResponse?> AddOrder(OrderAddRequest orderAddRequest)
    {
        //check for null parameter
        if (orderAddRequest == null)
        {
            throw new ArgumentNullException(nameof(orderAddRequest));
        }
        
        //validate orderAddRequest using fluent validations
        ValidationResult validationResult = await _orderAddRequestValidator.ValidateAsync(orderAddRequest);

        if (!validationResult.IsValid)
        {
            string errors = string.Join(", ", validationResult.Errors.Select(
                x => x.ErrorMessage));
            throw new ArgumentException(errors);
        }
        
        //validate order items using fluent validation
        foreach (OrderItemAddRequest orderItemAddRequest in orderAddRequest.OrderItems)
        {
            ValidationResult orderItemAddRequestValidationResult = await _orderItemAddRequestValidator.ValidateAsync(orderItemAddRequest);

            if (!orderItemAddRequestValidationResult.IsValid)
            {
                string errors = string.Join(", ",
                    orderItemAddRequestValidationResult.Errors.Select(x => x.ErrorMessage));
                throw new ArgumentException(errors);
            }
        }
        
        // Check if userid exists in users microservice
        
        //convert data from orderAddRequest to order
        var orderInput = _mapper.Map<Order>(orderAddRequest); // maps orderAddRequest to order type (it invokes 
        //orderAddREquestToOrderMappingProfile class)
        
        // generate values
        foreach (OrderItem orderItem in orderInput.OrderItems)
        {
            orderItem.TotalPrice  = orderItem.Quantity * orderItem.UnitPrice;
        }

        orderInput.TotalBill = orderInput.OrderItems.Sum(x => x.TotalPrice);
        
        //invoke repository
         Order? newOrder = await  _repository.AddOrder(orderInput);

         if (newOrder ==null)
         {
             return null;
         }
         
         OrderResponse newOrderResponse = _mapper.Map<OrderResponse>(newOrder); // map newOrder('order' type)
         // into orderToOrderResponseMappingProfile).
         return newOrderResponse;
    }

    public async Task<OrderResponse?> UpdateOrder(OrderUpdateRequest orderUpdateRequest)
    {
        //check for null parameter
        if (orderUpdateRequest == null)
        {
            throw new ArgumentNullException(nameof(orderUpdateRequest));
        }
        
        //validate orderUpdateRequest using fluent validations
        ValidationResult validationResult = await _orderUpdateRequestValidator.ValidateAsync(orderUpdateRequest);

        if (!validationResult.IsValid)
        {
            string errors = string.Join(", ", validationResult.Errors.Select(
                x => x.ErrorMessage));
            throw new ArgumentException(errors);
        }
        
        //validate order items using fluent validation
        foreach (OrderItemUpdateRequest orderItemUpdateRequest in orderUpdateRequest.OrderItems)
        {
            ValidationResult orderItemUpdateRequestValidationResult = await
                _orderItemUpdateRequestValidator.ValidateAsync(orderItemUpdateRequest);

            if (!orderItemUpdateRequestValidationResult.IsValid)
            {
                string errors = string.Join(", ",
                    orderItemUpdateRequestValidationResult.Errors.Select(x => x.ErrorMessage));
                throw new ArgumentException(errors);
            }
        }
        
        // Check if userid exists in users microservice
        
        //convert data from orderUpdateRequest to order
        var orderInput = _mapper.Map<Order>(orderUpdateRequest); // maps orderAddRequest to order type (it invokes 
        //orderAddRequestToOrderMappingProfile class)
        
        // generate values
        foreach (OrderItem orderItem in orderInput.OrderItems)
        {
            orderItem.TotalPrice  = orderItem.Quantity * orderItem.UnitPrice;
        }

        orderInput.TotalBill = orderInput.OrderItems.Sum(x => x.TotalPrice);
        
        //invoke repository
         Order? updatedOrder = await  _repository.UpdateOrder((orderInput);

         if (updatedOrder ==null)
         {
             return null;
         }
         
         OrderResponse updateOrderResponse = _mapper.Map<OrderResponse>(updatedOrder); // map newOrder('order' type)
         // into orderToOrderResponseMappingProfile).
         return updateOrderResponse;
    }

    public async Task<bool> DeleteOrder(Guid orderId)
    {
        throw new NotImplementedException();
    }
}