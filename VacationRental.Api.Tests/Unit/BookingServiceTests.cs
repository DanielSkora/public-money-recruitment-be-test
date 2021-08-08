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
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _bookingRepositoryMock = new Mock<IBookingRepository>();
        private readonly Mock<IRentalRepository> _rentalRepositoryMock = new Mock<IRentalRepository>();

        private readonly IBookingService _bookingService;

        public BookingServiceTests()
        {
            _bookingService = new BookingService(_bookingRepositoryMock.Object, _rentalRepositoryMock.Object);
        }

        [Fact]
        public void GivenCompleteModel_WhenCreateBooking_ThenAGetReturnsTheCreatedBooking()
        {
            // Arrange
            const int rentalId = 1;
            var rental = new RentalBuilder().WithId(rentalId).WithUnit(2).WithPreparationTimeInDays(1).Build();
            _rentalRepositoryMock.Setup(r => r.GetRentalById(rentalId)).Returns(() => rental);
            _bookingRepositoryMock.Setup(r => r.GetAllBookingForRental(rentalId))
                .Returns(() => new List<BookingViewModel>());
            var booking = new BookingBuilder()
                .WithId(1)
                .WithRentalId(rentalId)
                .WithNights(2)
                .WithStart(new DateTime(2002, 01, 01))
                .Build();
            _bookingRepositoryMock.Setup(r => r.Create(It.IsAny<BookingBindingModel>()))
                .Returns(() => new ResourceIdViewModel {Id = booking.Id});


            // Act
            var result =_bookingService.Create(new BookingBindingModel {RentalId = rentalId, Nights = 2, Start = new DateTime(2002, 01, 01)});

            // Assert
            Assert.Equal(booking.Id, result.Id);
            _bookingRepositoryMock.Verify(repository => repository.Create(It.IsAny<BookingBindingModel>()), Times.Once);
            _bookingRepositoryMock.Verify(repository => repository.GetAllBookingForRental(rentalId), Times.Once);
        }

        [Fact]
        public void GivenCompleteModel_WhenCheckingIfIsBookingAvailable_ThenAGetReturnsTrue()
        {
            // Arrange
            const int rentalId = 1;
            var rental = new RentalBuilder().WithId(rentalId).WithUnit(2).WithPreparationTimeInDays(1).Build();
            var booking = new BookingBuilder()
                .WithId(1)
                .WithRentalId(rentalId)
                .WithNights(2)
                .WithStart(new DateTime(2002, 01, 01))
                .Build();
            _bookingRepositoryMock.Setup(r => r.GetAllBookingForRental(rentalId))
                .Returns(() => new List<BookingViewModel> { booking });


            // Act
            var result = _bookingService.IsBookingAvailable(new RentalViewModel
            {
                Id = rentalId,
                Units = rental.Units,
                PreparationTimeInDays = rental.PreparationTimeInDays
            }, new BookingBindingModel
            {
                RentalId = rentalId,
                Nights = 2,
                Start = new DateTime(2002, 01, 01)
            });

            // Assert
            Assert.True(result);
            _bookingRepositoryMock.Verify(repository => repository.GetAllBookingForRental(rentalId), Times.Once);
        }

        [Fact]
        public void GivenCompleteModel_WhenCheckingIfIsBookingAvailable_ThenAGetReturnsFalse()
        {
            // Arrange
            const int rentalId = 1;
            var rental = new RentalBuilder().WithId(rentalId).WithUnit(1).WithPreparationTimeInDays(1).Build();
            var booking = new BookingBuilder()
                .WithId(1)
                .WithRentalId(rentalId)
                .WithNights(2)
                .WithStart(new DateTime(2002, 01, 01))
                .Build();
            _bookingRepositoryMock.Setup(r => r.GetAllBookingForRental(rentalId))
                .Returns(() => new List<BookingViewModel> { booking });


            // Act
            var result = _bookingService.IsBookingAvailable(new RentalViewModel
            {
                Id = rentalId,
                Units = rental.Units,
                PreparationTimeInDays = rental.PreparationTimeInDays
            }, new BookingBindingModel
            {
                RentalId = rentalId,
                Nights = 2,
                Start = new DateTime(2002, 01, 01)
            });

            // Assert
            Assert.False(result);
            _bookingRepositoryMock.Verify(repository => repository.GetAllBookingForRental(rentalId), Times.Once);
        }

        [Fact]
        public void GivenCompleteModelWithOverlappingBooking_WhenCreateBooking_ThenThrowsApplicationException()
        {
            // Arrange
            const int rentalId = 1;
            var rental = new RentalBuilder().WithId(rentalId).WithUnit(1).WithPreparationTimeInDays(1).Build();
            _rentalRepositoryMock.Setup(r => r.GetRentalById(rentalId)).Returns(() => rental);
            var booking = new BookingBuilder()
                .WithId(1)
                .WithRentalId(rentalId)
                .WithNights(2)
                .WithStart(new DateTime(2002, 01, 01))
                .Build();
            _bookingRepositoryMock.Setup(r => r.GetAllBookingForRental(rentalId))
                .Returns(() => new List<BookingViewModel> { booking });



            // Act & Assert
            Assert.Throws<ApplicationException>(() => _bookingService.Create(new BookingBindingModel { RentalId = rentalId, Nights = 2, Start = new DateTime(2002, 01, 01) }));
            _bookingRepositoryMock.Verify(repository => repository.Create(It.IsAny<BookingBindingModel>()), Times.Never);
            _bookingRepositoryMock.Verify(repository => repository.GetAllBookingForRental(rentalId), Times.Once);
        }

        [Fact]
        public void GivenCompleteModelWithNegativeNights_WhenCreateBooking_ThenThrowsApplicationException()
        {
            // Arrange
            const int rentalId = 1;
  
            // Act & Assert
            Assert.Throws<ApplicationException>(() => _bookingService.Create(new BookingBindingModel { RentalId = rentalId, Nights = -2, Start = new DateTime(2002, 01, 01) }));
            _bookingRepositoryMock.Verify(repository => repository.Create(It.IsAny<BookingBindingModel>()), Times.Never);
            _bookingRepositoryMock.Verify(repository => repository.GetAllBookingForRental(rentalId), Times.Never);
        }

    }
}