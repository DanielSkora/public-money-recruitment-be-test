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
    public class RentalServiceTests
    {
        private readonly Mock<IRentalRepository> _rentalRepositoryMock = new Mock<IRentalRepository>();
        private readonly Mock<IBookingRepository> _bookingRepositoryMock = new Mock<IBookingRepository>();
        private readonly Mock<IBookingService> _bookingServiceMock = new Mock<IBookingService>();

        private readonly IRentalService _rentalService;

        public RentalServiceTests()
        {
            _rentalService = new RentalService(_rentalRepositoryMock.Object, _bookingRepositoryMock.Object, _bookingServiceMock.Object);
        }
        
        [Fact]
        public void GivenCompleteModel_WhenCreateRental_ThenAGetReturnsTheCreatedRental()
        {
            // Arrange
            var rentalId = 1;
            var rentalRequest = new RentalBindingModel {Units = 1, PreparationTimeInDays = 1};
            _rentalRepositoryMock.Setup(r => r.CreateRental(rentalRequest))
                .Returns(() => new ResourceIdViewModel {Id = rentalId});
            // Act
            var result = _rentalService.CreateRental(rentalRequest);

            // Assert
            Assert.Equal(rentalId, result.Id);
            _rentalRepositoryMock.Verify(repository => repository.CreateRental(It.IsAny<RentalBindingModel>()), Times.Once);
        }

        [Fact]
        public void GivenCompleteModel_WhenUpdateRental_ThenAGetReturnsTheUpdatedRental()
        {
            // Arrange
            var rental = new RentalBuilder().WithId(1).WithUnit(1).WithPreparationTimeInDays(1).Build();
            var rentalRequest = new RentalUpdateModel { Id = rental.Id, Units = 4, PreparationTimeInDays = 2 };
            _rentalRepositoryMock.Setup(r => r.GetRentalById(rental.Id))
                .Returns(() => rental);
            _rentalRepositoryMock.Setup(r => r.UpdateRental(rentalRequest)).Returns(() => new RentalViewModel
            {
                Id = rental.Id,
                Units = rentalRequest.Units,
                PreparationTimeInDays = rentalRequest.PreparationTimeInDays
            });
            var booking = new BookingBuilder()
                .WithId(1)
                .WithRentalId(rental.Id)
                .WithNights(2)
                .WithStart(new DateTime(2002, 01, 01))
                .Build();
            _bookingRepositoryMock.Setup(r => r.GetAllBookingForRental(rental.Id))
                .Returns(() => new List<BookingViewModel> { booking });
            _bookingServiceMock
                .Setup(r => r.IsBookingAvailable(It.IsAny<RentalViewModel>(), It.IsAny<BookingBindingModel>()))
                .Returns(true);

            // Act
            var result = _rentalService.UpdateRental(rentalRequest);

            // Assert
            Assert.Equal(rental.Id, result.Id);
            Assert.Equal(rentalRequest.Units, result.Units);
            Assert.Equal(rentalRequest.PreparationTimeInDays, result.PreparationTimeInDays);
            _rentalRepositoryMock.Verify(repository => repository.GetRentalById(It.IsAny<int>()), Times.Once);
            _rentalRepositoryMock.Verify(repository => repository.UpdateRental(It.IsAny<RentalUpdateModel>()), Times.Once);
            _bookingRepositoryMock.Verify(repository => repository.GetAllBookingForRental(It.IsAny<int>()), Times.Once);
            _bookingServiceMock.Verify(repository => repository.IsBookingAvailable(It.IsAny<RentalViewModel>(), It.IsAny<BookingBindingModel>()), Times.Once);
        }

        [Fact]
        public void GivenCompleteModelWithOverlappingBookings_WhenUpdateRental_ThenThrowsApplicationException()
        {
            // Arrange
            var rental = new RentalBuilder().WithId(1).WithUnit(1).WithPreparationTimeInDays(1).Build();
            var rentalRequest = new RentalUpdateModel { Id = rental.Id, Units = 4, PreparationTimeInDays = 2 };
            _rentalRepositoryMock.Setup(r => r.GetRentalById(rental.Id))
                .Returns(() => rental);
            var booking = new BookingBuilder()
                .WithId(1)
                .WithRentalId(rental.Id)
                .WithNights(2)
                .WithStart(new DateTime(2002, 01, 01))
                .Build(); 
            _bookingRepositoryMock.Setup(r => r.GetAllBookingForRental(rental.Id))
                .Returns(() => new List<BookingViewModel>{ booking });
            _bookingServiceMock
                .Setup(r => r.IsBookingAvailable(It.IsAny<RentalViewModel>(), It.IsAny<BookingBindingModel>()))
                .Returns(false);


            // Assert
            Assert.Throws<ApplicationException>(() => _rentalService.UpdateRental(rentalRequest));
            _rentalRepositoryMock.Verify(repository => repository.GetRentalById(It.IsAny<int>()), Times.Once);
            _rentalRepositoryMock.Verify(repository => repository.UpdateRental(It.IsAny<RentalUpdateModel>()), Times.Never);
            _bookingRepositoryMock.Verify(repository => repository.GetAllBookingForRental(It.IsAny<int>()), Times.Once);
            _bookingServiceMock.Verify(repository => repository.IsBookingAvailable(It.IsAny<RentalViewModel>(), It.IsAny<BookingBindingModel>()), Times.Once);
        }

        [Fact]
        public void GivenCompleteModelWithNegativeUnit_WhenCreateRental_ThenThrowsApplicationException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ApplicationException>(() => _rentalService.CreateRental(new RentalBindingModel() { Units = -1, PreparationTimeInDays = 1}));
            _rentalRepositoryMock.Verify(repository => repository.CreateRental(It.IsAny<RentalBindingModel>()), Times.Never);
        }

        [Fact]
        public void GivenCompleteModelWithNegativePreparationTimeInDays_WhenCreateRental_ThenThrowsApplicationException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ApplicationException>(() => _rentalService.CreateRental(new RentalBindingModel() { Units = 1, PreparationTimeInDays = -1 }));
            _rentalRepositoryMock.Verify(repository => repository.CreateRental(It.IsAny<RentalBindingModel>()), Times.Never);
        }
    }
}