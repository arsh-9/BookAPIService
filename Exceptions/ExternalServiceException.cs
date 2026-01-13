using System.Net;
namespace BookAPIService.Exceptions;
public sealed class ExternalServiceException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string? ResponseBody { get; }

    public ExternalServiceException(
        string message)
        : base(message)
    {

    }
}
