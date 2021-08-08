namespace VacationRental.Api.Infrastructure.Services
{
    using System;
    using Interfaces;
    using Models;
    using Repositories.Interfaces;

    public class RentalService : IRentalService
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingService _bookingService;

        public RentalService(IRentalRepository rentalRepository, IBookingRepository bookingRepository, IBookingService bookingService)
        {
            _rentalRepository = rentalRepository;
            _bookingRepository = bookingRepository;
            _bookingService = bookingService;
        }

        public RentalViewModel GetRentalById(int rentalId)
        {
            return _rentalRepository.GetRentalById(rentalId);
        }

        public ResourceIdViewModel CreateRental(RentalBindingModel model)
        {
            ValidateRental(model.PreparationTimeInDays, model.Units);

            return _rentalRepository.CreateRental(model);
        }

        public RentalViewModel UpdateRental(RentalUpdateModel model)
        {
            ValidateRental(model.PreparationTimeInDays, model.Units);

            var rental = _rentalRepository.GetRentalById(model.Id);

            foreach (var booking in _bookingRepository.GetAllBookingForRental(model.Id))
            {
                if (!_bookingService.IsBookingAvailable(new RentalViewModel
                {
                    Id = rental.Id,
                    Units = model.Units,
                    PreparationTimeInDays = model.PreparationTimeInDays
                }, new BookingBindingModel
                {
                    RentalId = booking.RentalId,
                    Nights = booking.Nights,
                    Start = booking.Start
                }))
                    throw new ApplicationException("Not available");
            }

            return _rentalRepository.UpdateRental(model);
        }

        private static void ValidateRental(int preparationTimeInDays, int units)
        {
            if (preparationTimeInDays < 0)
                throw new ApplicationException("Preparation Time In Days must be positive");
            if (units < 0)
                throw new ApplicationException("Units must be positive");
        }
    }
}