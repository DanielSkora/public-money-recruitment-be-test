namespace VacationRental.Api.Infrastructure.Services.Interfaces
{
    using Models;

    public interface IRentalService
    {
        RentalViewModel GetRentalById(int rentalId);

        ResourceIdViewModel CreateRental(RentalBindingModel model);

        RentalViewModel UpdateRental(RentalUpdateModel model);
    }
}