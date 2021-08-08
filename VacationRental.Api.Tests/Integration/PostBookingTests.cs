namespace VacationRental.Api.Tests.Integration
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Models;
    using Xunit;

    [Collection("Integration")]
    public class PostBookingTests : TestBase
    {
        private readonly HttpClient _client;

        public PostBookingTests(IntegrationFixture fixture) : base(fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostBooking_ThenAGetReturnsTheCreatedBooking()
        {
            ResourceIdViewModel postRentalResult = await CreateRental(4, 0);


            var postBookingRequest = new BookingBindingModel
            {
                 RentalId = postRentalResult.Id,
                 Nights = 3,
                 Start = new DateTime(2001, 01, 01)
            };

            ResourceIdViewModel postBookingResult = await CreateBooking(postBookingRequest);

            using (var getBookingResponse = await _client.GetAsync($"/api/v1/bookings/{postBookingResult.Id}"))
            {
                Assert.True(getBookingResponse.IsSuccessStatusCode);

                var getBookingResult = await getBookingResponse.Content.ReadAsAsync<BookingViewModel>();
                Assert.Equal(postBookingRequest.RentalId, getBookingResult.RentalId);
                Assert.Equal(postBookingRequest.Nights, getBookingResult.Nights);
                Assert.Equal(postBookingRequest.Start, getBookingResult.Start);
            }
        }

        [Fact]
        public async Task GivenWrongsNightsInRequest_WhenPostBooking_ThenThrowsApplicationException()
        {
            var postBookingRequest = new BookingBindingModel
            {
                RentalId = 1,
                Nights = -3,
                Start = new DateTime(2001, 01, 01)
            };
            
            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                using (await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest))
                {
                }
            });
        }

        [Fact]
        public async Task GivenWrongsRentalIdInRequest_WhenPostBooking_ThenThrowsApplicationException()
        {
            var postBookingRequest = new BookingBindingModel
            {
                RentalId = -1,
                Nights = 3,
                Start = new DateTime(2001, 01, 01)
            };

            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                using (await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest))
                {
                }
            });
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostBooking_ThenAPostReturnsErrorWhenThereIsOverbookingAfterRent()
        {
            ResourceIdViewModel postRentalResult = await CreateRental(1, 0);

            var postBooking1Request = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = 3,
                Start = new DateTime(2002, 01, 01)
            };

            await CreateBooking(postBooking1Request);

            var postBooking2Request = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = 1,
                Start = new DateTime(2002, 01, 02)
            };

            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                using (await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
                {
                }
            });
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostBooking_ThenAPostReturnsErrorWhenThereIsOverbookingBeforeRent()
        {
            ResourceIdViewModel postRentalResult = await CreateRental(1, 0);

            var postBooking1Request = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = 3,
                Start = new DateTime(2002, 01, 02)
            };

            await CreateBooking(postBooking1Request);

            var postBooking2Request = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = 2,
                Start = new DateTime(2002, 01, 01)
            };

            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                using (await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
                {
                }
            });
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostBooking_ThenAPostReturnsErrorWhenThereIsOverbookingAfterRentOnPreparationTime()
        {
            ResourceIdViewModel postRentalResult = await CreateRental(1, 3);

            var postBooking1Request = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = 3,
                Start = new DateTime(2002, 01, 01)
            };

            await CreateBooking(postBooking1Request);

            var postBooking2Request = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = 2,
                Start = new DateTime(2002, 01, 06)
            };

            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                using (await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
                {
                }
            });
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostBooking_ThenAPostReturnsErrorWhenThereIsOverbookingBeforeRentOnPreparationTime()
        {
            ResourceIdViewModel postRentalResult = await CreateRental(1, 3);

            var postBooking1Request = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = 3,
                Start = new DateTime(2002, 01, 06)
            };

            await CreateBooking(postBooking1Request);

            var postBooking2Request = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = 2,
                Start = new DateTime(2002, 01, 03)
            };

            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                using (await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
                {
                }
            });
        }
    }
}
