/* Representa todos os eventos que podem ser emitidos no sistema */
/* Instalar package Metiatr.Contracts */
using MediatR;

namespace Bookify.Domain.Abstractions;

/* INotification usa implementação de publish e subscribe */
public  interface IDomainEvent : INotification
{
}

/* Utilizado em Entity */
