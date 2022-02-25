namespace FlightService.Infrastructure.Requests;

public enum RequestResult
{
    NotFound,
    Ok,
    BadRequest,
    Conflict,
    Created,
    Error
}