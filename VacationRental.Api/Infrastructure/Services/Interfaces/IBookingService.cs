namespace VacationRental.Api.Infrastructure.Services.Interfaces
{
    using Models;

    public interface IBookingService
    {
        BookingViewModel GetById(int bookingId);

        ResourceIdViewModel Create(BookingBindingModel model);

        bool IsBookingAvailable(RentalViewModel rental, BookingBindingModel model);
    }
}