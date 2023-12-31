﻿using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using Dapper;

namespace Bookify.Application.Apartments.SearchApartments;
internal sealed class SearchApartmentsQueryHandler : IQueryHandler<SearchApartmentsQuery, IReadOnlyList<ApartmentResponse>>
{
    private static readonly int[] ActiveBookingStatuses =
    {
        (int)BookingStatus.Reserved,
        (int)BookingStatus.Confirmed,
        (int)BookingStatus.Completed,
    };


    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public SearchApartmentsQueryHandler(ISqlConnectionFactory connection)
    {
        _sqlConnectionFactory = connection;
    }

    public async Task<Result<IReadOnlyList<ApartmentResponse>>> Handle(SearchApartmentsQuery request, CancellationToken cancellationToken)
    {
        if (request.StartDate > request.EndDate)
        {
            return new List<ApartmentResponse>();
        }

        using var connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                a.id AS Id,
                a.name AS Name,
                a.description AS Description,
                a.price_amount AS Price,
                a.price_currency AS Currency,
                a.address_country AS Country,
                a.address_state AS State,
                a.address_zip_code AS ZipCode,
                a.address_city AS City,
                a.address_street AS Street
            FROM apartments AS a
            WHERE NOT EXISTS
            (
                SELECT 1
                FROM bookings As b
                WHERE b.apartment_id = a.id
                AND b.duration_start <= @EndDate
                AND b.duration_end >= @StartDate
                AND b.status = ANY(@ActiveBookingStatuses)
            )
            """;

        // O código abaixo utiliza 2 classes para propagar os dados vindo do DB via drapper. No QueryAsync é informado que metade dos dados será projetado em ApartmentResponse e outra metade em AddressResponse
        // e o objeto final será o terceiro parâmetro que é ApartmentResponse.
        // Com isso é informado o splitOn dizendo para o drapper exatamente a partir de que campo do select, ele deverá fazer a separação. Ao informar Country, todos os campos do select até o primeiro campo
        // imediatamente antes de Country será projetado em ApartmentResponse, a partir de Country será projetado em Address Response. Após o parâmetro sql é informado uma função dizendo que a propriedade
        // Address de apartmentResponse será os dadso de AddresResponse do mesmo registro.
        var apartments = await connection.QueryAsync<ApartmentResponse, AddressResponse, ApartmentResponse>(
            sql,
            (apartment, address) =>
            {
                apartment.Address = address;
                return apartment;
            },
            new
            {
                request.EndDate,
                request.StartDate,
                ActiveBookingStatuses
            },
            splitOn: "Country");

        return apartments.ToList();
    }
}
