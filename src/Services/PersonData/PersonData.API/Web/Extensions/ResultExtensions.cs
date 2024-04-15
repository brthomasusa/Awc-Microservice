using AWC.Shared.Kernel.Utilities;

namespace AWC.PersonData.API.Web.Extensions;

public static class ResultExtensions
{
    public static IResult ToBadRequestProblemDetails(this Result result)
    {
        return result.IsSuccess
            ? throw new InvalidOperationException()
            : Results.Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Bad Request",
            detail: result.Error.Message,
            type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            extensions: new Dictionary<string, object?>
            {
            { "errors", new[] { result.Error } }
            });
    }

    public static IResult ToNotFoundProblemDetails(this Result result)
    {
        return result.IsSuccess
            ? throw new InvalidOperationException()
            : Results.Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Not Found",
            detail: result.Error.Message,
            type: "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            extensions: new Dictionary<string, object?>
            {
            { "errors", new[] { result.Error } }
            });
    }

    public static IResult ToInternalServerErrorProblemDetails(this Result result, string errorMessage)
    {
        return result.IsSuccess
            ? throw new InvalidOperationException()
            : Results.Problem(
            detail: errorMessage,
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Internal Server Error",
            type: "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            extensions: new Dictionary<string, object?>
            {
            { "errors", new[] { result.Error } }
            });
    }
}
