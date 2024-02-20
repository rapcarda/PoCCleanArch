﻿using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrasctucture.Repositories;
internal sealed class BookingRepository : Repository<Booking>, IBookingRespository
{
    private static readonly BookingStatus[] ActiveBookingStatus =
    {
        BookingStatus.Reserved,
        BookingStatus.Confirmed,
        BookingStatus.Completed
    };
    
    public BookingRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> IsOverlappingAsync(Apartment apartmentId, DateRange duration, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Booking>()
            .AnyAsync(
            booking =>
                booking.ApartmentId == apartmentId.Id &&
                booking.Duration.Start <= duration.End &&
                booking.Duration.End >= duration.Start &&
                ActiveBookingStatus.Contains(booking.Status),
            cancellationToken);
    }
}
