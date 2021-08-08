namespace VacationRental.Api.Infrastructure.Services
{
    using System;
    using System.Linq;
    using Interfaces;
    using Models;
    using Repositories.Interfaces;

    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRentalRepository _rentalRepository;

        public BookingService(IBookingRepository bookingRepository, IRentalRepository rentalRepository)
        {
            _bookingRepository = bookingRepository;
            _rentalRepository = rentalRepository;
        }

        public BookingViewModel GetById(int bookingId)
        {
            return _bookingRepository.GetById(bookingId);
        }

        public ResourceIdViewModel Create(BookingBindingModel model)
        {
            if (model.Nights <= 0)
                throw new ApplicationException("Nigts must be positive");

            var rental = _rentalRepository.GetRentalById(model.RentalId);

            var isBookingAvailable = IsBookingAvailable(rental, model);
            if (!isBookingAvailable) 
                throw new ApplicationException("Not available");

            return _bookingRepository.Create(model);
        }
        
        public bool IsBookingAvailable(RentalViewModel rental, BookingBindingModel model)
        {
            var bookingsForRental = _bookingRepository.GetAllBookingForRental(model.RentalId);
            
            var count = bookingsForRental.Count(booking => model.Start < booking.Start.AddDays(booking.Nights).AddDays(rental.PreparationTimeInDays) && booking.Start < model.Start.AddDays(model.Nights).AddDays(rental.PreparationTimeInDays)); ;
            
            if (count >= rental.Units)
                    return false;
            
            return true;
        }
    }
}