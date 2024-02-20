using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.Services;
using Bookify.Domain.Bookings.ValueObjects;
using Bookify.Domain.Users;

namespace Bookify.Application.Bookings.ReserveBooking;

internal sealed class ReserveBookingCommandHandler : ICommandHandler<ReserveBookingCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IBookingRespository _bookingRespoitory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PricingService _pricingService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ReserveBookingCommandHandler(IUserRepository userRepository,
        IApartmentRepository apartmentRepository, IBookingRespository bookingRespoitory,
        IUnitOfWork unitOfWork, PricingService pricingService, IDateTimeProvider dateTimeProvider)
    {
        _userRepository=userRepository;
        _apartmentRepository=apartmentRepository;
        _bookingRespoitory=bookingRespoitory;
        _unitOfWork=unitOfWork;
        _pricingService=pricingService;
        _dateTimeProvider=dateTimeProvider;
    }

    public async Task<Result<Guid>> Handle(ReserveBookingCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<Guid>(UserErrors.NotFoud);
        }

        var apartment = await _apartmentRepository.GetByIdAsync(request.ApartmentId, cancellationToken);

        if (apartment is null)
        {
            return Result.Failure<Guid>(ApartmentErrors.NotFound);
        }

        var duration = DateRange.Create(request.StartDate, request.EndDate);

        if (await _bookingRespoitory.IsOverlappingAsync(apartment, duration, cancellationToken))
        {
            return Result.Failure<Guid>(BookingErrors.Overlap);
        }

        //try para tratamento de concorrência
        //Geralmente tem um valor no DB que gerencia o optmistic concurrency. Isso é usado quando for persistir as alterações no DB, verifica se a versão do BD é diferente da em memória
        //Neste caso não pode ser nenhum campo do booking, por que ele ainda não existe no BD, por isso podemos usar ou Apartment ou User.
        //Se analisar o método de Reserve no Booking, tem um campo "LastBookedOnUtc", do Apartment. Ver em ApartmentConfiguration
        try
        {
            var booking = Booking.Reserve(apartment,
                user.Id,
                duration,
                //  Utilizar o utcNow como abaixo não é errado, porém tem uma forma melhor, que deixa o código mais testável, podendo mockar o utcNow,
                //   que é utilizando uma interface.
                //utcNow: DateTime.UtcNow, 
                _dateTimeProvider.UtcNow,
                _pricingService);

            _bookingRespoitory.Add(booking);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return booking.Id;
        }
        catch (ConcurrencyException) 
        {
            //Neste caso, se der erro de concorrência do registro quer dizer que outro usuário fez a reserva antes, por isso retorna erro de overlap
            return Result.Failure<Guid>(BookingErrors.Overlap);
        }
    }
}
