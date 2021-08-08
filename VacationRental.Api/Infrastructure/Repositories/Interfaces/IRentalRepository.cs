namespace VacationRental.Api.Infrastructure.Repositories.Interfaces
{
    using Models;

    public interface IRentalRepository
    {
        RentalViewModel GetRentalById(int rentalId);

        ResourceIdViewModel CreateRental(RentalBindingModel model);

        RentalViewModel UpdateRental(RentalUpdateModel model);
    }
}