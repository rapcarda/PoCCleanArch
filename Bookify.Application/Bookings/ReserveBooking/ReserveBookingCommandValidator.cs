using FluentValidation;

namespace Bookify.Application.Bookings.ReserveBooking;
public class ReserveBookingCommandValidator : AbstractValidator<ReserveBookingCommand>
{
    // Estas validações serão executada quando a pipeline configurada para o validator for chamado.
    public ReserveBookingCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.ApartmentId).NotEmpty();
        RuleFor(c => c.StartDate).LessThan(c => c.EndDate);
    }
}
