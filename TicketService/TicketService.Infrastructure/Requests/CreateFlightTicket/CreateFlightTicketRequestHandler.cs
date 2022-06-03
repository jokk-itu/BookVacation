using iText.Html2pdf;
using MediatR;
using TicketService.Domain;
using TicketService.Infrastructure.Services;

namespace TicketService.Infrastructure.Requests.CreateFlightTicket;

public class CreateFlightTicketRequestHandler : IRequestHandler<CreateFlightTicketRequest, RequestResult>
{
    private readonly IMinioService _minioService;

    public CreateFlightTicketRequestHandler(IMinioService minioService)
    {
        _minioService = minioService;
    }


    public async Task<RequestResult> Handle(CreateFlightTicketRequest request, CancellationToken cancellationToken)
    {
        var html = "<!DOCTYPE html><html><head><title>FlightTicket</title></head><body><h1>FlightTicket</h1></body></html>";

        await using var pdf = new MemoryStream();
        HtmlConverter.ConvertToPdf(html, pdf);
        var isSent = await _minioService.PutTicketAsync(
            BucketName.FlightTicketBucket,
            Guid.NewGuid().ToString(),
            pdf.ToArray(),
            cancellationToken);

        return isSent ? RequestResult.Created : RequestResult.Error;
    }
}