using System.Collections.Concurrent;
using System.Linq.Expressions;
using BluesoftBank.Application.Common.Results;
using FluentValidation;
using MediatR;

namespace BluesoftBank.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    // Cache compilado por tipo para evitar reflexión en cada llamada.
    private static readonly ConcurrentDictionary<Type, Func<string, object>> _failureCache = new();

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
            return CreateFailureResult(responseType, errors);

        if (responseType == typeof(Result))
            return (TResponse)(object)Result.Failure(errors);

        throw new ValidationException(failures);
    }

    private static TResponse CreateFailureResult(Type responseType, string errors)
    {
        var factory = _failureCache.GetOrAdd(responseType, static t =>
        {
            var method = t.GetMethod("Failure", [typeof(string)])
                ?? throw new InvalidOperationException($"El tipo {t.Name} no expone un método estático Failure(string).");
            var param = Expression.Parameter(typeof(string), "error");
            var call = Expression.Call(method, param);
            var convert = Expression.Convert(call, typeof(object));
            return Expression.Lambda<Func<string, object>>(convert, param).Compile();
        });
        return (TResponse)factory(errors);
    }
}
