using iText.Html2pdf;
using MediatR;
using TicketService.Domain;
using TicketService.Domain.Templates.HotelTicket;
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
        var html = await new HotelTicketModel(request.HotelId, request.RoomId)
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