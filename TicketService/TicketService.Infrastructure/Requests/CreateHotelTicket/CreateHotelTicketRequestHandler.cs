using iText.Html2pdf;
using MediatR;
using TicketService.Domain;
using TicketService.Infrastructure.Services;

namespace TicketService.Infrastructure.Requests.CreateHotelTicket;

public class CreateHotelTicketRequestHandler : IRequestHandler<CreateHotelTicketRequest, RequestResult>
{
    private readonly IMinioService _minioService;

    public CreateHotelTicketRequestHandler(IMinioService minioService)
    {
        _minioService = minioService;
    }

    public async Task<RequestResult> Handle(CreateHotelTicketRequest request, CancellationToken cancellationToken)
    {
        var html = "<!DOCTYPE html><html><head><title>HotelTicket</title></head><body><h1>HotelTicket</h1></body></html>";

        await using var pdf = new MemoryStream();
        HtmlConverter.ConvertToPdf(html, pdf);
        var isSent = await _minioService.PutTicketAsync(
            BucketName.HotelTicketBucket,
            Guid.NewGuid().ToString(),
            pdf.ToArray(),
            cancellationToken);

        return isSent ? RequestResult.Created : RequestResult.Error;
    }
}