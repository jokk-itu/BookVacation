using BlobStorage;
using iText.Html2pdf;
using Mediator;
using MediatR;
using TicketService.Domain;

namespace TicketService.Infrastructure.Requests.CreateCarTicket;

public class CreateCarTicketCommandHandler : ICommandHandler<CreateCarTicketCommand, Unit>
{
    private readonly IMinioService _minioService;

    public CreateCarTicketCommandHandler(IMinioService minioService)
    {
        _minioService = minioService;
    }

    public async Task<Response<Unit>> Handle(CreateCarTicketCommand command, CancellationToken cancellationToken)
    {
        var html = "<!DOCTYPE html><html><head><title>CarTicket</title></head><body><h1>CarTicket</h1></body></html>";

        await using var pdf = new MemoryStream();
        HtmlConverter.ConvertToPdf(html, pdf);
        var isSent = await _minioService.CreateAsync(
            BucketName.CarTicketBucket,
            Guid.NewGuid().ToString(),
            pdf.ToArray(),
            cancellationToken);

        return isSent
            ? new Response<Unit>()
            : new Response<Unit>(ResponseCode.Error, new[] { "Unknown error occured" });
    }
}