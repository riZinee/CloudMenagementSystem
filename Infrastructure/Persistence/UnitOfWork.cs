using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IMediator _mediator;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public UnitOfWork(AppDbContext context, IMediator mediator, IBackgroundTaskQueue taskQueue, IServiceScopeFactory serviceScopeFactory)
        {
            _context = context;
            _mediator = mediator;
            _taskQueue = taskQueue;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
            await EnqueueDomainEventsAsync();
        }

        public async Task EnqueueDomainEventsAsync()
        {
            var domainEvents = _context.ChangeTracker
                .Entries<IHasDomainEvents>()
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            foreach (var domainEvent in domainEvents)
            {
                _taskQueue.QueueBackgroundWorkItem(async token =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    await mediator.Publish(domainEvent, token);
                });
            }

            foreach (var entry in _context.ChangeTracker.Entries<IHasDomainEvents>())
            {
                entry.Entity.ClearEvents();
            }
        }
    }

}
