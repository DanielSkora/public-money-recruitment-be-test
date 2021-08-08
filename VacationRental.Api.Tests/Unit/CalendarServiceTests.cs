namespace VacationRental.Api.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Infrastructure.Repositories.Interfaces;
    using Infrastructure.Services;
    using Infrastructure.Services.Interfaces;
    using Models;
    using Moq;
    using Xunit;

    [Collection("Unit")]
    public class CalendarServiceTests
    {
        private readonly Mock<IBookingRepository> _bookingRepositoryMock = new Mock<IBookingRepository>();
        private readonly Mock<IRentalRepository> _rentalRepositoryMock = new Mock<IRentalRepository>();

        private readonly ICalendarService _calendarService;

        public CalendarServiceTests()
        {
            _calendarService = new CalendarService(_rentalRepositoryMock.Object, _bookingRepositoryMock.Object);
        }

        [Fact]
        public void GivenCompleteModel_WhenGettingCalendar_ThenAGetReturnsTheCalendarViewModel()
        {
            // Arrange
            const int rentalId = 1;
            var rental = new RentalBuilder().WithId(rentalId).WithUnit(2).WithPreparationTimeInDays(1).Build();
            _rentalRepositoryMock.Setup(r => r.GetRentalById(rentalId)).Returns(() => rental);
            var booking = new BookingBuilder()
                .WithId(1)
                .WithRentalId(rentalId)
                .WithNights(1)
                .WithStart(new DateTime(2002, 01, 02))
                .Build();
            _bookingRepositoryMock.Setup(r => r.GetAllBookingForRental(rentalId))
                .Returns(() => new List<BookingViewModel> { booking });


            // Act
            var result = _calendarService.GetCalendar(rentalId, new DateTime(2002, 01, 01), 3);

            // Assert
            Assert.Equal(rentalId, result.RentalId);
            Assert.Collection(result.Dates,
                item => Assert.Equal(item.Date, new DateTime(2002, 01, 01)),
                item => Assert.Equal(item.Date, new DateTime(2002, 01, 02)),
                item => Assert.Equal(item.Date, new DateTime(2002, 01, 03)));
            Assert.Empty(result.Dates[0].Bookings);
            Assert.Empty(result.Dates[0].PreparationTimes);
            Assert.Equal(1, result.Dates[1].Bookings[0].Id);
            Assert.Equal(1, result.Dates[1].Bookings[0].Unit);
            Assert.Empty(result.Dates[1].PreparationTimes);
            Assert.Empty(result.Dates[2].Bookings);
            Assert.Equal(1, result.Dates[2].PreparationTimes[0].Unit);

            _rentalRepositoryMock.Verify(repository => repository.GetRentalById(rentalId), Times.Once);
            _bookingRepositoryMock.Verify(repository => repository.GetAllBookingForRental(rentalId), Times.Once);
        }

        [Fact]
        public void GivenCompleteModelWithNegativeNights_WhenGettingCalendar_ThenThrowsApplicationException()
        {
            // Arrange
            const int rentalId = 1;

            // Act & Assert
            Assert.Throws<ApplicationException>(() => _calendarService.GetCalendar(rentalId, new DateTime(2002, 01, 01), -1));
            _rentalRepositoryMock.Verify(repository => repository.GetRentalById(rentalId), Times.Never);
        }
    }
}