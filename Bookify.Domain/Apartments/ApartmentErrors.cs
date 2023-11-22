using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Apartments;
public static class ApartmentErrors
{
    public static Error NotFound = new (
        "Apartment.NotFound",
        "The apartment with the specified indentifier was not foud")
}
