namespace BusinessLogicLayer.DTO;

public record OrderUpdateRequest(
    Guid OrderID,
    Guid UserID,
    DateTime OrderDate,
    List<OrderItemAddRequest> OrderItems)
{
    public OrderUpdateRequest(): this(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, null) { }
}