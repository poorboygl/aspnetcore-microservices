using EventBus.Messages.Interfaces;

namespace EventBus.Messages.Events;

public record class BasketCheckoutEvent : IntegrationBaseEvent, IBasketCheckoutEvent
{
    public string UserName { get; set; }
    public decimal TotalPrice { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string ShippingAddress { get; set; }
    public string InvoiceAddress { get; set; }
}