namespace TicketService.Infrastructure.Requests;

public enum RequestResult
{
    NotFound,
    Ok,
    BadRequest,
    Conflict,
    Created,
    Error
}