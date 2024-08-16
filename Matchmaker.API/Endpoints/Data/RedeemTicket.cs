using API.Routes;
using Application.Commands.Data;
using Application.Commands.Match;
using Application.Common.DTOs.Data;
using Application.Common.DTOs.Match;
using Application.Common.Models;
using Ardalis.ApiEndpoints;
using Matchmaker.API.Routes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Matchmaker.API.Endpoints.Data;

[Route(DataRoutes.ReedemTicket)]
public class RedeemTicket : EndpointBaseAsync
    .WithRequest<string>
    .WithActionResult<IResponse<RedeemTicketResponse>>
{
    private readonly IMediator _mediator;

    public RedeemTicket(IMediator mediator) => _mediator = mediator;

    [HttpGet, Produces("application/json")]
    public override async Task<ActionResult<IResponse<RedeemTicketResponse>>> HandleAsync(
        string ticket,
        CancellationToken cancellationToken = new()) => Ok(await _mediator.Send(new RedeemTicketCommand(ticket), cancellationToken));
}