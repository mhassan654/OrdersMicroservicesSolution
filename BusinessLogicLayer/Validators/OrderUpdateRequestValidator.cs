using BusinessLogicLayer.DTO;
using FluentValidation;

namespace BusinessLogicLayer.Validators;

public class OrderUpdateRequestValidator:AbstractValidator<OrderUpdateRequest>
{
    public OrderUpdateRequestValidator()
    {
        //user id
        RuleFor(temp=>temp.UserID).NotEmpty().WithErrorCode(
            "User ID can't be blank");
        
        RuleFor(temp=>temp.OrderID).NotEmpty().WithErrorCode(
            "Order ID can't be blank");
        
        RuleFor(temp=>temp.OrderDate).NotEmpty().WithErrorCode(
            "Order Date can't be blank");
        
        RuleFor(temp=>temp.OrderItems).NotEmpty().WithErrorCode(
            "OrderItems can't be blank");
    }
}