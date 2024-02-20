using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrasctucture;
public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    private readonly IPublisher _publisher;

    public ApplicationDbContext(DbContextOptions options, IPublisher publisher) : base(options)
    {
        _publisher = publisher;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    //Implementar a publicação de eventos do domain aqui, pois é onde manipula o unit of work
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        try
        {
            //Este método tem um problema sério. Após o saveChanges é feito o publish dos eventos, porém, pode dar erro, então o método de SaveChanges retornaria erro, porém, o "commit" dos dados já foi feito
            //Chamar o Publish antes do SaveChangesAsync resolveria o problema, porém, não é correto, já que um evento é uma ação que já aconteceu.
            var result = await base.SaveChangesAsync(cancellationToken);

            await PublishDomainEventsAsync();

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            //Parte do tratamento de concorrência. Próximo passo do tratamento ver em ReserveBookingCommandHandler onde tratará a concorrência também.
            throw new ConcurrencyException("Concurrency exception occurred.", ex);
        }

        //Documentação para optimistic concurrency
        //https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=data-annotations
        //https://www.npgsql.org/efcore/modeling/concurrency.html?tabs=data-annotations

    }

    private async Task PublishDomainEventsAsync()
    {
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.GetDomainEvents();
                entity.ClearDomainEvents();
                return domainEvents; ;
            })
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent);
        }
    }
}
