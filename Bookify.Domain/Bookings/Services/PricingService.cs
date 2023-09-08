using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings.ValueObjects;
using Bookify.Domain.Shared;

namespace Bookify.Domain.Bookings.Services;
public class PricingService
{
    public PricingDetails CalculatePrice(Apartment apartment, DateRange period)
    {
        var currency = apartment.Price.Currency;

        var priceForPeriod = new Money(
            apartment.Price.Amount * period.LengthInDays);

        decimal percentageUpCharge = 0;
        foreach (var amenity in apartment.amenities)
        {
            percentageUpCharge += amenity switch
            {
                Amenity.GardemView or Amenity.MoutainView => 0.05m,
                Amenity.AirConditioning => 0.01m,
                Amenity.Parking => 0.01m,
                _ => 0
            };
        }

        var amenitiesUpCharge = Money.Zero();
        if(percentageUpCharge > 0)
        {
            amenitiesUpCharge = new Money(priceForPeriod.Amount * percentageUpCharge, currency);
        }

        var totalPrice = Money.Zero();

        totalPrice += priceForPeriod;

        if (!apartment.CleaningFee.IsZero())
        {
            totalPrice += apartment.CleaningFee;
        }

        totalPrice += amenitiesUpCharge;

        return new PricingDetails(priceForPeriod, apartment.CleaningFee, amenitiesUpCharge, totalPrice);
    }


}
