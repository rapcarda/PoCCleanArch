using Bookify.Domain.Apartments;

namespace Bookify.Infrasctucture.Repositories;
internal sealed class ApartmentRepository : Repository<Apartment>, IApartmentRepository
{
    public ApartmentRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
