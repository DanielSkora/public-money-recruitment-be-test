namespace VacationRental.Api.Infrastructure.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Models;

    public interface IBookingRepository
    {
        BookingViewModel GetById(int bookingId);
        
        List<BookingViewModel> GetAllBookingForRental(int rentalId);

        ResourceIdViewModel Create(BookingBindingModel model);
    }
}