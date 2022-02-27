using iText.Html2pdf;
using MediatR;
using TicketService.Domain;
using TicketService.Domain.Templates.CarTicket;
using TicketService.Infrastructure.Services;

namespace TicketService.Infrastructure.Requests.CreateCarTicket;

public class CreateCarTicketRequestHandler : IRequestHandler<CreateCarTicketRequest, RequestResult>
{
    private readonly IMinioService _minioService;

    public CreateCarTicketRequestHandler(IMinioService minioService)
    {
        _minioService = minioService;
    }

    public async Task<RequestResult> Handle(CreateCarTicketRequest request, CancellationToken cancellationToken)
    {
        var html = await new CarTicketModel(request.CarId, request.RentingCompanyId)
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