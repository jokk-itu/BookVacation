using iText.Html2pdf;
using MediatR;
using TicketService.Domain;
using TicketService.Domain.Templates.FlightTicket;
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
        var html = await new FlightTicketModel(request.FlightId)
            .RenderViewAsync(cancellationToken);

        await using var pdf = new MemoryStream();
        HtmlConverter.ConvertToPdf(html, pdf);
        var isSent = await _minioService.PutTicketAsync(
            BucketName.CarTicketBucket,
            Guid.NewGuid().ToString(),
            pdf.ToArray(),
            cancellationToken);

        return isSent ? RequestResult.Created : RequestResult.Error;
    }
}