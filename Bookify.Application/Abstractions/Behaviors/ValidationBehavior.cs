using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Exceptions;
using FluentValidation;
using MediatR;

namespace Bookify.Application.Abstractions.Behaviors;
public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    // Dependencia para o validator. Injeção de qualquer validator que o command possa ter, um ou mais
    private readonly IEnumerable<IValidator<IRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<IRequest>> validators)
    {
        _validators=validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Se não tiver nenhum validator configurado para este command, então segue em frente
        if (!_validators.Any())
        {
            return await next();
        }

        // Cria o context do validation e passa para ele a instância do command
        var context = new ValidationContext<TRequest>(request);

        // Executa as validações com - _validators iterando em cada uma com o select validator.Validate
        // Se as validações (qualquer) retornar erro, será pego no selectMany
        // e irá projetar os erros para ValidationError
        var validationErrors = _validators
            .Select(validator => validator.Validate(context))
            .Where(validationResult => validationResult.Errors.Any())
            .SelectMany(validationResult => validationResult.Errors)
            .Select(validationFailure => new ValidationError(
                validationFailure.PropertyName,
                validationFailure.ErrorMessage))
            .ToList();

        // Se teve qualquer erro na validação, lança uma exceção customizada
        if (validationErrors.Any())
        {
            throw new Exceptions.ValidationException(validationErrors);
        }

        return await next();
    }
}
