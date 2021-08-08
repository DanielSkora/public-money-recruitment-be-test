namespace VacationRental.Api.Tests.Integration
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Models;
    using Xunit;

    [CollectionDefinition("Integration")]
    public class TestBase
    {
        private readonly HttpClient _client;

        public TestBase(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }

        public async Task<ResourceIdViewModel> CreateRental(int unit, int preparationTimeInDays)
        {
            var postRentalRequest = new RentalBindingModel
            {
                Units = unit,
                PreparationTimeInDays = preparationTimeInDays
            };

            ResourceIdViewModel postRentalResult;
            using (var postRentalResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", postRentalRequest))
            {
                Assert.True(postRentalResponse.IsSuccessStatusCode);
                postRentalResult = await postRentalResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            return postRentalResult;
        }

        public async Task<ResourceIdViewModel> CreateRental(RentalBindingModel model)
        {
            return await CreateRental(model.Units, model.PreparationTimeInDays);
        }

        public async Task<ResourceIdViewModel> CreateBooking(int rentalId, int nights, DateTime start)
        {
            var postBooking1Request = new BookingBindingModel
            {
                RentalId = rentalId,
                Nights = nights,
                Start = start
            };

            ResourceIdViewModel postBookingResult;
            using (var postBookingResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request))
            {
                Assert.True(postBookingResponse.IsSuccessStatusCode);
                postBookingResult = await postBookingResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            return postBookingResult;
        }

        public async Task<ResourceIdViewModel> CreateBooking(BookingBindingModel model)
        {
            return await CreateBooking(model.RentalId, model.Nights, model.Start);
        }
    }
}