using Application.Interfaces;
using MediatR;

namespace Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IMediator _mediator;

        public UnitOfWork(AppDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
            await _mediator.DispatchDomainEventsAsync(_context);
        }
    }

}
