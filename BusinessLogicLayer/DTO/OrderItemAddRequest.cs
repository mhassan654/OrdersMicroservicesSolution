namespace BusinessLogicLayer.DTO;

public record OrderItemAddRequest(
    Guid ProductID,
    decimal UnitPrice,
    int Quantity)
{
    public OrderItemAddRequest(): this(Guid.NewGuid(), default, default){}
}