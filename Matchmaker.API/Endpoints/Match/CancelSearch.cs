using API.Routes;
using Application.Commands.Match;
using Ardalis.ApiEndpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Matchmaker.API.Endpoints.Match;

[Route(MatchRoutes.CancelSearch)]
public class CancelSearch : EndpointBaseAsync
    .WithoutRequest
    .WithoutResult
{
    private readonly IMediator _mediator;

    public CancelSearch(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new CancelSearchCommand(), cancellationToken);
        return NoContent();
    }
}