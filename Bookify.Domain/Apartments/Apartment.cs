﻿using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments.ValueObjects;
using Bookify.Domain.Shared;

namespace Bookify.Domain.Apartments;

public sealed class Apartment : Entity
{
    public Apartment(
        Guid id, 
        Name name, 
        Description description, 
        Address address, 
        Money price, 
        Money cleaningFee, 
        DateOnly? lastBookedOnUtc, 
        List<Amenity> amenities) 
        : base(id)
    {
        Name = name;
        Description = description;
        Address = address;
        Price = price;
        CleaningFee = cleaningFee;
        LastBookedOnUtc = lastBookedOnUtc;
        this.amenities = amenities;
    }

    public Name Name { get; private set; }

    public Description Description { get; private set; }

    public Address Address { get; private set; }

    public Money Price { get; private set; }

    public Money CleaningFee { get; private set; }

    public DateOnly? LastBookedOnUtc { get; private set; }

    public List<Amenity> amenities { get; private set; } = new();
}