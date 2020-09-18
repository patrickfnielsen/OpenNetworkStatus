using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenNetworkStatus.Extensions;

namespace OpenNetworkStatus.Services.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _logger.LogDebug("HandlingCommand {CommandName} - command: ({@Command})", request.GetGenericTypeName(), request);
            var response = await next();
            _logger.LogDebug("HandledCommand {CommandName} - response: {@Response}", request.GetGenericTypeName(), response);

            return response;
        }
    }
}