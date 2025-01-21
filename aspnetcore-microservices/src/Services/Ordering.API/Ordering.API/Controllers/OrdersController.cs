using AutoMapper;
using Contracts.Messages;
using Contracts.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Ordering.Application.Features.V1.Orders;
using Ordering.Domain.Entities;
using Shared.Services.Email;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Ordering.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IEmailSMTPService _emailSMTPService;
    private readonly IMessageProducer _messageProducer;
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public OrdersController(IMediator mediator, IEmailSMTPService emailSMTPService, IMessageProducer messageProducer, 
                                    IOrderRepository orderRepository, IMapper mapper)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _emailSMTPService = emailSMTPService;
        _messageProducer = messageProducer ?? throw new ArgumentNullException(nameof(messageProducer));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

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


    [HttpPost]
    [ProducesResponseType(typeof(long), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<long>> CreateOrder([FromBody] OrderDto orderDto)
    {
        var order = _mapper.Map<Order>(orderDto);
        var addedOrder = await _orderRepository.CreateOrder(order);
        await _orderRepository.SaveChangeAsync();
        var result = _mapper.Map<OrderDto>(addedOrder);
        _messageProducer.SendMessage(result);
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
