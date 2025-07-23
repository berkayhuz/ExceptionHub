using ExceptionHub.Abstractions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace ExceptionHub.Workers;

/// <summary>
/// Decorates any <see cref="IHostedService"/> (most commonly a <see cref="BackgroundService"/>)
/// and funnels unhandled exceptions through <see cref="IExceptionMapper"/> for centralized classification.
/// </summary>
/// <typeparam name="T">Concrete hosted‑service type to decorate.</typeparam>
public sealed class ExceptionHandlingServiceDecorator<T> : IHostedService, IDisposable
    where T : IHostedService
{
    private readonly T _inner;
    private readonly IExceptionMapper _mapper;
    private Task? _executingTask;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlingServiceDecorator{T}"/> class.
    /// </summary>
    /// <param name="inner">The inner hosted service to be decorated.</param>
    /// <param name="mapper">The exception mapper used to classify unhandled exceptions.</param>
    public ExceptionHandlingServiceDecorator(
        T inner,
        IExceptionMapper mapper)
    {
        _inner = inner;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _executingTask = _inner.StartAsync(cancellationToken);

            if (_executingTask is { IsCompleted: false })
            {
                _ = _executingTask.ContinueWith(
                    t => HandleException(t.Exception!),
                    TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
            }

            if (_inner is BackgroundService bg && bg.ExecuteTask != null)
            {
                _ = bg.ExecuteTask.ContinueWith(
                    t => HandleException(t.Exception!),
                    TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            HandleException(ex);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _inner.StopAsync(cancellationToken);
            if (_executingTask != null)
                await _executingTask;
        }
        catch (Exception ex)
        {
            HandleException(ex);
            throw;
        }
    }

    private void HandleException(Exception ex)
    {
        _ = _mapper.Map(ex) ?? new ExceptionMapping(
            StatusCodes.Status500InternalServerError,
            "background_service_error",
            ErrorType.Unexpected);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_inner is IDisposable d)
            d.Dispose();
    }
}
