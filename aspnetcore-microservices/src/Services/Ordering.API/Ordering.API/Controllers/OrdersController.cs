using Contracts.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common.Models;
using Ordering.Application.Features.V1.Orders;
using Shared.Services.Email;
using System.ComponentModel.DataAnnotations;

namespace Ordering.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IEmailSMTPService _emailSMTPService;

    public OrdersController(IMediator mediator, IEmailSMTPService emailSMTPService)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _emailSMTPService = emailSMTPService;
    }

    private static class RouteNames
    {
        public const string GetOrders = nameof(GetOrders);
    }

    [HttpGet("{username}", Name = RouteNames.GetOrders)]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUserName([Required] string username)
    {
        var query = new GetOrderQuery(username);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("test-email")]
    public async Task<IActionResult> TestEmail()
    {
        var message = new MailRequest
        {
            Body = "Hello",
            Subject = "Test",
            ToAddress = "poorboygl.001@gmail.com"
        };
        await _emailSMTPService.SendEmailAsync(message);

        return Ok();
    }
}
