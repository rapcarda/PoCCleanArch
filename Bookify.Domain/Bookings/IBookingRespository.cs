using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings.ValueObjects;

namespace Bookify.Domain.Bookings;
public interface IBookingRespository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> IsOverlappingAsync(Apartment apartmentId, DateRange duration, CancellationToken cancellationToken = default);

    void Add(Booking booking);
}
