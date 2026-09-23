using Microsoft.AspNetCore.Http;
using Relay.SharedKernel;

namespace Relay.Routing.Results;

public static class ResultExtensions
{
    public static IResult Match<T>(
        this Result<T> result,
        Func<T, IResult> onSuccess,
        Func<Error, IResult> onFailure)
    {
        return result.IsSuccess ? onSuccess(result.Value!) : onFailure(result.Error);
    }
}
