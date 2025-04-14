namespace BusinessLogicLayer.DTO;

public record OrderUpdateRequest(
    Guid OrderID,
    Guid UserId,
    DateTime OrderDate,
    List<OrderItemUpdateRequest> OrderItems)
{
    public OrderUpdateRequest(): this(default, default, default, []) { }
}