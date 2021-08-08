namespace VacationRental.Api.Infrastructure.Services
{
    using System;
    using System.Collections.Generic;
    using Interfaces;
    using Models;
    using Repositories.Interfaces;

    public class CalendarService : ICalendarService
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly IBookingRepository _bookingRepository;

        public CalendarService(IRentalRepository rentalRepository, IBookingRepository bookingRepository)
        {
            _rentalRepository = rentalRepository;
            _bookingRepository = bookingRepository;
        }

        public CalendarViewModel GetCalendar(int rentalId, DateTime start, int nights)
        {
            if (nights < 0)
                throw new ApplicationException("Nights must be positive");
            var rental = _rentalRepository.GetRentalById(rentalId);

            var result = new CalendarViewModel
            {
                RentalId = rentalId,
                Dates = new List<CalendarDateViewModel>()
            };

            var allBookingForRental = _bookingRepository.GetAllBookingForRental(rentalId);

            for (var i = 0; i < nights; i++)
            {
                var date = new CalendarDateViewModel
                {
                    Date = start.Date.AddDays(i),
                    Bookings = new List<CalendarBookingViewModel>(),
                    PreparationTimes = new List<CalendarPreparationTimeViewModel>()
                };

                var unit = 1;

                foreach (var booking in allBookingForRental)
                {
                    var bookingEnd = booking.Start.AddDays(booking.Nights);

                    if (booking.Start <= date.Date && bookingEnd > date.Date)
                    {
                        date.Bookings.Add(new CalendarBookingViewModel { Id = booking.Id, Unit = unit });
                    }

                    if (bookingEnd <= date.Date && bookingEnd.AddDays(rental.PreparationTimeInDays) > date.Date)
                    {
                        date.PreparationTimes.Add(new CalendarPreparationTimeViewModel {Unit = unit });
                    }
                    unit++;
                }

                result.Dates.Add(date);
            }

            return result;
        }
    }
}