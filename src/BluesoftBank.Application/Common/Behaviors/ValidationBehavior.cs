using BluesoftBank.Application.Common.Results;
using FluentValidation;
using MediatR;

namespace BluesoftBank.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);
        var failures = validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next(cancellationToken);

        var errors = string.Join("; ", failures.Select(f => f.ErrorMessage));

        // Devuelve Result.Failure si TResponse es Result<T> o Result
        var responseType = typeof(TResponse);

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var failureMethod = responseType.GetMethod("Failure", [typeof(string)])
                ?? responseType.BaseType?.GetMethod("Failure", [typeof(string)]);

            if (failureMethod is not null)
                return (TResponse)failureMethod.Invoke(null, [errors])!;
        }

        if (responseType == typeof(Result))
            return (TResponse)(object)Result.Failure(errors);

        throw new ValidationException(failures);
    }
}
