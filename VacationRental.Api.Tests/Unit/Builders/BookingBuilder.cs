namespace VacationRental.Api.Tests.Unit.Builders
{
    using System;
    using Models;

    public class BookingBuilder
    {
        private readonly BookingViewModel _booking;

        public BookingBuilder()
        {
            _booking = new BookingViewModel();
        }

        public BookingBuilder WithId(int id)
        {
            _booking.Id = id;
            return this;
        }

        public BookingBuilder WithRentalId(int rentalId)
        {
            _booking.RentalId = rentalId;
            return this;
        }

        public BookingBuilder WithNights(int nights)
        {
            _booking.Nights = nights;
            return this;
        }

        public BookingBuilder WithStart(DateTime start)
        {
            _booking.Start = start;
            return this;
        }

        public BookingViewModel Build()
        {
            return _booking;
        }
    }
}