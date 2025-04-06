namespace BusinessLogicLayer.DTO;

public record OrderUpdateRequest(
    Guid OrderID,
    Guid UserID,
    DateTime OrderDate,
    List<OrderItemUpdateRequest> OrderItems)
{
    public OrderUpdateRequest(): this(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, null) { }
}