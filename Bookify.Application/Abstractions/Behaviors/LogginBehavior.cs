using Bookify.Application.Abstractions.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bookify.Application.Abstractions.Behaviors;

// O log é apenas para IBaseCommand, pois o log deve ser usado apenas pra command e não queries
// TRequest é o command em si e TResponse o response do command 
public class LogginBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{

    private readonly ILogger<TRequest> _logger;

    public LogginBehavior(ILogger<TRequest> logger)
    {
        _logger=logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Pegar o nome do command que esta disparando o log
        var name = request.GetType().Name;

        try
        {
            _logger.LogInformation("Executing command {Command}", name);

            var result = await next();

            _logger.LogInformation("Command {Command} processed successfuly", name);

            return result;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Command {Command} processing failed", name);
            throw;
        }
        
    }
}
