using Contracts.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common.Models;
using Ordering.Application.Features.V1.Orders;
using Shared.Services.Email;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Ordering.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ISmtpEmailService _mailService;

    public OrdersController(IMediator mediator, ISmtpEmailService mailServic)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _mailService = mailServic ?? throw new ArgumentNullException(nameof(mailServic));
    }

    private static class RouteNames
    {
        public const string GetOrders = nameof(GetOrders);
    }

    [HttpGet("{username}", Name = RouteNames.GetOrders)]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUserName([Required] string username)
    {
        var query = new GetOrdersQuery(username);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    [HttpGet("test-email")]
    public async Task<IActionResult> TestEmail()
    {
        var message = new MailRequest()
        {
            Body = "<h1>Hello ae Csoft</h1>",
            Subject = "Test send email tedu ordering service",
            ToAddress = "hai.tc21@gmail.com"
        };
        await _mailService.SendEmailAsync(message);
        return Ok();
    }
}