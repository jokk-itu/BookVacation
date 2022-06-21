using BlobStorage;
using iText.Html2pdf;
using Mediator;
using MediatR;
using TicketService.Domain;

namespace TicketService.Infrastructure.Requests.CreateFlightTicket;

public class CreateFlightTicketCommandHandler : ICommandHandler<CreateFlightTicketCommand, Unit>
{
    private readonly IMinioService _minioService;

    public CreateFlightTicketCommandHandler(IMinioService minioService)
    {
        _minioService = minioService;
    }


    public async Task<Response<Unit>> Handle(CreateFlightTicketCommand command, CancellationToken cancellationToken)
    {
        var html =
            "<!DOCTYPE html><html><head><title>FlightTicket</title></head><body><h1>FlightTicket</h1></body></html>";

        await using var pdf = new MemoryStream();
        HtmlConverter.ConvertToPdf(html, pdf);
        var isSent = await _minioService.CreateAsync(
            BucketName.FlightTicketBucket,
            Guid.NewGuid().ToString(),
            pdf.ToArray(),
            cancellationToken);

        return isSent ? new Response<Unit>() : new Response<Unit>(ResponseCode.Ok, new[] { "Unknown error occured" });
    }
}