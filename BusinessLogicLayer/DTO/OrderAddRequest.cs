namespace BusinessLogicLayer.DTO;

public record OrderAddRequest(
    Guid UserID,
    DateTime OrderDate,
    List<OrderItemAddRequest> OrderItems)
{
    public OrderAddRequest(): this(Guid.NewGuid(), DateTime.UtcNow, default) { }
}