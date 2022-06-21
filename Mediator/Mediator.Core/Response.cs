namespace Mediator;

public class Response<T>
{
    public Response()
    {
    }

    public Response(T body)
    {
        Body = body;
        ResponseCode = ResponseCode.Ok;
    }

    public Response(ResponseCode responseCode)
    {
        ResponseCode = responseCode;
    }

    public Response(ResponseCode responseCode, T body)
    {
        Body = body;
        ResponseCode = responseCode;
    }

    public Response(ResponseCode responseCode, IEnumerable<string> errors)
    {
        ResponseCode = responseCode;
        Errors = errors;
    }

    public ResponseCode ResponseCode { get; }

    public IEnumerable<string> Errors { get; } = new List<string>();

    public T? Body { get; }
}