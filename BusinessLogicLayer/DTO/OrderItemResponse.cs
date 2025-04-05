namespace BusinessLogicLayer.DTO;

public record OrderItemResponse(
    Guid ProductID,
    decimal UnitPrice,
    int Quantity)
{
    public OrderItemResponse() : this(default, default, default)
    {
        
    }
};